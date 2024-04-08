using StardewModdingAPI;

namespace LostBookMenu
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool MenuInLibrary { get; set; } = true;
        public SButton MenuKey { get; set; } = SButton.None;
        public string MenuTitle { get; set; } = "Stardew Valley Library";
        public string MissingText { get; set; } = "???";
        public bool LegacyTitles { get; set; } = false;
        public int WindowWidth { get; set; } = 1050;
        public int WindowHeight { get; set; } = 600;
        public int GridColumns { get; set; } = 7;
        public int xOffset { get; set; } = -1;
        public int yOffset { get; set; } = 160;
        public float CoverScale { get; set; } = 8;
        public int HorizontalSpace { get; set; } = 16;
        public int VerticalSpace { get; set; } = 20;
    }
}
