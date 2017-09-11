using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Battleships
{
    class GameField
    {
        public Vector2 field_position;
        public bool turn_finished;
        public int player_health, shots_fired;
        public Rectangle grid;

        Vector2 tile_position;
        Texture2D spritesheet;
        Point mouse_position;
        bool placement, holding_ship;
        string name;
        Color color;
        MouseState mouse_state, old_mouse_state;
        KeyboardState key_state, old_key_state;
        Tile[,] tiles;
        Ship[] ships;
        Ship temp_ship;

        int temp_posX;
        int temp_posY;
        int temp_width;
        int temp_height;
        float temp_rot;
        Vector2 temp_origin;


        public GameField(Vector2 field_pos, ContentManager content, Color player_color, string player_name)
        {
            placement = false;
            turn_finished = false;
            spritesheet = content.Load<Texture2D>(@"battleship");
            field_position = field_pos;
            player_health = 17;
            name = player_name;
            color = player_color;

            tiles = new Tile[10, 10];
            ships = new Ship[5];

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tile_position.X = i * 50 + field_position.X;
                    tile_position.Y = j * 50 + field_position.Y;
                    tiles[i, j] = new Tile(spritesheet, tile_position, color);
                    grid = new Rectangle((int)tiles[0, 0].position.X, (int)tiles[0, 0].position.Y, j * 50 + 50, i * 50 + 50);
                }
            }

            for (int i = 0; i < ships.GetLength(0); i++)
            {
                int ship_size = 250 - ((int)(i - Math.Floor(i * 0.4)) * 50);
                ships[i] = new Ship(spritesheet, tiles[0, i].position, new Rectangle(0, i * 50, ship_size, 50));
            }
        }

        public bool shipPlacement()
        {
            return placement;
        }

        public bool turnFinished
        {
            get { return turn_finished; }
            set { turn_finished = value; }
        }

        private void PlacingShips()
        {
            if (mouse_state.LeftButton == ButtonState.Pressed)
            {
                for (int k = 0; k < ships.GetLength(0) && holding_ship == false; k++)
                {
                    if (ships[k].boundingbox.Contains(mouse_position))
                    {
                        holding_ship = true;
                        temp_ship = ships[k];
                        temp_posX = (int)ships[k].position.X;
                        temp_posY = (int)ships[k].position.Y;
                        temp_width = ships[k].boundingbox.Width;
                        temp_height = ships[k].boundingbox.Height;
                        temp_rot = ships[k].rotation;
                        temp_origin = ships[k].origin;
                    }
                }
                if (holding_ship == true)
                {
                    temp_ship.position.X = mouse_position.X - 25;
                    temp_ship.position.Y = mouse_position.Y - 25;
                    temp_ship.boundingbox.X = mouse_position.X - 25;
                    temp_ship.boundingbox.Y = mouse_position.Y - 25;

                    if (key_state.IsKeyDown(Keys.Q) && old_key_state.IsKeyUp(Keys.Q))
                    {
                        temp_ship.rotation = (float)Math.PI / 2.0f;
                        temp_ship.origin = new Vector2(0, 50);
                        temp_ship.boundingbox = new Rectangle(mouse_position.X - 25, mouse_position.Y - 25, temp_ship.sheet_rectangle.Height, temp_ship.sheet_rectangle.Width);
                    }
                    else if (key_state.IsKeyDown(Keys.W) && old_key_state.IsKeyUp(Keys.W))
                    {
                        temp_ship.rotation = 0f;
                        temp_ship.origin = new Vector2(0, 0);
                        temp_ship.boundingbox = new Rectangle(mouse_position.X - 25, mouse_position.Y - 25, temp_ship.sheet_rectangle.Width, temp_ship.sheet_rectangle.Height);
                    }
                }
            }
            else if (mouse_state.LeftButton == ButtonState.Released && holding_ship == true)
            {
                for (int i = 0; i < tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < tiles.GetLength(1); j++)
                    {
                        if (grid.Intersects(temp_ship.boundingbox))
                        {
                            if (tiles[j, i].boundingbox.Contains(mouse_position))
                            {
                                temp_ship.position.X = tiles[j, i].position.X;
                                temp_ship.position.Y = tiles[j, i].position.Y;
                                temp_ship.boundingbox.X = (int)temp_ship.position.X;
                                temp_ship.boundingbox.Y = (int)temp_ship.position.Y;
                                foreach (Ship s in ships)
                                {
                                    if (!grid.Contains(temp_ship.boundingbox) || s.boundingbox.Intersects(temp_ship.boundingbox) && s != temp_ship)
                                    {
                                        ResetShip();
                                    }
                                }
                            }
                            else if (!grid.Contains(mouse_position))
                            {
                                ResetShip();
                            }
                        }
                        else
                        {
                            ResetShip();
                        }
                    }
                }
                holding_ship = false;
                temp_ship = null;
            }
            if (key_state.IsKeyDown(Keys.Space) && old_key_state.IsKeyUp(Keys.Space))
            {
                placement = true;
            }
        }

        private void ResetShip()
        {
            temp_ship.position.X = temp_posX;
            temp_ship.position.Y = temp_posY;
            temp_ship.rotation = temp_rot;
            temp_ship.boundingbox = new Rectangle(temp_posX, temp_posY, temp_width, temp_height);
            temp_ship.origin = temp_origin;
        }

        private void ShootingShips()
        {
            if (mouse_state.LeftButton == ButtonState.Pressed && old_mouse_state.LeftButton == ButtonState.Released && turn_finished == false && grid.Contains(mouse_position))
            {
                for (int i = 0; i < tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < tiles.GetLength(1); j++)
                    {
                        for (int k = 0; k < ships.GetLength(0); k++)
                        {
                            if (tiles[j, i].boundingbox.Contains(mouse_position) && tiles[j, i].boundingbox.Intersects(ships[k].boundingbox) && tiles[j, i].hit == false)
                            {
                                ships[k].health -= 1;
                                player_health -= 1;
                                shots_fired += 1;
                                tiles[j, i].hit = true;
                                tiles[j, i].contains_ship = true;
                                turn_finished = true;
                            }
                        }
                        if (tiles[j, i].boundingbox.Contains(mouse_position) && tiles[j, i].hit == false)
                        {
                            shots_fired += 1;
                            tiles[j, i].hit = true;
                            tiles[j, i].contains_ship = false;
                            turn_finished = true;
                        }
                    }
                }             
            }
        }

        public void Update()
        {
            old_key_state = key_state;
            key_state = Keyboard.GetState();
            old_mouse_state = mouse_state;
            mouse_state = Mouse.GetState();
            mouse_position = new Point(mouse_state.X, mouse_state.Y);

            if (placement == false)
            {
                PlacingShips();
            }
            else if (placement == true)
            {
                ShootingShips();
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.DrawString(spriteFont, name, new Vector2(grid.X+(grid.Width/2-spriteFont.MeasureString(name).Length() / 2), grid.Y-30), color);
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[j, i].Draw(spriteBatch);
                }
            }
            for (int i = 0; i < ships.GetLength(0); i++)
            {
                if (placement == false || key_state.IsKeyDown(Keys.A) || ships[i].health == 0)
                {
                    ships[i].Draw(spriteBatch);
                }
            }
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tiles[j, i].hit == true)
                    {
                        if (tiles[j, i].contains_ship == true)
                        {
                            spriteBatch.Draw(spritesheet, tiles[j, i].position, new Rectangle(150, 100, 50, 50), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(spritesheet, tiles[j, i].position, new Rectangle(200, 100, 50, 50), Color.White);
                        }
                    }
                }
            }

        }
    }
}
