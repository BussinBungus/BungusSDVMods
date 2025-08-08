using StardewModdingAPI;
using HolesAndFalling.Config;
using HolesAndFalling.Framework;

// To-Do ideas: remove other object dialogue like in Confirm Mine Ladder but for books, treasure floors, etc?
// Other recommended mods: see Confirm Mine Ladder, Ladder Chance Framework, Better Bombs, Passable Descents, Safe Reading!

namespace HolesAndFalling
{
    public partial class ModEntry : Mod
    {
        public static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            I18n.Init(Helper.Translation);
            EventManager.Init(Helper, ModManifest, Config, Monitor);
            PatchManager.Init(Helper, ModManifest, Config, Monitor);
        }
    }
}
