#region License
//
// Test.cs
//
// Author:
//       Stefanos A. <stapostol@gmail.com>
//
// Copyright (c) 2014 Stefanos Apostolopoulos
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
using System.Linq;

namespace OpenTK.Rift.Test
{
    class Test : GameWindow
    {
        static readonly OculusRift ovr = new OculusRift();
        static readonly DisplayDevice display =
            (Enumerable
                .Range((int)DisplayIndex.First, (int)DisplayIndex.Sixth)
                .Select(i => DisplayDevice.GetDisplay(DisplayIndex.First + i))
                .Where(d => d != null && d.Width == ovr.HResolution && d.Height == ovr.VResolution)
                .FirstOrDefault()) ??
            DisplayDevice.Default;

        public Test(int width, int height)
            : base(
                display.Width,
                display.Height,
                new GraphicsMode(32, 24, 8, 16),
                "OpenTK Oculus Rift Test",
                GameWindowFlags.Fullscreen,
                display)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
        }

        protected override void OnUnload(EventArgs e)
        {
            ovr.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[Key.Escape])
                Close();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit |
                ClearBufferMask.StencilBufferBit);

            SwapBuffers();
        }
            
        public static void Main()
        {
            using (Toolkit.Init())
            {
                var display =
                    DisplayDevice.GetDisplay(DisplayIndex.Second) ??
                    DisplayDevice.GetDisplay(DisplayIndex.Default);

                using (var gw = new Test(display.Width, display.Height))
                {
                    gw.Run(60);
                }
            }
        }
    }
}

