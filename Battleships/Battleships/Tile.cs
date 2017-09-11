using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Battleships
{
    class Tile
    {
        Texture2D texture;
        public Vector2 position;
        public Rectangle boundingbox;
        public bool hit, contains_ship;
        MouseState mouse_state, old_mouse_state;
        Color color;

        public Tile(Texture2D texture, Vector2 position, Color player_color)
        {
            this.texture = texture;
            this.position = position;
            color = player_color;
            boundingbox = new Rectangle((int)position.X, (int)position.Y, 50, 50);
        }

        public void Update()
        {
            old_mouse_state = mouse_state;
            mouse_state = Mouse.GetState();
            if (boundingbox.Contains(new Point(mouse_state.X, mouse_state.Y)) && mouse_state.LeftButton == ButtonState.Pressed && old_mouse_state.LeftButton == ButtonState.Released && hit == false)
            {
                hit = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color water_color = new Color(86, 156, 214);
            spriteBatch.Draw(texture, position, new Rectangle(200, 150, 50, 50), water_color);
            spriteBatch.Draw(texture, position, new Rectangle(200, 200, 50, 50), color);
        }
    }
}