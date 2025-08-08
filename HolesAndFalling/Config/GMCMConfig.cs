using HolesAndFalling.APIs;
using StardewModdingAPI;
using StardewValley;

namespace HolesAndFalling.Config
{
    internal class GMCMConfig
    {
        private static IModHelper Helper;
        private static IManifest ModManifest;
        private static ModConfig Config;
        private static IMonitor Monitor;
        public static void Init(IModHelper helper, IManifest manifest, ModConfig config, IMonitor monitor)
        {
            Helper = helper;
            ModManifest = manifest;
            Config = config;
            Monitor = monitor;

            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => onConfigMenuSaved(Config)
            );

            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Configs");

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_CornerMessage_Name(),
                tooltip: () => I18n.Config_CornerMessage_Tooltip(),
                getValue: () => Config.cornerMessage,
                setValue: value => Config.cornerMessage = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_PortableHole_Name(),
                tooltip: () => I18n.Config_PortableHole_Tooltip(),
                getValue: () => Config.portableHole,
                setValue: value => Config.portableHole = value
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => I18n.Config_HoleRecipe_Name(),
                tooltip: () => I18n.Config_HoleRecipe_Tooltip(),
                getValue: () => Config.holeRecipe,
                setValue: value => Config.holeRecipe = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_UnlockAutomatically_Name(),
                tooltip: () => I18n.Config_UnlockAutomatically_Tooltip(),
                getValue: () => Config.unlockHoleAutomatically,
                setValue: value => Config.unlockHoleAutomatically = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Config_SoftFall_Name(),
                tooltip: () => I18n.Config_SoftFall_Tooltip(),
                getValue: () => Config.softFall,
                setValue: value => Config.softFall = value
            );
        }
        public static void onConfigMenuSaved(ModConfig config)
        {
            Helper.WriteConfig(config);
            Helper.GameContent.InvalidateCache("Data/Objects");
            Helper.GameContent.InvalidateCache("Data/CraftingRecipes");

            if (config.unlockHoleAutomatically && !Game1.player.craftingRecipes.ContainsKey("Portable Hole"))
            {
                Game1.player.craftingRecipes.Add("Portable Hole", 0);
            }
        }
    }
}