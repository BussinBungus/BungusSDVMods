using StardewModdingAPI;
using StardewModdingAPI.Events;
using HeartEventHelper.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BmFont;

namespace HeartEventHelper.Framework
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

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private static void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            GMCMConfig.Init(Helper, ModManifest, Config, Monitor);
        }
        private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            string iconsPath = Path.Combine(Helper.DirectoryPath, "Assets", "Icons");
            string fontsPath = Path.Combine(Helper.DirectoryPath, "Assets", "Fonts");

            if (e.NameWithoutLocale.IsEquivalentTo("LooseSprites\\font_bold"))
            {
                foreach (string iconPaths in Directory.EnumerateFiles(iconsPath, "*.png"))
                {
                    string icon = Path.GetFileNameWithoutExtension(iconPaths);
                    if (Config.icon1 == icon)
                    {
                        e.Edit(asset =>
                        {
                            var editor = asset.AsImage();
                            Texture2D sourceImage = Helper.ModContent.Load<Texture2D>(Path.Combine("Assets", "Icons", $"{icon}.png"));
                            editor.PatchImage(sourceImage, targetArea: new Rectangle(16, 128, 8, 16));
                        },
                        AssetEditPriority.Late);
                    }
                    if (Config.icon2 == icon)
                    {
                        e.Edit(asset =>
                        {
                            var editor = asset.AsImage();
                            Texture2D sourceImage = Helper.ModContent.Load<Texture2D>(Path.Combine("Assets", "Icons", $"{icon}.png"));
                            editor.PatchImage(sourceImage, targetArea: new Rectangle(24, 128, 8, 16));
                        },
                        AssetEditPriority.Late);
                    }
                    if (Config.icon3 == icon)
                    {
                        e.Edit(asset =>
                        {
                            var editor = asset.AsImage();
                            Texture2D sourceImage = Helper.ModContent.Load<Texture2D>(Path.Combine("Assets", "Icons", $"{icon}.png"));
                            editor.PatchImage(sourceImage, targetArea: new Rectangle(32, 128, 8, 16));
                        },
                        AssetEditPriority.Late);
                    }
                }
            }

            foreach (string fontPaths in Directory.EnumerateFiles(fontsPath, "*.fnt"))
            {
                string font = Path.GetFileNameWithoutExtension(fontPaths);
                if (e.NameWithoutLocale.IsEquivalentTo($"Fonts\\{font}"))
                {
                    e.LoadFromModFile<XmlSource>($"Assets/Fonts/{font}.fnt", AssetLoadPriority.Exclusive);
                }
            }

            if (e.NameWithoutLocale.IsEquivalentTo($"Fonts\\Chinese_3"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    Texture2D sourceImage = Helper.ModContent.Load<Texture2D>(Path.Combine("Assets", "Fonts", "ja_ko_zh.png"));
                    editor.PatchImage(sourceImage, targetArea: new Rectangle(0, 128, 48, 16));
                },
                AssetEditPriority.Late);
            }

            if (e.NameWithoutLocale.IsEquivalentTo($"Fonts\\Japanese_1"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    Texture2D sourceImage = Helper.ModContent.Load<Texture2D>(Path.Combine("Assets", "Fonts", "ja_ko_zh.png"));
                    editor.PatchImage(sourceImage, targetArea: new Rectangle(0, 176, 48, 16));
                },
                AssetEditPriority.Late);
            }
            
            if (e.NameWithoutLocale.IsEquivalentTo($"Fonts\\Korean_11"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    Texture2D sourceImage = Helper.ModContent.Load<Texture2D>(Path.Combine("Assets", "Fonts", "ja_ko_zh.png"));
                    editor.PatchImage(sourceImage, targetArea: new Rectangle(0, 480, 48, 16));
                },
                AssetEditPriority.Late);
            }
        }
    }
}