using System;
using System.Collections.Generic;
using System.Text;

namespace AwakeningItemTool
{
    public class BinFileModel
    {
        private static readonly Encoding SHIFT_JIS = Encoding.GetEncoding("Shift_JIS");

        public const int HEADER_SIZE = 0x20;
        private const int FILE_SIZE_OFFSET = 0x0;
        private const int DATA_REGION_SIZE_OFFSET = 0x4;
        private const int POINTER_REGION_1_COUNT_OFFSET = 0x8;
        private const int POINTER_REGION_2_COUNT_OFFSET = 0xC;

        public int fileSize;
        public int dataRegionSize;
        public int pointerCount1;
        public int pointerCount2;
        public int pointerRegion1Size;
        public int pointerRegion2Size;
        public int labelSize;
        public byte[] dataRegionBytes;

        public List<Pointer> region1Pointers;
        public List<Pointer> region2Pointers;
        public List<Label> labels;

        public BinFileModel(byte[] binFileBytes)
        {
            byte[] intBuffer = new byte[4];

            this.ReadHeader(binFileBytes);

            // Copy dataRegion over to an array for safekeeping.
            // I don't know if I'll ever even do anything with it.
            this.dataRegionBytes = new byte[this.dataRegionSize];
            Array.Copy(binFileBytes, HEADER_SIZE, this.dataRegionBytes, 0, this.dataRegionSize);

            // Next, read in all the labels. We'll need them to help understand the pointers.
            this.ReadLabels(binFileBytes);

            // Now that we have all the labels, we can read in the pointers.
            // If the pointer is pointing to a label, we can correctly associate it now.
            this.ReadPointerRegion1(binFileBytes);
            this.ReadPointerRegion2(binFileBytes);
        }

        public byte[] OutputFileAsBytes()
        {
            byte[] fileBytes = new byte[this.fileSize];
            byte[] headerBytes = OutputHeaderAsBytes();
            byte[] pointerRegion1Bytes = OutputRegion1PointersAsBytes();
            byte[] pointerRegion2Bytes = OutputRegion2PointersAsBytes();
            byte[] labelBytes = OutputLabelsAsBytes();

            Array.Copy(headerBytes, 0, fileBytes, 0, HEADER_SIZE);
            Array.Copy(this.dataRegionBytes, 0, fileBytes, HEADER_SIZE, this.dataRegionSize);
            Array.Copy(pointerRegion1Bytes, 0, fileBytes, HEADER_SIZE + this.dataRegionSize, this.pointerRegion1Size);
            Array.Copy(pointerRegion2Bytes, 0, fileBytes, HEADER_SIZE + this.dataRegionSize + this.pointerRegion1Size, this.pointerRegion2Size);
            Array.Copy(labelBytes, 0, fileBytes, HEADER_SIZE + this.dataRegionSize + this.pointerRegion1Size + this.pointerRegion2Size, this.labelSize);
            return fileBytes;
        }

        public void PointDataAtOffsetToLabel(int dataOffset, string labelString)
        {
            // First, check to see if the label actually exists in the file.
            // If not, add it.
            Label labelInFile = GetLabelForString(labelString);
            if (labelInFile == null)
            {
                labelInFile = AddLabelToFile(labelString);
            }

            // Next, try to see if a pointer currently exists at the
            // supplied offset within the data. If it does, we can simply
            // repoint it to the new label. If not, we must create a new pointer.
            Pointer pointerAtOffset = GetPointerAtOffset(dataOffset);
            if (pointerAtOffset == null)
            {
                pointerAtOffset = AddPointerToFile(dataOffset, labelInFile);
            }

            // Update the pointer to point towards the label's location.
            pointerAtOffset.Label = labelInFile;
            pointerAtOffset.PointerFromDataRegion = labelInFile.Offset - HEADER_SIZE;

            // Update the data region to point to this new offset
            byte[] newOffsetBytes = this.ConvertIntToBytes(pointerAtOffset.PointerFromDataRegion);
            Array.Copy(newOffsetBytes, 0, this.dataRegionBytes, pointerAtOffset.PointerIntoDataRegion, 4);
        }

        public void IncreaseDataRegionSizeAtOffset(int valueToIncreaseBy, int offset)
        {
            int correctedOffset = offset - HEADER_SIZE;

            // First, copy the current data value array to a new array of the correct size up to the offset
            byte[] newDataRegionBytes = new byte[this.dataRegionSize + valueToIncreaseBy];
            Array.Copy(this.dataRegionBytes, 0, newDataRegionBytes, 0, correctedOffset);

            // Then, copy the remaining data AFTER the inserted bytes. This means there will be
            // valueToIncreaseBy 0x00s starting at the offset, which is what we want.
            Array.Copy(this.dataRegionBytes, correctedOffset, newDataRegionBytes, correctedOffset + valueToIncreaseBy, this.dataRegionSize - correctedOffset);

            // Next, update the fileSize, the dataRegionSize, and the dataRegionBytes
            this.fileSize += valueToIncreaseBy;
            this.dataRegionSize += valueToIncreaseBy;
            this.dataRegionBytes = newDataRegionBytes;

            // We pushed the part of the data region after the offset down, which messes with any pointer
            // that points into that section. Let's update those pointers.
            UpdatePointersAfterOffsetWithIncreasedValue(valueToIncreaseBy, correctedOffset);

            // By increasing the size, we moved every label, which mangles every pointer.
            // Fix these all up before exiting the method.
            UpdateLabelsAndPointersAfterFileSizeIncrease(valueToIncreaseBy);
        }

        public Pointer GetPointerThatPointsFromDataRegion(int pointerFromDataRegion)
        {
            foreach (Pointer p in this.region1Pointers)
            {
                if (p.PointerFromDataRegion == pointerFromDataRegion)
                {
                    return p;
                }
            }

            return null;
        }

        private Label AddLabelToFile(string label)
        {
            int newLabelOffset = this.fileSize;
            Label newLabel = new Label(label, newLabelOffset);
            this.labels.Add(newLabel);

            // Update the fileSize and labelSize to accomodate the new label
            // Make sure to get the length in bytes, not in characters
            // Also accommodate the null terminator
            int labelLength = SHIFT_JIS.GetBytes(label).Length;
            this.fileSize += labelLength + 1;
            this.labelSize += labelLength + 1;

            return newLabel;
        }

        private Pointer AddPointerToFile(int dataOffset, Label labelInFile)
        {
            // First, create the new pointer and add it to the list of region 1 pointers
            int dataOffsetAfterHeader = dataOffset - HEADER_SIZE;
            int labelOffsetAfterHeader = labelInFile.Offset - HEADER_SIZE;
            Pointer newPointer = new Pointer(dataOffsetAfterHeader, labelOffsetAfterHeader, labelInFile);
            this.region1Pointers.Add(newPointer);

            // Update the fileSize, pointerRegion1Size, and pointerCount1 to accomodate the new pointer
            this.fileSize += 4;
            this.pointerRegion1Size += 4;
            this.pointerCount1 += 1;

            // Unfortunately, adding a new pointer pushes every label down, which messes up every offset.
            // Call this method to fix these all up.
            UpdateLabelsAndPointersAfterFileSizeIncrease(4);

            return newPointer;
        }

        private void UpdateLabelsAndPointersAfterFileSizeIncrease(int sizeIncrease)
        {
            // First, let's update every label to be aware of its new location.
            UpdateLabelOffsetsAfterAddingValue(sizeIncrease);

            // Now that every label knows its new location, update every pointer
            // so that the pointerFromDataRegion is accurate.
            UpdatePointersWithCurrentLabelOffsets();

            // Lastly, update the data region to reflect the new pointers
            UpdateDataRegionWithCurrentPointers();
        }

        private void ReadHeader(byte[] binFileBytes)
        {
            byte[] intBuffer = new byte[4];

            Array.Copy(binFileBytes, FILE_SIZE_OFFSET, intBuffer, 0, 4);
            this.fileSize = BitConverter.ToInt32(intBuffer, 0);

            Array.Copy(binFileBytes, DATA_REGION_SIZE_OFFSET, intBuffer, 0, 4);
            this.dataRegionSize = BitConverter.ToInt32(intBuffer, 0);

            Array.Copy(binFileBytes, POINTER_REGION_1_COUNT_OFFSET, intBuffer, 0, 4);
            this.pointerCount1 = BitConverter.ToInt32(intBuffer, 0);
            this.pointerRegion1Size = pointerCount1 * 4;

            Array.Copy(binFileBytes, POINTER_REGION_2_COUNT_OFFSET, intBuffer, 0, 4);
            this.pointerCount2 = BitConverter.ToInt32(intBuffer, 0);
            this.pointerRegion2Size = pointerCount2 * 8;

            // We can determine label region size from the sizes of everything else
            this.labelSize = this.fileSize - (HEADER_SIZE + this.dataRegionSize + this.pointerRegion1Size + this.pointerRegion2Size);
        }

        private void ReadLabels(byte[] binFileByes)
        {
            this.labels = new List<Label>();

            // Read in label text one byte at a time until we hit the null terminator.
            // Keep track of the start of the current label's starting offset as we go.
            int firstLabelOffset = this.fileSize - this.labelSize;
            int currentLabelOffset = 0;
            List<byte> labelByteList = new List<byte>();
            for (int i = firstLabelOffset; i < this.fileSize; i++)
            {
                byte currentByte = binFileByes[i];
                if (currentByte != 0x00)
                {
                    // If this is the first character in the label, set the starting offset
                    // to this character's offset.
                    if (currentLabelOffset == 0)
                    {
                        currentLabelOffset = i;
                    }

                    labelByteList.Add(currentByte);
                }
                else
                {
                    // Reached the end of the current label. Create a new Label object and
                    // reset the variable we use to track the start of the current label.
                    byte[] shiftJisBytes = labelByteList.ToArray();
                    string labelText = SHIFT_JIS.GetString(shiftJisBytes, 0, shiftJisBytes.Length);
                    this.labels.Add(new Label(labelText, currentLabelOffset));
                    labelByteList = new List<byte>();
                    currentLabelOffset = 0;
                }
            }
        }

        private void ReadPointerRegion1(byte[] binFileBytes)
        {
            this.region1Pointers = new List<Pointer>();
            byte[] intBuffer = new byte[4];
            int firstPointerOffset = HEADER_SIZE + this.dataRegionSize;

            for (int currentOffset = firstPointerOffset;
                currentOffset < (firstPointerOffset + this.pointerRegion1Size);
                currentOffset += 4)
            {
                // Each pointer entry points to some entry in the data region, which points
                // to something else. Let's start by getting the pointer to the data region.
                Array.Copy(binFileBytes, currentOffset, intBuffer, 0, 4);
                int pointerIntoDataRegion = BitConverter.ToInt32(intBuffer, 0);

                // Next, let's find out what the data region points to. We have to offset
                // by HEADER_SIZE because the game internally does that on all the pointers.
                Array.Copy(binFileBytes, pointerIntoDataRegion + HEADER_SIZE, intBuffer, 0, 4);
                int pointerFromDataRegion = BitConverter.ToInt32(intBuffer, 0);

                // This tool is meant for adding labels. To make pointer manipulation easier,
                // we need to associate any pointer that is pointing to a label with the Label
                // object itself. Check if the location the data region is pointing at is also
                // the location of a label.
                Label currentPointerLabel = null;
                foreach (Label label in this.labels)
                {
                    if (label.Offset == (pointerFromDataRegion + HEADER_SIZE))
                    {
                        currentPointerLabel = label;
                        break;
                    }
                }

                region1Pointers.Add(new Pointer(pointerIntoDataRegion, pointerFromDataRegion, currentPointerLabel));
            }
        }

        private void ReadPointerRegion2(byte[] binFileBytes)
        {
            this.region2Pointers = new List<Pointer>();
            byte[] intBuffer = new byte[4];
            int firstPointerOffset = HEADER_SIZE + this.dataRegionSize + this.pointerRegion1Size;

            for (int currentOffset = firstPointerOffset;
                currentOffset < (firstPointerOffset + this.pointerRegion2Size);
                currentOffset += 8)
            {
                // Each pointer entry points to some entry in the data region, which points
                // to something else. Let's start by getting the pointer to the data region.
                Array.Copy(binFileBytes, currentOffset, intBuffer, 0, 4);
                int pointerIntoDataRegion = BitConverter.ToInt32(intBuffer, 0);

                // Next, let's find out what the data region points to. We have to offset
                // by HEADER_SIZE because the game internally does that on all the pointers.
                Array.Copy(binFileBytes, pointerIntoDataRegion + HEADER_SIZE, intBuffer, 0, 4);
                int pointerFromDataRegion = BitConverter.ToInt32(intBuffer, 0);

                // Pointer Region 2 pointers actually contain a second pointer to a label.
                // Just try to follow that to find the label. The offset is from the start of
                // the label region.
                Array.Copy(binFileBytes, currentOffset + 4, intBuffer, 0, 4);
                int labelOffsetFromPointer = BitConverter.ToInt32(intBuffer, 0);
                Label currentPointerLabel = null;
                foreach (Label label in this.labels)
                {
                    if (label.Offset == (labelOffsetFromPointer + HEADER_SIZE + this.dataRegionSize + this.pointerRegion1Size + this.pointerRegion2Size))
                    {
                        currentPointerLabel = label;
                        break;
                    }
                }

                region2Pointers.Add(new Pointer(pointerIntoDataRegion, pointerFromDataRegion, currentPointerLabel));
            }
        }

        private byte[] OutputHeaderAsBytes()
        {
            byte[] headerBytes = new byte[HEADER_SIZE];
            byte[] fileSizeBytes = this.ConvertIntToBytes(this.fileSize);
            byte[] dataRegionSizeBytes = this.ConvertIntToBytes(this.dataRegionSize);
            byte[] pointerCount1Bytes = this.ConvertIntToBytes(this.pointerCount1);
            byte[] pointerCount2Bytes = this.ConvertIntToBytes(this.pointerCount2);
            Array.Copy(fileSizeBytes, 0, headerBytes, FILE_SIZE_OFFSET, 4);
            Array.Copy(dataRegionSizeBytes, 0, headerBytes, DATA_REGION_SIZE_OFFSET, 4);
            Array.Copy(pointerCount1Bytes, 0, headerBytes, POINTER_REGION_1_COUNT_OFFSET, 4);
            Array.Copy(pointerCount2Bytes, 0, headerBytes, POINTER_REGION_2_COUNT_OFFSET, 4);
            return headerBytes;
        }

        private byte[] OutputRegion1PointersAsBytes()
        {
            byte[] pointerRegionBytes = new byte[this.pointerRegion1Size];
            for (int i = 0; i < this.pointerCount1; i++)
            {
                byte[] currentPointerBytes = this.ConvertIntToBytes(this.region1Pointers[i].PointerIntoDataRegion);
                Array.Copy(currentPointerBytes, 0, pointerRegionBytes, i * 4, 4);
            }

            return pointerRegionBytes;
        }

        private byte[] OutputRegion2PointersAsBytes()
        {
            byte[] pointerRegionBytes = new byte[this.pointerRegion2Size];
            for (int i = 0; i < this.pointerCount2; i++)
            {
                byte[] currentPointerBytes = this.ConvertIntToBytes(this.region2Pointers[i].PointerIntoDataRegion);
                Array.Copy(currentPointerBytes, 0, pointerRegionBytes, i * 8, 4);

                // get the offset of the pointer's label from the start of the label region
                int labelOffsetToOutput = this.region2Pointers[i].Label.Offset - (HEADER_SIZE + this.dataRegionSize + this.pointerRegion1Size + this.pointerRegion2Size);
                currentPointerBytes = this.ConvertIntToBytes(labelOffsetToOutput);
                Array.Copy(currentPointerBytes, 0, pointerRegionBytes, (i * 8) + 4, 4);
            }

            return pointerRegionBytes;
        }

        private byte[] OutputLabelsAsBytes()
        {
            byte[] labelRegionBytes = new byte[this.labelSize];
            int currentOffset = 0;
            foreach (Label label in this.labels)
            {
                byte[] labelBytes = SHIFT_JIS.GetBytes(label.Text);
                Array.Copy(labelBytes, 0, labelRegionBytes, currentOffset, labelBytes.Length);

                // We need to include the null terminator, since we removed it when we parsed the text out.
                // Since the default value of a byte is 0x00, just tell the offset to skip the next byte.
                currentOffset += (labelBytes.Length + 1);
            }

            return labelRegionBytes;
        }

        private byte[] ConvertIntToBytes(int value)
        {
            byte[] intBytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(intBytes);
            }

            return intBytes;
        }

        private Label GetLabelForString(string labelString)
        {
            foreach (Label label in this.labels)
            {
                if (label.Text == labelString)
                {
                    return label;
                }
            }

            return null;
        }

        private Pointer GetPointerAtOffset(int dataOffset)
        {
            int dataOffsetAfterHeader = dataOffset - HEADER_SIZE;

            foreach (Pointer pointer in this.region1Pointers)
            {
                if (pointer.PointerIntoDataRegion == dataOffsetAfterHeader)
                {
                    return pointer;
                }
            }

            return null;
        }

        private void UpdateLabelOffsetsAfterAddingValue(int value)
        {
            foreach (Label label in this.labels)
            {
                label.Offset += value;
            }
        }

        private void UpdatePointersWithCurrentLabelOffsets()
        {
            foreach (Pointer pointer in this.region1Pointers)
            {
                if (pointer.Label != null)
                {
                    pointer.PointerFromDataRegion = pointer.Label.Offset - HEADER_SIZE;
                }
            }
        }

        private void UpdatePointersAfterOffsetWithIncreasedValue(int valueToIncreaseBy, int offsetWithinDataRegion)
        {
            foreach (Pointer pointer in this.region1Pointers)
            {
                if (pointer.PointerIntoDataRegion > offsetWithinDataRegion)
                {
                    pointer.PointerIntoDataRegion += valueToIncreaseBy;
                    if (pointer.Label == null)
                    {
                        // The pointer from the data region is itself pointing to something within the data region.
                        // If that was moved because of the increased size, change that pointer as well.
                        if (pointer.PointerFromDataRegion > offsetWithinDataRegion)
                        {
                            pointer.PointerFromDataRegion += valueToIncreaseBy;
                        }
                    }
                }
            }

            foreach (Pointer pointer in this.region2Pointers)
            {
                if (pointer.PointerIntoDataRegion > offsetWithinDataRegion)
                {
                    pointer.PointerIntoDataRegion += valueToIncreaseBy;
                    if (pointer.Label == null)
                    {
                        // The pointer from the data region is itself pointing to something within the data region.
                        // If that was moved because of the increased size, change that pointer as well.
                        if (pointer.PointerFromDataRegion > offsetWithinDataRegion)
                        {
                            pointer.PointerFromDataRegion += valueToIncreaseBy;
                        }
                    }
                }
            }
        }

        private void UpdateDataRegionWithCurrentPointers()
        {
            foreach (Pointer pointer in this.region1Pointers)
            {
                byte[] newPointerBytes = ConvertIntToBytes(pointer.PointerFromDataRegion);
                Array.Copy(newPointerBytes, 0, this.dataRegionBytes, pointer.PointerIntoDataRegion, 4);
            }
        }
    }
}
