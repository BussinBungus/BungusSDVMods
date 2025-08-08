using StardewValley;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using HolesAndFalling.Config;

namespace HolesAndFalling.Framework
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

        public static void playSoundAfterDelayHandler(string soundName, int delay, GameLocation location = null, Vector2? position = null, int pitch = -1, bool local = false)
        {
            delay = Config.cornerMessage ? delay - 800 : delay;
            DelayedAction.playSoundAfterDelay(soundName, delay, location, position, pitch, local);
        }
        public static void globalFadeToBlackHandler(Game1.afterFadeFunction afterFade, float fadeSpeed)
        {
            if (Config.cornerMessage)
            {
                afterFade.Invoke();
            }
            else
            {
                Game1.globalFadeToBlack(afterFade, fadeSpeed);
            }
        }
        public static void storeFarmerHealthHandler(Farmer farmer, int targetHealth)
        {
            farmer.health = Config.softFall ? Game1.player.health : targetHealth;
        }
        public static void drawObjectDialogueHandler(string message)
        {
            if (Config.cornerMessage)
            {
                Game1.addHUDMessage(HUDMessage.ForCornerTextbox(message));
            }
            else
            {
                Game1.drawObjectDialogue(message);
            }
        }
        public static void messagePauseHandler(bool value)
        {
            Game1.messagePause = Config.cornerMessage ? Game1.messagePause : value;
        }
        public static void set_fadeToBlackAlphaHandler(float alpha)
        {
            Game1.fadeToBlackAlpha = Config.cornerMessage ? Game1.fadeToBlackAlpha : alpha;
        }
        public static bool recursiveTryToCreateHoleDown(Vector2 centerTile, MineShaft location, int maxIterations = 16)
        {
            int iterations = 0;
            Queue<Vector2> positionsToCheck = new Queue<Vector2>();
            positionsToCheck.Enqueue(centerTile);
            List<Vector2> closedList = new List<Vector2>();
            for (; iterations < maxIterations; iterations++)
            {
                if (positionsToCheck.Count <= 0)
                {
                    break;
                }
                Vector2 currentPoint = positionsToCheck.Dequeue();
                closedList.Add(currentPoint);
                if (!location.IsTileOccupiedBy(currentPoint) && location.isTileOnClearAndSolidGround(currentPoint) && location.doesTileHaveProperty((int)currentPoint.X, (int)currentPoint.Y, "Type", "Back") != null && location.doesTileHaveProperty((int)currentPoint.X, (int)currentPoint.Y, "Type", "Back").Equals("Stone"))
                {
                    location.createLadderDown((int)currentPoint.X, (int)currentPoint.Y, true);
                    doCreateHoleExtras(currentPoint, location);
                    return true;
                }
                Vector2[] directionsTileVectors = Utility.DirectionsTileVectors;
                foreach (Vector2 v in directionsTileVectors)
                {
                    if (!closedList.Contains(currentPoint + v))
                    {
                        positionsToCheck.Enqueue(currentPoint + v);
                    }
                }
            }
            return false;
        }
        public static void doCreateHoleExtras(Vector2 p, MineShaft l)
        {
            if (Game1.currentLocation == l) { l.playSound("hoeHit"); }

            string? startSound = Game1.currentLocation == l ? "sandyStep" : null;
            l.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f, Color.White * 0.5f)
            {
                interval = 80f
            });
            l.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f - new Vector2(16f, 16f), Color.White * 0.5f)
            {
                delayBeforeAnimationStart = 150,
                interval = 80f,
                scale = 0.75f,
                startSound = startSound
            });
            l.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f + new Vector2(32f, 16f), Color.White * 0.5f)
            {
                delayBeforeAnimationStart = 300,
                interval = 80f,
                scale = 0.75f,
                startSound = startSound
            });
            l.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f - new Vector2(32f, -16f), Color.White * 0.5f)
            {
                delayBeforeAnimationStart = 450,
                interval = 80f,
                scale = 0.75f,
                startSound = startSound
            });
            l.temporarySprites.Add(new TemporaryAnimatedSprite(5, p * 64f - new Vector2(-16f, 16f), Color.White * 0.5f)
            {
                delayBeforeAnimationStart = 600,
                interval = 80f,
                scale = 0.75f,
                startSound = startSound
            });
        }
    }
}