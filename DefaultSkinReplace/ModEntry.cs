using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.FarmAnimals;

namespace DefaultSkinReplace
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        /// <summary>Get all animals from the world and friendship data.</summary>
        public IEnumerable<FarmAnimal> GetFarmAnimals()
        {
            List<FarmAnimal> animals = new List<FarmAnimal>();
            Utility.ForEachLocation(delegate (GameLocation location)
            {
                animals.AddRange(location.animals.Values.ToList());
                return true;
            });
            return animals;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised on day start.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            foreach (FarmAnimal animal in this.GetFarmAnimals())
            {
                FarmAnimalData data = animal.GetAnimalData();

                if (data != null)
                {
                    if (animal.skinID.Value == null && data.Skins != null)
                    {
                        Monitor.Log($"Detected a skinless {animal.type}.");
                        Random random = Utility.CreateRandom(animal.myID.Value);
                        Monitor.Log($"Generated random from animal ID {animal.myID.Value}.");
                        float totalWeight = 1f;
                        foreach (FarmAnimalSkin skin2 in data.Skins)
                        {
                            Monitor.Log($"Adding {skin2.Id} weight of {skin2.Weight}");
                            totalWeight += skin2.Weight;
                            Monitor.Log($"Total weight now {totalWeight}");
                        }
                        totalWeight = Utility.RandomFloat(0f, totalWeight, random);
                        Monitor.Log($"Randomized, total weight now {totalWeight}");
                        foreach (FarmAnimalSkin skin in data.Skins)
                        {
                            totalWeight -= skin.Weight;
                            Monitor.Log($"{skin.Id} weight subtracted, total weight now {totalWeight}");
                            if (totalWeight <= 0f)
                            {
                                animal.skinID.Value = skin.Id;
                                Monitor.Log($"Total weight <0, skin set to {skin.Id}");
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}