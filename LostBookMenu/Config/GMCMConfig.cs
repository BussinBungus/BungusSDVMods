using LostBookMenu.APIs;
using StardewModdingAPI;

namespace LostBookMenu.Config
{
    internal class GMCMConfig
    {
        private static IModHelper Helper;
        private static IManifest ModManifest;
        private static ModConfig Config;
        private static IMonitor Monitor;

        public static IGenericModConfigMenuApi ConfigMenu;
        public static void Init(IModHelper helper, IManifest manifest, ModConfig config, IMonitor monitor)
        {
            Helper = helper;
            ModManifest = manifest;
            Config = config;
            Monitor = monitor;

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            ConfigMenu = configMenu;

            // register mod configs
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => onConfigMenuSaved()
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Basic Settings"
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod Enabled",
                getValue: () => Config.ModEnabled,
                setValue: value => Config.ModEnabled = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Menu Book in Library",
                getValue: () => Config.MenuInLibrary,
                setValue: value => Config.MenuInLibrary = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Menu Key",
                getValue: () => Config.MenuKey,
                setValue: value => Config.MenuKey = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Menu Style",
                getValue: () => Config.MenuStyle,
                setValue: value => Config.MenuStyle = value,
                allowedValues: new string[] { "Original", "Smaller", "Custom" },
                fieldId: "menuStyle"
            );

            configMenu.AddPageLink(
                mod: ModManifest,
                pageId: "Advanced",
                text: () => "Go to Advanced Settings"
            );
            configMenu.AddPage(
                mod: ModManifest,
                pageId: "Advanced",
                pageTitle: () => "Advanced Settings"
            );
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Advanced Settings"
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Cover Scale",
                getValue: () => Config.CoverScale,
                setValue: value => Config.CoverScale = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Number of Columns",
                getValue: () => Config.GridColumns,
                setValue: value => Config.GridColumns = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "X Offset",
                tooltip: () => "Distance from the left border to start displaying books. '-1' uses the default calculation to center.",
                getValue: () => Config.xOffset,
                setValue: value => Config.xOffset = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Y Offset",
                tooltip: () => "Distance from the upper border to start displaying books.",
                getValue: () => Config.yOffset,
                setValue: value => Config.yOffset = value
            );
            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => "Note: The below settings will only take effect if Menu Style is set to 'Custom'. If Menu Style is changed from 'Custom', the settings below will be overwritten."
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Always Visible Titles",
                tooltip: () => "Use the original 'always visible' titles instead of the new 'tooltip' titles.",
                getValue: () => Config.LegacyTitles,
                setValue: value => Config.LegacyTitles = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Window Width",
                getValue: () => Config.WindowWidth,
                setValue: value => Config.WindowWidth = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Window Height",
                getValue: () => Config.WindowHeight,
                setValue: value => Config.WindowHeight = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Horizontal Spacing",
                tooltip: () => "The horizontal space between two books.",
                getValue: () => Config.HorizontalSpace,
                setValue: value => Config.HorizontalSpace = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Vertical Spacing",
                tooltip: () => "The vertical space between two books.",
                getValue: () => Config.VerticalSpace,
                setValue: value => Config.VerticalSpace = value
            );
        }
        public static void onConfigMenuSaved()
        {
            if (Config.MenuStyle != "Custom")
            {
                bool isOriginalStyle = Config.MenuStyle == "Original";

                Config.LegacyTitles = isOriginalStyle;
                Config.WindowWidth = isOriginalStyle ? 1600 : 1050;
                Config.WindowHeight = isOriginalStyle ? 900 : 600;
                Config.HorizontalSpace = isOriginalStyle ? 96 : 16;
                Config.VerticalSpace = isOriginalStyle ? 116 : 20;
            }
            Helper.WriteConfig(Config);
        }
    }
}
