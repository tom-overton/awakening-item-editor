using System;
using System.Collections.Generic;

namespace AwakeningItemTool
{
    public class ItemModel
    {
        private BinFileModel binFileModel;
        private int itemEntryStartOffset;
        private int itemEntryEndOffset;
        private int itemEntryCount;

        public List<ItemEntry> itemEntries = new List<ItemEntry>();

        public ItemModel(byte[] uncompressedGameDataBytes)
        {
            this.binFileModel = new BinFileModel(uncompressedGameDataBytes);
            FindItemEntryStartAndEndOffset();
            CalculateItemEntryCount();
            ReadItemEntries();
        }

        public byte[] OutputUpdatedGameDataBytes()
        {
            int sizeOfItemBlock = this.itemEntryEndOffset - this.itemEntryStartOffset;
            byte[] updatedItemBlock = new byte[sizeOfItemBlock];
            int currentItemBlockOffset = 0;
            foreach (ItemEntry entry in this.itemEntries)
            {
                byte[] entryBytes = entry.OutputAsByteArray();
                Array.Copy(entryBytes, 0, updatedItemBlock, currentItemBlockOffset, Constants.ITEM_ENTRY_LENGTH);
                currentItemBlockOffset += Constants.ITEM_ENTRY_LENGTH;
            }

            // For now, just inject this into the same spot in the data region
            Array.Copy(updatedItemBlock, 0, this.binFileModel.dataRegionBytes, itemEntryStartOffset, sizeOfItemBlock);
            return this.binFileModel.OutputFileAsBytes();
        }

        private void FindItemEntryStartAndEndOffset()
        {
            foreach (Pointer pointer in this.binFileModel.region2Pointers)
            {
                if (pointer.Label != null && pointer.Label.Text == "ItemData")
                {
                    this.itemEntryStartOffset = pointer.PointerIntoDataRegion;
                }
                else if (pointer.Label != null && pointer.Label.Text == "ItemDataNum")
                {
                    this.itemEntryEndOffset = pointer.PointerIntoDataRegion;
                }
            }
        }

        private void CalculateItemEntryCount()
        {
            this.itemEntryCount = (this.itemEntryEndOffset - this.itemEntryStartOffset) / Constants.ITEM_ENTRY_LENGTH;
        }

        private void ReadItemEntries()
        {
            int currentOffset = this.itemEntryStartOffset;
            for (int i = 0; i < this.itemEntryCount; i++)
            {
                // To make it easier on ourselves, isolate the bytes for the current entry
                byte[] itemEntryBytes = new byte[Constants.ITEM_ENTRY_LENGTH];
                Array.Copy(this.binFileModel.dataRegionBytes, currentOffset, itemEntryBytes, 0, Constants.ITEM_ENTRY_LENGTH);

                ItemEntry entry = new ItemEntry(itemEntryBytes, binFileModel);
                this.itemEntries.Add(entry);

                currentOffset += Constants.ITEM_ENTRY_LENGTH;
            }
        }

    }
}
