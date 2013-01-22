﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystem.ScreenSystem;
using XNASwarms.Borders.Walls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XnxSwarmsData.Debug;

namespace XNASwarms.Borders
{
    public class Border
    {

        private GameScreen gameScreen;
        private List<Wall> borderWalls;
        Texture2D borderTexture;
        int rightBound, bottomBound;
        private IDebugScreen debugScreen; 

        public Border(GameScreen gamescreen, List<Wall> borderwalls, ScreenManager screenmanger)
        {
            debugScreen = gamescreen.ScreenManager.Game.Services.GetService(typeof(IDebugScreen)) as IDebugScreen;

            gameScreen = gamescreen;
            borderWalls = borderwalls;
            borderTexture = screenmanger.Content.Load<Texture2D>("Backgrounds/gray");

            rightBound = borderWalls.Where(s => s.GetWallOrientation() == WallOrientationType.Horizontal).First().GetLength();
            bottomBound = borderWalls.Where(s => s.GetWallOrientation() == WallOrientationType.Vertical).First().GetLength();
        }

        public void LoadContent()
        {
            
        }

        public void Update(List<Individual> individuals)
        {

            int numberOfSwarm = individuals.Count;
            for (int i = 0; i < numberOfSwarm; i++)
            {
                Individual currentInd = individuals[i];
                int currentX = (int)currentInd.getX();
                int currentY = (int)currentInd.getY();

                if (currentX > rightBound)
                {
                    //Right
                    Wall wall = borderWalls.Where(s => s.GetSideType() == WallSideType.Right).First();
                    HandleWallAction(wall.GetWallActionType(), wall.GetWallOrientation(), currentInd);
                    debugScreen.AddDebugItem("BORDER RIGHT", currentInd.getX().ToString(), XnaSwarmsData.Debug.DebugFlagType.Odd);
                }
                else if (currentX < -rightBound)
                {
                    //Left
                    Wall wall = borderWalls.Where(s => s.GetSideType() == WallSideType.Left).First();
                    HandleWallAction(wall.GetWallActionType(), wall.GetWallOrientation(), currentInd);
                    debugScreen.AddDebugItem("BORDER LEFT", currentInd.getX().ToString());

                }

                if (currentY > bottomBound)
                {
                    //Bottom
                    Wall wall = borderWalls.Where(s => s.GetSideType() == WallSideType.Bottom).First();
                    HandleWallAction(wall.GetWallActionType(), wall.GetWallOrientation(), currentInd);

                }
                else if (currentY < -bottomBound)
                {
                    //Top
                    Wall wall = borderWalls.Where(s => s.GetSideType() == WallSideType.Top).First();
                    HandleWallAction(wall.GetWallActionType(), wall.GetWallOrientation(), currentInd);
                }
            }
        }

        private void HandleWallAction(WallActionType wallactiontype, WallOrientationType wallorientationtype, Individual currentind)
        {
            switch (wallactiontype)
            {
                case Walls.WallActionType.Bounce:
                    if(wallorientationtype == WallOrientationType.Vertical)
                    {
                        currentind.BounceXWall();
                    }
                    else
                    {
                        currentind.BounceYWall();
                    }
                    break;
                case Walls.WallActionType.Portal:
                    if(wallorientationtype == WallOrientationType.Vertical)
                    {
                        currentind.TravelThroughXWall();
                    }
                    else
                    {
                        currentind.TravelThroughYWall();
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spritebatch, SwarmsCamera camera)
        {
            for (int i = 0; i < borderWalls.Count; i++)
            {
                //Vector2 position = camera.ConvertScreenToWorldAndDisplayUnits(new Vector2(0, 0));

                spritebatch.Draw(borderTexture, new Rectangle(
                    borderWalls[i].GetX(),
                    borderWalls[i].GetY(), (int)borderWalls[i].GetWidth(), (int)borderWalls[i].GetHeight()),
                    null,
                    Color.Gray,
                    0f,
                    new Vector2(0,0),
                    SpriteEffects.None, 0);
            }
        }

        public string GetWallTypeAsText()
        {
            return borderWalls[0].GetWallActionType().ToString();
        }
    }
}