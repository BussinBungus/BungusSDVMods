using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using Object = StardewValley.Object;
using HolesAndFalling.Config;

namespace HolesAndFalling.Framework
{
    internal class PatchManager
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

            PatchHelpers.Init(Helper, ModManifest, Config, Monitor);

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                    original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.enterMineShaft)),
                    transpiler: new HarmonyMethod(typeof(PatchManager), nameof(enterMineShaft_transpiler)));
            harmony.Patch(
                    original: AccessTools.Method(typeof(MineShaft), "afterFall"),
                    transpiler: new HarmonyMethod(typeof(PatchManager), nameof(afterFall_transpiler)));
            harmony.Patch(
                    original: AccessTools.Method(typeof(Object), nameof(Object.placementAction)),
                    prefix: new HarmonyMethod(typeof(PatchManager), nameof(placementAction_prefix)));
        }
        private static IEnumerable<CodeInstruction> enterMineShaft_transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new(instructions);

            matcher.Start()
                .MatchStartForward(new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(DelayedAction), nameof(DelayedAction.playSoundAfterDelay))))
                .ThrowIfNotMatch($"Could not patch at playSoundAfterDelay in enterMineShaft, {ModManifest.Name} may not work.")
                .SetInstruction(CodeInstruction.Call(typeof(PatchHelpers), nameof(PatchHelpers.playSoundAfterDelayHandler)));

            matcher.Start()
                .MatchStartForward(new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(DelayedAction), nameof(DelayedAction.playSoundAfterDelay))))
                .ThrowIfNotMatch($"Could not patch at playSoundAfterDelay in enterMineShaft, {ModManifest.Name} may not work.")
                .SetInstruction(CodeInstruction.Call(typeof(PatchHelpers), nameof(PatchHelpers.playSoundAfterDelayHandler)));

            matcher.Start()
                .MatchStartForward(new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Game1), "globalFadeToBlack")))
                .ThrowIfNotMatch($"Could not patch at globalFadeToBlack in enterMineShaft, {ModManifest.Name} may not work.")
                .SetInstructionAndAdvance(CodeInstruction.Call(typeof(PatchHelpers), nameof(PatchHelpers.globalFadeToBlackHandler)));

            matcher.Start()
                .MatchStartForward(new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(Farmer), nameof(Farmer.health))))
                .ThrowIfNotMatch($"Could not patch at storeFarmerHealth in enterMineShaft, {ModManifest.Name} may not work.")
                .SetInstructionAndAdvance(CodeInstruction.Call(typeof(PatchHelpers), nameof(PatchHelpers.storeFarmerHealthHandler)));

            return matcher.InstructionEnumeration();
        }
        private static IEnumerable<CodeInstruction> afterFall_transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new(instructions);

            matcher.Start()
                .MatchStartForward(new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Game1), nameof(Game1.drawObjectDialogue), new[] { typeof(string) })))
                .ThrowIfNotMatch($"Could not patch at drawObjectDialogue in afterFall, {ModManifest.Name} may not work.")
                .SetInstructionAndAdvance(CodeInstruction.Call(typeof(PatchHelpers), nameof(PatchHelpers.drawObjectDialogueHandler)));

            matcher.Start()
                .MatchStartForward(new CodeMatch(OpCodes.Stsfld, AccessTools.Field(typeof(Game1), nameof(Game1.messagePause))))
                .ThrowIfNotMatch($"Could not patch at messagePause in afterFall, {ModManifest.Name} may not work.")
                .SetInstructionAndAdvance(CodeInstruction.Call(typeof(PatchHelpers), nameof(PatchHelpers.messagePauseHandler)));

            matcher.Start()
                .MatchStartForward(new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Game1), "set_fadeToBlackAlpha")))
                .ThrowIfNotMatch($"Could not patch at set_fadeToBlackAlpha in afterFall, {ModManifest.Name} may not work.")
                .SetInstructionAndAdvance(CodeInstruction.Call(typeof(PatchHelpers), nameof(PatchHelpers.set_fadeToBlackAlphaHandler)));

            return matcher.InstructionEnumeration();
        }
        private static bool placementAction_prefix(Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who = null)
        {
            Vector2 placementTile = new Vector2(x / 64, y / 64);
            __instance.Location = location;
            __instance.TileLocation = placementTile;
            __instance.owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;

            if (__instance.QualifiedItemId == $"(O){ModManifest.UniqueID}_PortableHole")
            {
                if (location is MineShaft { mineLevel: > 120 } mineShaft
                    && mineShaft.shouldCreateLadderOnThisLevel()
                    && PatchHelpers.recursiveTryToCreateHoleDown(placementTile, mineShaft))
                {
                    MineShaft.numberOfCraftedStairsUsedThisRun++;
                    __result = true;
                }
                else
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                    __result = false;
                }
                return false;
            }
            return true;
        }
    }
}