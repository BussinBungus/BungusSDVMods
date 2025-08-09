using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using HolesAndFalling.Config;
using HolesAndFalling.APIs;

namespace HolesAndFalling.Framework
{
    internal class ToolbarHelper
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

            var toolbarIcons = Helper.ModRegistry.GetApi<IIconicFrameworkApi>("furyx639.ToolbarIcons");
            if (toolbarIcons is null)
                return;

            toolbarIcons.AddToolbarIcon(
                "PortableHoles.PlaceHole",
                $"{ModManifest.UniqueID}\\Assets\\icon",
                new Rectangle(0, 0, 16, 16),
                () => I18n.Button_PortableHole_Title(),
                () => I18n.Button_PortableHole_Tooltip(),
                () => createToolbarHole(Game1.currentLocation, Game1.player.Tile));
        }
        public static void createToolbarHole(GameLocation location, Vector2 placementTile)
        {
            if (location is MineShaft { mineLevel: > 120 } mineShaft
                        && mineShaft.shouldCreateLadderOnThisLevel()
                        && PatchHelpers.recursiveTryToCreateHoleDown(placementTile, mineShaft))
                return;
            else { Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053")); }
        }
    }
}
