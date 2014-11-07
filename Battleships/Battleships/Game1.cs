using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Battleships
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Texture2D logo;
        Vector2 logo_pos;
        GameField[] gamefield;
        KeyboardState key_state, old_key_state;
        MouseState mouse_state, old_mouse_state;
        Color player_color;
        string player_name, start_game, exit_game, change_res;
        Vector2 start_pos, exit_pos, resolution_pos;

        enum GameState { GameMenu, PlayerOnePlacement, PlayerTwoPlacement, PlayerOne, PlayerTwo }
        GameState currentGameState = GameState.GameMenu;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 780;
            IsMouseVisible = true;
        }


        protected override void Initialize()
        { base.Initialize(); }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("SpriteFont1");
            logo = Content.Load<Texture2D>(@"battleshiplogo");
            gamefield = new GameField[2];
            start_game = "Start new game";
            exit_game = "Exit game";
            change_res = "Change window width";
            Positions();
            CreateGameFields();
        }


        protected override void UnloadContent()
        { }


        private void CreateGameFields()
        {
            for (int i = 0; i < gamefield.Length; i++)
            {
                if (i == 0)
                {
                    player_color = new Color(86, 156, 214);
                    player_name = "PLAYER ONE";
                }
                else if (i == 1)
                {
                    player_color = new Color(214, 111, 111);
                    player_name = "PLAYER TWO";
                }
                int posX = i * (Window.ClientBounds.Width - 600);
                gamefield[i] = new GameField(new Vector2(posX + 50, 50), this.Content, player_color, player_name);
            }
        }


        private void Positions()
        {
            start_pos = new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(start_game).Length() / 2, 400);
            resolution_pos = new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(change_res).Length() / 2, 450);
            exit_pos = new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(exit_game).Length() / 2, 500);
            logo_pos = new Vector2(GraphicsDevice.Viewport.Width / 2 - logo.Width / 2, 200);
        }


        private void MenuUpdate()
        {
            Point mouse_position = new Point(mouse_state.X, mouse_state.Y);
            if (mouse_state.LeftButton == ButtonState.Pressed && old_mouse_state.LeftButton == ButtonState.Released)
            {
                Rectangle start_rect = new Rectangle((int)start_pos.X, (int)start_pos.Y, (int)spriteFont.MeasureString(start_game).Length(), 20);
                Rectangle exit_rect = new Rectangle((int)exit_pos.X, (int)exit_pos.Y, (int)spriteFont.MeasureString(exit_game).Length(), 20);
                Rectangle resolution_rect = new Rectangle((int)resolution_pos.X, (int)resolution_pos.Y, (int)spriteFont.MeasureString(change_res).Length(), 20);
                if (start_rect.Contains(mouse_position))
                {
                    currentGameState = GameState.PlayerOnePlacement;
                }
                else if (resolution_rect.Contains(mouse_position))
                {
                    if (graphics.PreferredBackBufferWidth == 1280)
                    {
                        graphics.PreferredBackBufferWidth = 1600;
                    }
                    else if (graphics.PreferredBackBufferWidth == 1600)
                    {
                        graphics.PreferredBackBufferWidth = 1280;
                    }
                    graphics.ApplyChanges();
                    Positions();
                }
                else if (exit_rect.Contains(mouse_position))
                {
                    this.Exit();
                }
            }
        }


        protected override void Update(GameTime gameTime)
        {
            old_key_state = key_state;
            key_state = Keyboard.GetState();
            old_mouse_state = mouse_state;
            mouse_state = Mouse.GetState();

            if (key_state.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (gamefield[0].player_health == 0 || gamefield[1].player_health == 0)
            {
                if (key_state.IsKeyDown(Keys.R) && old_key_state.IsKeyUp(Keys.R))
                {
                    currentGameState = GameState.GameMenu;
                    Array.Clear(gamefield, 0, gamefield.Length);
                    CreateGameFields();
                }
            }

            switch (currentGameState)
            {
                case GameState.GameMenu:
                    MenuUpdate();
                    break;

                case GameState.PlayerOnePlacement:
                    gamefield[0].Update();
                    if (gamefield[0].shipPlacement() == true && key_state.IsKeyDown(Keys.D) && old_key_state.IsKeyUp(Keys.D))
                    {
                        currentGameState = GameState.PlayerTwoPlacement;
                    }
                    break;

                case GameState.PlayerTwoPlacement:
                    gamefield[1].Update();
                    if (gamefield[1].shipPlacement() == true && key_state.IsKeyDown(Keys.D) && old_key_state.IsKeyUp(Keys.D))
                    {
                        currentGameState = GameState.PlayerOne;
                    }
                    break;

                case GameState.PlayerOne:
                    if (gamefield[1].player_health != 0)
                    {
                        if (gamefield[1].turnFinished == false)
                        {
                            gamefield[1].Update();
                        }
                        else if (gamefield[1].turnFinished == true && key_state.IsKeyDown(Keys.Space) && old_key_state.IsKeyUp(Keys.Space))
                        {
                            gamefield[1].turnFinished = false;
                            currentGameState = GameState.PlayerTwo;
                        }
                    }
                    break;

                case GameState.PlayerTwo:
                    if (gamefield[0].player_health != 0)
                    {
                        if (gamefield[0].turnFinished == false)
                        {
                            gamefield[0].Update();
                        }
                        else if (gamefield[0].turnFinished == true && key_state.IsKeyDown(Keys.Space) && old_key_state.IsKeyUp(Keys.Space))
                        {
                            gamefield[0].turnFinished = false;
                            currentGameState = GameState.PlayerOne;
                        }
                    }
                    break;
            }
            base.Update(gameTime);
        }


        private void MenuDraw()
        {
            spriteBatch.Draw(logo, logo_pos, Color.White);
            spriteBatch.DrawString(spriteFont, start_game, start_pos, Color.White);
            spriteBatch.DrawString(spriteFont, change_res, resolution_pos, Color.White);
            spriteBatch.DrawString(spriteFont, exit_game, exit_pos, Color.White);
        }


        private void InGameDraw()
        {
            gamefield[0].Draw(spriteBatch, spriteFont);
            gamefield[1].Draw(spriteBatch, spriteFont);

            if (currentGameState != GameState.PlayerOnePlacement && currentGameState != GameState.PlayerTwoPlacement)
            {
                spriteBatch.DrawString(spriteFont, @"Total shots fired: " + gamefield[0].shots_fired, new Vector2(gamefield[1].grid.X, gamefield[1].grid.Bottom + 50), Color.White);
                spriteBatch.DrawString(spriteFont, @"Total shots fired: " + gamefield[1].shots_fired, new Vector2(gamefield[0].grid.X, gamefield[0].grid.Bottom + 50), Color.White);
                for (int i = 0; i < gamefield.GetLength(0); i++)
                {
                    string shot = @"Shot fired, press 'Space' to continue.";
                    if (gamefield[i].turn_finished == true && gamefield[i].player_health != 0)
                    {
                        spriteBatch.DrawString(spriteFont, shot, new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(shot).Length() / 2, gamefield[i].grid.Bottom + 100), Color.White);
                    }
                }
            }
        }


        private void GameOverDraw()
        {
            if (gamefield[0].player_health == 0 || gamefield[1].player_health == 0)
            {
                string to_menu = @"Press R to return to the menu.";
                spriteBatch.DrawString(spriteFont, to_menu, new Vector2(GraphicsDevice.Viewport.Width / 2 - spriteFont.MeasureString(to_menu).Length() / 2, gamefield[0].grid.Bottom + 150), Color.White);
                if (gamefield[0].player_health == 0)
                {
                    spriteBatch.DrawString(spriteFont, @"Loser!", new Vector2(gamefield[0].grid.X, gamefield[0].grid.Bottom + 20), Color.White);
                    spriteBatch.DrawString(spriteFont, @"Winner!", new Vector2(gamefield[1].grid.X, gamefield[1].grid.Bottom + 20), Color.White);
                }
                else if (gamefield[1].player_health == 0)
                {
                    spriteBatch.DrawString(spriteFont, @"Loser!", new Vector2(gamefield[1].grid.X, gamefield[1].grid.Bottom + 20), Color.White);
                    spriteBatch.DrawString(spriteFont, @"Winner!", new Vector2(gamefield[0].grid.X, gamefield[0].grid.Bottom + 20), Color.White);
                }
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            Color background = new Color(35, 35, 35);
            GraphicsDevice.Clear(background);
            spriteBatch.Begin();
            string place_ships = "Press 'Q' to turn the ship vertically and 'W' to turn it horizontally\nPress 'Space' to lock ships in place.";
            string switch_player = "Press 'D' to continue.";

            switch (currentGameState)
            {
                case GameState.GameMenu:
                    MenuDraw();
                    break;

                case GameState.PlayerOnePlacement:
                    InGameDraw();
                    if (gamefield[0].shipPlacement() == false)
                        spriteBatch.DrawString(spriteFont, place_ships, new Vector2(gamefield[0].grid.X, gamefield[0].grid.Bottom + 100), Color.White);
                    else
                        spriteBatch.DrawString(spriteFont, switch_player, new Vector2(gamefield[0].grid.X, gamefield[0].grid.Bottom + 100), Color.White);
                    break;

                case GameState.PlayerTwoPlacement:
                    InGameDraw();
                    if (gamefield[1].shipPlacement() == false)
                        spriteBatch.DrawString(spriteFont, place_ships, new Vector2(gamefield[1].grid.X, gamefield[1].grid.Bottom + 100), Color.White);
                    else
                        spriteBatch.DrawString(spriteFont, switch_player, new Vector2(gamefield[1].grid.X, gamefield[1].grid.Bottom + 100), Color.White);
                    break;

                case GameState.PlayerOne:
                    InGameDraw();
                    GameOverDraw();
                    break;

                case GameState.PlayerTwo:
                    InGameDraw();
                    GameOverDraw();
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}