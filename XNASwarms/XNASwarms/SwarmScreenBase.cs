﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ScreenSystem.ScreenSystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using XNASwarms.Borders;
using XNASwarms.Borders.Walls;

namespace XNASwarms
{
    class SwarmScreenBase : GameScreen
    {
        protected Recipe[] recipes;
        protected PopulationSimulator populationSimulator;
        Random rand;
        Dictionary<int, Individual> Supers;
        public int width, height;
        Texture2D superAgentTexture;
        protected SwarmsCamera Camera;
        protected Border Border;
        private float TimePerFrame;
        private int FramesPerSec;

        public SwarmScreenBase()
        {
            FramesPerSec = 14;
            TimePerFrame = (float)1 / FramesPerSec;
        }

        public override void LoadContent()
        {
            width = ScreenManager.GraphicsDevice.Viewport.Width/2;
            height = ScreenManager.GraphicsDevice.Viewport.Height;
            Camera = new SwarmsCamera(ScreenManager.GraphicsDevice);
            superAgentTexture = ScreenManager.Content.Load<Texture2D>("Backgrounds/gray");
            populationSimulator = new PopulationSimulator(0, 0, recipes);
            Supers = new Dictionary<int, Individual>();
            rand = new Random();
            Border = new Border(this, WallFactory.FourPortal(800, 400, 1), ScreenManager);
            foreach (Individual ind in populationSimulator.getPopulation())
            {
                //ind.getGenome().inducePointMutations(rand.NextDouble(), 2);
                //ind.getGenome().inducePointMutations(rand.NextDouble(), 3);
            }
            Supers.Add(0, new Individual());

            base.LoadContent();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            Border.Update(populationSimulator.getSwarmInBirthOrder().ToList<Individual>());
            populationSimulator.stepSimulation(Supers.Values.ToList<Individual>(), 20);   
            Camera.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

            for (int i = 0; i < Supers.Count; i++)
            {
                Vector2 position = Camera.ConvertScreenToWorldAndDisplayUnits(new Vector2((int)Supers[i].getX(), (int)Supers[i].getY()));

                ScreenManager.SpriteBatch.Draw(superAgentTexture, new Rectangle(
                    (int)Supers[i].getX(),
                    (int)Supers[i].getY(), 6, 6),
                    null,
                    Color.Red,
                    0f,
                    new Vector2(3, 3),
                    SpriteEffects.None, 0);
            }

            Border.Draw(ScreenManager.SpriteBatch, Camera);

            base.Draw(gameTime);
        }

        public override void HandleInput(InputHelper input, Microsoft.Xna.Framework.GameTime gameTime)
        {
            
            Supers.Clear();
            
            var surfacetouches = input.SurfaceTouches;
            if (surfacetouches.Count > 0)
            {
                //Surface
                for (int i = 0; i < surfacetouches.Count; i++)
                {
                    Vector2 position = Camera.ConvertScreenToWorldAndDisplayUnits(new Vector2(surfacetouches[i].X, surfacetouches[i].Y));

                    Supers[i] = new Individual(((double)position.X),
                         ((double)position.Y),
                         0.0, 0.0, new Parameters());
                }
            }
            else if (input.Touches.Count > 0)
            {
                //Everthing else touch
                for (int i = 0; i < input.Touches.Count; i++)
                {
                    Vector2 position = Camera.ConvertScreenToWorldAndDisplayUnits(new Vector2(input.Touches[i].Position.X, input.Touches[i].Position.Y));

                    Supers[i] = new Individual(((double)position.X),
                         ((double)position.Y),
                         0.0, 0.0, new Parameters());
                }
            }
            else if(Supers.Count <= 0)
            {
                //Mouse
                Supers.Add(0, new Individual());
                for (int i = 0; i < Supers.Count; i++)
                {
                    Vector2 position = Camera.ConvertScreenToWorldAndDisplayUnits(input.Cursor);

                    Supers[i] = new Individual(((double)position.X),
                         ((double)position.Y),
                         0,0, new Parameters());
                }
            }

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();

                switch (gesture.GestureType)
                {
                    case GestureType.Pinch:
                        float scaleFactor = PinchZoom.GetScaleFactor(gesture.Position, gesture.Position2,
                            gesture.Delta, gesture.Delta2);
                        //Vector2 panDelta = PinchZoom.GetTranslationDelta(gesture.Position, gesture.Position2,
                        //    gesture.Delta, gesture.Delta2, spritePos, scaleFactor);
                        Camera.Zoom *= scaleFactor;
                        //spritePos += panDelta;

                        // Or, rather than doing it manually, you can just call ApplyPinchZoom instead which does the
                        // above work for you.
                        /*PinchZoom.ApplyPinchZoom(gesture, ref spritePos, ref spriteScale);*/
                        break;
                }
            }

            //for(int i = 0; i < 45; i ++)
            //{
            //    var pos = Camera.ConvertScreenToWorldAndDisplayUnits(new Vector2(40*i,10));
            //    var posb = Camera.ConvertScreenToWorldAndDisplayUnits(new Vector2(40 * i,920));
            //    var pos1 = Camera.ConvertScreenToWorldAndDisplayUnits(new Vector2(10, 40*i));
            //    var pos1b = Camera.ConvertScreenToWorldAndDisplayUnits(new Vector2(1665, 40 * i));

            //    Supers.Add(Supers.Count, new Individual(pos.X, pos.Y, 0, 0, new Parameters(0, 0, 0, 0, 0, 100, 0, 1)));
            //    Supers.Add(Supers.Count, new Individual(posb.X, posb.Y, 0, 0, new Parameters(0, 0, 0, 0, 0, 100, 0, 1)));
            //    Supers.Add(Supers.Count, new Individual(pos1.X, pos1.Y, 0, 0, new Parameters(0, 0, 0, 0, 0, 100, 0, 1)));
            //    Supers.Add(Supers.Count, new Individual(pos1b.X, pos1b.Y, 0, 0, new Parameters(0, 0, 0, 0, 0, 100, 0, 1)));
            //}
            

            HandleCamera(input, gameTime);
            
        }

        //private void UpdateSupers(ReadOnlyTouchPointCollection touches, InputHelper input)
        //{
         

        //    for (int i = 0; i < touches.Count; i++)
        //    {
        //        Vector2 position = Camera.ConvertScreenToWorldAndDisplayUnits(new Vector2(touches[i].X,touches[i].Y));

        //        Supers[i] = new Individual(((double)position.X),
        //             ((double)position.Y),
        //             0.0, 0.0, new Parameters());
        //    }
        //}

        private void HandleCamera(InputHelper input, GameTime gameTime)
        {
            Vector2 camMove = Vector2.Zero;
            float MoveSpeed = 100f;

            if (input.KeyboardState.IsKeyDown(Keys.Up))
            {
                camMove.Y -= MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (input.KeyboardState.IsKeyDown(Keys.Down))
            {
                camMove.Y += MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (input.KeyboardState.IsKeyDown(Keys.Left))
            {
                camMove.X -= MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (input.KeyboardState.IsKeyDown(Keys.Right))
            {
                camMove.X += MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (camMove != Vector2.Zero)
            {
                Camera.MoveCamera(camMove);
            }
            if (input.KeyboardState.IsKeyDown(Keys.OemPlus))
            {
                Camera.Zoom += 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;
            }
            if (input.KeyboardState.IsKeyDown(Keys.OemMinus))
            {
                Camera.Zoom -= 5f * (float)gameTime.ElapsedGameTime.TotalSeconds * Camera.Zoom / 20f;
            }
            if (input.IsNewKeyPress(Keys.Escape))
            {
                Camera.ResetCamera();
            }
        }
    }
}
