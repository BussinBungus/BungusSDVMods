using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LostBookMenu.Framework
{
    public class CoverData
    {
        public string texturePath;
        public string title;
        public float scale;
        public int width = 16;
        public int height = 16;
        public int frames = 1;
        public int frameWidth;
        public float frameSeconds;
        [JsonIgnore]
        public Texture2D texture;
    }
}