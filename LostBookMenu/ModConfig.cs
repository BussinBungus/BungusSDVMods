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
        public int WindowWidth { get; set; } = 1600;
        public int WindowHeight { get; set; } = 900;
        public int GridColumns { get; set; } = 7;
        public int CoverWidth { get; set; } = 128;
        public int CoverHeight { get; set; } = 128;
        public int GridSpace { get; set; } = 96;
    }
}
