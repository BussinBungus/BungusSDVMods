using LostBookMenu.APIs;
using LostBookMenu.Config;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using System;
using System.Linq;

namespace LostBookMenu.Framework
{
    internal class VPPHelper
    {
        private static IModHelper Helper;
        private static IManifest ModManifest;
        private static ModConfig Config;
        private static IMonitor Monitor;
        public static void Init(IModHelper helper, IManifest manifest, ModConfig config, IMonitor monitor) {
            Helper = helper;
            ModManifest = manifest;
            Config = config;
            Monitor = monitor;
        }
        public static void addVPPBuff(int bookID)
        {
            // get VPP's API (if it's installed)
            var VPP = Helper.ModRegistry.GetApi<IVanillaPlusProfessions>("KediDili.VanillaPlusProfessions");
            if (VPP is null)
                return;

            if (VPP.GetTalentsForPlayer().Contains("LostAndFound"))
            {
                string name = "";
                int index = 0;
                BuffEffects buffEffects = new();
                switch (bookID)
                {
                    case 0:  // 0 - Tips on Farming - +1 Farming
                        name = "Farming";
                        buffEffects.FarmingLevel.Value = 1;
                        index = 0;
                        break;
                    case 1: // 1 - A book by Marnie - +2 Farming?
                        name = "Farming";
                        buffEffects.FarmingLevel.Value = 2;
                        index = 0;
                        break;
                    case 2:  // 2 - On Foraging - +2 Foraging?
                        name = "Foraging";
                        buffEffects.ForagingLevel.Value = 2;
                        index = 5;
                        break;
                    case 3: // 3 - Fisherman act 1 - +1 Fishing?
                        name = "Fishing";
                        buffEffects.FishingLevel.Value = 1;
                        index = 1;
                        break;
                    case 4: // 4 - How deep the mines go? - +1 Mining 
                        name = "Mining";
                        buffEffects.MiningLevel.Value = 1;
                        index = 2;
                        break;
                    case 5:  // 5 - A note that hints on people will give you cooking recipes if you befriend them - Magnetism
                        name = "Magnetism";
                        buffEffects.MagneticRadius.Value = 40;
                        index = 8;
                        break;
                    case 6: // 6 - Scarecrows - +3 Farming
                        name = "Farming";
                        buffEffects.FarmingLevel.Value = 3;
                        index = 0;
                        break;
                    case 7: // 7 - The secret of Stardrop - Max Stamina?
                        name = "Max Stamina";
                        buffEffects.MaxStamina.Value = 40;
                        index = 16;
                        break;
                    case 8: // 8 - Journey of Prairie King - +2 Attack
                        name = "Attack";
                        buffEffects.Attack.Value = 2;
                        index = 11;
                        break;
                    case 9: // 9 - A study on Diamond yields - +3 Mining?
                        name = "Mining";
                        buffEffects.MiningLevel.Value = 3;
                        index = 2;
                        break;
                    case 10: // 10 - Brewmaster's Guide - +4 Farming
                        name = "Farming";
                        buffEffects.FarmingLevel.Value = 4;
                        index = 0;
                        break;
                    case 11:  // 11 - Mysteries of Dwarves - +3 Mining?
                        name = "Mining";
                        buffEffects.MiningLevel.Value = 3;
                        index = 2;
                        break;
                    case 12: // 12 - Hightlights from the book of yoba - +4 Foraging
                        name = "Foraging";
                        buffEffects.ForagingLevel.Value = 4;
                        index = 5;
                        break;
                    case 13: // 13 - Marriage Guide - +3 Luck
                        name = "Luck";
                        buffEffects.LuckLevel.Value = 3;
                        index = 4;
                        break;
                    case 14: // 14 - The fisherman act 2 - +3 Fishing?
                        name = "Fishing";
                        buffEffects.FishingLevel.Value = 3;
                        index = 1;
                        break;
                    case 15: // 15 - A note explaining how crystalariums work - +4 Mining?
                        name = "Mining";
                        buffEffects.MiningLevel.Value = 4;
                        index = 2;
                        break;
                    case 16: // 16 - Secrets of the Legendary Fish - +4 Fishing
                        name = "Fishing";
                        buffEffects.FishingLevel.Value = 4;
                        index = 1;
                        break;
                    case 17: // 17 - A note that hints at Qi's casino quest - +4 Luck
                        name = "Luck";
                        buffEffects.LuckLevel.Value = 4;
                        index = 4;
                        break;
                    case 18: // 18 - Note From Gunther - +4 Speed
                        name = "Speed";
                        buffEffects.Speed.Value = 4;
                        index = 9;
                        break;
                    case 19: // 19 - Goblins by Jasper - +4 Defense
                        name = "Defense";
                        buffEffects.Defense.Value = 4;
                        index = 10;
                        break;
                    case 20: // 20 - Easter Egg book - +4 Speed
                        name = "Speed";
                        buffEffects.Speed.Value = 4;
                        index = 9;
                        break;
                    default:
                        break;
                }

                Game1.player.buffs.Apply(new("Kedi.VPP.LostAndFound", "Lost Books", "Lost Books", -2, Game1.buffsIcons, index, buffEffects, false, name, ""));
            }
        }
    }
}