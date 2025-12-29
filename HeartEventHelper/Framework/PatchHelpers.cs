using HeartEventHelper.Config;
using StardewModdingAPI;
using StardewValley;

namespace HeartEventHelper.Framework
{
    internal class PatchHelpers
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
        }

        public static Response[] GetQuickQuestionAnswers(Response[] answers)
        {
            try
            {
                string currentCommand = Game1.CurrentEvent.GetCurrentCommand();
                string[] scriptsSplit = currentCommand.Substring(currentCommand.IndexOf("(break)") + 7).Split("(break)");

                // Monitor.Log($"Current Event Command: {Game1.CurrentEvent.GetCurrentCommand()}", LogLevel.Error);
                // foreach (string script in scriptsSplit) { Monitor.Log($"script: {script}", LogLevel.Error); }

                for (int i = 0; i < answers.Length; i++)
                {
                    int friendship = 0;

                    if (ArgUtility.TryGet(scriptsSplit, i, out var script, out var error))
                    {
                        string[] scriptCommands = script.Split('\\', StringSplitOptions.RemoveEmptyEntries);

                        // foreach (string command in scriptCommands) { Monitor.Log($"command: {command}", LogLevel.Error); }

                        foreach (string command in scriptCommands)
                        {
                            string[] argSplit = ArgUtility.SplitBySpaceQuoteAware(command);

                            // foreach (string arg in argSplit) { Monitor.Log($"arg: {arg}", LogLevel.Error); }

                            string commandName = argSplit[0];

                            switch (commandName)
                            {
                                case "friendship":
                                case "friend":
                                    string reaction = argSplit[2];
                                    Monitor.Log($"friendship command at answer {i + 1}, gives {reaction} pts", LogLevel.Trace);
                                    friendship += int.Parse(reaction);
                                    break;
                                case "fork":
                                    ArgUtility.TryGet(argSplit, 1, out var req, out var error2);
                                    ArgUtility.TryGetOptional(argSplit, 2, out var forkEventID, out error2);
                                    if (forkEventID == null)
                                    {
                                        forkEventID = req;
                                        req = null;
                                    }
                                    if ((req == null && Game1.CurrentEvent.specialEventVariable1) || Game1.player.mailReceived.Contains(req) || Game1.player.dialogueQuestionsAnswered.Contains(req))
                                    {
                                        Monitor.Log($"fork command at answer {i + 1}, attempting branch to event {forkEventID}", LogLevel.Trace);
                                        friendship += BranchHandler(forkEventID);
                                    }
                                    break;
                                case "switchEvent":
                                    string switchEventID = argSplit[1];
                                    Monitor.Log($"switchEvent command at answer {i + 1}, attempting branch to event {switchEventID}", LogLevel.Trace);
                                    friendship += BranchHandler(switchEventID);
                                    break;
                            }
                        }
                    }
                    answers[i] = AddReactionToResponse(answers[i], friendship.ToString());
                }
                return answers;
            }
            catch (Exception e)
            {
                Monitor.Log($"GetQuickQuestionAnswers in HeartEventHelper failed! Here's some info, please report:", LogLevel.Error);
                Monitor.Log($"Event ID: {Game1.CurrentEvent.id}", LogLevel.Error);
                Monitor.Log($"Event Location: {Game1.currentLocation.Name}", LogLevel.Error);
                List<string> actors = new List<string>();
                foreach (NPC actor in Game1.CurrentEvent.actors) { actors.Add(actor.Name); }
                Monitor.Log($"Event Actors: {string.Join(", ", actors.ToArray())}", LogLevel.Error);
                Monitor.Log("Event Error Message: ", LogLevel.Error);
                Monitor.Log(e.Message, LogLevel.Error);
                return answers;
            }
        }
        public static Response[] GetQuestionAnswers(Response[] answers)
        {
            int forkedFriendship = 0;
            int unforkedFriendship = 0;
            int numToFork = 0;

            try
            {
                for (int i = 0; i < Game1.CurrentEvent.eventCommands.Length; i++)
                {
                    string command1 = Game1.CurrentEvent.eventCommands[i];

                    if (command1 == Game1.CurrentEvent.GetCurrentCommand())
                    {
                        string[] command1Split = ArgUtility.SplitBySpaceQuoteAware(command1);

                        if (!int.TryParse(command1Split[1].Substring(4), out numToFork))
                        {
                            break;
                        }

                        for (int j = i + 1; j < Game1.CurrentEvent.eventCommands.Length; j++)
                        {
                            string command2 = Game1.CurrentEvent.eventCommands[j];
                            string[] command2Split = ArgUtility.SplitBySpaceQuoteAware(command2);
                            string command2Name = command2Split[0];

                            if (command2Name == "question" || command2Name == "end")
                            {
                                break;
                            }

                            switch (command2Name)
                            {
                                case "friendship":
                                case "friend":
                                    string reaction = command2Split[2];
                                    Monitor.Log($"friendship command at index {j}, gives {reaction} pts", LogLevel.Trace);
                                    unforkedFriendship += int.Parse(reaction);
                                    break;
                                case "fork":
                                    ArgUtility.TryGet(command2Split, 1, out var req, out var error);
                                    ArgUtility.TryGetOptional(command2Split, 2, out var forkEventID, out error);
                                    if (forkEventID == null)
                                    {
                                        forkEventID = req;
                                        req = null;
                                    }
                                    if (req == null || Game1.player.mailReceived.Contains(req) || Game1.player.dialogueQuestionsAnswered.Contains(req))
                                    {
                                        Monitor.Log($"fork command at index {j}, attempting branch to event {forkEventID}", LogLevel.Trace);
                                        forkedFriendship += BranchHandler(forkEventID);
                                    }
                                    break;
                                case "switchEvent":
                                    string switchEventID = command2Split[1];
                                    Monitor.Log($"switchEvent command at index {j}, attempting branch to event {switchEventID}", LogLevel.Trace);
                                    unforkedFriendship += BranchHandler(switchEventID);
                                    break;
                            }
                        }
                    }
                }
                for (int i = 0; i < answers.Length; i++)
                {
                    bool numToForkIsSafe = numToFork >= 0 && numToFork < answers.Length;
                    bool forkedToggle = Game1.currentLocation.currentEvent.specialEventVariable1;
                    bool shouldFork = numToForkIsSafe ? (answers[i] == answers[numToFork] && !forkedToggle) || (answers[i] != answers[numToFork] && forkedToggle) : forkedToggle;

                    if (shouldFork)
                    {
                        answers[i] = AddReactionToResponse(answers[i], forkedFriendship.ToString());
                    }
                    else
                    {
                        answers[i] = AddReactionToResponse(answers[i], unforkedFriendship.ToString());
                    }
                }
                return answers;
            }
            catch (Exception e)
            {
                Monitor.Log("GetQuestionAnswers in HeartEventHelper failed! Here's some info, please report:", LogLevel.Error);
                Monitor.Log($"Event ID: {Game1.CurrentEvent.id}", LogLevel.Error);
                Monitor.Log($"Event Location: {Game1.currentLocation.Name}", LogLevel.Error);
                List<string> actors = new List<string>();
                foreach (NPC actor in Game1.CurrentEvent.actors) { actors.Add(actor.Name); }
                Monitor.Log($"Event Actors: {string.Join(", ", actors.ToArray())}", LogLevel.Error);
                Monitor.Log("Event Error Message: ", LogLevel.Error);
                Monitor.Log(e.Message, LogLevel.Error);
                return answers;
            }
        }
        public static int BranchHandler(string eventID, string? location = null)
        {
            Event branchEvent = Game1.currentLocation.findEventById(eventID);
            int friendship = 0;
            location ??= Game1.currentLocation.Name;

            if (branchEvent == null)
            {
                if (Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Temp").TryGetValue(eventID, out var eventScript1))
                {
                    branchEvent = new Event(eventScript1);
                }
                else if (Game1.content.Load<Dictionary<string, string>>($"Data\\Events\\{location}").TryGetValue(eventID, out var eventScript2))
                {
                    branchEvent = new Event(eventScript2);
                }
                else
                {
                    Monitor.Log($"BranchHandler in HeartEventHelper couldn't find branching event with ID {eventID}, friendship calculation may be inaccurate. Please report!", LogLevel.Error);
                    return 0;
                }
            }

            for (int i = 0; i < branchEvent.eventCommands.Length; i++)
            {
                string command = branchEvent.eventCommands[i];
                string[] commandSplit = ArgUtility.SplitBySpaceQuoteAware(command);
                string commandName = commandSplit[0];

                if (commandName == "question" || commandName == "end")
                {
                    break;
                }

                switch (commandName)
                {
                    case "friendship":
                    case "friend":
                        string reaction = commandSplit[2];
                        Monitor.Log($"friendship command in branch {eventID} at index {i}, gives {commandSplit[2]} pts", LogLevel.Trace);
                        friendship += int.Parse(reaction);
                        break;
                    case "fork":
                        ArgUtility.TryGet(commandSplit, 1, out var req, out var error);
                        ArgUtility.TryGetOptional(commandSplit, 2, out var forkEventID, out error);
                        if (forkEventID == null)
                        {
                            forkEventID = req;
                            req = null;
                        }
                        if ((req == null && Game1.CurrentEvent.specialEventVariable1) || Game1.player.mailReceived.Contains(req) || Game1.player.dialogueQuestionsAnswered.Contains(req))
                        {
                            Monitor.Log($"fork command at index {i}, attempting branch to event {forkEventID}", LogLevel.Trace);
                            friendship += BranchHandler(forkEventID); // recursion (spooky), may remove
                        }
                        break;
                    case "changeLocation":
                        location = commandSplit[1];
                        break;
                    case "switchEvent":
                        string switchEventID = commandSplit[1];
                        Monitor.Log($"switchEvent command at index {i}, attempting branch to event {switchEventID}", LogLevel.Trace);
                        friendship += BranchHandler(switchEventID, location); // recursion (spooky), may remove
                        break;
                }
            }
            return friendship;
        }
        public static Response AddReactionToResponse(Response response, string reaction)
        {
            if (response.responseText == "") { return response; }

            string newResponseText = GetReactionText(response.responseText, int.Parse(reaction));
            return new Response(response.responseKey, newResponseText, response.hotkey);
        }
        public static NPCDialogueResponse AddReactionToNPCDialogueResponse(NPCDialogueResponse response)
        {
            if (response.responseText == "") { return response; }

            int friendshipChange = response.id is null ? 0 : response.friendshipChange; // handles $y where id is null
            string newResponseText = GetReactionText(response.responseText, friendshipChange);
            return new NPCDialogueResponse(response.id, response.friendshipChange, response.npcReactionKey, newResponseText, response.extraArgument, response.hotkey);
        }
        public static string GetReactionText(string response, int friendship)
        {
            string beforePositive = ReplaceBrackets(Config.beforePositive, friendship);
            string beforeNeutral = ReplaceBrackets(Config.beforeNeutral, friendship);
            string beforeNegative = ReplaceBrackets(Config.beforeNegative, friendship);
            string afterPositive = ReplaceBrackets(Config.afterPositive, friendship);
            string afterNeutral = ReplaceBrackets(Config.afterNeutral, friendship);
            string afterNegative = ReplaceBrackets(Config.afterNegative, friendship);

            if (friendship > 0) { return $"{beforePositive}{response}{afterPositive}"; }
            if (friendship < 0) { return $"{beforeNegative}{response}{afterNegative}"; }
            else return $"{beforeNeutral}{response}{afterNeutral}";
        }
        public static string ReplaceBrackets(string config, int friendship)
        {
            int friendshipAbsolute = Math.Abs(friendship);
            float friendshipHearts = friendship / (float)250;

            config = config.Replace("{#}", friendship.ToString());
            config = config.Replace("{#_abs}", friendshipAbsolute.ToString());
            config = config.Replace("{#_heart}", friendshipHearts.ToString((friendship % 250 == 0) ? "0" : "0.0"));

            config = config.Replace("{1}", "¢");
            config = config.Replace("{2}", "£");
            config = config.Replace("{3}", "¤");
            config = config.Replace("{heart}", "♡");

            return config;
        }
    }
}