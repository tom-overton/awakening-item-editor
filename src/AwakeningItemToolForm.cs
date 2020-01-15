using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AwakeningItemTool
{
    public partial class AwakeningItemToolForm : Form
    {
        private static readonly Encoding SHIFT_JIS = Encoding.GetEncoding("Shift_JIS");

        private FileManager fileManager;
        private ItemModel model;
        private string filename;
        private ItemEntry currentItemEntry = null;
        private Dictionary<WeaponRank, int> weaponRankToSelectedIndex = new Dictionary<WeaponRank, int>();

        public AwakeningItemToolForm()
        {
            InitializeComponent();
            this.InitializeWeaponTypeComboBox();
            this.InitializeWeaponRankComboBox();
            this.InitializeItemEffectComboBox();
            this.InitializeWeaponSubtypeComboBox();
            this.InitializeWeaponStrengthComboBox();
        }

        #region Initialization Methods

        private void InitializeWeaponTypeComboBox()
        {
            foreach (var value in Enum.GetValues(typeof(WeaponType)))
            {
                string comboBoxItem = ((int)value).ToString("X2") + " - " + value.ToString();
                this.weaponTypeComboBox.Items.Add(comboBoxItem);
            }
        }

        private void InitializeWeaponRankComboBox()
        {
            int currentIndex = 0;
            foreach (var value in Enum.GetValues(typeof(WeaponRank)))
            {
                string comboBoxItem = ((int)value).ToString() + " - " + value.ToString();
                this.weaponRankComboBox.Items.Add(comboBoxItem);
                weaponRankToSelectedIndex.Add((WeaponRank)value, currentIndex);
                currentIndex++;
            }
        }

        private void InitializeItemEffectComboBox()
        {
            this.itemEffectTypeComboBox.Items.Add("0x00 - None");
            this.itemEffectTypeComboBox.Items.Add("0x01 - Use/Staff Recovers HP");
            this.itemEffectTypeComboBox.Items.Add("0x02 - Heals All Allies In Range");
            this.itemEffectTypeComboBox.Items.Add("0x03 - Stat Booster");
            this.itemEffectTypeComboBox.Items.Add("0x04 - Arms Scroll");
            this.itemEffectTypeComboBox.Items.Add("0x05 - Seed of Trust");
            this.itemEffectTypeComboBox.Items.Add("0x06 - Use/Staff Increases Res");
            this.itemEffectTypeComboBox.Items.Add("0x07 - Unused");
            this.itemEffectTypeComboBox.Items.Add("0x08 - Tonic");
            this.itemEffectTypeComboBox.Items.Add("0x09 - Master Seal");
            this.itemEffectTypeComboBox.Items.Add("0x0A - Second Seal");
            this.itemEffectTypeComboBox.Items.Add("0x0B - Hammerne");
            this.itemEffectTypeComboBox.Items.Add("0x0C - Rescue");
            this.itemEffectTypeComboBox.Items.Add("0x0D - Reeking Box");
            this.itemEffectTypeComboBox.Items.Add("0x0E - Rift Door");
            this.itemEffectTypeComboBox.Items.Add("0x0F - Teaches Skill");
            this.itemEffectTypeComboBox.Items.Add("0x10 - Dread Scroll");
            this.itemEffectTypeComboBox.Items.Add("0x11 - Wedding Bouquet");
        }

        private void InitializeWeaponSubtypeComboBox()
        {
            this.weaponSubtypeComboBox.Items.Add("0x00 - Unclassified");
            this.weaponSubtypeComboBox.Items.Add("0x01 - Basic Sword");
            this.weaponSubtypeComboBox.Items.Add("0x02 - Basic Lance");
            this.weaponSubtypeComboBox.Items.Add("0x03 - Throwing Lance");
            this.weaponSubtypeComboBox.Items.Add("0x04 - Basic Axe");
            this.weaponSubtypeComboBox.Items.Add("0x05 - Throwing Axe");
            this.weaponSubtypeComboBox.Items.Add("0x06 - Basic Bow");
            this.weaponSubtypeComboBox.Items.Add("0x07 - Basic Fire");
            this.weaponSubtypeComboBox.Items.Add("0x08 - Basic Thunder");
            this.weaponSubtypeComboBox.Items.Add("0x09 - Basic Wind");
            this.weaponSubtypeComboBox.Items.Add("0x0A - Basic Dark");
            this.weaponSubtypeComboBox.Items.Add("0x0B - Brave/Slayer/Magic/Killer");
        }

        private void InitializeWeaponStrengthComboBox()
        {
            this.weaponStrengthComboBox.Items.Add("0x00 - Unclassified");
            this.weaponStrengthComboBox.Items.Add("0x01 - Bronze Equivalent");
            this.weaponStrengthComboBox.Items.Add("0x02 - Iron Equivalent");
            this.weaponStrengthComboBox.Items.Add("0x03 - Steel Equivalent");
            this.weaponStrengthComboBox.Items.Add("0x04 - Silver/Brave Equivalent");
        }

        #endregion

        private void openGameDatabinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openGameDataDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.filename = openGameDataDialog.FileName;
                try
                {
                    this.fileManager = new FileManager(filename);
                    this.model = new ItemModel(this.fileManager.DecompressedBytes);
                    for (int i = 0; i < this.model.itemEntries.Count; i++)
                    {
                        itemSelectorComboBox.Items.Add(ItemNameUtility.GetNameForItemByIid(this.model.itemEntries[i].ItemLabel.Label.Text));
                    }

                    this.EnableControlsUponGameDataLoad();

                    // Show the "None" item upon first load.
                    itemSelectorComboBox.SelectedIndex = 0;

                    // Enable the Save GameData.bin option now that we have a model to save
                    saveGameDatabinToolStripMenuItem.Enabled = true;

                    // Based on whether the input file is compressed or not, set the filter on the
                    // save GameData dialog to save the file as either .bin.lz or .bin
                    if (this.fileManager.IsCompressedInputFile)
                    {
                        this.saveGameDataDialog.Filter = "GameData file|*.bin.lz";
                    }
                    else
                    {
                        this.saveGameDataDialog.Filter = "Uncompressed GameData file|*.bin";
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorBox("An error occurred while opening the GameData.bin file: \n" + ex.Message);
                }
            }
        }

        private void ShowErrorBox(string message)
        {
            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void saveGameDatabinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveGameDataDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string outputFilename = saveGameDataDialog.FileName;
                byte[] outputBytes = this.model.OutputUpdatedGameDataBytes();
                this.fileManager.WriteToFile(outputBytes, outputFilename);
            }
        }

        private void itemSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.currentItemEntry = this.model.itemEntries[itemSelectorComboBox.SelectedIndex];

            // Item Bitflags
            this.InitializeItemBitflagCheckboxes();

            // Labels and IDs
            this.itemLabelTextBox.Text = currentItemEntry.ItemLabel.Label.Text;
            this.itemNameLabelTextBox.Text = currentItemEntry.ItemNameLabel.Label.Text;
            this.itemDescriptionLabelTextBox.Text = currentItemEntry.ItemDescriptionLabel.Label.Text;
            this.itemIdTextBox.Text = "0x" + currentItemEntry.ItemId.ToString("X2");
            this.iconIdTextBox.Text = "0x" + currentItemEntry.IconId.ToString("X4");

            // Worth and Uses
            this.worthPerUseTextBox.Text = currentItemEntry.WorthPerUse.ToString();
            this.numberOfUsesTextBox.Text = currentItemEntry.NumberOfUses.ToString();
            this.worthTextBox.Text = (currentItemEntry.NumberOfUses * currentItemEntry.WorthPerUse).ToString();

            // Combat Parameters
            this.weaponTypeComboBox.SelectedIndex = (int)currentItemEntry.WeaponType;
            this.EnableOrDisableCombatParameters(currentItemEntry.WeaponType != WeaponType.None);
            this.weaponRankComboBox.Enabled = currentItemEntry.DoesHaveWeaponRank;
            this.weaponRankComboBox.SelectedIndex = this.weaponRankToSelectedIndex[currentItemEntry.WeaponRank];
            this.mightTextBox.Text = currentItemEntry.Might.ToString();
            this.accuracyTextBox.Text = currentItemEntry.Accuracy.ToString();
            this.criticalTextBox.Text = currentItemEntry.Critical.ToString();
            this.minimumRangeTextBox.Text = currentItemEntry.MinimumRange.ToString();
            this.maximumRangeTextBox.Text = currentItemEntry.MaximumRange.ToString();
            this.fliersCheckbox.Checked = currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstFliers;
            this.dragonsCheckbox.Checked = currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstDragons;
            this.beastsCheckbox.Checked = currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstBeasts;
            this.monstersCheckbox.Checked = currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstMonsters;
            this.armorsCheckbox.Checked = currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstArmors;
            this.fellDragonsCheckbox.Checked = currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstFellDragons;

            // Item Effects
            this.itemEffectTypeComboBox.SelectedIndex = (int)currentItemEntry.ItemEffectType;
            this.itemEffectLabelTextBox.Text = currentItemEntry.PointerToItemEffectLabel.Label != null ? currentItemEntry.PointerToItemEffectLabel.Label.Text : "NULL";
            this.UpdateEffectParameterLabel();
            this.SetEffectParameterTextBox();
            this.strIncreaseTextBox.Text = currentItemEntry.AmountOfStrengthIncrease.ToString();
            this.magIncreaseTextBox.Text = currentItemEntry.AmountOfMagicIncrease.ToString();
            this.sklIncreaseTextBox.Text = currentItemEntry.AmountOfSkillIncrease.ToString();
            this.spdIncreaseTextBox.Text = currentItemEntry.AmountOfSpeedIncrease.ToString();
            this.lckIncreaseTextBox.Text = currentItemEntry.AmountOfLuckIncrease.ToString();
            this.defIncreaseTextBox.Text = currentItemEntry.AmountOfDefenseIncrease.ToString();
            this.resIncreaseTextBox.Text = currentItemEntry.AmountOfResistanceIncrease.ToString();
            this.movIncreaseTextBox.Text = currentItemEntry.AmountOfMovementIncrease.ToString();

            // Misc.
            this.weaponSubtypeComboBox.SelectedIndex = (int)currentItemEntry.WeaponSubType;
            this.weaponStrengthComboBox.SelectedIndex = (int)currentItemEntry.WeaponStrengthEquivalent;
            this.staffBaseExpTextBox.Text = currentItemEntry.StaffBaseExp.ToString();
            this.itemAvailabilityByte1TextBox.Text = "0x" + currentItemEntry.ItemAvailabilityByte1.ToString("X2");
            this.itemAvailabilityByte2TextBox.Text = "0x" + currentItemEntry.ItemAvailabilityByte2.ToString("X2");
            this.unknownTextBox.Text = "0x" + currentItemEntry.Unknown.ToString("X2");

            // View as Hex
            this.UpdateHexViewOfItem();
        }

        #region Methods to Enable/Disable Controls

        private void EnableControlsUponGameDataLoad()
        {
            // Item Bitflags
            this.isUsableFlagCheckbox.Enabled = true;
            this.doesPerformDoubleAttackFlagCheckbox.Enabled = true;
            this.isMagicWeaponFlagCheckbox.Enabled = true;
            this.isLongDistanceWeaponCheckbox.Enabled = true;
            this.effectIncreasedByHalfMagicFlagCheckbox.Enabled = true;
            this.specialEffectIsDisabledFlagCheckbox.Enabled = true;
            this.canOpenChestsFlagCheckbox.Enabled = true;
            this.canOpenDoorsFlagCheckbox.Enabled = true;
            this.purchaseItemsAtHalfPriceFlagCheckbox.Enabled = true;
            this.discoverSecretShopsFlagCheckbox.Enabled = true;
            this.cannotBeSoldFlagCheckbox.Enabled = true;
            this.cannotTradeStoreDiscardFlagCheckbox.Enabled = true;
            this.infiniteDurabilityFlagCheckbox.Enabled = true;
            this.isRegaliaWeaponFlagCheckbox.Enabled = true;
            this.doesRaiseUserStatsFlagCheckbox.Enabled = true;
            this.doesRegenerateHpFlagCheckbox.Enabled = true;
            this.absorbsHpFlagCheckbox.Enabled = true;
            this.hasAstraSkillFlagCheckbox.Enabled = true;
            this.hasSolSkillFlagCheckbox.Enabled = true;
            this.hasLunaSkillFlagCheckbox.Enabled = true;
            this.hasIgnisSkillFlagCheckbox.Enabled = true;
            this.hasVengeanceSkillFlagCheckbox.Enabled = true;
            this.hasDespoilSkillFlagCheckbox.Enabled = true;
            this.hasSwordbreakerSkillFlagCheckbox.Enabled = true;
            this.hasLancebreakerSkillFlagCheckbox.Enabled = true;
            this.hasAxebreakerSkillFlagCheckbox.Enabled = true;
            this.hasBowbreakerSkillFlagCheckbox.Enabled = true;
            this.hasTomebreakerSkillFlagCheckbox.Enabled = true;
            this.hasPatienceSkillFlagCheckbox.Enabled = true;
            this.hasUnderdogSkillFlagCheckbox.Enabled = true;
            this.isHealingStaffFlagCheckbox.Enabled = true;
            this.isStatusStaffFlagCheckbox.Enabled = true;
            this.healsAllInRangeFlagCheckbox.Enabled = true;
            this.isFoundItemFlagCheckbox.Enabled = true;
            this.chromOnlyFlagCheckbox.Enabled = true;
            this.lordOnlyFlagCheckbox.Enabled = true;
            this.myrmidonSwordmasterOnlyFlagCheckbox.Enabled = true;
            this.archerSniperOnlyFlagCheckbox.Enabled = true;
            this.darkMageSorcererOnlyFlagCheckbox.Enabled = true;
            this.lucinaOnlyFlagCheckbox.Enabled = true;
            this.walhartOnlyFlagCheckbox.Enabled = true;
            this.owainOnlyFlagCheckbox.Enabled = true;
            this.menOnlyFlagCheckbox.Enabled = true;
            this.womenOnlyFlagCheckbox.Enabled = true;
            this.isForgedWeaponFlagCheckbox.Enabled = true;
            this.cannotBeForgedFlagCheckbox.Enabled = true;
            this.isBasicItemFlagCheckbox.Enabled = true;
            this.enemyOnlyFlagCheckbox.Enabled = true;
            this.doesSummonToWorldMapFlagCheckbox.Enabled = true;
            this.forcesAnimationsOffFlagCheckbox.Enabled = true;
            this.cannotBuyFromStreetpassFlagCheckbox.Enabled = true;
            this.isSupremeEmblemFlagCheckbox.Enabled = true;
            this.isGoldFlagCheckbox.Enabled = true;
            this.isDlcItemFlagCheckbox.Enabled = true;
            this.priceIncreasesOnHardFlagCheckbox.Enabled = true;
            this.unused1FlagCheckbox.Enabled = true;
            this.unused2FlagCheckbox.Enabled = true;
            this.unused3FlagCheckbox.Enabled = true;
            this.unused4FlagCheckbox.Enabled = true;
            this.unused5FlagCheckbox.Enabled = true;
            this.unused6FlagCheckbox.Enabled = true;
            this.unused7FlagCheckbox.Enabled = true;
            this.unused8FlagCheckbox.Enabled = true;
            this.unused9FlagCheckbox.Enabled = true;

            // Labels and IDs
            this.itemIdTextBox.Enabled = true;
            this.iconIdTextBox.Enabled = true;

            // Worth and Uses
            this.worthPerUseTextBox.Enabled = true;
            this.numberOfUsesTextBox.Enabled = true;

            // Combat Parameters
            this.weaponTypeComboBox.Enabled = true;

            // Item Effects
            this.itemEffectTypeComboBox.Enabled = true;
            this.effectParameterTextBox.Enabled = true;
            this.strIncreaseTextBox.Enabled = true;
            this.magIncreaseTextBox.Enabled = true;
            this.sklIncreaseTextBox.Enabled = true;
            this.spdIncreaseTextBox.Enabled = true;
            this.lckIncreaseTextBox.Enabled = true;
            this.defIncreaseTextBox.Enabled = true;
            this.resIncreaseTextBox.Enabled = true;
            this.movIncreaseTextBox.Enabled = true;

            // Misc
            this.weaponSubtypeComboBox.Enabled = true;
            this.weaponStrengthComboBox.Enabled = true;
            this.staffBaseExpTextBox.Enabled = true;
            this.itemAvailabilityByte1TextBox.Enabled = true;
            this.itemAvailabilityByte2TextBox.Enabled = true;
            this.unknownTextBox.Enabled = true;
        }

        private void EnableOrDisableCombatParameters(bool shouldEnable)
        {
            this.weaponRankComboBox.Enabled = shouldEnable;
            this.weaponRankComboBox.Enabled = shouldEnable;
            this.mightTextBox.Enabled = shouldEnable;
            this.accuracyTextBox.Enabled = shouldEnable;
            this.criticalTextBox.Enabled = shouldEnable;
            this.minimumRangeTextBox.Enabled = shouldEnable;
            this.maximumRangeTextBox.Enabled = shouldEnable;
            this.fliersCheckbox.Enabled = shouldEnable;
            this.dragonsCheckbox.Enabled = shouldEnable;
            this.beastsCheckbox.Enabled = shouldEnable;
            this.monstersCheckbox.Enabled = shouldEnable;
            this.armorsCheckbox.Enabled = shouldEnable;
            this.fellDragonsCheckbox.Enabled = shouldEnable;
        }

        #endregion

        private void InitializeItemBitflagCheckboxes()
        {
            this.isUsableFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsUsable;
            this.doesPerformDoubleAttackFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DoesPerformDoubleAttack;
            this.isMagicWeaponFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsMagicWeapon;
            this.isLongDistanceWeaponCheckbox.Checked = currentItemEntry.ItemBitflags.IsLongDistanceWeapon;
            this.effectIncreasedByHalfMagicFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsEffectIncreasedByHalfMagic;
            this.specialEffectIsDisabledFlagCheckbox.Checked = currentItemEntry.ItemBitflags.SpecialEffectIsDisabled;
            this.canOpenChestsFlagCheckbox.Checked = currentItemEntry.ItemBitflags.CanOpenChests;
            this.canOpenDoorsFlagCheckbox.Checked = currentItemEntry.ItemBitflags.CanOpenDoors;
            this.purchaseItemsAtHalfPriceFlagCheckbox.Checked = currentItemEntry.ItemBitflags.PurchaseItemsAtHalfPrice;
            this.discoverSecretShopsFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DiscoverSecretShops;
            this.cannotBeSoldFlagCheckbox.Checked = currentItemEntry.ItemBitflags.CannotBeSold;
            this.cannotTradeStoreDiscardFlagCheckbox.Checked = currentItemEntry.ItemBitflags.CannotTradeStoreOrDiscard;
            this.infiniteDurabilityFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasInfiniteDurability;
            this.isRegaliaWeaponFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsRegaliaWeapon;
            this.doesRaiseUserStatsFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DoesRaiseUserStats;
            this.doesRegenerateHpFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DoesRegenerateHp;
            this.absorbsHpFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DoesAbsorbHpFromEnemy;
            this.hasAstraSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasAstraSkill;
            this.hasSolSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasSolSkill;
            this.hasLunaSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasLunaSkill;
            this.hasIgnisSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasIgnisSkill;
            this.hasVengeanceSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasVengeanceSkill;
            this.hasDespoilSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasDespoilSkill;
            this.hasSwordbreakerSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasSwordbreakerSkill;
            this.hasLancebreakerSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasLancebreakerSkill;
            this.hasAxebreakerSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasAxebreakerSkill;
            this.hasBowbreakerSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasBowbreakerSkill;
            this.hasTomebreakerSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasTomebreakerSkill;
            this.hasPatienceSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasPatienceSkill;
            this.hasUnderdogSkillFlagCheckbox.Checked = currentItemEntry.ItemBitflags.HasUnderdogSkill;
            this.isHealingStaffFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsHealingStaff;
            this.isStatusStaffFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsStatusStaff;
            this.healsAllInRangeFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DoesHealAllInRange;
            this.isFoundItemFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsFoundItem;
            this.chromOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.ChromOnly;
            this.lordOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.LordOnly;
            this.myrmidonSwordmasterOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.MyrmidonSwordmasterOnly;
            this.archerSniperOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.ArcherSniperOnly;
            this.darkMageSorcererOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DarkMageSorcererOnly;
            this.lucinaOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.LucinaOnly;
            this.walhartOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.WalhartOnly;
            this.owainOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.OwainOnly;
            this.menOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.MenOnly;
            this.womenOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.WomenOnly;
            this.isForgedWeaponFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsForgedWeapon;
            this.cannotBeForgedFlagCheckbox.Checked = currentItemEntry.ItemBitflags.CannotBeForged;
            this.isBasicItemFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsBasicItem;
            this.enemyOnlyFlagCheckbox.Checked = currentItemEntry.ItemBitflags.EnemyOnly;
            this.doesSummonToWorldMapFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DoesSummonToWorldMap;
            this.forcesAnimationsOffFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DoesForceAnimationsOff;
            this.cannotBuyFromStreetpassFlagCheckbox.Checked = currentItemEntry.ItemBitflags.CannotPurchaseFromStreetpassTeam;
            this.isSupremeEmblemFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsSupremeEmblem;
            this.isGoldFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsGold;
            this.isDlcItemFlagCheckbox.Checked = currentItemEntry.ItemBitflags.IsReservedDlcItem;
            this.priceIncreasesOnHardFlagCheckbox.Checked = currentItemEntry.ItemBitflags.DoesPriceIncreaseOnHardOrAbove;
            this.unused1FlagCheckbox.Checked = currentItemEntry.ItemBitflags.Unused1;
            this.unused2FlagCheckbox.Checked = currentItemEntry.ItemBitflags.Unused2;
            this.unused3FlagCheckbox.Checked = currentItemEntry.ItemBitflags.Unused3;
            this.unused4FlagCheckbox.Checked = currentItemEntry.ItemBitflags.Unused4;
            this.unused5FlagCheckbox.Checked = currentItemEntry.ItemBitflags.Unused5;
            this.unused6FlagCheckbox.Checked = currentItemEntry.ItemBitflags.Unused6;
            this.unused7FlagCheckbox.Checked = currentItemEntry.ItemBitflags.Unused7;
            this.unused8FlagCheckbox.Checked = currentItemEntry.ItemBitflags.Unused8;
            this.unused9FlagCheckbox.Checked = currentItemEntry.ItemBitflags.Unused9;
        }

        private void UpdateEffectParameterLabel()
        {
            // Hide all labels, then show the correct one
            this.hpIncreaseLabel.Visible = false;
            this.hpRestoredLabel.Visible = false;
            this.skillIdToLearnLabel.Visible = false;

            if (this.currentItemEntry.ItemEffectType == ItemEffectType.UseOrStaffRecoversHP ||
                this.currentItemEntry.ItemEffectType == ItemEffectType.HealsAllAlliesInRange ||
                this.currentItemEntry.ItemBitflags.DoesRegenerateHp)
            {
                this.hpRestoredLabel.Visible = true;
            }
            else if (this.currentItemEntry.ItemEffectType == ItemEffectType.TeachesSkill)
            {
                this.skillIdToLearnLabel.Visible = true;
            }
            else
            {
                this.hpIncreaseLabel.Visible = true;
            }
        }

        private void SetEffectParameterTextBox()
        {
            if (currentItemEntry.ItemEffectType == ItemEffectType.TeachesSkill)
            {
                this.effectParameterTextBox.Text = "0x" + currentItemEntry.EffectParameter.ToString("X2");
            }
            else
            {
                this.effectParameterTextBox.Text = currentItemEntry.EffectParameter.ToString("");
            }
        }

        private void UpdateHexViewOfItem()
        {
            byte[] itemBytes = currentItemEntry.OutputAsByteArray();
            StringBuilder hexStringBuilder = new StringBuilder();
            hexStringBuilder.AppendFormat("{0:X2}", itemBytes[0]);
            for (int i = 1; i < itemBytes.Length; i++)
            {
                hexStringBuilder.Append(" ");
                hexStringBuilder.AppendFormat("{0:X2}", itemBytes[i]);
            }

            this.hexViewTextBox.Text = hexStringBuilder.ToString();
        }

        private void ValidateAndSetUnsignedByte(TextBox changedTextBox, ref byte byteToChange)
        {
            string value = changedTextBox.Text;
            byte inputByte;
            try
            {
                if (value.StartsWith("0x"))
                {
                    string cleanedValue = value.Remove(0, 2);
                    inputByte = Convert.ToByte(cleanedValue, 16);
                }
                else
                {
                    inputByte = Convert.ToByte(value);
                }
            }
            catch
            {
               changedTextBox.BackColor = Color.FromArgb(255, 128, 128);
               saveGameDatabinToolStripMenuItem.Enabled = false;
               return;
            }

            byteToChange = inputByte;
            changedTextBox.BackColor = Color.White;
            saveGameDatabinToolStripMenuItem.Enabled = true;
        }

        private void ValidateAndSetUInt16(TextBox changedTextBox, ref UInt16 byteToChange)
        {
            string value = changedTextBox.Text;
            ushort inputShort;
            try
            {
                if (value.StartsWith("0x"))
                {
                    string cleanedValue = value.Remove(0, 2);
                    inputShort = Convert.ToUInt16(cleanedValue, 16);
                }
                else
                {
                    inputShort = Convert.ToUInt16(value);
                }
            }
            catch
            {
                changedTextBox.BackColor = Color.FromArgb(255, 128, 128);
                saveGameDatabinToolStripMenuItem.Enabled = false;
                return;
            }

            byteToChange = inputShort;
            changedTextBox.BackColor = Color.White;
            saveGameDatabinToolStripMenuItem.Enabled = true;
        }

        #region Item Bitflag Checkbox Callbacks

        private void isUsableFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsUsable = this.isUsableFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void doesPerformDoubleAttackFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DoesPerformDoubleAttack = this.doesPerformDoubleAttackFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isMagicWeaponFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsMagicWeapon = this.isMagicWeaponFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isLongDistanceWeaponCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsLongDistanceWeapon = this.isLongDistanceWeaponCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void effectIncreasedByHalfMagicFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsEffectIncreasedByHalfMagic = this.effectIncreasedByHalfMagicFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void specialEffectIsDisabledFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.SpecialEffectIsDisabled = this.specialEffectIsDisabledFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void canOpenChestsFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.CanOpenChests = this.canOpenChestsFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void canOpenDoorsFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.CanOpenDoors = this.canOpenDoorsFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void purchaseItemsAtHalfPriceFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.PurchaseItemsAtHalfPrice = this.purchaseItemsAtHalfPriceFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void discoverSecretShopsFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DiscoverSecretShops = this.discoverSecretShopsFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void cannotBeSoldFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.CannotBeSold = this.cannotBeSoldFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void cannotTradeStoreDiscardFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.CannotTradeStoreOrDiscard = this.cannotTradeStoreDiscardFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void infiniteDurabilityFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasInfiniteDurability = this.infiniteDurabilityFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isRegaliaWeaponFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsRegaliaWeapon = this.isRegaliaWeaponFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void doesRaiseUserStatsFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DoesRaiseUserStats = this.doesRaiseUserStatsFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void doesRegenerateHpFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DoesRegenerateHp = this.doesRegenerateHpFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void absorbsHpFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DoesAbsorbHpFromEnemy = this.absorbsHpFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasAstraSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasAstraSkill = this.hasAstraSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasSolSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasSolSkill = this.hasSolSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasLunaSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasLunaSkill = this.hasLunaSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasIgnisSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasIgnisSkill = this.hasIgnisSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasVengeanceSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasVengeanceSkill = this.hasVengeanceSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasDespoilSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasDespoilSkill = this.hasDespoilSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasSwordbreakerSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasSwordbreakerSkill = this.hasSwordbreakerSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasLancebreakerSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasLancebreakerSkill = this.hasLancebreakerSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasAxebreakerSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasAxebreakerSkill = this.hasAxebreakerSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasBowbreakerSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasBowbreakerSkill = this.hasBowbreakerSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasTomebreakerSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasTomebreakerSkill = this.hasTomebreakerSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasPatienceSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasPatienceSkill = this.hasPatienceSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void hasUnderdogSkillFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.HasUnderdogSkill = this.hasUnderdogSkillFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isHealingStaffFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsHealingStaff = this.isHealingStaffFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isStatusStaffFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsStatusStaff = this.isStatusStaffFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void healsAllInRangeFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DoesHealAllInRange = this.healsAllInRangeFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isFoundItemFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsFoundItem = this.isFoundItemFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void chromOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.ChromOnly = this.chromOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void lordOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.LordOnly = this.lordOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void myrmidonSwordmasterOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.MyrmidonSwordmasterOnly = this.myrmidonSwordmasterOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void archerSniperOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.ArcherSniperOnly = this.archerSniperOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void darkMageSorcererOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DarkMageSorcererOnly = this.darkMageSorcererOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void lucinaOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.LucinaOnly = this.lucinaOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void walhartOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.WalhartOnly = this.walhartOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void owainOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.OwainOnly = this.owainOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void menOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.MenOnly = this.menOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void womenOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.WomenOnly = this.womenOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isForgedWeaponFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsForgedWeapon = this.isForgedWeaponFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void cannotBeForgedFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.CannotBeForged = this.cannotBeForgedFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isBasicItemFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsBasicItem = this.isBasicItemFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void enemyOnlyFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.EnemyOnly = this.enemyOnlyFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void doesSummonToWorldMapFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DoesSummonToWorldMap = this.doesSummonToWorldMapFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void forcesAnimationsOffFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DoesForceAnimationsOff = this.forcesAnimationsOffFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void cannotBuyFromStreetpassFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.CannotPurchaseFromStreetpassTeam = this.cannotBuyFromStreetpassFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isSupremeEmblemFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsSupremeEmblem = this.isSupremeEmblemFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isGoldFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsGold = this.isGoldFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void isDlcItemFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.IsReservedDlcItem = this.isDlcItemFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void priceIncreasesOnHardFlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.DoesPriceIncreaseOnHardOrAbove = this.priceIncreasesOnHardFlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void unused1FlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.Unused1 = this.unused1FlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void unused2FlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.Unused2 = this.unused2FlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void unused3FlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.Unused3 = this.unused3FlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void unused4FlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.Unused4 = this.unused4FlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void unused5FlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.Unused5 = this.unused5FlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void unused6FlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.Unused6 = this.unused6FlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void unused7FlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.Unused7 = this.unused7FlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void unused8FlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.Unused8 = this.unused8FlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void unused9FlagCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemBitflags.Unused9 = this.unused9FlagCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        #endregion

        #region Labels and IDs Callbacks
        private void itemIdTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.itemIdTextBox, ref currentItemEntry.ItemId);
            this.UpdateHexViewOfItem();
        }

        private void iconIdTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUInt16(this.iconIdTextBox, ref currentItemEntry.IconId);
            this.UpdateHexViewOfItem();
        }

        #endregion

        #region Worth and Uses Control Callbacks

        private void worthPerUseTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUInt16(this.worthPerUseTextBox, ref currentItemEntry.WorthPerUse);
            this.worthTextBox.Text = (currentItemEntry.NumberOfUses * currentItemEntry.WorthPerUse).ToString();
            this.UpdateHexViewOfItem();
        }

        private void numberOfUsesTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.numberOfUsesTextBox, ref currentItemEntry.NumberOfUses);
            this.worthTextBox.Text = (currentItemEntry.NumberOfUses * currentItemEntry.WorthPerUse).ToString();
            this.UpdateHexViewOfItem();
        }

        #endregion

        #region Combat Parameters Control Callbacks

        private void weaponTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentItemEntry.WeaponType = (WeaponType)this.weaponTypeComboBox.SelectedIndex;
            this.EnableOrDisableCombatParameters(currentItemEntry.WeaponType != WeaponType.None);
            this.UpdateHexViewOfItem();
        }

        private void weaponRankComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentItemEntry.WeaponRank = (WeaponRank)Enum.GetValues(typeof(WeaponRank)).GetValue(this.weaponRankComboBox.SelectedIndex);
            currentItemEntry.DoesHaveWeaponRank = currentItemEntry.WeaponRank != WeaponRank.None;
            this.UpdateHexViewOfItem();
        }

        private void mightTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.mightTextBox, ref currentItemEntry.Might);
            this.UpdateHexViewOfItem();
        }

        private void accuracyTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.accuracyTextBox, ref currentItemEntry.Accuracy);
            this.UpdateHexViewOfItem();
        }

        private void criticalTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.criticalTextBox, ref currentItemEntry.Critical);
            this.UpdateHexViewOfItem();
        }

        private void minimumRangeTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.minimumRangeTextBox, ref currentItemEntry.MinimumRange);
            this.UpdateHexViewOfItem();
        }

        private void maximumRangeTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.maximumRangeTextBox, ref currentItemEntry.MaximumRange);
            this.UpdateHexViewOfItem();
        }

        #region Effective Against Checkbox Callbacks

        private void fliersCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstFliers = this.fliersCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void dragonsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstDragons = this.dragonsCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void beastsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstBeasts = this.beastsCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void monstersCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstMonsters = this.monstersCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void armorsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstArmors = this.armorsCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        private void fellDragonsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentItemEntry.EffectivenessBitflags.IsEffectiveAgainstFellDragons = this.fellDragonsCheckbox.Checked;
            this.UpdateHexViewOfItem();
        }

        #endregion

        #endregion

        #region Item Effects Callbacks

        private void itemEffectTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentItemEntry.ItemEffectType = (ItemEffectType)Enum.GetValues(typeof(ItemEffectType)).GetValue(this.itemEffectTypeComboBox.SelectedIndex);
            this.UpdateHexViewOfItem();
        }

        private void effectParameterTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.effectParameterTextBox, ref currentItemEntry.EffectParameter);
            this.UpdateHexViewOfItem();
        }

        private void strIncreaseTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.strIncreaseTextBox, ref currentItemEntry.AmountOfStrengthIncrease);
            this.UpdateHexViewOfItem();
        }

        private void magIncreaseTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.magIncreaseTextBox, ref currentItemEntry.AmountOfMagicIncrease);
            this.UpdateHexViewOfItem();
        }

        private void sklIncreaseTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.sklIncreaseTextBox, ref currentItemEntry.AmountOfSkillIncrease);
            this.UpdateHexViewOfItem();
        }

        private void spdIncreaseTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.spdIncreaseTextBox, ref currentItemEntry.AmountOfSpeedIncrease);
            this.UpdateHexViewOfItem();
        }

        private void lckIncreaseTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.lckIncreaseTextBox, ref currentItemEntry.AmountOfLuckIncrease);
            this.UpdateHexViewOfItem();
        }

        private void defIncreaseTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.defIncreaseTextBox, ref currentItemEntry.AmountOfDefenseIncrease);
            this.UpdateHexViewOfItem();
        }

        private void resIncreaseTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.resIncreaseTextBox, ref currentItemEntry.AmountOfResistanceIncrease);
            this.UpdateHexViewOfItem();
        }

        private void movIncreaseTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.movIncreaseTextBox, ref currentItemEntry.AmountOfMovementIncrease);
            this.UpdateHexViewOfItem();
        }

        #endregion

        #region Misc Callbacks

        private void weaponSubtypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentItemEntry.WeaponSubType = (WeaponSubType)Enum.GetValues(typeof(WeaponSubType)).GetValue(this.weaponSubtypeComboBox.SelectedIndex);
            this.UpdateHexViewOfItem();
        }

        private void weaponStrengthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentItemEntry.WeaponStrengthEquivalent = (WeaponStrengthEquivalent)Enum.GetValues(typeof(WeaponStrengthEquivalent)).GetValue(this.weaponStrengthComboBox.SelectedIndex);
            this.UpdateHexViewOfItem();
        }

        private void staffBaseExpTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.staffBaseExpTextBox, ref currentItemEntry.StaffBaseExp);
            this.UpdateHexViewOfItem();
        }

        private void itemAvailabilityByte1TextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.itemAvailabilityByte1TextBox, ref currentItemEntry.ItemAvailabilityByte1);
            this.UpdateHexViewOfItem();
        }

        private void itemAvailabilityByte2TextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.itemAvailabilityByte2TextBox, ref currentItemEntry.ItemAvailabilityByte2);
            this.UpdateHexViewOfItem();
        }

        private void unknownTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateAndSetUnsignedByte(this.unknownTextBox, ref currentItemEntry.Unknown);
            this.UpdateHexViewOfItem();
        }

        #endregion
    }
}
