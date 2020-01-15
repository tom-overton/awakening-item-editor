namespace AwakeningItemTool
{
    // Note that in this class, the pointers are offset by the header like in game.
    // That means to get what they're actually pointing at, add HEADER_SIZE.
    public class Pointer
    {
        public int PointerIntoDataRegion;
        public int PointerFromDataRegion;
        public Label Label;

        public Pointer(int pointerIntoDataRegion, int pointerFromDataRegion, Label label)
        {
            this.PointerIntoDataRegion = pointerIntoDataRegion;
            this.PointerFromDataRegion = pointerFromDataRegion;
            this.Label = label;
        }
    }
}
