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
using System.Drawing;
using System.Linq;

namespace Test
{
    class GL1 : GameWindow
    {
        static readonly OculusRift Rift = new OculusRift();
        static readonly DisplayDevice RiftDisplay =
            (Enumerable
                .Range((int)DisplayIndex.First, (int)DisplayIndex.Sixth)
                .Select(i => DisplayDevice.GetDisplay(DisplayIndex.First + i))
                .Where(d => d != null && d.Width == Rift.HResolution && d.Height == Rift.VResolution)
                .FirstOrDefault()) ??
            DisplayDevice.Default;

        readonly OculusCamera Camera = new OculusCamera(
            Rift,
            new Vector3(0, 1.6f, 5f),
            Quaternion.Identity);

        float angle;

        public GL1()
            : base(
                RiftDisplay.Width,
                RiftDisplay.Height,
                new GraphicsMode(32, 24, 0, 16),
                "OpenTK Oculus Rift Test",
                //Rift.IsConnected ? 
                //    GameWindowFlags.Fullscreen :
                    GameWindowFlags.Default,
                RiftDisplay)
        {
        }

        #region Protected Members

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnUnload(EventArgs e)
        {
            Rift.Dispose();
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

            if (Keyboard[Key.Right])
                half_ipd *= 1.01f;
            if (Keyboard[Key.Left])
                half_ipd *= 0.99f;
            if (Keyboard[Key.Down])
                Camera.Position = Vector3.Multiply(Camera.Position, new Vector3(1, 1, 1.01f));
            if (Keyboard[Key.Up])
                Camera.Position = Vector3.Multiply(Camera.Position, new Vector3(1, 1, 0.99f));
            if (Keyboard[Key.BracketLeft])
                Rift.PredictionDelta *= 0.99f;
            if (Keyboard[Key.BracketRight])
                Rift.PredictionDelta *= 1.01f;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawScene(CameraType.StereoLeft);
            DrawScene(CameraType.StereoRight);

            SwapBuffers();
        }

        #endregion

        #region Private Members

        float half_ipd = Rift.InterpupillaryDistance * 0.5f;
        void DrawScene(CameraType camera_type)
        {
            Matrix4 matrix;

            SetupViewport(camera_type);

            Camera.GetProjectionMatrix(camera_type, out matrix);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref matrix);

            Camera.GetModelviewMatrix(camera_type, out matrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref matrix);

            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
            DrawCube();
        }

        void SetupViewport(CameraType camera_type)
        {
            switch (camera_type)
            {
                case CameraType.Default:
                    GL.Viewport(0, 0, Width, Height);
                    break;

                case CameraType.StereoLeft:
                    GL.Viewport(0, 0, Width / 2, Height);
                    break;

                case CameraType.StereoRight:
                    GL.Viewport(Width / 2, 0, Width / 2, Height);
                    break;
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
            using (var gw = new GL1())
            {
                gw.Run(60.0);
            }
        }

        #endregion
    }
}

