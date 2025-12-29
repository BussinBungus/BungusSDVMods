using HarmonyLib;
using HeartEventHelper.Config;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;
using System.Reflection.Emit;

namespace HeartEventHelper.Framework
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
                original: AccessTools.Method(typeof(Dialogue), "parseDialogueString"),
                transpiler: new HarmonyMethod(typeof(PatchManager), nameof(parseDialogueString_Transpiler)));
            harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), "Question"),
                transpiler: new HarmonyMethod(typeof(PatchManager), nameof(Question_Transpiler)));
            harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), "QuickQuestion"),
                transpiler: new HarmonyMethod(typeof(PatchManager), nameof(QuickQuestion_Transpiler)));
        }
        public static IEnumerable<CodeInstruction> parseDialogueString_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Monitor.Log($"Transpiling Dialogue.parseDialogueString");

            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            for (int i = 0; i < codes.Count; i++)
            {
                newCodes.Add(codes[i]);

                if (codes[i].opcode == OpCodes.Newobj && (ConstructorInfo)codes[i].operand == AccessTools.Constructor(typeof(NPCDialogueResponse), new System.Type[] { typeof(string), typeof(int), typeof(string), typeof(string), typeof(string), typeof(Keys) }))
                {
                    newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchHelpers), nameof(PatchHelpers.AddReactionToNPCDialogueResponse))));
                }
            }
            return newCodes.AsEnumerable();
        }
        public static IEnumerable<CodeInstruction> QuickQuestion_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Monitor.Log($"Transpiling Event.DefaultCommands.QuickQuestion");

            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr && (string)codes[i].operand == "quickQuestion")
                {
                    newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchHelpers), nameof(PatchHelpers.GetQuickQuestionAnswers)))); // call GetQuickQuestionAnswers
                    newCodes.Add(codes[i]); // load "quickQuestion"
                }
                else
                    newCodes.Add(codes[i]);
            }
            return newCodes.AsEnumerable();
        }
        public static IEnumerable<CodeInstruction> Question_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Monitor.Log($"Transpiling Event.DefaultCommands.Question");

            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_0)
                {
                    newCodes.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchHelpers), nameof(PatchHelpers.GetQuestionAnswers)))); // call GetQuestionAnswers
                    newCodes.Add(codes[i]); // load dialogueKey
                }
                else
                    newCodes.Add(codes[i]);
            }
            return newCodes.AsEnumerable();
        }
    }
}