using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Battleships
{
    class Ship
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle sheet_rectangle;
        public Rectangle boundingbox;
        public float rotation;
        public Vector2 origin;
        public int health;

        public Ship(Texture2D texture, Vector2 position, Rectangle sheet_rectangle)
        {
            this.texture = texture;
            this.position = position;
            this.sheet_rectangle = sheet_rectangle;
            boundingbox = new Rectangle((int)position.X, (int)position.Y, this.sheet_rectangle.Width, this.sheet_rectangle.Height);
            health = sheet_rectangle.Width / 50;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sheet_rectangle, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
