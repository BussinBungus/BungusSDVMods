using StardewModdingAPI;

namespace LostBookMenu.Config
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool MenuInLibrary { get; set; } = true;
        public SButton MenuKey { get; set; } = SButton.None;
        public string MenuStyle { get; set; } = "Original";
        public bool LegacyTitles { get; set; } = true;
        public int WindowWidth { get; set; } = 1600;
        public int WindowHeight { get; set; } = 900;
        public int GridColumns { get; set; } = 7;
        public int xOffset { get; set; } = -1;
        public int yOffset { get; set; } = 160;
        public float CoverScale { get; set; } = 8;
        public int HorizontalSpace { get; set; } = 96;
        public int VerticalSpace { get; set; } = 116;
    }
}
