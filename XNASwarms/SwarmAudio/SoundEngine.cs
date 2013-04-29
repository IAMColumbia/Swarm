﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSS;
using Microsoft.Xna.Framework;

namespace SwarmAudio
{
    public class SoundEngine
    {

        static int handle;

        public static void Init()
        {
            string audFile = "Soundfile Segment and Localize.aud";

            audFile = "PoemGraspThrow_11.22.12.aud";
            audFile = "Cluster_datatest.aud";

            //if (VSSCSharpClient.BeginSoundServerAt("127.0.0.1") != 1)
            if (VSSCSharpClient.BeginSoundServer() != 1)
            {
                Console.WriteLine("Could Not Connect to VSS...");
                Console.WriteLine("Please make sure VSS is running on localhost. also make sure the SOUNDS folder with the audio files for demo is in the same folder as VSS.exe");
                Console.WriteLine("If BeginSoundServerAt is called VSS must be running on the specified ip address.");
                Console.ReadKey();
                return;
            }

            handle = VSSCSharpClient.AUDinit(audFile);

            if (handle < 0)
            {
                Console.WriteLine(string.Format("Failed to load audfile {0}\n", audFile));
                Console.ReadKey();
                return;
            }
        }

        public static void TestData()
        {
            //VSSCSharpClient.AUDupdate(handle, "test", 0, new float[] { 0.0f });
            VSSCSharpClient.AUDupdate(handle, "playSeq", 0, new float[] { 0.0f });
        }

        public static void SendClusterXY(float x, float y)
        {
            VSSCSharpClient.AUDupdate(handle, "SendXYposition", 2, new float[] { x, y });
        }

        public static void StartCluster()
        {
            VSSCSharpClient.AUDupdate(handle, "startCluster", 1, new float[] { 1 });
        }

        public static void StopCluster()
        {
            VSSCSharpClient.AUDupdate(handle, "stopCluster", 1, new float[] { 0 });
        }

        public static void Play()
        {
            //VSSCSharpClient.AUDupdate(handle, "playSeq", 0, new float[] { 0.0f });
            SoundEngine.StartCluster();
        }

        public static void Pause()
        {
            //VSSCSharpClient.AUDupdate(handle, "pause", 0, new float[] { 0 });
            SoundEngine.StopCluster();
        }

        public static void UpdateCluster(float numAgents, Vector2 center, float area, float averageAgentEnergy, float clusterVelocity, Vector3 symmetry)
        {
            VSSCSharpClient.AUDupdate(handle, "updateCluster", 9, new float[] { numAgents, center.X, center.Y, area, averageAgentEnergy, clusterVelocity, symmetry.X, symmetry.Y, symmetry.Z});
        }

        public static void SendNumAgents(float numAgents)
        {
            VSSCSharpClient.AUDupdate(handle, "SendNumAgents", 1, new float[] { numAgents });
        }

        public static void SendArea(float area)
        {
            VSSCSharpClient.AUDupdate(handle, "SendArea", 1, new float[] { area });
        }

        public static void SendAgentEnergy(float energy)
        {
            VSSCSharpClient.AUDupdate(handle, "SendAgentEnergy", 1, new float[] { energy });
        }

        public static void SendXYsymmetry(float symmetry)
        {
            VSSCSharpClient.AUDupdate(handle, "SendXYsymmetry", 1, new float[] { symmetry });
        }
    }
}
