using System;

namespace AwakeningItemTool
{
    public enum WeaponType
    {
        Sword = 0,
        Lance = 1,
        Axe = 2,
        Bow = 3,
        Tome = 4,
        Staff = 5,
        Dragonstone = 6,
        Beaststone = 7,
        Claw = 8,
        Breath = 9,
        None = 10
    }

    // Brave Axe is erroneously grouped under BasicAxe
    // instead of BraveSlayerMagicOrKillerWeapon like it should.
    public enum WeaponSubType
    {
        Unclassified = 0,
        BasicSword = 1,
        BasicLance = 2,
        ThrowingLance = 3,
        BasicAxe = 4,
        ThrowingAxe = 5,
        BasicBow = 6,
        BasicFire = 7,
        BasicThunder = 8,
        BasicWind = 9,
        BasicDark = 10,
        BraveSlayerMagicOrKillerWeapon = 11
    }

    public enum WeaponStrengthEquivalent
    {
        Unclassified = 0,
        BronzeEquivalent = 1,
        IronEquivalent = 2,
        SteelEquivalent = 3,
        SilverOrBraveEquivalent = 4
    }

    public enum ItemEffectType
    {
        None = 0,
        UseOrStaffRecoversHP = 1,
        HealsAllAlliesInRange = 2,
        StatBooster = 3,
        ArmsScroll = 4,
        SeedOfTrust = 5,
        UseOrStaffTemporarilyIncreasesRes = 6,
        Unused = 7,
        Tonic = 8,
        MasterSeal = 9,
        SecondSeal = 10,
        Hammerne = 11,
        Rescue = 12,
        ReekingBox = 13,
        RiftDoor = 14,
        TeachesSkill = 15,
        DreadScroll = 16,
        WeddingBouquet = 17
    }

    public enum WeaponRank
    {
        None = 0,
        E = 1,
        D = 16,
        C = 36,
        B = 61,
        A = 91
    }

    // The Bolt Axe is only available in Infinite Regalia, but
    // has AvailableInNormalShops for this value for some reason.
    public enum ItemAvailabilityByte1
    {
        UnavailableForPurchase = 0,
        AvailableInNormalShops = 1,
        AvailableInMerchantShops = 2,
        AvailableInInfiniteRegalia = 3
    }

    public class ItemEntry
    {
        private const int BITFLAG_OFFSET = 0x00;
        private const int IID_POINTER_OFFSET = 0x08;
        private const int MIID_POINTER_OFFSET = 0x0C;
        private const int MIID_H_POINTER_OFFSET = 0x10;
        private const int ICON_ID_OFFSET = 0x14;
        private const int WORTH_PER_USE_OFFSET = 0x16;
        private const int WEAPON_TYPE_OFFSET = 0x18;
        private const int WEAPON_SUBTYPE_OFFSET = 0x19;
        private const int WEAPON_STRENGTH_EQUIVALENT_OFFSET = 0x1A;
        private const int ITEM_EFFECT_TYPE_OFFSET = 0x1B;
        private const int WEAPON_RANK_OFFSET = 0x1C;
        private const int DOES_HAVE_WEAPON_RANK_OFFSET = 0x1D;
        private const int STAFF_BASE_EXP_OFFSET = 0x1E;
        private const int NUMBER_OF_USES_OFFSET = 0x1F;
        private const int MIGHT_OFFSET = 0x20;
        private const int ACCURACY_OFFSET = 0x21;
        private const int CRITICAL_OFFSET = 0x22;
        private const int MINIMUM_RANGE_OFFSET = 0x23;
        private const int MAXIMUM_RANGE_OFFSET = 0x24;
        private const int AMOUNT_OF_MOVEMENT_INCREASE_OFFSET = 0x25;
        private const int ITEM_AVAILABILITY_BYTE_1_OFFSET = 0x26; // This might be 2 bytes
        private const int ITEM_AVAILABILITY_BYTE_2_OFFSET = 0x27; // This might be 2 bytes
        private const int EFFECT_PARAMETER_OFFSET = 0x28;
        private const int AMOUNT_OF_STRENGTH_INCREASE_OFFSET = 0x29;
        private const int AMOUNT_OF_MAGIC_INCREASE_OFFSET = 0x2A;
        private const int AMOUNT_OF_SKILL_INCREASE_OFFSET = 0x2B;
        private const int AMOUNT_OF_SPEED_INCREASE_OFFSET = 0x2C;
        private const int AMOUNT_OF_LUCK_INCREASE_OFFSET = 0x2D;
        private const int AMOUNT_OF_DEFENSE_INCREASE_OFFSET = 0x2E;
        private const int AMOUNT_OF_RESISTANCE_INCREASE_OFFSET = 0x2F;
        private const int EFFECTIVENESS_BITFLAG_OFFSET = 0x30;
        private const int ITEM_ID_OFFSET = 0x32;
        private const int UNKNOWN = 0x33; // Might be byte 2 of Item ID?
        private const int POINTER_TO_ITEM_EFFECT_LABEL_OFFSET = 0x34;

        public ItemBitflags ItemBitflags;
        public Pointer ItemLabel;
        public Pointer ItemNameLabel;
        public Pointer ItemDescriptionLabel;
        public UInt16 IconId;
        public UInt16 WorthPerUse;
        public WeaponType WeaponType;
        public WeaponSubType WeaponSubType;
        public WeaponStrengthEquivalent WeaponStrengthEquivalent;
        public ItemEffectType ItemEffectType;
        public WeaponRank WeaponRank;
        public bool DoesHaveWeaponRank;
        public byte StaffBaseExp;
        public byte NumberOfUses;
        public byte Might;
        public byte Accuracy;
        public byte Critical;
        public byte MinimumRange;
        public byte MaximumRange;
        public byte AmountOfMovementIncrease;
        public byte ItemAvailabilityByte1;
        public byte ItemAvailabilityByte2;
        public byte EffectParameter;
        public byte AmountOfStrengthIncrease;
        public byte AmountOfMagicIncrease;
        public byte AmountOfSkillIncrease;
        public byte AmountOfSpeedIncrease;
        public byte AmountOfLuckIncrease;
        public byte AmountOfDefenseIncrease;
        public byte AmountOfResistanceIncrease;
        public EffectivenessBitflags EffectivenessBitflags;
        public byte ItemId;
        public byte Unknown;
        public Pointer PointerToItemEffectLabel;

        public ItemEntry(byte[] itemEntryBytes, BinFileModel model)
        {
            this.ParseBitFlags(itemEntryBytes);
            this.ItemLabel = this.GetPointerAtOffsetWithinEntry(itemEntryBytes, IID_POINTER_OFFSET, model);
            this.ItemNameLabel = this.GetPointerAtOffsetWithinEntry(itemEntryBytes, MIID_POINTER_OFFSET, model);
            this.ItemDescriptionLabel = this.GetPointerAtOffsetWithinEntry(itemEntryBytes, MIID_H_POINTER_OFFSET, model);
            this.IconId = GetUInt16FromByteArrayAtOffset(itemEntryBytes, ICON_ID_OFFSET);
            this.WorthPerUse = GetUInt16FromByteArrayAtOffset(itemEntryBytes, WORTH_PER_USE_OFFSET);
            this.WeaponType = (WeaponType)itemEntryBytes[WEAPON_TYPE_OFFSET];
            this.WeaponSubType = (WeaponSubType)itemEntryBytes[WEAPON_SUBTYPE_OFFSET];
            this.WeaponStrengthEquivalent = (WeaponStrengthEquivalent)itemEntryBytes[WEAPON_STRENGTH_EQUIVALENT_OFFSET];
            this.ItemEffectType = (ItemEffectType)itemEntryBytes[ITEM_EFFECT_TYPE_OFFSET];
            this.WeaponRank = (WeaponRank)itemEntryBytes[WEAPON_RANK_OFFSET];
            this.DoesHaveWeaponRank = itemEntryBytes[DOES_HAVE_WEAPON_RANK_OFFSET] == 0x01;
            this.StaffBaseExp = itemEntryBytes[STAFF_BASE_EXP_OFFSET];
            this.NumberOfUses = itemEntryBytes[NUMBER_OF_USES_OFFSET];
            this.Might = itemEntryBytes[MIGHT_OFFSET];
            this.Accuracy = itemEntryBytes[ACCURACY_OFFSET];
            this.Critical = itemEntryBytes[CRITICAL_OFFSET];
            this.MinimumRange = itemEntryBytes[MINIMUM_RANGE_OFFSET];
            this.MaximumRange = itemEntryBytes[MAXIMUM_RANGE_OFFSET];
            this.AmountOfMovementIncrease = itemEntryBytes[AMOUNT_OF_MOVEMENT_INCREASE_OFFSET];
            this.ItemAvailabilityByte1 = itemEntryBytes[ITEM_AVAILABILITY_BYTE_1_OFFSET];
            this.ItemAvailabilityByte2 = itemEntryBytes[ITEM_AVAILABILITY_BYTE_2_OFFSET];
            this.EffectParameter = itemEntryBytes[EFFECT_PARAMETER_OFFSET];
            this.AmountOfStrengthIncrease = itemEntryBytes[AMOUNT_OF_STRENGTH_INCREASE_OFFSET];
            this.AmountOfMagicIncrease = itemEntryBytes[AMOUNT_OF_MAGIC_INCREASE_OFFSET];
            this.AmountOfSkillIncrease = itemEntryBytes[AMOUNT_OF_SKILL_INCREASE_OFFSET];
            this.AmountOfSpeedIncrease = itemEntryBytes[AMOUNT_OF_SPEED_INCREASE_OFFSET];
            this.AmountOfLuckIncrease = itemEntryBytes[AMOUNT_OF_LUCK_INCREASE_OFFSET];
            this.AmountOfDefenseIncrease = itemEntryBytes[AMOUNT_OF_DEFENSE_INCREASE_OFFSET];
            this.AmountOfResistanceIncrease = itemEntryBytes[AMOUNT_OF_RESISTANCE_INCREASE_OFFSET];
            this.EffectivenessBitflags = new EffectivenessBitflags(itemEntryBytes[EFFECTIVENESS_BITFLAG_OFFSET]);
            this.ItemId = itemEntryBytes[ITEM_ID_OFFSET];
            this.Unknown = itemEntryBytes[UNKNOWN];
            this.PointerToItemEffectLabel = GetPointerAtOffsetWithinEntry(itemEntryBytes, POINTER_TO_ITEM_EFFECT_LABEL_OFFSET, model);
        }

        public byte[] OutputAsByteArray()
        {
            byte[] result = new byte[Constants.ITEM_ENTRY_LENGTH];

            byte[] itemBitflagsBytes = BitConverter.GetBytes(this.ItemBitflags.ConvertBitFlagsToInt());
            Array.Copy(itemBitflagsBytes, 0, result, BITFLAG_OFFSET, 8);

            byte[] itemLabelPointerBytes = BitConverter.GetBytes(this.ItemLabel.PointerFromDataRegion);
            Array.Copy(itemLabelPointerBytes, 0, result, IID_POINTER_OFFSET, 4);

            byte[] itemNameLabelPointerBytes = BitConverter.GetBytes(this.ItemNameLabel.PointerFromDataRegion);
            Array.Copy(itemNameLabelPointerBytes, 0, result, MIID_POINTER_OFFSET, 4);

            byte[] itemDescriptionLabelPointerBytes = BitConverter.GetBytes(this.ItemDescriptionLabel.PointerFromDataRegion);
            Array.Copy(itemDescriptionLabelPointerBytes, 0, result, MIID_H_POINTER_OFFSET, 4);

            byte[] iconIdBytes = BitConverter.GetBytes(this.IconId);
            Array.Copy(iconIdBytes, 0, result, ICON_ID_OFFSET, 2);

            byte[] worthPerUseBytes = BitConverter.GetBytes(this.WorthPerUse);
            Array.Copy(worthPerUseBytes, 0, result, WORTH_PER_USE_OFFSET, 2);

            result[WEAPON_TYPE_OFFSET] = (byte)this.WeaponType;
            result[WEAPON_SUBTYPE_OFFSET] = (byte)this.WeaponSubType;
            result[WEAPON_STRENGTH_EQUIVALENT_OFFSET] = (byte)this.WeaponStrengthEquivalent;
            result[ITEM_EFFECT_TYPE_OFFSET] = (byte)this.ItemEffectType;
            result[WEAPON_RANK_OFFSET] = (byte)this.WeaponRank;
            result[DOES_HAVE_WEAPON_RANK_OFFSET] = (byte)(this.DoesHaveWeaponRank ? 0x01 : 0x00);

            result[STAFF_BASE_EXP_OFFSET] = this.StaffBaseExp;
            result[NUMBER_OF_USES_OFFSET] = this.NumberOfUses;
            result[MIGHT_OFFSET] = this.Might;
            result[ACCURACY_OFFSET] = this.Accuracy;
            result[CRITICAL_OFFSET] = this.Critical;
            result[MINIMUM_RANGE_OFFSET] = this.MinimumRange;
            result[MAXIMUM_RANGE_OFFSET] = this.MaximumRange;
            result[AMOUNT_OF_MOVEMENT_INCREASE_OFFSET] = this.AmountOfMovementIncrease;
            result[ITEM_AVAILABILITY_BYTE_1_OFFSET] = this.ItemAvailabilityByte1;
            result[ITEM_AVAILABILITY_BYTE_2_OFFSET] = this.ItemAvailabilityByte2;
            result[EFFECT_PARAMETER_OFFSET] = this.EffectParameter;
            result[AMOUNT_OF_STRENGTH_INCREASE_OFFSET] = this.AmountOfStrengthIncrease;
            result[AMOUNT_OF_MAGIC_INCREASE_OFFSET] = this.AmountOfMagicIncrease;
            result[AMOUNT_OF_SKILL_INCREASE_OFFSET] = this.AmountOfSkillIncrease;
            result[AMOUNT_OF_SPEED_INCREASE_OFFSET] = this.AmountOfSpeedIncrease;
            result[AMOUNT_OF_LUCK_INCREASE_OFFSET] = this.AmountOfLuckIncrease;
            result[AMOUNT_OF_DEFENSE_INCREASE_OFFSET] = this.AmountOfDefenseIncrease;
            result[AMOUNT_OF_RESISTANCE_INCREASE_OFFSET] = this.AmountOfResistanceIncrease;

            byte[] effectivenessBitflagsBytes = BitConverter.GetBytes(this.EffectivenessBitflags.ConvertBitFlagsToInt());
            Array.Copy(effectivenessBitflagsBytes, 0, result, EFFECTIVENESS_BITFLAG_OFFSET, 2);

            result[ITEM_ID_OFFSET] = this.ItemId;
            result[UNKNOWN] = this.Unknown;

            byte[] pointerToItemEffectLabelBytes = BitConverter.GetBytes(this.PointerToItemEffectLabel.PointerFromDataRegion);
            Array.Copy(pointerToItemEffectLabelBytes, 0, result, POINTER_TO_ITEM_EFFECT_LABEL_OFFSET, 4);

            return result;
        }

        private void ParseBitFlags(byte[] itemEntryBytes)
        {
            byte[] bitflagByteBuffer = new byte[8];
            Array.Copy(itemEntryBytes, BITFLAG_OFFSET, bitflagByteBuffer, 0, 8);
            UInt64 bitflagsInt = BitConverter.ToUInt64(bitflagByteBuffer, 0);
            this.ItemBitflags = new ItemBitflags(bitflagsInt);
        }

        private Pointer GetPointerAtOffsetWithinEntry(byte[] itemEntryBytes, int offsetWithinEntry, BinFileModel model)
        {
            byte[] intBuffer = new byte[4];
            Array.Copy(itemEntryBytes, offsetWithinEntry, intBuffer, 0, 4);
            int labelPointer = BitConverter.ToInt32(intBuffer, 0);
            return model.GetPointerThatPointsFromDataRegion(labelPointer);
        }

        private UInt16 GetUInt16FromByteArrayAtOffset(byte[] byteArray, int offset)
        {
            byte[] intBuffer = new byte[2];
            Array.Copy(byteArray, offset, intBuffer, 0, 2);
            return BitConverter.ToUInt16(intBuffer, 0);
        }

        private UInt32 GetUInt32FromByteArrayAtOffset(byte[] byteArray, int offset)
        {
            byte[] intBuffer = new byte[4];
            Array.Copy(byteArray, offset, intBuffer, 0, 4);
            return BitConverter.ToUInt32(intBuffer, 0);
        }
    }
}
