using HolesAndFalling.Config;
using StardewModdingAPI;
using StardewValley.GameData.Objects;

namespace HolesAndFalling.Framework
{
    internal class Data
    {
        private static IModHelper Helper;
        private static IManifest ModManifest;
        private static ModConfig Config;
        private static IMonitor Monitor;

        public static KeyValuePair<string, ObjectData> objectPortableHole;
        public static KeyValuePair<string, string> recipePortableHole;

        public static void Init(IModHelper helper, IManifest manifest, ModConfig config, IMonitor monitor)
        {
            Helper = helper;
            ModManifest = manifest;
            Config = config;
            Monitor = monitor;

            objectPortableHole = new KeyValuePair<string, ObjectData>(
                $"{ModManifest.UniqueID}_PortableHole",
                new ObjectData()
                {
                    Name = $"{ModManifest.UniqueID}_PortableHole",
                    DisplayName = I18n.Item_PortableHole_Name(),
                    Description = I18n.Item_PortableHole_Description(),
                    Type = "Crafting",
                    Category = -8,
                    Texture = $"{ModManifest.UniqueID}\\Assets\\texture",
                    SpriteIndex = 1
                });

            recipePortableHole = new KeyValuePair<string, string>(
                "Portable Hole",
                $"{Config.holeRecipe}/Field/{ModManifest.UniqueID}_PortableHole/false/Mining 10/{I18n.Item_PortableHole_Name()}");
        }
    }
}
