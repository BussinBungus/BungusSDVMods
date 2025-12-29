using GenericModConfigMenu;
using StardewModdingAPI;
using StardewValley;

namespace HeartEventHelper.Config
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

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod configs
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => onConfigMenuSaved(Config)
            );

            // add original configs
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Configs"
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text Before Positive",
                getValue: () => Config.beforePositive,
                setValue: value => Config.beforePositive = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text Before Neutral",
                getValue: () => Config.beforeNeutral,
                setValue: value => Config.beforeNeutral = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text Before Negative",
                getValue: () => Config.beforeNegative,
                setValue: value => Config.beforeNegative = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text After Positive",
                getValue: () => Config.afterPositive,
                setValue: value => Config.afterPositive = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text After Neutral",
                getValue: () => Config.afterNeutral,
                setValue: value => Config.afterNeutral = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text After Negative",
                getValue: () => Config.afterNegative,
                setValue: value => Config.afterNegative = value
            );

            // add configs previously in Content Patcher
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Icons"
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Icon 1 (¢)",
                tooltip: () => "The icon that replaces '¢'.",
                allowedValues: new string[] { "smile", "sunny", "thumbsup", "tinyheart", "iridiumstar", "meh", "frown", "raincloud", "tear", "thumbsdown", "x" },
                getValue: () => Config.icon1,
                setValue: value => Config.icon1 = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Icon 2 (£)",
                tooltip: () => "The icon that replaces '£'.",
                allowedValues: new string[] { "smile", "sunny", "thumbsup", "tinyheart", "iridiumstar", "meh", "frown", "raincloud", "tear", "thumbsdown", "x" },
                getValue: () => Config.icon2,
                setValue: value => Config.icon2 = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Icon 3 (¤)",
                tooltip: () => "The icon that replaces '¤'.",
                allowedValues: new string[] { "smile", "sunny", "thumbsup", "tinyheart", "iridiumstar", "meh", "frown", "raincloud", "tear", "thumbsdown", "x" },
                getValue: () => Config.icon3,
                setValue: value => Config.icon3 = value
            );

            // add notes
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => "Notes"
            );
            configMenu.AddParagraph(
                mod: ModManifest,
                text: () =>
                "Use '{#}', including the brackets, to insert the number of friendship points gained or lost. " +
                "Use '{#_abs}' to insert the absolute value of friendship points, and '{#_heart}' to insert the number of hearts gained or lost (points/250). " +
                "The special characters to use the custom icons are '¢', '£', and '¤', or alternatively '{1}', '{2}', and '{3}'. " +
                "Use the Icons dropdowns options to change the icons assigned to these characters. " +
                "Other common special characters include '♡' or '{heart}' (heart), '*' (star), '`' (up arrow), and '_' (down arrow if SVE or DaisyNiko's tilesheets are installed). " +
                "Spaces aren't automatically added between the default dialogue and affixes, so they must be included in the boxes above if desired. " +
                "To disable changes to neutral responses, clear the 'Text Before Neutral' and 'Text After Neutral' fields. "
            );
        }
        public static void onConfigMenuSaved(ModConfig config)
        {
            Helper.WriteConfig(config);
            Helper.GameContent.InvalidateCache("LooseSprites/font_bold");
        }
    }
}
