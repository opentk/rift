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
    class GL2 : GameWindow
    {
        static readonly OculusRift Rift = new OculusRift();
        static readonly DisplayDevice RiftDisplay = Rift.GetDisplay();

        static readonly string VertexShader = @"
#version 110

attribute vec3 VertexPosition;
attribute vec2 VertexTexCoord;
varying vec2 TexCoord;

void main()
{
    gl_Position = vec4(VertexPosition, 1.0);
    TexCoord = VertexTexCoord;
}
";

        static readonly string FragmentShader = @"
#version 110

varying vec2 TexCoord;

uniform sampler2D Texture;
uniform vec2 LensCenter;
uniform vec2 ScreenCenter;
uniform vec2 OutputScale;
uniform vec2 InputScale;
uniform vec4 K; // barrel distortion factors
uniform vec4 A; // aberration correction factors

vec2 barrel(vec2 tex)
{
    vec2 theta = (tex - LensCenter) * InputScale;
    float r2 = dot(theta, theta);
    vec2 wrap = theta * (K.x + r2 * (K.y + r2 * (K.z + r2 * K.w)));
    return ScreenCenter + wrap * OutputScale;
}

void main()
{
    gl_FragColor = texture2D(Texture, barrel(TexCoord));
}
";

        struct Vertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;

            public Vertex(Vector3 position, Vector2 texcoord)
            {
                Position = position;
                TexCoord = texcoord;
            }
        }

        readonly OculusCamera Camera = new OculusCamera(
            Rift,
            new Vector3(0, 1.6f, 5f),
            Quaternion.Identity);

        // Even though the final image will be displayed
        // at the resolution of the Oculus Rift (1280x800
        // for the devkit), we can increase our rendering
        // resolution (oversample) to improve visual quality.
        // Try increasing the Render* values to 1920x1200.
        readonly int RenderWidth = RiftDisplay.Width;
        readonly int RenderHeight = RiftDisplay.Height;
        readonly int DisplayWidth = RiftDisplay.Width;
        readonly int DisplayHeight = RiftDisplay.Height;

        // Store the previous mouse state to extract
        // movement deltas (i.e. relative mouse motions)
        MouseState mouse_prev;

        // Set to true if the OpenGL context supports
        // multisampled FBO rendering. Multisampling is
        // important for removing jaggies that break
        // visual immersion.
        bool supports_multisampling;

        // OpenGL resources used by this sample:
        int vertex_buffer; // VBO for fullscreen quad
        int element_buffer; // VBO elements (indices) for fullscreen quad
        int color_buffer; // multisampled color renderbuffer for rendering
        int depth_buffer; // depth renderbuffer for rendering
        int render_fbo; // FBO for multisampled rendering
        int resolve_fbo; // FBO for resolving to texture
        int texture; // texture to be sampled by the distortion program
        int program_distort; // handle of the GLSL distortion program

        public GL2()
            : base(
                RiftDisplay.Width,
                RiftDisplay.Height,
                new GraphicsMode(32, 0),
                "OpenTK Oculus Rift Test",
                Rift.IsConnected ? 
                    GameWindowFlags.Fullscreen :
                    GameWindowFlags.Default,
                RiftDisplay)
        {
        }

        #region Protected Members

        protected override void OnLoad(EventArgs e)
        {
            CheckVersion();
            CreateShaders();
            CreateFBOs();

            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnUnload(EventArgs e)
        {
            // Here we should destroy all OpenGL resources.
            // Since the application will exit completely,
            // we let the operating system handle that for us.

            Rift.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard[Key.Escape])
            {
                Close();
            }

            var mouse = OpenTK.Input.Mouse.GetState();
            if (mouse != mouse_prev)
            {
                var delta = new Vector3
                {
                    X = mouse.X - mouse_prev.X,
                    Y = mouse.Y - mouse_prev.Y,
                    Z = mouse.WheelPrecise - mouse_prev.WheelPrecise
                };

                Camera.Orientation = Quaternion.FromAxisAngle(
                    delta.Normalized(),
                    delta.Length);

                mouse_prev = mouse;
            }

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

        void CheckVersion()
        {
            // Retrieve OpenGL version number
            var version_str = GL.GetString(StringName.Version);
            var major = Int32.Parse(version_str[0].ToString());
            var minor = Int32.Parse(version_str[2].ToString());
            var version = major * 100 + minor * 10;
            if (version < 200)
            {
                throw new NotSupportedException(String.Format(
                    "This sample requires OpenGL 2.0 or higher." +
                    "It appears that your drivers only support OpenGL {0}." +
                    "Try updating your drivers or using a newer graphics card.",
                    version_str));
            }

            var extensions = GL.GetString(StringName.Extensions)
                .Split(' ');
            if (!extensions.Contains("GL_EXT_framebuffer_object"))
            {
                throw new NotSupportedException(
                    "This sample requires GL_EXT_framebuffer_object." +
                    "Try updating your drivers or using a newer graphics card.");
            }

            supports_multisampling =
                extensions.Contains("GL_EXT_framebuffer_multisample") &&
                extensions.Contains("GL_EXT_framebuffer_blit");
        }

        void CheckFBO()
        {
            var status = GL.Ext.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception(String.Format(
                    "Failed to create multisampled framebuffer. Error {0}",
                    status));
            }
        }

        void CreateVBOs()
        {
            vertex_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(4 * Vector3.SizeInBytes),
                new Vertex[]
                {
                    new Vertex(new Vector3(-1, -1, 0), new Vector2(0, 0)),
                    new Vertex(new Vector3(1, -1, 0), new Vector2(1, 0)),
                    new Vertex(new Vector3(1, 1, 0), new Vector2(1, 1)),
                    new Vertex(new Vector3(-1, 1, 0), new Vector2(0, 1)),
                },
                BufferUsageHint.StaticDraw);
            CheckErrors();

            element_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                new IntPtr(4 * BlittableValueType<short>.Stride),
                new short[] { 0, 1, 2, 3 },
                BufferUsageHint.StaticDraw);
            CheckErrors();
        }

        void CreateShaders()
        {
            int status = 0;
            string log = String.Empty;

            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, VertexShader);
            GL.CompileShader(vs);
            GL.GetShader(vs, ShaderParameter.CompileStatus, out status);
            if (status != 1)
            {
                log = GL.GetShaderInfoLog(vs);
                throw new Exception(String.Format("Failed to compile vertex shader. Error {0}: {1}",
                    status, log));
            }

            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, FragmentShader);
            GL.CompileShader(fs);
            GL.GetShader(fs, ShaderParameter.CompileStatus, out status);
            if (status != 1)
            {
                log = GL.GetShaderInfoLog(fs);
                throw new Exception(String.Format("Failed to compile fragment shader. Error {0}: {1}",
                    status, log));
            }

            program_distort = GL.CreateProgram();
            GL.AttachShader(program_distort, vs);
            GL.AttachShader(program_distort, fs);
            GL.LinkProgram(program_distort);
            GL.GetProgram(program_distort, GetProgramParameterName.LinkStatus, out status);
            if (status != 1)
            {
                log = GL.GetProgramInfoLog(program_distort);
                throw new Exception(String.Format("Failed to link shader. Error {0}: {1}",
                    status, log));
            }

            CheckErrors();
        }

        void SetUniform(int program, string uniform_name, Vector2 value)
        {
            int location = GL.GetUniformLocation(program, uniform_name);
            if (location >= 0)
            {
                GL.Uniform2(location, value);
            }
        }

        void SetUniform(int program, string uniform_name, Vector4 value)
        {
            int location = GL.GetUniformLocation(program, uniform_name);
            if (location >= 0)
            {
                GL.Uniform4(location, value);
            }
        }

        void CreateFBOs()
        {
            if (supports_multisampling)
            {
                int samples = GL.GetInteger(GetPName.MaxSamples);

                color_buffer = GL.Ext.GenRenderbuffer();
                GL.Ext.BindRenderbuffer(RenderbufferTarget.Renderbuffer, color_buffer);
                GL.Ext.RenderbufferStorageMultisample(
                    RenderbufferTarget.Renderbuffer,
                    samples,
                    RenderbufferStorage.Rgb8,
                    RenderWidth,
                    RenderHeight);

                depth_buffer = GL.Ext.GenRenderbuffer();
                GL.Ext.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depth_buffer);
                GL.Ext.RenderbufferStorageMultisample(
                    RenderbufferTarget.Renderbuffer,
                    samples,
                    RenderbufferStorage.DepthComponent24,
                    RenderWidth,
                    RenderHeight);

                render_fbo = GL.Ext.GenFramebuffer();
                GL.Ext.BindFramebuffer(FramebufferTarget.Framebuffer, render_fbo);
                GL.Ext.FramebufferRenderbuffer(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.ColorAttachment0,
                    RenderbufferTarget.Renderbuffer,
                    color_buffer);
                GL.Ext.FramebufferRenderbuffer(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.DepthAttachment,
                    RenderbufferTarget.Renderbuffer,
                    depth_buffer);

                CheckFBO();
            }

            texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8,
                DisplayWidth, DisplayHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte,
                IntPtr.Zero);

            resolve_fbo = GL.Ext.GenFramebuffer();
            GL.Ext.BindFramebuffer(FramebufferTarget.Framebuffer, resolve_fbo);
            GL.Ext.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D,
                texture,
                0);

            CheckFBO();
        }

        float half_ipd = Rift.InterpupillaryDistance * 0.5f;
        void DrawScene(CameraType camera_type)
        {
            Matrix4 matrix;

            if (supports_multisampling)
            {
                GL.Ext.BindFramebuffer(FramebufferTarget.Framebuffer, render_fbo);
            }
            else
            {
                GL.Ext.BindFramebuffer(FramebufferTarget.Framebuffer, resolve_fbo);
            }

            SetupViewport(camera_type);

            Camera.GetProjectionMatrix(camera_type, out matrix);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref matrix);

            Camera.GetModelviewMatrix(camera_type, out matrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref matrix);

            //GL.Rotate(angle, 0.0f, 1.0f, 0.0f);
            DrawCube();

            if (supports_multisampling)
            {
                // We need to resolve the multisampled renderbuffer to a texture
                // in order to feed it to the distortion program.
                // Note: when Render* resolution is different than Display* resolution,
                // we can improve image quality by implementing a downsampling filter
                // in a shader (e.g. bicubic or lanczos), instead of using
                // BlitFramebufferFilter.Linear
                GL.Ext.BindFramebuffer(FramebufferTarget.ReadFramebuffer, render_fbo);
                GL.Ext.BindFramebuffer(FramebufferTarget.DrawFramebuffer, resolve_fbo);
                GL.Ext.BlitFramebuffer(
                    0, 0, RenderWidth, RenderHeight,
                    0, 0, DisplayWidth, DisplayHeight,
                    ClearBufferMask.ColorBufferBit,
                    BlitFramebufferFilter.Linear);
            }

            float scale = 1.0f;
            float distortion_x_center = 
                camera_type == CameraType.StereoLeft ? 0.25f :
                camera_type == CameraType.StereoRight ? -0.25f :
                0;
            float x =
                camera_type == CameraType.StereoRight ? 0.5f :
                0;
            float y = 0;
            float w = RenderWidth;
            float h = RenderHeight;
            float aspect = w / h;

            var lens_center =
                new Vector2(
                    x + (w + distortion_x_center * 0.5f) * 0.5f,
                    y + h * 0.5f);
            var screen_center =
                new Vector2(
                    x + w * 0.5f,
                    y + h * 0.5f);
            var input_scale =
                new Vector2(
                    w * scale * 0.5f,
                    h * scale * aspect * 0.5f);
            var output_scale =
                new Vector2(
                    2.0f / w,
                    (2.0f / h) / aspect);

            GL.UseProgram(program_distort);
            SetUniform(program_distort, "LensCenter", lens_center);
            SetUniform(program_distort, "ScreenCenter", screen_center);
            SetUniform(program_distort, "InputScale", input_scale);
            SetUniform(program_distort, "OutputScale", output_scale);
            SetUniform(program_distort, "K", Rift.DistortionK);
            SetUniform(program_distort, "A", Rift.ChromaAbAberration);

            GL.Ext.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
            GL.DrawElements(PrimitiveType.TriangleStrip, 4, DrawElementsType.UnsignedShort, 0);
        }

        void CheckErrors()
        {
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                throw new Exception(error.ToString());
            }
        }

        void SetupViewport(CameraType camera_type)
        {
            switch (camera_type)
            {
                case CameraType.Default:
                    GL.Viewport(
                        0, 0,
                        RenderWidth, RenderHeight);
                    break;

                case CameraType.StereoLeft:
                    GL.Viewport(
                        0, 0,
                        RenderWidth / 2, RenderHeight);
                    break;

                case CameraType.StereoRight:
                    GL.Viewport(
                        RenderWidth / 2, 0,
                        RenderWidth / 2, RenderHeight);
                    break;
            }
        }

        void DrawCube()
        {
            GL.UseProgram(0);

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
            //using (Toolkit.Init())
            Toolkit.Init();
            using (var gw = new GL2())
            {
                gw.Run(60.0);
            }
        }

        #endregion
    }
}

