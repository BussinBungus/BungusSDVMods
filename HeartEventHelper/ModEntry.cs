using HeartEventHelper.Config;
using HeartEventHelper.Framework;
using StardewModdingAPI;

namespace HeartEventHelper
{ 
    public partial class ModEntry : Mod
    {
        public static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            EventManager.Init(Helper, ModManifest, Config, Monitor);
            PatchManager.Init(Helper, ModManifest, Config, Monitor);
        }
    }
}

// good event IDs for testing:
// 40 - Elliott bar, uses $r
// 43 - Elliott boat, uses question fork and $r, fork to custom map
// 15389722 - special order board event, uses quickQuestion, no friendship changes
// 6963327 - Abigail 14 heart, uses quickQuestion, no friendship changes
// 502969 - Linus trash event, only remaining vanilla $y use
// 96 - Gus event with Pam paying tab, uses friendship instead of $r
// 371652 - Linus well vs farm, fork to friendship
// 181928 - Penny field trip, uses switchEvent
// 3910975 - Shane rock bottom event, uses changeLocation
// 38 - Penny bathhouse event, -6 heart rejection lol
