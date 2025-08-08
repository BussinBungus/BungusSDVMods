using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;
using HolesAndFalling.Config;

namespace HolesAndFalling.Framework
{
    internal class EventManager
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

            Data.Init(Helper, ModManifest, Config, Monitor);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private static void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            ToolbarHelper.Init(Helper, ModManifest, Config, Monitor);
            GMCMConfig.Init(Helper, ModManifest, Config, Monitor);
        }
        private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo($"{ModManifest.UniqueID}\\Assets\\texture") && Config.portableHole)
            {
                e.LoadFromModFile<Texture2D>("Assets/texture.png", AssetLoadPriority.Exclusive);
            }
            if (e.NameWithoutLocale.IsEquivalentTo($"{ModManifest.UniqueID}\\Assets\\icon"))
            {
                e.LoadFromModFile<Texture2D>("Assets/icon.png", AssetLoadPriority.Exclusive);
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects") && Config.portableHole)
            {
                e.Edit(asset => asset.AsDictionary<string, ObjectData>().Data.Add(Data.objectPortableHole));
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes") && Config.portableHole)
            {
                e.Edit(asset => asset.AsDictionary<string, string>().Data.Add(Data.recipePortableHole));
            }
        }
    }
}