using LostBookMenu.Config;
using LostBookMenu.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace LostBookMenu
{
    /// TODO: add BigSelect sound when opening menu, restructure
    public partial class ModEntry : Mod
    {
        private static ModConfig Config;

        public static Dictionary<string, CoverData> bookData = new Dictionary<string, CoverData>();
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            if (!Config.ModEnabled)
                return;

            BookMenu.Init(Helper, ModManifest, Config, Monitor);
            VPPHelper.Init(Helper, ModManifest, Config, Monitor);

            GameLocation.RegisterTileAction($"{ModManifest.UniqueID}_OpenMenu", HandleOpenMenu);

            Helper.Events.Content.AssetRequested += onAssetRequested;
            Helper.Events.GameLoop.GameLaunched += onGameLaunched;
            Helper.Events.GameLoop.DayStarted += onDayStarted;
            Helper.Events.Input.ButtonPressed += onButtonPressed;
        }
        private void onAssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("bungus.BookMenu/covers"))
            {
                e.LoadFrom(() => coverDictionary, StardewModdingAPI.Events.AssetLoadPriority.Exclusive);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/ArchaeologyHouse"))
            {
                e.Edit(asset => patchMuseumMap(asset));
            }
        }

        private void onDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            bookData = Helper.GameContent.Load<Dictionary<string, CoverData>>("bungus.BookMenu/covers");
            foreach (var key in bookData.Keys.ToArray())
            {
                bookData[key].scale = Config.CoverScale;
                bookData[key].title = Helper.Translation.Get($"BookName.{key}");
                if (!string.IsNullOrEmpty(bookData[key].texturePath))
                {
                    bookData[key].texture = Helper.GameContent.Load<Texture2D>(bookData[key].texturePath);
                }
                else if (Helper.ModRegistry.IsLoaded("Airyn.RandomLibraryBookCovers"))
                {
                    bookData[key].texture = Helper.ModContent.Load<Texture2D>(Path.Combine("Assets", $"book_{(int.Parse(key) < 10 ? "0" + key : key)}.png"));
                }
                else
                {
                    bookData[key].texture = Helper.ModContent.Load<Texture2D>(Path.Combine("Assets", "cover.png"));
                }

            }
        }

        private void onGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            GMCMConfig.Init(Helper, ModManifest, Config, Monitor);
        }

        private void onButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button == Config.MenuKey)
            {
                OpenMenu();
            }
        }

        private void OpenMenu()
        {
            if (Config.ModEnabled && Context.IsPlayerFree)
            {
                Game1.playSound("bigSelect");
                Game1.activeClickableMenu = new BookMenu();
            }
        }

        private bool HandleOpenMenu(GameLocation location, string[] args, Farmer player, Point point)
        {
            OpenMenu();
            return true;
        }

        private IAssetData patchMuseumMap(IAssetData asset)
        {
            Map map = asset.AsMap().Data;

            Layer front = map.GetLayer("Front");
            Layer buildings = map.GetLayer("Buildings");

            // add book to library at (7, 8)
            front.Tiles[7, 8] = new StaticTile(
                layer: front,
                tileSheet: map.GetTileSheet("untitled tile sheet"),
                tileIndex: 675,
                blendMode: BlendMode.Alpha
            );

            // add OpenMenu action to library at (7, 9)
            Tile tile = buildings.PickTile(new Location(7 * Game1.tileSize, 9 * Game1.tileSize), Game1.viewport.Size);
            if (tile != null)
                tile.Properties["Action"] = $"{ModManifest.UniqueID}_OpenMenu";

            return asset;
        }
    }
}