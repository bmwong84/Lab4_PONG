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

namespace IWKS_3400_Lab3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        clsSprite mySpriteBall;    // currently MemberBerry.png
        clsSprite playerPaddle;   // currently set to pong_paddle
        clsSprite enemyPaddle;   // also set to pong_paddle

    /*    SpriteFont Font;
        Color buttonColor = new Color(0,0,0);
        bool display = false;
        string buttonText = "Show Text";
        string text = "Hello world i just activated a button ";    */

            // Part of the Enum method
        Texture2D mControllerDetectScreenBakcground;
        Texture2D mTitleScreenBackground;
        Texture2D mPauseMenu;

        enum ScreenState
        {
            ControllerDetect,
            Title,
            Play,
            Pause
        }

        //Current Screen State
        ScreenState mCurrentScreen;

        PlayerIndex mPlayerOne;


        SpriteBatch spriteBatch;    /* group of sprites (with same settings) */

        
//        MainMenu main = new MainMenu(); 

        SoundEffect soundEffect;    // add a sound effect resource
        SoundEffect movingSoundEffect;

        int screenWidth = 1280; // Screen Width in pixels
        int screenHeight = 720;    //Screen Height in pixels
        int midSprite = 32;    // The middle of the sprite (This value is arbitrary but depends on clsSprite sizes)
        float gameVelocity = 5;  // The velocity of the ball after a collision with a paddle. Also the speed of the player

        AudioEngine audioEngine;    /* adjusts settings of device audio services */
        SoundBank soundBank;    /* colletion of sound cues we created */
        WaveBank waveBank;    /* collection of wav files */
        WaveBank streamingWaveBank;    /* background music wavebank */
        Cue cue;     /* class tool. need for certain methods */
        Cue musicCue; /* the cue to start music */

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);    /* handles the configuration and management of the graphics device */

            // changing back buffer size changes the window size (in windowed mode) (values are arbitrary within a range)
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;


            Content.RootDirectory = "Content";   /* naming our root dirctory to the name "Content" */
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //Load backgrounds
            mControllerDetectScreenBakcground = Content.Load<Texture2D>("ControllerDetectScreen");
            mTitleScreenBackground = Content.Load<Texture2D>("TitleScreen");
            mPauseMenu = Content.Load<Texture2D>("paused");

            //Initialize the current screen state to the screen we want to display first
            mCurrentScreen = ScreenState.ControllerDetect;

            //Load Menu
            //main.LoadContent(Content);

            

            // Load (2) 2D texture sprites <ball.png>
            mySpriteBall = new clsSprite(Content.Load<Texture2D>("ball32"),
                new Vector2(screenWidth - 32f, 0f), new Vector2(32f, 32f),
                screenWidth, screenHeight);//  (0,0) is top left of screen, (64, 64) is size of sprite 

            playerPaddle = new clsSprite(Content.Load<Texture2D>("pong_paddle"),     /* (218,118) represents the top right of screen,  64^2 is size */
               new Vector2(40f, 118f), new Vector2(10f, 64f),
               screenWidth, screenHeight);

            enemyPaddle = new clsSprite(Content.Load<Texture2D>("pong_paddle"),
                new Vector2(screenWidth - 40f, 118f), new Vector2(10f, 64f),
                screenWidth, screenHeight);

            // Load Sound Effect Resource
            soundEffect = Content.Load<SoundEffect>("LASER");
            movingSoundEffect = Content.Load<SoundEffect>("LASER");

            audioEngine = new AudioEngine("Content\\Lab3Sounds.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Sound Bank.xsb");

            streamingWaveBank = new WaveBank(audioEngine, "Content\\Music.xwb");

            audioEngine.Update();
//            musicCue = soundBank.GetCue("Galactic_Damages");
//            musicCue.Play();

            mySpriteBall.velocity = new Vector2(-5, 5);   /* setting initial x and y velocity (values arbitrary) */

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            // Free previously allocated resources
            mySpriteBall.texture.Dispose();
            playerPaddle.texture.Dispose();
            enemyPaddle.texture.Dispose();
            spriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            switch (mCurrentScreen)
            {
                case ScreenState.ControllerDetect:
                    {
                        UpdateControllerDeleterScreen();
                        break;
                    }
                case ScreenState.Title:
                    {
                        UpdateTitleScreen();
                        break;
                    }
                case ScreenState.Play:
                    {
                        //Move the sprite(s)
                        mySpriteBall.Move();

                        playerPaddle.PlayerMoveRestrition();
                        enemyPaddle.ChaseBallSprite(mySpriteBall);
                        // playerPaddle.ChaseBallSprite(mySpriteBall);    // This if for CPU vs CPU

                        keyBoardControls(playerPaddle);
                        // mouseControls(playerPaddle);   // This is for mouse control options
             

                        if (mySpriteBall.Collides(enemyPaddle))    /* calling our circle collide method for the two sprites */
                        {
                            rhsCPUCollision(mySpriteBall, enemyPaddle);
                            gameVelocity += 0.2f;
                            enemyPaddle.cpuChaseSpeed += 0.15f;

                            soundEffect.Play();
                        }

                        if (mySpriteBall.Collides(playerPaddle))
                        {
                            lhsPlayerCollision(mySpriteBall, playerPaddle); // Player vs CPU
                                                                            // lhsCPUCollision(mySpriteBall, playerPaddle);   // this is for CPU vs CPU
                            gameVelocity += 0.2f;
                            enemyPaddle.cpuChaseSpeed += 0.15f;

                            soundEffect.Play();
                        }

                        //Update the audio engine
                        audioEngine.Update();
                        // Check if the game is to be paused.
                        UpdatePlayScreen();
                        break;
                    }
                case ScreenState.Pause:
                    {
                        UpdateTitleScreen();
                        break;
                    }
            }



            base.Update(gameTime);    // updates graphic renders 
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);    /* sets the background fill color (im going against the grain with BurlyWood) */

            // TODO: Add your drawing code here

            //Draw the sprite using Alpha Blend, which uses transperency information if available
            // In 4.0, this behavior is the default; in XNA 3.1, it is not
            spriteBatch.Begin();
          //  main.Draw(spriteBatch);
          switch (mCurrentScreen)
            {
                case ScreenState.ControllerDetect:
                    {
                        DrawControllerDetectScreen();
                        break;
                    }
                case ScreenState.Title:
                    {
                        DrawTitleScreen();
                        break;
                    }
                case ScreenState.Play:
                    {
                        DrawGame();
                        break;
                    }
                case ScreenState.Pause:
                    {
                        DrawPauseMenu();
                        DrawGame();
                        break;
                    }
            }


            spriteBatch.End();

            base.Draw(gameTime);    /* draws the game recursively */
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       // Methods for Game1. Must be Private!! 

       // Pre: Game has started.  
       // Post: The method runs recursively as a part of the update method. Player alters Sprite Velocity
        private void keyBoardControls(clsSprite playerSprite)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                playerSprite.position += new Vector2(0, -gameVelocity);
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                playerSprite.position += new Vector2(0, gameVelocity);
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                playerSprite.position += new Vector2(0, 0);    // No x movement in pong
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                playerSprite.position += new Vector2(0, 0);    // No x movement in pong
            }
        }

        // Pre: Game has started and sprites are in motion.
        // Post: Method runs recursively in the update method. Sprite follows mouse Y position.
        private void mouseControls(clsSprite playerSprite)
        {
            if (playerPaddle.position.X < Mouse.GetState().X)
            {
                playerPaddle.position += new Vector2(0, 0);
            }
            if (playerPaddle.position.X > Mouse.GetState().X)
            {
                playerPaddle.position += new Vector2(0, 0);
            }
            if (playerPaddle.position.Y < Mouse.GetState().Y)
            {
                playerPaddle.position += new Vector2(0, gameVelocity);   // No x direction movement allowed
            }
            if (playerPaddle.position.Y > Mouse.GetState().Y)
            {
                playerPaddle.position += new Vector2(0, -gameVelocity);    // No X direction movement allowed
            }
        }

        //Pre: A collision has occured on the right hand side of the screen. 
        //Post: The velocity of the pong is adjusted based on where it hit the CPU paddle
        private void rhsCPUCollision(clsSprite lhs, clsSprite rhs)
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);

            lhs.position.X -= 1;  // prevents sprites getting stuck together
            if ((lhs.position.Y + midSprite) > (rhs.position.Y + midSprite))
            {
                lhs.velocity = new Vector2(-gameVelocity, gameVelocity);
            }
            if ((lhs.position.Y + midSprite) <= (rhs.position.Y + midSprite))
            {
                lhs.velocity = new Vector2(-gameVelocity, -gameVelocity);
            }
        }

        //Pre: A collision has occured on the left hand side of the screen. 
        //Post: The velocity of the pong is adjusted based on where it hit the CPU paddle
        private void lhsCPUCollision(clsSprite lhs, clsSprite rhs)
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);

            lhs.position.X += 1;  // prevents sprites getting stuck together
            if ((lhs.position.Y + lhs.halfSizeY) > (rhs.position.Y + rhs.halfSizeY))
            {
                lhs.velocity = new Vector2(gameVelocity, gameVelocity);
            }
            if ((lhs.position.Y + lhs.halfSizeY) <= (rhs.position.Y + rhs.halfSizeY))
            {
                lhs.velocity = new Vector2(gameVelocity, -gameVelocity);
            }
        }

        //Pre: A collision has occured between the pongBall and the left hand side player sprite
        //Post: modifies the velocity and direction based on player movements (if player isn't moving, position determines trajectory)
        private void lhsPlayerCollision(clsSprite lhs, clsSprite rhs)
        {
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            KeyboardState keyboardState = Keyboard.GetState();
             
            lhs.position.X += 1;    // This helps prevent sticking collision (ball direction ->)
            if (keyboardState.IsKeyDown(Keys.Up) || ((lhs.position.Y + lhs.halfSizeY) < (rhs.position.Y + rhs.halfSizeY)))  // if we are moving up, so will the ball
            {
                lhs.velocity = new Vector2(gameVelocity, -gameVelocity);
            }
            if (keyboardState.IsKeyDown(Keys.Down) || ((lhs.position.Y + lhs.halfSizeY) > (rhs.position.Y + rhs.halfSizeY)))   // if we are moving down, so will the ball
            {
                lhs.velocity = new Vector2(gameVelocity, gameVelocity);
            }
        }  

        //Pre: 
        //Post:
        private void UpdateControllerDeleterScreen()
        {
            //Poll all the gamepads (and the keyboard) to check to see
            //which controller will be the player one controller
            for (int i = 0; i < 2; i++)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A) == true)
                {
                    mPlayerOne = (PlayerIndex)i;
                    mCurrentScreen = ScreenState.Title;
                    return;       
                }
            }
        }

        //Pre:
        //Post:
        private void UpdateTitleScreen()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.B) == true )
            {
                mCurrentScreen = ScreenState.ControllerDetect;
                return;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) == true)
            {
                mCurrentScreen = ScreenState.Play;
                return;
            }
        }

        //Pre:
        //Post:
        private void UpdatePlayScreen()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P) == true)
            {
                mCurrentScreen = ScreenState.Pause;
            }
        }


        //Pre:  Controller Detect Screen is initialized. Method is called in Draw() and Update()
        //Post: Controller Screen is the screen state
        private void DrawControllerDetectScreen()
        {
            spriteBatch.Draw(mControllerDetectScreenBakcground, Vector2.Zero, Color.White);
        }

        //Pre: Background title initialized. Method is callied in Draw() and Update()
        //Post: Title Screen is the screen state
        private void DrawTitleScreen()
        {
            spriteBatch.Draw(mTitleScreenBackground, Vector2.Zero, Color.White);
        }

        private void DrawPauseMenu()
        {
            spriteBatch.Draw(mPauseMenu, Vector2.Zero, Color.White);
        }

        //Pre:
        //Post: Draws the in game sprites (players and objects) 
        private void DrawGame()
        {
            mySpriteBall.Draw(spriteBatch);
            playerPaddle.Draw(spriteBatch);
            enemyPaddle.Draw(spriteBatch);
        }
    }
}
