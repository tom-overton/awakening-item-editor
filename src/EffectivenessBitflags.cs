using System;

namespace AwakeningItemTool
{
    public class EffectivenessBitflags
    {
        [Flags]
        public enum EffectivenessBitflagsEnum : UInt16
        {
            IsEffectiveAgainstFliers = 0x0001,
            IsEffectiveAgainstDragons = 0x0002,
            IsEffectiveAgainstBeasts = 0x0004,
            IsEffectiveAgainstMonsters = 0x0008,
            IsEffectiveAgainstArmors = 0x0010,
            IsEffectiveAgainstFellDragons = 0x0020
        }

        private EffectivenessBitflagsEnum effectivenessBitflags;

        public bool IsEffectiveAgainstFliers
        {
            get { return (this.effectivenessBitflags & EffectivenessBitflagsEnum.IsEffectiveAgainstFliers) != 0; }
            set { this.effectivenessBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.effectivenessBitflags, EffectivenessBitflagsEnum.IsEffectiveAgainstFliers, value); }
        }

        public bool IsEffectiveAgainstDragons
        {
            get { return (this.effectivenessBitflags & EffectivenessBitflagsEnum.IsEffectiveAgainstDragons) != 0; }
            set { this.effectivenessBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.effectivenessBitflags, EffectivenessBitflagsEnum.IsEffectiveAgainstDragons, value); }
        }

        public bool IsEffectiveAgainstBeasts
        {
            get { return (this.effectivenessBitflags & EffectivenessBitflagsEnum.IsEffectiveAgainstBeasts) != 0; }
            set { this.effectivenessBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.effectivenessBitflags, EffectivenessBitflagsEnum.IsEffectiveAgainstBeasts, value); }
        }

        public bool IsEffectiveAgainstMonsters
        {
            get { return (this.effectivenessBitflags & EffectivenessBitflagsEnum.IsEffectiveAgainstMonsters) != 0; }
            set { this.effectivenessBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.effectivenessBitflags, EffectivenessBitflagsEnum.IsEffectiveAgainstMonsters, value); }
        }

        public bool IsEffectiveAgainstArmors
        {
            get { return (this.effectivenessBitflags & EffectivenessBitflagsEnum.IsEffectiveAgainstArmors) != 0; }
            set { this.effectivenessBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.effectivenessBitflags, EffectivenessBitflagsEnum.IsEffectiveAgainstArmors, value); }
        }

        public bool IsEffectiveAgainstFellDragons
        {
            get { return (this.effectivenessBitflags & EffectivenessBitflagsEnum.IsEffectiveAgainstFellDragons) != 0; }
            set { this.effectivenessBitflags = this.SetOrUnsetFlagOnItemBitflagsEnum(this.effectivenessBitflags, EffectivenessBitflagsEnum.IsEffectiveAgainstFellDragons, value); }
        }

        public EffectivenessBitflags(UInt16 effectivenessInt)
        {
            this.effectivenessBitflags = (EffectivenessBitflagsEnum)effectivenessInt;
        }

        public UInt16 ConvertBitFlagsToInt()
        {
            return (UInt16)this.effectivenessBitflags;
        }

        private EffectivenessBitflagsEnum SetOrUnsetFlagOnItemBitflagsEnum(EffectivenessBitflagsEnum initialValue, EffectivenessBitflagsEnum flagToChange, bool shouldBeSet)
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
