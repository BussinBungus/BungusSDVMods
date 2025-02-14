﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LostBookMenu
{
    public class BookMenu : IClickableMenu
    {
        public List<ClickableTextureComponent> currentBookList = new List<ClickableTextureComponent>();
        public int booksFound = Game1.netWorldState.Value.LostBooksFound;
        public const int totalLostBooks = 21;
        public static int lastID;
        
        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static void Init(IModHelper helper, IMonitor monitor) {
            Helper = helper;
            Monitor = monitor;
        }

        public BookMenu() : base(Game1.uiViewport.Width / 2 - (ModEntry.Config.WindowWidth + borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (ModEntry.Config.WindowHeight + borderWidth * 2) / 2, ModEntry.Config.WindowWidth + borderWidth * 2, ModEntry.Config.WindowHeight+ borderWidth * 2, false)
        {
            resetPositions();
        }

        private void resetPositions()
        {
            currentBookList = new List<ClickableTextureComponent>();
            populateBookList();
            snapToDefaultClickableComponent();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            xPositionOnScreen = Math.Max(0, Game1.uiViewport.Width / 2 - (ModEntry.Config.WindowWidth + borderWidth * 2) / 2);
            yPositionOnScreen = Math.Max(0, Game1.uiViewport.Height / 2 - (ModEntry.Config.WindowHeight + borderWidth * 2) / 2);
            width = Math.Min(Game1.uiViewport.Width, ModEntry.Config.WindowWidth + borderWidth * 2);
            height = Math.Min(Game1.uiViewport.Height, ModEntry.Config.WindowHeight + borderWidth * 2);

            resetPositions();
        }

        // adds each of the 21 lost books to the ClickableTextureComponent list
        private void populateBookList()
        {
            currentBookList.Clear();

            for (int i = 0; i < totalLostBooks; i++)
            {
                string id = i.ToString();

                // get cover texture data
                if (ModEntry.bookData.TryGetValue(id, out CoverData data))
                {
                    addBook(id, i, data);
                }
            }
            populateClickableComponentList();
        }

        private void addBook(string id, int i, CoverData data)
        {
            int gridX = i % ModEntry.Config.GridColumns;
            int gridY = i / ModEntry.Config.GridColumns;
            int coverWidth = (int)(ModEntry.Config.CoverScale * data.width);
            int coverHeight = (int)(ModEntry.Config.CoverScale * data.height);
            int xOffsetDefault = Math.Max(0, (width - (borderWidth * 2) - (coverWidth + ModEntry.Config.HorizontalSpace) * ModEntry.Config.GridColumns + ModEntry.Config.HorizontalSpace) / 2);
            int xOffset = borderWidth + (ModEntry.Config.xOffset == -1 ? xOffsetDefault : ModEntry.Config.xOffset);
            int yOffset = borderWidth + ModEntry.Config.yOffset;

            Rectangle textureBounds = new Rectangle(0, 0, data.texture.Width, data.texture.Height);
            if (data.frames > 1)
            {
                textureBounds.Size = new Point(data.frameWidth, data.texture.Height);
            }

            int ccX = xPositionOnScreen + xOffset + gridX * (coverWidth + ModEntry.Config.HorizontalSpace);
            int ccY = yPositionOnScreen + yOffset + gridY * (coverHeight + ModEntry.Config.VerticalSpace);
            currentBookList.Add(new ClickableTextureComponent(id, new Rectangle(ccX, ccY, coverWidth, coverHeight), "", data.title, data.texture, textureBounds, data.scale, false)
            {
                myID = i,
                upNeighborID = i - ModEntry.Config.GridColumns,
                downNeighborID = i + ModEntry.Config.GridColumns,
                leftNeighborID = i - 1,
                rightNeighborID = i + 1,
            });
        }
        public override void snapToDefaultClickableComponent()
        {
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;

            if (currentlySnappedComponent == null)
                setCurrentlySnappedComponentTo(lastID);

            snapCursorToCurrentSnappedComponent();
        }

        public override void performHoverAction(int x, int y)
        {
            this.upperRightCloseButton?.tryHover(x, y, 0.5f);

            foreach (var cc in currentBookList)
            {
                ModEntry.bookData.TryGetValue(cc.name, out CoverData data);
                var s = (cc.myID <= booksFound) ? data.title : ModEntry.SHelper.Translation.Get("MissingName");
                
                cc.tryHover(x, y);
                if (cc.containsPoint(x, y))
                {
                    cc.hoverText = s;
                }
                else if (!ModEntry.Config.LegacyTitles) { cc.hoverText = ""; }
            }
        }

        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true, null, false, false); // draw menu box
            SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.SHelper.Translation.Get("MenuName"), xPositionOnScreen + width/2, yPositionOnScreen + spaceToClearTopBorder, width - borderWidth * 4); // draw title scroll

            foreach (var cc in currentBookList)
            {
                int frameOffset = (ModEntry.bookData.TryGetValue(cc.name, out CoverData data) && data.frames > 1) ? (int)(Game1.currentGameTime.TotalGameTime.TotalSeconds / data.frameSeconds) % data.frames : 0;

                // draw book covers
                cc.draw(b, (cc.myID <= booksFound) ? Color.White : Color.DimGray * 0.1f, 1, frameOffset);
                cc.draw(b, (cc.myID <= booksFound) ? Color.White : Color.Black * 0.2f, 1, frameOffset);

            }
            // draw book titles
            foreach (var cc in currentBookList)
            {
                if (ModEntry.Config.LegacyTitles) // old titles under covers
                {
                    var s = (cc.myID <= booksFound) ? cc.hoverText : ModEntry.SHelper.Translation.Get("MissingName");
                    var scale = 1f;
                    var split = s.Split(' ');
                    int lines = 0;
                    for (int i = 0; i < split.Length; i++)
                    {
                        string str = split[i];
                        if (i < split.Length - 1 && Game1.smallFont.MeasureString(str + " " + split[i + 1]).X < cc.bounds.Width * 1.5f)
                        {
                            str += " " + split[i + 1];
                            i++;
                        }
                        var m = Game1.smallFont.MeasureString(str) * scale;
                        //var y = cc.bounds.Y + cc.bounds.Height + (int)(m.Y * (lines * 0.8f + 0.5f));
                        var y = cc.bounds.Y + cc.bounds.Height + (int)(m.Y * (lines * 0.8f + 0.1f));

                        //b.DrawString(Game1.smallFont, str, new Vector2(cc.bounds.X + (cc.bounds.Width - m.X) / 2 - 1, y + 1), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
                        //b.DrawString(Game1.smallFont, str, new Vector2(cc.bounds.X + (cc.bounds.Width - m.X) / 2, y), Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
                        Utility.drawTextWithShadow(b, str, Game1.smallFont, new Vector2(cc.bounds.X + (cc.bounds.Width - m.X) / 2, y), Game1.textColor);
                        lines++;
                    }
                }
                else // new tooltip titles
                {
                    if (!cc.hoverText.Equals(""))
                    {
                        drawHoverText(b, cc.hoverText, Game1.smallFont);
                    }
                }
            }
            drawMouse(b);
            base.draw(b);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            for (int i = 0; i < totalLostBooks; i++)
            {
                var cc = currentBookList[i];
                if(cc.containsPoint(x, y))
                {
                    lastID = i; // allows the menu to remember the last selection next time it's opened

                    Game1.playSound("bigSelect");
                    string book = Game1.content.LoadString("Strings\\Notes:" + i).Replace('\n', '^');
                    string bookTitle = cc.hoverText;
                    string missing = Game1.parseText(Game1.content.LoadString("Strings\\Notes:Missing"));
                    if (i <= booksFound)
                    {
                        Game1.player.mailReceived.Add("lb_" + i);
                        if (Game1.player.currentLocation.Name == "ArchaeologyHouse")
                        {
                            Game1.player.currentLocation.removeTemporarySpritesWithIDLocal(i);
                        }
                        addVPPBuffs(i);
                        Game1.activeClickableMenu = new ViewerLetter(book, bookTitle, !Game1.mailbox.Contains("lb_" + i));
                    }
                    else
                    {
                        Game1.activeClickableMenu = new ViewerDialogue(missing);
                    }
                    return;
                }
            }
            base.receiveLeftClick(x, y, playSound);
        }
        public void addVPPBuffs(int bookID)
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