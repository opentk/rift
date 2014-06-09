#region License
//
// Program.cs
//
// Author:
//       Stefanos A. <stapostol@gmail.com>
//
// Copyright (c) 2006-2014 Stefanos Apostolopoulos
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenTK.Rift.Test
{
    class Program : GameWindow
    {
        readonly HMDisplay Display;
        readonly HMDisplayDescription DisplayDesc;
        readonly EyeRenderDescription[] EyeDesc =
            new EyeRenderDescription[2];

        // GL textures for VR rendering
        readonly int[] EyeTexture = new int[2];

        int frame;
        float angle;
        Vector3 position;

        public Program()
        {
            Display = CreateVRDisplay();
            if (Display == HMDisplay.Zero)
            {
                throw new NotSupportedException("Failed to connect to a VR device.");
            }

            // General properties of the HMDisplay.
            // These properties do not generally change at runtime.
            DisplayDesc = Display.GetDescription();

            // Current rendering properties of the HMDisplay.
            // These properties can be modified at runtime, if desired.
            var left = Display.GetRenderDescription(
                EyeType.Left, DisplayDesc.DefaultEyeFovLeft);
            var right = Display.GetRenderDescription(
                EyeType.Right, DisplayDesc.DefaultEyeFovRight);

            // Store the left and right eyes in the recommended rendering order.
            // This reduces latency.
            EyeDesc[0] = DisplayDesc.EyeRenderOrderFirst == EyeType.Left ? left : right;
            EyeDesc[1] = DisplayDesc.EyeRenderOrderSecond == EyeType.Left ? left : right;
        }

        #region Protected Members

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);

            EyeTexture[0] = GL.GenTexture();
            EyeTexture[1] = GL.GenTexture();
        }

        protected override void OnUnload(EventArgs e)
        {
            if (Display != HMDisplay.Zero)
            {
                Display.Destroy();
            }
            VR.Shutdown();
        }

        protected override void OnResize(EventArgs e)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[Key.Escape])
            {
                Close();
            }

            angle += 16 * (float)TargetUpdatePeriod;

            if (Keyboard[Key.Down])
                position = Vector3.Multiply(position, new Vector3(1, 1, 1.01f));
            if (Keyboard[Key.Up])
                position = Vector3.Multiply(position, new Vector3(1, 1, 0.99f));
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Display.BeginFrame(frame++);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int i = 0; i < 1; i++)
            {
                var eye = EyeDesc[i].Eye;
                var pose = Display.BeginEyeRender(eye);
                var fov = EyeDesc[i].Fov;

                DrawScene(eye, fov);

                var texture = new VRTexture();
                texture.Header.Api = RenderApiType.OpenGL;
                texture.Header.RenderViewport = GetViewport(eye);
                texture.Header.TextureSize = Display.GetFovTextureSize(eye, fov, 1.0f);
                texture.GL.TexId = EyeTexture[i];

                Display.EndEyeRender(eye, pose, ref texture);
            }

            //SwapBuffers();
            Display.EndFrame();
        }

        #endregion

        #region Private Members

        HMDisplay CreateVRDisplay()
        {
            var device = HMDisplay.Zero;

            // Initialize libOVR and open the first connected device.
            // If no device is connected, create a debug device for testing.
            if (!VR.Initialize())
            {
                var error = VR.GetLastError();
                throw new NotSupportedException("libOVR initialization failed with: " + error);
            }

            int device_count = VR.Detect();
            Console.WriteLine("Detected {0} device(s).", device_count);

            if (device_count > 0)
            {
                Console.Write("Opening device 0... ");
                device = VR.Create(0);

                if (device != HMDisplay.Zero)
                {
                    Console.WriteLine("success!");
                }
                else
                {
                    Console.WriteLine("failed.");
                    Console.WriteLine("Error was '{0}'", VR.GetLastError());
                }
            }

            if (device == HMDisplay.Zero)
            {
                Console.Write("Opening debug device... ");
                device = VR.CreateDebug(HMDisplayType.DK1);

                if (device != HMDisplay.Zero)
                {
                    Console.WriteLine("success!");
                }
                else
                {
                    Console.WriteLine("failed.");
                    Console.WriteLine("Error was '{0}'", VR.GetLastError());
                }
            }

            return device;
        }

        void DrawScene(EyeType eye, FovPort fov)
        {
            var proj = VR.Projection(fov, 0.1f, 128.0f, true);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref proj);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(position);
            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);

            DrawCube();
        }

        VRRectangle GetViewport(EyeType eye)
        {
            switch (eye)
            {
                case EyeType.Left:
                    return new VRRectangle(0, 0, Width / 2, Height);

                case EyeType.Right:
                    return new VRRectangle(Width / 2, 0, Width / 2, Height);

                default:
                    return new VRRectangle(0, 0, Width, Height);
            }
        }

        void DrawCube()
        {
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(Color.Silver);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);

            GL.Color3(Color.Honeydew);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);

            GL.Color3(Color.Moccasin);

            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);

            GL.Color3(Color.IndianRed);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);

            GL.Color3(Color.PaleVioletRed);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);

            GL.Color3(Color.ForestGreen);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);

            GL.End();
        }

        #endregion

        #region Main

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (Toolkit.Init())
            using (var gw = new Program())
            {
                gw.Run(60.0);
            }
        }

        #endregion
    }
}

