using System;

namespace AwakeningItemTool
{
    public class ItemBitflags
    {
        [Flags]
        public enum ItemBitflagsEnum : UInt64
        {
            IsUsable = 0x000000000000000001,
            DoesPerformDoubleAttack = 0x000000000000000002,
            IsMagicWeapon = 0x000000000000000004,
            IsLongDistanceWeapon = 0x000000000000000008,
            IsEffectIncreasedByHalfMagicOffset = 0x000000000000000010,
            SpecialEffectIsDisabled = 0x000000000000000020,
            CanOpenChests = 0x000000000000000040,
            CanOpenDoors = 0x000000000000000080,
            PurchaseItemsAtHalfPrice = 0x0000000000000100,
            DiscoverSecretShops = 0x0000000000000200,
            CannotBeSold = 0x0000000000000400,
            CannotTradeStoreOrDiscard = 0x0000000000000800,
            HasInfiniteDurability = 0x0000000000001000,
            IsRegaliaWeapon = 0x0000000000002000,
            DoesRaiseUserStats = 0x0000000000004000,
            DoesRegenerateHp = 0x0000000000008000,
            DoesAbsorbHpFromEnemy = 0x0000000000010000,
            HasAstraSkill = 0x0000000000020000,
            HasSolSkill = 0x0000000000040000,
            HasLunaSkill = 0x0000000000080000,
            HasIgnisSkill = 0x0000000000100000,
            HasVengeanceSkill = 0x0000000000200000,
            HasDespoilSkill = 0x0000000000400000,
            HasSwordbreakerSkill = 0x0000000000800000,
            HasLancebreakerSkill = 0x0000000001000000,
            HasAxebreakerSkill = 0x0000000002000000,
            HasBowbreakerSkill = 0x0000000004000000,
            HasTomebreakerSkill = 0x0000000008000000,
            HasPatienceSkill = 0x0000000010000000,
            HasUnderdogSkill = 0x0000000020000000,
            IsHealingStaff = 0x0000000040000000,
            IsStatusStaff = 0x0000000080000000,
            DoesHealAllInRange = 0x0000000100000000,
            IsFoundItem = 0x0000000200000000,
            ChromOnly = 0x0000000400000000,
            LordOnly = 0x0000000800000000,
            MyrmidonSwordmasterOnly = 0x0000001000000000,
            ArcherSniperOnly = 0x0000002000000000,
            DarkMageSorcererOnly = 0x0000004000000000,
            LucinaOnly = 0x0000008000000000,
            WalhartOnly = 0x0000010000000000,
            OwainOnly = 0x0000020000000000,
            MenOnly = 0x0000040000000000,
            WomenOnly = 0x0000080000000000,
            IsForgedWeapon = 0x0000100000000000,
            CannotBeForged = 0x0000200000000000,
            IsBasicItem = 0x0000400000000000,
            EnemyOnly = 0x0000800000000000,
            DoesSummonToWorldMap = 0x0001000000000000,
            DoesForceAnimationsOff = 0x0002000000000000,
            CannotPurchaseFromStreetpassTeam = 0x0004000000000000,
            IsSupremeEmblem = 0x0008000000000000,
            IsGold = 0x0010000000000000,
            IsReservedDlcItem = 0x0020000000000000,
            DoesPriceIncreaseOnHardOrAbove = 0x0040000000000000,
            Unused1 = 0x0080000000000000,
            Unused2 = 0x0100000000000000,
            Unused3 = 0x0200000000000000,
            Unused4 = 0x0400000000000000,
            Unused5 = 0x0800000000000000,
            Unused6 = 0x1000000000000000,
            Unused7 = 0x2000000000000000,
            Unused8 = 0x4000000000000000,
            Unused9 = 0x8000000000000000,
        }

        private ItemBitflagsEnum itemBitflags;

        #region Public properties to access individual flags

        // True if the item can be used within the "Use" menu
        public bool IsUsable
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsUsable) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsUsable, value); }
        }

        // True if the weapon always performs a double attack (e.g., like a Brave Sword)
        public bool DoesPerformDoubleAttack
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DoesPerformDoubleAttack) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DoesPerformDoubleAttack, value); }
        }

        public bool IsMagicWeapon
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsMagicWeapon) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsMagicWeapon, value); }
        }

        public bool IsLongDistanceWeapon
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsLongDistanceWeapon) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsLongDistanceWeapon, value); }
        }

        public bool IsEffectIncreasedByHalfMagic
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsEffectIncreasedByHalfMagicOffset) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsEffectIncreasedByHalfMagicOffset, value); }
        }

        public bool SpecialEffectIsDisabled
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.SpecialEffectIsDisabled) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.SpecialEffectIsDisabled, value); }
        }

        public bool CanOpenChests
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.CanOpenChests) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.CanOpenChests, value); }
        }

        public bool CanOpenDoors
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.CanOpenDoors) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.CanOpenDoors, value); }
        }

        public bool PurchaseItemsAtHalfPrice
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.PurchaseItemsAtHalfPrice) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.PurchaseItemsAtHalfPrice, value); }
        }

        public bool DiscoverSecretShops
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DiscoverSecretShops) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DiscoverSecretShops, value); }
        }

        public bool CannotBeSold
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.CannotBeSold) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.CannotBeSold, value); }
        }

        public bool CannotTradeStoreOrDiscard
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.CannotTradeStoreOrDiscard) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.CannotTradeStoreOrDiscard, value); }
        }

        // True if the item cannot be broken
        public bool HasInfiniteDurability
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasInfiniteDurability) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasInfiniteDurability, value); }
        }

        public bool IsRegaliaWeapon
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsRegaliaWeapon) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsRegaliaWeapon, value); }
        }

        // True if the item, when equipped, raises the user's stats
        public bool DoesRaiseUserStats
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DoesRaiseUserStats) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DoesRaiseUserStats, value); }
        }

        // True if the item, when equipped, restores HP to the user at the start of their turn
        public bool DoesRegenerateHp
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DoesRegenerateHp) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DoesRegenerateHp, value); }
        }

        // True if the weapon restores HP equivalent to 1/2 of the damage dealt to the enemy
        public bool DoesAbsorbHpFromEnemy
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DoesAbsorbHpFromEnemy) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DoesAbsorbHpFromEnemy, value); }
        }

        public bool HasAstraSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasAstraSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasAstraSkill, value); }
        }

        public bool HasSolSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasSolSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasSolSkill, value); }
        }

        public bool HasLunaSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasLunaSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasLunaSkill, value); }
        }

        public bool HasIgnisSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasIgnisSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasIgnisSkill, value); }
        }

        public bool HasVengeanceSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasVengeanceSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasVengeanceSkill, value); }
        }

        public bool HasDespoilSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasDespoilSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasDespoilSkill, value); }
        }

        public bool HasSwordbreakerSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasSwordbreakerSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasSwordbreakerSkill, value); }
        }

        public bool HasLancebreakerSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasLancebreakerSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasLancebreakerSkill, value); }
        }

        public bool HasAxebreakerSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasAxebreakerSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasAxebreakerSkill, value); }
        }

        public bool HasBowbreakerSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasBowbreakerSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasBowbreakerSkill, value); }
        }

        public bool HasTomebreakerSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasTomebreakerSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasTomebreakerSkill, value); }
        }

        public bool HasPatienceSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasPatienceSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasPatienceSkill, value); }
        }

        public bool HasUnderdogSkill
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.HasUnderdogSkill) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.HasUnderdogSkill, value); }
        }

        public bool IsHealingStaff
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsHealingStaff) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsHealingStaff, value); }
        }

        public bool IsStatusStaff
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsStatusStaff) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsStatusStaff, value); }
        }

        public bool DoesHealAllInRange
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DoesHealAllInRange) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DoesHealAllInRange, value); }
        }

        public bool IsFoundItem
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsFoundItem) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsFoundItem, value); }
        }

        public bool ChromOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.ChromOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.ChromOnly, value); }
        }

        public bool LordOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.LordOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.LordOnly, value); }
        }

        public bool MyrmidonSwordmasterOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.MyrmidonSwordmasterOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.MyrmidonSwordmasterOnly, value); }
        }

        public bool ArcherSniperOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.ArcherSniperOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.ArcherSniperOnly, value); }
        }

        public bool DarkMageSorcererOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DarkMageSorcererOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DarkMageSorcererOnly, value); }
        }

        public bool LucinaOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.LucinaOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.LucinaOnly, value); }
        }

        public bool WalhartOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.WalhartOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.WalhartOnly, value); }
        }

        public bool OwainOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.OwainOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.OwainOnly, value); }
        }

        public bool MenOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.MenOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.MenOnly, value); }
        }

        public bool WomenOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.WomenOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.WomenOnly, value); }
        }

        public bool IsForgedWeapon
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsForgedWeapon) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsForgedWeapon, value); }
        }

        public bool CannotBeForged
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.CannotBeForged) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.CannotBeForged, value); }
        }

        public bool IsBasicItem
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsBasicItem) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsBasicItem, value); }
        }

        public bool EnemyOnly
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.EnemyOnly) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.EnemyOnly, value); }
        }

        public bool DoesSummonToWorldMap
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DoesSummonToWorldMap) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DoesSummonToWorldMap, value); }
        }

        public bool DoesForceAnimationsOff
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DoesForceAnimationsOff) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DoesForceAnimationsOff, value); }
        }

        public bool CannotPurchaseFromStreetpassTeam
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.CannotPurchaseFromStreetpassTeam) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.CannotPurchaseFromStreetpassTeam, value); }
        }

        public bool IsSupremeEmblem
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsSupremeEmblem) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsSupremeEmblem, value); }
        }

        public bool IsGold
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsGold) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsGold, value); }
        }

        public bool IsReservedDlcItem
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.IsReservedDlcItem) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.IsReservedDlcItem, value); }
        }

        public bool DoesPriceIncreaseOnHardOrAbove
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.DoesPriceIncreaseOnHardOrAbove) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.DoesPriceIncreaseOnHardOrAbove, value); }
        }

        public bool Unused1
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.Unused1) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.Unused1, value); }
        }

        public bool Unused2
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.Unused2) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.Unused2, value); }
        }

        public bool Unused3
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.Unused3) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.Unused3, value); }
        }

        public bool Unused4
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.Unused4) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.Unused4, value); }
        }

        public bool Unused5
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.Unused5) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.Unused5, value); }
        }

        public bool Unused6
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.Unused6) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.Unused6, value); }
        }

        public bool Unused7
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.Unused7) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.Unused7, value); }
        }

        public bool Unused8
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.Unused8) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.Unused8, value); }
        }

        public bool Unused9
        {
            get { return (this.itemBitflags & ItemBitflagsEnum.Unused9) != 0; }
            set { this.itemBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.itemBitflags, ItemBitflagsEnum.Unused9, value); }
        }

        #endregion

        public ItemBitflags(UInt64 bitflagsInt)
        {
            this.itemBitflags = (ItemBitflagsEnum)bitflagsInt;
        }

        public UInt64 ConvertBitFlagsToInt()
        {
            return (UInt64)this.itemBitflags;
        }

        private ItemBitflagsEnum SetOrUnsetFlagOnItemBitflagsEnum(ItemBitflagsEnum initialValue, ItemBitflagsEnum flagToChange, bool shouldBeSet)
        {
            if (shouldBeSet)
            {
                return initialValue | flagToChange;
            }
            else
            {
                return initialValue & ~flagToChange;
            }
        }
    }
}
