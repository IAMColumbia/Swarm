﻿using Microsoft.Xna.Framework;
using SwarmEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XNASwarms.Emitters
{
    public class BrushEmitter : EmitterBase, IGuideable, IAudioInfluenced, IMeteredAgents
    {
        public BrushEmitter(Vector2 position)
            : base(EmitterTypes.Brush, position, new BrushParameters())
        {
            
        }

        public override Individual Update()
        {
            return new Individual(0, this.Position.X, this.Position.Y, 1, 1, new BrushParameters(), EmitterTypes.Brush, StaticBrushParameters.IsMobile);
        }


        public void UpdatePosition(Vector2 position)
        {
            this.Position = position;
        }

        public void UpdateByAudio(double[] fftData)
        {
            this.Parameters = new Parameters(StaticBrushParameters.neighborhoodRadiusMax,
                                             fftData[0],
                                             StaticBrushParameters.maxSpeedMax,
                                             StaticBrushParameters.CohesiveForceMax,
                                             StaticBrushParameters.AligningForceMax,
                                             StaticBrushParameters.SeperatingForceMax,
                                             StaticBrushParameters.ChanceOfRandomSteeringMax,
                                             StaticBrushParameters.TendencyOfPaceKeepingMax);
        }

        public Vector2 lastPosition { get; set; }

        public bool canDraw { get; set; }

        public void CheckForSafeDistance(Vector2 position)
        {
            canDraw = false;
            if (lastPosition == null)
            {
                lastPosition = position;
                canDraw = true;
            }
            //TODO: Create helper method to determine this number
            if ((position.Y > (lastPosition.Y + Space)) ||
                (position.Y < (lastPosition.Y - Space)) ||
                (position.X > (lastPosition.X + Space)) ||
                (position.X < (lastPosition.X - Space)))
            {
                canDraw = true;
                lastPosition = position;
            }

            if (canDraw)
            {
                this.SetActive(true);
            }
            else
            {
                this.SetActive(false);
            }
        }


        public float Space
        {
            get
            {
               return 6;
            }
            set
            {
                Space = value;
            }
        }
    }
}
