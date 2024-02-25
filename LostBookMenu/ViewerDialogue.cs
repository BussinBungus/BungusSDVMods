using StardewValley;
using StardewValley.Menus;

namespace LostBookMenu
{
    internal class ViewerDialogue : DialogueBox
    {
        public ViewerDialogue(string dialogue) : base(dialogue)
        {
            Game1.afterDialogues = delegate
            {
                Game1.activeClickableMenu = new BookMenu();
            };
        }
    }
}