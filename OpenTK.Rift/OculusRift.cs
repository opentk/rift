#region License
//
// OVR.cs
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
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace OpenTK
{
    using ovrHmd = IntPtr;

    /// <summary>
    /// Provides high-level access to an Oculus Rift device.
    /// </summary>
    public class OculusRift : IDisposable
    {
        const string lib = "OVR";

        static int instance_count;
        static Toolkit opentk;
        bool disposed;

        // For compatibility with the old 0.2.x API
        IntPtr instance;
        HMDisplayDescription description;
        float prediction_delta;
        bool prediction_enabled;

        #region Constructors

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string filename);

        static OculusRift()
        {
            if (Configuration.RunningOnWindows &&
                !Configuration.RunningOnMono)
            {
                // When running on Windows, we need to load the
                // native library into the address space of the
                // application. This allows us to use DllImport
                // to load the correct (x86 or x64) library at
                // runtime.
                // Non-windows platforms rely on the dll.config
                // file to load the correct library.

                if (IntPtr.Size == 4)
                {
                    // Running on 32bit system
                    LoadLibrary("lib/x86/OVR.dll");
                }
                else
                {
                    // Running on 64bit system
                    LoadLibrary("lib/x64/OVR.dll");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTK.OculusRift"/> class.
        /// </summary>
        [Obsolete("Use the new official C-API instead via the static methods of this class.")]
        public OculusRift()
        {
            if (Interlocked.Increment(ref instance_count) == 1)
            {
                opentk = Toolkit.Init();
                if (!Initialize())
                {
                    throw new NotSupportedException("OculusRift failed to initialize");
                }
            }

            int count = Detect();
            if (count > 0)
            {
                instance = Create(0);
                if (instance != IntPtr.Zero)
                {
                    GetDescription(instance, out description);
                }
            }
        }

        #endregion

        #region Obsolete Members

        /// <summary>
        /// Gets a value indicating whether this instance is connected
        /// to an Oculus Rift device.
        /// </summary>
        [Obsolete]
        public bool IsConnected
        {
            get
            {
                CheckDisposed();
                return instance != IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets the x-coordinate of the Oculus Rift display device,
        /// in global desktop coordinates (px).
        /// </summary>
        [Obsolete]
        public int DesktopX
        {
            get
            {
                CheckDisposed();
                return description.WindowPos.X;
            }
        }

        /// <summary>
        /// Gets the y-coordinate of the Oculus Rift display device,
        /// in global desktop coordinates (px).
        /// </summary>
        [Obsolete]
        public int DesktopY
        {
            get
            {
                CheckDisposed();
                return description.WindowPos.Y;
            }
        }

        /// <summary>
        /// Horizontal resolution of the entire HMD screen in pixels.
        /// Half the HResolution is used for each eye.
        /// This value is 1280 for the Development Kit.
        /// </summary>
        [Obsolete]
        public int HResolution
        {
            get
            {
                CheckDisposed();
                return description.Resolution.Width;
            }
        }

        /// <summary>
        /// Vertical resolution of the entire HMD screen in pixels.
        /// This value is 800 for the Development Kit.
        /// </summary>
        [Obsolete]
        public int VResolution
        {
            get
            {
                CheckDisposed();
                return description.Resolution.Height;
            }
        }

        /// <summary>
        /// Horizontal dimension of the entire HMD screen in meters. Half
        /// HScreenSize is used for each eye. The current physical screen size is
        /// 149.76 x 93.6mm, which will be reported as 0.14976f x 0.0935f.
        /// </summary>
        [Obsolete]
        public float HScreenSize
        {
            get
            {
                CheckDisposed();
                return 0.14976f;
            }
        }

        /// <summary>
        /// Vertical dimension of the entire HMD screen in meters. Half
        /// HScreenSize is used for each eye. The current physical screen size is
        /// 149.76 x 93.6mm, which will be reported as 0.14976f x 0.0935f.
        /// </summary>
        [Obsolete]
        public float VScreenSize
        {
            get
            {
                CheckDisposed();
                return 0.0935f;
            }
        }

        /// <summary>
        /// Physical offset from the top of the screen to eye center, in meters.
        /// Currently half VScreenSize.
        /// </summary>
        [Obsolete]
        public float VScreenCenter
        {
            get
            {
                CheckDisposed();
                return VScreenSize / 2;
            }
        }

        /// <summary>
        /// Physical distance between the lens centers, in meters. Lens centers
        /// are the centers of distortion.
        /// </summary>
        [Obsolete]
        public float LensSeparationDistance
        {
            get
            {
                CheckDisposed();
                return 0.0635f;
            }
        }

        /// <summary>
        /// Configured distance between eye centers.
        /// </summary>
        [Obsolete]
        public float InterpupillaryDistance
        {
            get
            {
                CheckDisposed();
                return 0.064f;
            }
        }

        /// <summary>
        /// Distance from the eye to the screen, in meters. It combines
        /// distances from the eye to the lens, and from the lens to the screen.
        /// This value is needed to compute the fov correctly.
        /// </summary>
        [Obsolete]
        public float EyeToScreenDistance
        {
            get
            {
                CheckDisposed();
                return 0.041f;
            }
        }

        /// <summary>
        /// Gets the radial distortion correction coefficients.
        /// </summary>
        /// <value>The distortion k.</value>
        [Obsolete]
        public Vector4 DistortionK
        {
            get
            {
                CheckDisposed();
                return new Vector4(1.0f, 0.22f, 0.24f, 0.0f);
            }
        }

        /// <summary>
        /// Gets the chroma AB aberration coefficients.
        /// </summary>
        [Obsolete]
        public Vector4 ChromaAbAberration
        {
            get
            {
                CheckDisposed();
                return new Vector4(0.996f, -0.004f, 1.014f, 0.0f);
            }
        }

        #region Sensor Fusion

        /// <summary>
        /// Gets a <see cref="OpenTK.Quaternion"/> representing
        /// the current accumulated orientation. Most applications
        /// will want to use <see cref="PredictedOrientation"/> instead.
        /// Use <see cref="OpenTK.Quaternion.ToAxisAngle()"/> to
        /// convert this quaternion to an axis-angle representation.
        /// Use <see cref="OpenTK.Matrix4.CreateFromQuaternion(OpenTK.Quaternion)"/>
        /// to convert this quaternion to a rotation matrix.
        /// </summary>
        [Obsolete]
        public Quaternion Orientation
        {
            get
            {
                CheckDisposed();
                var state = GetSensorState(instance, IsPredictionEnabled ? PredictionDelta : 0);
                return state.Recorded.Pose.Orientation;
            }
        }

        /// <summary>
        /// Gets a <see cref="OpenTK.Quaternion"/> representing
        /// the predicted orientation after <see cref="PredictionDelta"/>
        /// seconds.
        /// </summary>
        [Obsolete]
        public Quaternion PredictedOrientation
        {
            get
            {
                CheckDisposed();
                var state = GetSensorState(instance, IsPredictionEnabled ? PredictionDelta : 0);
                return state.Predicted.Pose.Orientation;
            }
        }

        /// <summary>
        /// Gets the last absolute acceleration reading, in m/s^2.
        /// </summary>
        /// <value>The acceleration.</value>
        [Obsolete]
        public Vector3 Acceleration
        {
            get
            {
                CheckDisposed();
                var state = GetSensorState(instance, IsPredictionEnabled ? PredictionDelta : 0);
                return state.Recorded.AngularAcceleration;
            }
        }

        /// <summary>
        /// Gets the last angular velocity reading, in rad/s.
        /// </summary>
        /// <value>The angular velocity.</value>
        [Obsolete]
        public Vector3 AngularVelocity
        {
            get
            {
                CheckDisposed();
                var state = GetSensorState(instance, IsPredictionEnabled ? PredictionDelta : 0);
                return state.Recorded.AngularVelocity;
            }
        }

        /// <summary>
        /// Gets or sets the delta time for sensor prediction, in seconds.
        /// </summary>
        [Obsolete]
        public float PredictionDelta
        {
            get
            {
                CheckDisposed();
                return prediction_delta;
            }
            set
            {
                CheckDisposed();
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                prediction_delta = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether sensor prediction is enabled.
        /// </summary>
        [Obsolete]
        public bool IsPredictionEnabled
        {
            get
            {
                CheckDisposed();
                return prediction_enabled;
            }
            set
            {
                CheckDisposed();
                prediction_enabled = value;
            }
        }

        #endregion

        #endregion

        #region Private Members

        void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Releases all resource used by the <see cref="OpenTK.OculusRift"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="OpenTK.OculusRift"/>. The
        /// <see cref="Dispose()"/> method leaves the <see cref="OpenTK.OculusRift"/> in an unusable state. After calling
        /// <see cref="Dispose()"/>, you must release all references to the <see cref="OpenTK.OculusRift"/> so the garbage
        /// collector can reclaim the memory that the <see cref="OpenTK.OculusRift"/> was occupying.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    if (Interlocked.Decrement(ref instance_count) == 0)
                    {
                        Shutdown();
                        opentk.Dispose();
                        opentk = null;
                    }
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="OpenTK.OculusRift"/>
        /// is reclaimed by garbage collection.
        /// </summary>
        ~OculusRift()
        {
            Console.Error.WriteLine("[Warning] OculusRift instance leaked. Did you forget to call Dispose()?");
        }

        #endregion

        #region Public Members

        #region OVR Interface

        /// <summary>
        /// Initializes the Oculus Rift API.
        /// This function must be called successfully before
        /// using any Oculus Rift functionality.
        /// </summary>
        /// <returns><c>true</c> if the initialize was successful; <c>false</c> otherwise.</returns>
        [DllImport(lib, EntryPoint = "ovr_Initialize", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Initialize();

        /// <summary>
        /// Shuts down the Oculus Rift API.
        /// </summary>
        [DllImport(lib, EntryPoint = "ovr_Shutdown", CallingConvention = CallingConvention.Winapi)]
        public static extern void Shutdown();

        #endregion

        #region HMD Interface

        /// <summary>
        /// Detects or re-detects HMDs and reports the total number detected.
        /// Users can get information about each HMD by calling ovrHmd_Create with an index.
        /// </summary>
        /// <returns>The total number of HMDs detected.</returns>
        [DllImport(lib, EntryPoint = "ovrHmd_Detect", CallingConvention = CallingConvention.Winapi)]
        public static extern int Detect();

        /// <summary>
        /// Creates a handle to an HMD.
        /// </summary>
        /// <param name="index">
        /// Index of the HMD, between 0 and <see cref="Detect()"/> - 1.
        /// Note: index mappings can cange after each <see cref="Detect"/>.
        /// </param>
        /// <returns>
        /// A <see cref="System.IntPtr"/> representing a handle to an HMD, or <see cref="System.IntPtr.Zero"/>
        /// if the specified index is not valid.
        /// Valid handles must be freed with <see cref="Destroy"/>.
        /// </returns>
        [DllImport(lib, EntryPoint = "ovrHmd_Create", CallingConvention = CallingConvention.Winapi)]
        public static extern ovrHmd Create(int index);

        /// <summary>
        /// Destroys the specified HMD handle.
        /// </summary>
        /// <param name="hmd">A valid HMD handle obtained by <see cref="Create"/>.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_Destroy", CallingConvention = CallingConvention.Winapi)]
        public static extern void Destroy(ovrHmd hmd);

        /// <summary>
        /// Creates a "fake" HMD used for debugging only. This is not tied to specific hardware,
        /// but may be used to debug some of the related rendering.
        /// </summary>
        /// <returns>A <see cref="System.IntPtr"/> represtenting a debug HMD handle.</returns>
        /// <param name="type">The desired <see cref="HMDisplayType"/>.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_CreateDebug", CallingConvention = CallingConvention.Winapi)]
        public static extern ovrHmd CreateDebug(HMDisplayType type);

        /// <summary>
        /// Returns a string representing the last HMD error.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representing the last HMD error,
        /// or <see cref="System.String.Empty"/> if no error has occured.
        /// </returns>
        /// <param name="hmd">
        /// An HMD handle obtained by <see cref="Create"/> or <see cref="CreateDebug"/>,
        /// or <see cref="System.IntPtr.Zero"/> to obtain the global error state (for <see cref="Initialize"/> etc).
        /// </param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetLastError", CallingConvention = CallingConvention.Winapi)]
        static extern IntPtr GetLastError(ovrHmd hmd);

        /// <summary>
        /// Returns capability bits that are enabled at this time; described by <see cref="HMDisplayCaps"/>.
        /// Note that this value is different from <see cref="HMDisplayDescription.DisplayCaps"/>,
        /// which describes what capabilities are available.
        /// </summary>
        /// <returns>A bitwise combination of <see cref="HMDisplayCaps"/> flags representing the enabled capabilities.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetEnabledCaps", CallingConvention = CallingConvention.Winapi)]
        public static extern HMDisplayCaps GetEnabledCaps(ovrHmd hmd);

        /// <summary>
        /// Modifies capability bits described by ovrHmdCaps that can be modified,
        /// such as ovrHmd_LowPersistance.
        /// </summary>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="hmdCaps">A bitwise combination of the <see cref="HMDisplayCaps"/> flags to enable</param>
        [DllImport(lib, EntryPoint = "ovrHmd_SetEnabledCaps", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetEnabledCaps(ovrHmd hmd, HMDisplayCaps hmdCaps);

        #endregion

        #region Sensor Interface

        /// <summary>
        /// Starts sensor sampling, enabling specified capabilities, described by <see cref="SensorCaps"/>.
        /// </summary>
        /// <returns><c>true</c>, if sensor was started, <c>false</c> otherwise.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="supportedSensorCaps">
        /// Sensor capabilities that are requested at the time of the call. The function will succeed 
        /// even if these caps are not available (i.e. sensor or camera is unplugged). Support
        /// will automatically be enabled if such device is plugged in later. Software should
        /// check ovrSensorState.StatusFlags for real-time status.
        /// </param>
        /// <param name="requiredSensorCaps">
        /// Sensor capabilities required at the time of the call.
        /// If they are not available, the function will fail. Pass 0 if only specifying
        /// supportedSensorCaps.
        /// </param>
        /// <remarks>
        /// All sensor interface functions are thread-safe, allowing sensor state to be sampled
        /// from different threads.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_StartSensor", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool StartSensor(ovrHmd hmd,
            SensorCaps supportedSensorCaps,
            SensorCaps requiredSensorCaps);

        /// <summary>
        /// Stops sensor sampling, shutting down internal resources.
        /// </summary>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <remarks>
        /// All sensor interface functions are thread-safe, allowing sensor state to be sampled
        /// from different threads.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_StopSensor", CallingConvention = CallingConvention.Winapi)]
        public static extern void StopSensor(ovrHmd hmd);

        /// <summary>
        /// Resets sensor orientation.
        /// </summary>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <remarks>
        /// All sensor interface functions are thread-safe, allowing sensor state to be sampled
        /// from different threads.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_ResetSensor", CallingConvention = CallingConvention.Winapi)]
        public static extern void ResetSensor(ovrHmd hmd);

        /// <summary>
        /// Returns sensor state reading based on the specified absolute system time.
        /// </summary>
        /// <returns>The sensor state.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="absTime">
        /// Pass absTime value of 0.0 to request the most recent sensor reading; in this case
        /// both PredictedPose and SamplePose will have the same value.
        /// </param>
        /// <remarks>
        /// All sensor interface functions are thread-safe, allowing sensor state to be sampled
        /// from different threads.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_GetSensorState", CallingConvention = CallingConvention.Winapi)]
        public static extern SensorState GetSensorState(ovrHmd hmd, double absTime);

        /// <summary>
        /// Returns information about a sensor.
        /// Only valid after StartSensor.
        /// </summary>
        /// <returns><c>true</c>, if a sensor desc was obtained successfully; <c>false</c> otherwise.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="descOut">
        /// A non-null <see cref="System.Text.StringBuilder"/> instance.
        /// After a successful call, this will hold the sensor description.
        /// </param>
        /// <remarks>
        /// All sensor interface functions are thread-safe, allowing sensor state to be sampled
        /// from different threads.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_GetSensorDesc", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GetSensorDescription(ovrHmd hmd, StringBuilder descOut);

        #endregion

        #region Graphics Setup

        /// <summary>
        /// Fills in description about HMD; this is the same as filled in by <see cref="Create"/>.
        /// </summary>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="desc">A <see cref="HMDisplayDescription"/> instance containing information about an HMD.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetDesc", CallingConvention = CallingConvention.Winapi)]
        public static extern void GetDescription(ovrHmd hmd, out HMDisplayDescription desc);

        /// <summary>
        /// Gets the recommended texture size for a single eye and given FOV cone.
        /// Higher FOV will generally require larger textures to maintain quality.
        /// </summary>
        /// <returns>The recommended texture size for the specified FOV cone.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="eye">The <see cref="EyeType"/> for the left or right eye.</param>
        /// <param name="fov">A <see cref="FovPort"/> instance specifying the desired FOV cone.</param>
        /// <param name="pixelsPerDisplayPixel">
        /// Specifies that number of render target pixels per display
        /// pixel at center of distortion; 1.0 is the default value. Lower values
        /// can improve performance.
        /// </param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetFovTextureSize", CallingConvention = CallingConvention.Winapi)]
        public static extern OculusSize GetFovTextureSize(ovrHmd hmd, EyeType eye, FovPort fov,
            float pixelsPerDisplayPixel);

        #endregion

        #region SDK Rendering

        /// <summary>
        /// Configures rendering; fills in computed render parameters.
        /// This function can be called multiple times to change rendering settings.
        /// Pass in two eye view descriptors to generate complete rendering information
        /// for each eye.
        /// </summary>
        /// <returns><c>true</c>, if rendering was configured successfully; <c>false</c> otherwise.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="apiConfig">
        /// Provides D3D/OpenGL specific parameters. Pass null
        /// to shutdown rendering and release all resources.
        /// </param>
        /// <param name="distortionCaps">
        /// A bitwise combination of <see cref="DistortionCaps"/> flags
        /// that describe the distortion settings that will be applied.
        /// </param>
        /// <param name="eyeFovIn">An array of 2 <see cref="FovPort"/> instances in.</param>
        /// <param name="eyeRenderDescOut">
        /// When the function returns successfully, contains two <see cref="EyeRenderDescription"/>
        /// instances describing the rendering settings for each eye.
        /// </param>
        /// <remarks>
        /// This function support rendering of distortion by the SDK through direct
        /// access to the underlying rendering HW, such as D3D or GL.
        /// This is the recommended approach, as it allows for better support or future
        /// Oculus hardware and a range of low-level optimizations.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_ConfigureRendering", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ConfigureRendering(ovrHmd hmd,
            ref RenderConfiguration apiConfig,
            DistortionCaps distortionCaps,
            [MarshalAs(UnmanagedType.LPArray, SizeConst=2)] FovPort[] eyeFovIn,
            [Out][MarshalAs(UnmanagedType.LPArray, SizeConst=2)] EyeRenderDescription[] eyeRenderDescOut);

        /// <summary>
        /// Begins a frame, returning timing and orientation information useful for simulation.
        /// This should be called in the beginning of game rendering loop (on render thread).
        /// This function relies on <see cref="BeginFrameTiming"/> for some of its functionality.
        /// </summary>
        /// <returns>A <see cref="FrameTiming"/> instance.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="frameIndex">The frame index. Pass 0 for frame index if not using GetFrameTiming.</param>
        /// <remarks>
        /// This function support rendering of distortion by the SDK through direct
        /// access to the underlying rendering HW, such as D3D or GL.
        /// This is the recommended approach, as it allows for better support or future
        /// Oculus hardware and a range of low-level optimizations.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_BeginFrame", CallingConvention = CallingConvention.Winapi)]
        public static extern FrameTiming BeginFrame(ovrHmd hmd, int frameIndex);

        /// <summary>
        /// Ends frame, rendering textures to frame buffer. This may perform distortion and scaling
        /// internally, assuming is it not delegated to another thread. 
        /// Must be called on the same thread as BeginFrame. Calls <see cref="BeginFrameTiming"/> internally.
        /// Note: this Function will call SwapBuffers() and potentially wait for VSync.
        /// </summary>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <remarks>
        /// This function support rendering of distortion by the SDK through direct
        /// access to the underlying rendering HW, such as D3D or GL.
        /// This is the recommended approach, as it allows for better support or future
        /// Oculus hardware and a range of low-level optimizations.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_EndFrame", CallingConvention = CallingConvention.Winapi)]
        public static extern void EndFrame(ovrHmd hmd);

        /// <summary>
        /// Marks beginning of eye rendering. Must be called on the same thread as BeginFrame.
        /// This function uses ovrHmd_GetEyePose to predict sensor state that should be
        /// used rendering the specified eye.
        /// This combines current absolute time with prediction that is appropriate for this HMD.
        /// It is ok to call BeginEyeRender() on both eyes before calling <see cref="EndEyeRender"/>.
        /// If rendering one eye at a time, it is best to render eye specified by
        /// <see cref="HMDisplayDescription.EyeRenderOrderFirst"/> first.
        /// </summary>
        /// <returns>An <see cref="OculusPose"/> instance.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="eye">The <see cref="EyeType"/> for the left or right eye.</param>
        /// <remarks>
        /// This function support rendering of distortion by the SDK through direct
        /// access to the underlying rendering HW, such as D3D or GL.
        /// This is the recommended approach, as it allows for better support or future
        /// Oculus hardware and a range of low-level optimizations.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_BeginEyeRender", CallingConvention = CallingConvention.Winapi)]
        public static extern OculusPose BeginEyeRender(ovrHmd hmd, EyeType eye);

        /// <summary>
        /// Marks the end of eye rendering and submits the eye texture for display after it is ready.
        /// Rendering viewport within the texture can change per frame if necessary.
        /// Specified texture may be presented immediately or wait until <see cref="EndFrame"/> based
        /// on the implementation. The API performs distortion and scaling internally.
        /// </summary>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="eye">The <see cref="EyeType"/> for the left or right eye.</param>
        /// <param name="renderPose">
        /// A <see cref="OculusPose"/> returned by <see cref="BeginEyeRender"/>
        /// or a custom <see cref="OculusPose"/>.
        /// </param>
        /// <param name="eyeTexture">The <see cref="OculusTexture"/> to render.</param>
        /// <remarks>
        /// This function support rendering of distortion by the SDK through direct
        /// access to the underlying rendering HW, such as D3D or GL.
        /// This is the recommended approach, as it allows for better support or future
        /// Oculus hardware and a range of low-level optimizations.
        /// </remarks>
        [DllImport(lib, EntryPoint = "ovrHmd_EndEyeRender", CallingConvention = CallingConvention.Winapi)]
        public static extern void EndEyeRender(ovrHmd hmd, EyeType eye,
            OculusPose renderPose, ref OculusTexture eyeTexture);

        #endregion

        #region Game-Side Rendering Functions

        // These functions provide distortion data and render timing support necessary to allow
        // game rendering of distortion. Game-side rendering involves the following steps:
        //
        //  1. Setup ovrEyeDesc based on desired texture size and Fov.
        //     Call ovrHmd_GetRenderDesc to get the necessary rendering parameters for each eye.
        // 
        //  2. Use ovrHmd_CreateDistortionMesh to generate distortion mesh.
        //
        //  3. Use ovrHmd_BeginFrameTiming, ovrHmd_GetEyePose and ovrHmd_BeginFrameTiming
        //     in the rendering loop to obtain timing and predicted view orientation for
        //     each eye.
        //      - If relying on timewarp, use ovr_WaitTillTime after rendering+flush, followed
        //        by ovrHmd_GetEyeTimewarpMatrices to obtain timewarp matrices used 
        //        in distortion pixel shader to reduce latency.
        //

        /// <summary>
        /// Computes distortion viewport, view adjust and other rendering for the specified
        /// eye. This can be used instead of <see cref="ConfigureRendering"/> to help setup rendering on
        /// the game side.
        /// </summary>
        /// <returns>A <see cref="EyeRenderDescription"/> instance.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="eyeType">The <see cref="EyeType"/> for the left or right eye.</param>
        /// <param name="fov">The desired <see cref="FovPort"/> configuration.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetRenderDesc", CallingConvention = CallingConvention.Winapi)]
        public static extern EyeRenderDescription GetRenderDescription(ovrHmd hmd,
            EyeType eyeType, FovPort fov);

        /// <summary>
        /// Generate distortion mesh per eye.
        /// Distortion capabilities will depend on 'distortionCaps' flags; user should rely on
        /// appropriate shaders based on their settings.
        /// Distortion mesh data will be allocated and stored into the <see cref="DistortionMesh"/> data structure,
        /// which should be explicitly freed with <see cref="DestroyDistortionMesh"/>.
        /// Users should call <see cref="GetRenderScaleAndOffset"/> to get uvScale and Offset values for rendering.
        /// The function shouldn't fail unless theres is a configuration or memory error, in which case
        /// <see cref="DistortionMesh"/> values will be set to null.
        /// </summary>
        /// <returns><c>true</c>, if the <see cref="DistortionMesh"/> was successfully created, <c>false</c> otherwise.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="eyeType">The <see cref="EyeType"/> for the left or right eye.</param>
        /// <param name="fov">The desired <see cref="FovPort"/> configuration.</param>
        /// <param name="distortionCaps">A bitwise combination of <see cref="DistortionCaps"/> flags.</param>
        /// <param name="meshData">
        /// When this function returns successfully, contains a <see cref="DistortionMesh"/> instance.
        /// </param>
        [DllImport(lib, EntryPoint = "ovrHmd_CreateDistortionMesh", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CreateDistortionMesh(ovrHmd hmd,
            EyeType eyeType, FovPort fov,
            DistortionCaps distortionCaps,
            out DistortionMesh meshData);

        /// <summary>
        /// Frees a distortion mesh allocated by <see cref="CreateDistortionMesh"/>.
        /// </summary>
        /// <param name="meshData">
        /// The <see cref="DistortionMesh"/> to destroy.
        /// All meshData properties are cleared after this call returns.
        /// </param>
        [DllImport(lib, EntryPoint = "ovrHmd_DestroyDistortionMesh", CallingConvention = CallingConvention.Winapi)]
        public static extern void DestroyDistortionMesh(ref DistortionMesh meshData);

        /// <summary>
        /// Computes updated 'uvScaleOffsetOut' to be used with a distortion if render target size or
        /// viewport changes after the fact. This can be used to adjust render size every frame, if desired.
        /// </summary>
        /// <param name="fov">The desired <see cref="FovPort"/> configuration.</param>
        /// <param name="textureSize">The desired texture size.</param>
        /// <param name="renderViewport">The desired render viewport.</param>
        /// <param name="uvScaleOffsetOut">Returns the uv scale offset for each eye.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetRenderScaleAndOffset", CallingConvention = CallingConvention.Winapi)]
        public static extern void GetRenderScaleAndOffset(FovPort fov,
            OculusSize textureSize, OculusRectangle renderViewport,
            [Out][MarshalAs(UnmanagedType.LPArray, SizeConst=2)] Vector2[] uvScaleOffsetOut);

        /// <summary>
        /// Thread-safe timing function for the main thread. Caller should increment frameIndex
        /// with every frame and pass the index to RenderThread for processing.
        /// </summary>
        /// <returns>A <see cref="FrameTiming"/> instance.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="frameIndex">
        /// A monotonously increasing frame index.
        /// The user is responsible for incrementing this value on every frame.
        /// </param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetFrameTiming", CallingConvention = CallingConvention.Winapi)]
        public static extern FrameTiming GetFrameTiming(ovrHmd hmd, int frameIndex);

        /// <summary>
        /// Marks the beginning of the frame on the Render Thread.
        /// </summary>
        /// <returns>A <see cref="FrameTiming"/> instance.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="frameIndex">
        /// Pass 0 if <see cref="GetFrameTiming"/> is not being used. Otherwise,
        /// pass the same frame index as was used for GetFrameTiming on the main thread.
        /// </param>
        [DllImport(lib, EntryPoint = "ovrHmd_BeginFrameTiming", CallingConvention = CallingConvention.Winapi)]
        public static extern FrameTiming BeginFrameTiming(ovrHmd hmd, int frameIndex);

        /// <summary>
        /// Marks the end of game-rendered frame, tracking the necessary timing information. This
        /// function must be called immediately after Present/SwapBuffers + GPU sync. GPU sync is important
        /// before this call to reduce latency and ensure proper timing.
        /// </summary>
        /// <param name="hmd">A valid HMD handle.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_EndFrameTiming", CallingConvention = CallingConvention.Winapi)]
        public static extern void EndFrameTiming(ovrHmd hmd);

        /// <summary>
        /// Initializes and resets frame time tracking. This is typically not necessary, but
        /// is helpful if game changes VSync state or video mode. VSync is assumed to be enabled if this
        /// isn't called.
        /// </summary>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="frameIndex">Specifies the frame index to reset to.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_ResetFrameTiming", CallingConvention = CallingConvention.Winapi)]
        public static extern void ResetFrameTiming(ovrHmd hmd, int frameIndex);

        /// <summary>
        /// Predicts and returns Pose that should be used rendering the specified eye.
        /// Must be called between <see cref="BeginFrameTiming"/> and <see cref="EndFrameTiming"/>.
        /// </summary>
        /// <returns>The estimated <see cref="OculusPose"/> instance.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="eye">The <see cref="EyeType"/> for the left or right eye.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetEyePose", CallingConvention = CallingConvention.Winapi)]
        public static extern OculusPose GetEyePose(ovrHmd hmd, EyeType eye);

        /// <summary>
        /// Computes timewarp matrices used by distortion mesh shader, these are used to adjust
        /// for orientation change since the last call to ovrHmd_GetEyePose for this eye.
        /// The <see cref="DistortionVertex.TimeWarpFactor"/> is used to blend between the matrices,
        /// usually representing two different sides of the screen.
        /// Must be called on the same thread as <see cref="BeginFrameTiming"/>.
        /// </summary>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="eye">The <see cref="EyeType"/> for the left or right eye.</param>
        /// <param name="renderPose">The desired <see cref="OculusPose"/>.</param>
        /// <param name="twmOut">
        /// Returns two <see cref="OpenTK.Matrix4"/> instances, representing
        /// the timewarp matrix for each eye.
        /// </param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetEyeTimewarpMatrices", CallingConvention = CallingConvention.Winapi)]
        public static extern void GetEyeTimewarpMatrices(ovrHmd hmd, EyeType eye,
            OculusPose renderPose, [Out][MarshalAs(UnmanagedType.LPArray, SizeConst=2)] Matrix4[] twmOut);

        #endregion

        #region Stateless math setup functions

        /// <summary>
        /// Calculates the projection matrix for the specified fov, znear, zfar and rightHanded.
        /// </summary>
        /// <param name="fov">The desired <see cref="FovPort"/>.</param>
        /// <param name="znear">The desired near z-plane.</param>
        /// <param name="zfar">The desired far z-plane.</param>
        /// <param name="rightHanded">
        /// If set to <c>true</c> the returned matrix will be right handed.
        /// </param>
        [DllImport(lib, EntryPoint = "ovrMatrix4f_Projection", CallingConvention = CallingConvention.Winapi)]
        public static extern Matrix4 Projection(FovPort fov,
            float znear, float zfar, [MarshalAs(UnmanagedType.I1)] bool rightHanded);

        /// <summary>
        /// Used for 2D rendering, Y is down
        /// </summary>
        /// <returns>An <see cref="OpenTK.Matrix4"/> represensting an orthographic projection matrix.</returns>
        /// <param name="projection">The parent projection matrix, returned by <see cref="Projection"/>.</param>
        /// <param name="orthoScale">The x/y scale for the orthographic projection (1.0f / pixelsPerTanAngleAtCenter)</param>
        /// <param name="orthoDistance">The distance of the projection plane from the camera (e.g. 0.8m).</param>
        /// <param name="eyeViewAdjustX">X eye adjustment factor.</param>
        [DllImport(lib, EntryPoint = "ovrMatrix4f_OrthoSubProjection", CallingConvention = CallingConvention.Winapi)]
        public static extern Matrix4 OrthoSubProjection(Matrix4 projection, Vector2 orthoScale,
            float orthoDistance, float eyeViewAdjustX);

        /// <summary>
        /// Returns global, absolute high-resolution time in seconds. This is the same
        /// value as used in sensor messages.
        /// </summary>
        /// <returns>The time in seconds.</returns>
        [DllImport(lib, EntryPoint = "ovr_GetTimeInSeconds", CallingConvention = CallingConvention.Winapi)]
        public static extern double GetTimeInSeconds();

        /// <summary>
        /// Waits until the specified absolute time.
        /// </summary>
        /// <returns>The absolute time that was waited.</returns>
        /// <param name="absTime">The absolute time to wait.</param>
        [DllImport(lib, EntryPoint = "ovr_WaitTillTime", CallingConvention = CallingConvention.Winapi)]
        public static extern double WaitTillTime(double absTime);

        #endregion

        #region Latency Test interface

        /// <summary>
        /// Performs latency test processing and returns 'TRUE' if specified rgb color should
        /// be used to clear the screen.
        /// </summary>
        /// <returns><c>true</c>, if the latency test conluded successfully, <c>false</c> otherwise.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        /// <param name="rgbColorOut">
        /// When this function returns successfully, contains the optimal RGB color
        /// for clearing the screen.
        /// </param>
        [DllImport(lib, EntryPoint = "ovrHmd_ProcessLatencyTest", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ProcessLatencyTest(ovrHmd hmd,
            [Out][MarshalAs(UnmanagedType.LPStr, SizeConst = 3)] byte rgbColorOut);

        /// <summary>
        /// Returns the results of the latency test. This is a blocking call.
        /// </summary>
        /// <returns>A pointer to a null-terminated string that contains the results of the latency test.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetLatencyTestResult", CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetLatencyTestResultPointer(ovrHmd hmd);

        /// <summary>
        /// Returns the results of the latency test. This is a blocking call.
        /// Note: this function allocates memory.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that contains the results of the latency test.</returns>
        /// <param name="hmd">A valid HMD handle.</param>
        public static string GetLatencyTestResult(ovrHmd hmd)
        {
            return Marshal.PtrToStringAnsi(GetLatencyTestResultPointer(hmd));
        }

        /// <summary>
        /// Returns latency for HMDs that support internal latency testing via the
        /// pixel-read back method.
        /// </summary>
        /// <returns>
        /// The results of the latency test in seconds, or -1 if the latency test failed.
        /// </returns>
        /// <param name="hmd">A valid HMD handle.</param>
        [DllImport(lib, EntryPoint = "ovrHmd_GetMeasuredLatencyTest2", CallingConvention = CallingConvention.Winapi)]
        public static extern double GetMeasuredLatencyTest2(ovrHmd hmd);

        #endregion

        #endregion
    }

    #region Enumerations

    /// <summary>
    /// Enumerates all head-mounted display types that we support.
    /// </summary>
    public enum HMDisplayType
    {
        /// <summary>
        /// Head-mounted displays are not supported
        /// </summary>
        None             = 0,

        /// <summary>
        /// The Development Kit v1
        /// </summary>
        DK1              = 3,

        /// <summary>
        /// The High-Definition Development Kit
        /// </summary>
        DKHD             = 4,

        /// <summary>
        /// The Crystal Cove prototype
        /// </summary>
        CrystalCoveProto = 5,

        /// <summary>
        /// The Development Kit v2
        /// </summary>
        DK2              = 6,

        /// <summary>
        /// Some HMD other then the one in the enumeration
        /// </summary>
        Other
    }

    /// <summary>
    /// Head-mounted display capability flags reported by device.
    /// </summary>
    [Flags]
    public enum HMDisplayCaps
    {
        /// <summary>
        /// This HMD exists (as opposed to being unplugged).
        /// This is a read-only flag.
        /// </summary>
        Present = 0x0001,

        /// <summary>
        /// HMD and sensor are available for use
        /// (if not owned by another app).
        /// This is a read-only flag.
        /// </summary>
        Available = 0x0002,

        // These flags are intended for use with the new driver display mode.
        /*
        ovrHmdCap_ExtendDesktop     = 0x0004,   // Read only, means display driver is in compatibility mode.
        ovrHmdCap_DisplayOff        = 0x0040,   // Turns off Oculus HMD screen and output.
        ovrHmdCap_NoMirrorToWindow  = 0x2000,   // Disables mirrowing of HMD output to the window;
                                                // may improve rendering performance slightly.
        */

        /// <summary>
        /// Supports low persistence mode.
        /// This flag can be modified by <see cref="OpenTK.OculusRift.SetEnabledCaps"/>
        /// </summary>
        LowPersistence = 0x0080,

        /// <summary>
        /// Supports pixel reading for continuous latency testing.
        /// This flag can be modified by <see cref="OpenTK.OculusRift.SetEnabledCaps"/>
        /// </summary>
        LatencyTest = 0x0100,

        /// <summary>
        /// Adjust prediction dynamically based on DK2 Latency.
        /// This flag can be modified by <see cref="OpenTK.OculusRift.SetEnabledCaps"/>
        /// </summary>
        DynamicPrediction = 0x0200,

        /// <summary>
        /// Support rendering without VSync for debugging
        /// This flag can be modified by <see cref="OpenTK.OculusRift.SetEnabledCaps"/>
        /// </summary>
        NoVSync = 0x1000,

        /// <summary>
        /// 
        /// This flag can be modified by <see cref="OpenTK.OculusRift.SetEnabledCaps"/>
        /// </summary>
        NoRestore = 0x4000,

        /// <summary>
        /// Defines which bits can be modified by <see cref="OpenTK.OculusRift.SetEnabledCaps"/>
        /// </summary>
        WritableMask = 0x1380
    }

    /// <summary>
    /// Sensor capability bits reported by device.
    /// Used with ovrHmd_StartSensor.
    /// </summary>
    [Flags]
    public enum SensorCaps
    {
        /// <summary>
        /// Supports orientation tracking (IMU).
        /// </summary>
        Orientation    = 0x0010,

        /// <summary>
        /// Supports yaw correction through magnetometer or other means.
        /// </summary>
        YawCorrection  = 0x0020,

        /// <summary>
        /// Supports positional tracking.
        /// </summary>
        Position       = 0x0040,
    }

    /// <summary>
    /// Distortion capability bits reported by device.
    /// Used with <see cref="OculusRift.ConfigureRendering"/> and
    /// <see cref="OculusRift.CreateDistortionMesh"/>.
    /// </summary>
    [Flags]
    public enum DistortionCaps
    {
        /// <summary>
        /// Supports chromatic aberration correction.
        /// </summary>
        Chromatic  = 0x01,

        /// <summary>
        /// Supports timewarp.
        /// </summary>
        TimeWarp   = 0x02,

        /// <summary>
        /// Supports vignetting around the edges of the view.
        /// </summary>
        Vignette   = 0x08
    }

    /// <summary>
    /// Specifies which eye is being used for rendering.
    /// This type explicitly does not include a third "NoStereo" option, as such is
    /// not required for an HMD-centered API.
    /// </summary>
    public enum EyeType
    {
        /// <summary>
        /// The left eye.
        /// </summary>
        Left  = 0,

        /// <summary>
        /// The right eye.
        /// </summary>
        Right = 1,

        /// <summary>
        /// The number of eyes (2).
        /// </summary>
        Count = 2
    }

    /// <summary>
    /// Bit flags describing the current status of sensor tracking.
    /// </summary>
    [Flags]
    public enum StatusFlags
    {
        /// <summary>
        /// Orientation is currently tracked (connected and in use).
        /// </summary>
        OrientationTracked    = 0x0001,

        /// <summary>
        /// Position is currently tracked (false if out of range).
        /// </summary>
        PositionTracked       = 0x0002,

        /// <summary>
        /// Position tracking HW is connected.
        /// </summary>
        PositionConnected     = 0x0020,

        /// <summary>
        /// HMD Display is connected and available.
        /// </summary>
        HMDisplayConnected = 0x0080
    }

    /// <summary>
    /// These types are used to hide platform-specific details when passing
    /// render device, OS and texture data to the APIs.
    /// </summary>
    /// <remarks>
    /// The benefit of having these wrappers vs. platform-specific API functions is
    /// that they allow game glue code to be portable. A typical example is an
    /// engine that has multiple back ends, say GL and D3D. Portable code that calls
    /// these back ends may also use LibOVR. To do this, back ends can be modified
    /// to return portable types such as ovrTexture and ovrRenderAPIConfig.
    /// </remarks>
    public enum RenderApiType
    {
        /// <summary>
        /// No rendering API.
        /// </summary>
        None,

        /// <summary>
        /// The desktop OpenGL API.
        /// </summary>
        OpenGL,

        /// <summary>
        /// The Android OpenGL ES API.
        /// May include extra native window pointers, etc.
        /// </summary>
        Android_GLES,

        /// <summary>
        /// The Windows Direct3D 9 API.
        /// </summary>
        D3D9,

        /// <summary>
        /// The Windows Direct3D 10 API.
        /// </summary>
        D3D10,

        /// <summary>
        /// The Windows Direct3D 11 API.
        /// </summary>
        D3D11,

        /// <summary>
        /// The maximum number of supported APIs.
        /// </summary>
        Count
    }

    #endregion

    #region Structures

    /// <summary>
    /// Defines a 2d point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OculusPoint
    {
        /// <summary>
        /// The x coordinate of this <see cref="OculusPoint"/>.
        /// </summary>
        public int X;

        /// <summary>
        /// The y coordinate of this <see cref="OculusPoint"/>.
        /// </summary>
        public int Y;
    }

    /// <summary>
    /// Defines a 2d size.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OculusSize
    {
        /// <summary>
        /// The width of this <see cref="OculusSize"/>.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of this <see cref="OculusSize"/>.
        /// </summary>
        public int Height;
    }

    /// <summary>
    /// Defines a 2d rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OculusRectangle
    {
        /// <summary>
        /// An <see cref="OculusPoint"/> resresenting the top-left corner of this <see cref="OculusRectangle"/>.
        /// </summary>
        public OculusPoint Point;

        /// <summary>
        /// An <see cref="OculusSize"/> resresenting the width and height of this <see cref="OculusRectangle"/>.
        /// </summary>
        public OculusSize Size;

        /// <summary>
        /// Gets or sets the x coordinate of this <see cref="OculusRectangle"/>.
        /// </summary>
        public int X
        {
            get { return Point.X; }
            set { Point.X = value; }
        }

        /// <summary>
        /// Gets or sets the y coordinate of this <see cref="OculusRectangle"/>.
        /// </summary>
        public int Y
        {
            get { return Point.Y; }
            set { Point.Y = value; }
        }

        /// <summary>
        /// Gets or sets the width of this <see cref="OculusRectangle"/>.
        /// </summary>
        public int Width
        {
            get { return Size.Width; }
            set { Size.Width = value; }
        }

        /// <summary>
        /// Gets or sets the height of this <see cref="OculusRectangle"/>.
        /// </summary>
        public int Height
        {
            get { return Size.Height; }
            set { Size.Height = value; }
        }
    }

    /// <summary>
    /// Position and orientation together.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OculusPose
    {
        /// <summary>
        /// A <see cref="OpenTK.Quaternion"/> describing the orientation of this <see cref="OculusPose"/>.
        /// </summary>
        public Quaternion Orientation;

        /// <summary>
        /// A <see cref="OpenTK.Vector3"/> describing the position of this <see cref="OculusPose"/>.
        /// </summary>
        public Vector3 Position;
    }

    /// <summary>
    /// Full pose (rigid body) configuration with first and second derivatives.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PoseState
    {
        /// <summary>
        /// The <see cref="OculusPose"/> for this <see cref="PoseState"/>.
        /// </summary>
        public OculusPose Pose;

        /// <summary>
        /// A <see cref="OpenTK.Vector3"/> representing the angular velocity of this <see cref="PoseState"/>.
        /// </summary>
        public Vector3 AngularVelocity;

        /// <summary>
        /// A <see cref="OpenTK.Vector3"/> representing the linear velocity of this <see cref="PoseState"/>.
        /// </summary>
        public Vector3 LinearVelocity;

        /// <summary>
        /// A <see cref="OpenTK.Vector3"/> representing the angular acceleration of this <see cref="PoseState"/>.
        /// </summary>
        public Vector3 AngularAcceleration;

        /// <summary>
        /// A <see cref="OpenTK.Vector3"/> representing the linear acceleration of this <see cref="PoseState"/>.
        /// </summary>
        public Vector3 LinearAcceleration;

        /// <summary>
        /// Absolute time of this state sample.
        /// </summary>
        public double TimeInSeconds;
    }

    /// <summary>
    /// Field Of View (FOV) in tangent of the angle units.
    /// As an example, for a standard 90 degree vertical FOV, we would 
    /// have: { UpTan = tan(90 degrees / 2), DownTan = tan(90 degrees / 2) }.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FovPort
    {
        /// <summary>
        /// The up tangent for this <see cref="FovPort"/>.
        /// </summary>
        public float UpTan;

        /// <summary>
        /// The down tangent for this <see cref="FovPort"/>.
        /// </summary>
        public float DownTan;

        /// <summary>
        /// The left tangent for this <see cref="FovPort"/>.
        /// </summary>
        public float LeftTan;

        /// <summary>
        /// The right tangent for this <see cref="FovPort"/>.
        /// </summary>
        public float RightTan;
    }

    /// <summary>
    /// Describes a head-mounted display device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HMDisplayDescription
    {
        /// <summary>
        /// Handle of this HMD.
        /// </summary>
        public ovrHmd Handle;

        /// <summary>
        /// A <see cref="HMDisplayType"/> value representing the type of the display.
        /// </summary>
        public HMDisplayType Type;

        IntPtr ProductNamePtr;
        IntPtr ManufacturerPtr;

        /// <summary>
        /// A <see cref="System.String"/> describing the product: "Oculus Rift DK1", etc.
        /// </summary>
        public string ProductName
        {
            get { return Marshal.PtrToStringAnsi(ProductNamePtr); }
        }

        /// <summary>
        /// A <see cref="System.String"/> describing the manufacturer of the product.
        /// </summary>
        /// <value>The manufacturer.</value>
        public string Manufacturer
        {
            get { return Marshal.PtrToStringAnsi(ManufacturerPtr); }
        }

        /// <summary>
        /// Capability bits described by <see cref="HMDisplayCaps"/>.
        /// </summary>
        public HMDisplayCaps DisplayCaps;

        /// <summary>
        /// Capability bits described by <see cref="SensorCaps"/>.
        /// </summary>
        public SensorCaps SensorCaps;

        /// <summary>
        /// Capability bits described by <see cref="DistortionCaps"/> .
        /// </summary>
        public DistortionCaps DistortionCaps;

        /// <summary>
        /// Resolution of the entire HMD screen (for both eyes) in pixels.
        /// </summary>
        public OculusSize Resolution;

        /// <summary>
        /// Where monitor window should be on screen or (0,0).
        /// </summary>
        public OculusPoint WindowPos;

        /// <summary>
        /// Recommended (default) optical FOV for the left eye.
        /// </summary>
        public FovPort DefaultEyeFovLeft;

        /// <summary>
        /// Recommended (default) optical FOV for the right eye.
        /// </summary>
        public FovPort DefaultEyeFovRight;

        /// <summary>
        /// Maximum optical FOVs for the left eye.
        /// </summary>
        public FovPort MaxEyeFovLeft;

        /// <summary>
        /// Maximum optical FOVs for the right eye.
        /// </summary>
        public FovPort MaxEyeFovRight;

        /// <summary>
        /// Preferred eye rendering order for best performance.
        /// Can help reduce latency on sideways-scanned screens.
        /// </summary>
        public EyeType EyeRenderOrderFirst;

        /// <summary>
        /// Preferred eye rendering order for best performance.
        /// Can help reduce latency on sideways-scanned screens.
        /// </summary>
        public EyeType EyeRenderOrderSecond;

        // Display that HMD should present on.
        // TBD: It may be good to remove this information relying on WidowPos instead.
        // Ultimately, we may need to come up with a more convenient alternative,
        // such as a API-specific functions that return adapter, ot something that will
        // work with our monitor driver.

        IntPtr DisplayDeviceNamePtr;

        /// <summary>
        /// Returns the name of the display: "\\\\.\\DISPLAY3", etc. Can be used in EnumDisplaySettings/CreateDC.
        /// This property is only valid on Windows; on other platforms it will return <see cref="System.String.Empty"/>
        /// </summary>
        public string DisplayDeviceName
        {
            get
            {
                if (DisplayDeviceNamePtr != IntPtr.Zero)
                {
                    return Marshal.PtrToStringAnsi(DisplayDeviceNamePtr);
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns the id of the display.
        /// This property is only valid on Mac OS X.
        /// </summary>
        public int DisplayId;
    }

    /// <summary>
    /// State of the sensor at a given absolute time.
    /// </summary>
    public struct SensorState
    {
        /// <summary>
        /// Predicted pose configuration at requested absolute time.
        /// One can determine the time difference between predicted and actual
        /// readings by comparing <see cref="PoseState.TimeInSeconds"/>.
        /// </summary>
        public PoseState Predicted;

        /// <summary>
        /// Actual recorded pose configuration based on the sensor sample at a 
        /// moment closest to the requested time.
        /// </summary>
        public PoseState Recorded;

        /// <summary>
        /// Sensor temperature reading, in degrees Celsius, as sample time.
        /// </summary>
        public float Temperature;

        /// <summary>
        /// Sensor status described by <see cref="StatusFlags"/>.
        /// </summary>
        public StatusFlags StatusFlags;
    }

    /// <summary>
    /// Rendering information for each eye, computed either by <see cref="OculusRift.ConfigureRendering"/>
    /// or by <see cref="OculusRift.GetRenderDescription"/>, based on the specified Fov.
    /// Note that the rendering viewport is not included here as it can be 
    /// specified separately and modified per frame though:
    ///    (a) calling <see cref="OculusRift.GetRenderScaleAndOffset"/>with game-rendered api,
    /// or (b) passing different values in <see cref="OculusTexture"/> in case of SDK-rendered distortion.
    /// </summary>
    public struct EyeRenderDescription
    {
        /// <summary>
        /// The <see cref="EyeType"/> described by this instance.
        /// </summary>
        public EyeType Eye;

        /// <summary>
        /// The <see cref="FovPort"/> configuration for this instance.
        /// </summary>
        public FovPort Fov;

        /// <summary>
        /// An <see cref="OculusRectangle"/> representing the viewport after distortion.
        /// </summary>
        public OculusRectangle DistortedViewport;

        /// <summary>
        /// A <see cref="OpenTK.Vector2"/> represting the number of display pixels that will fit in tan(angle) = 1.
        /// </summary>
        public Vector2 PixelsPerTanAngleAtCenter;

        /// <summary>
        /// A <see cref="OpenTK.Vector3"/> represting the translation that must be applied to the view matrix.
        /// </summary>
        public Vector3 ViewAdjust;
    }

    /// <summary>
    /// Describes a vertex used for distortion; this is intended to be converted into
    /// the engine-specific format.
    /// Some fields may be unused based on the selected <see cref="DistortionCaps"/>.
    /// TexG and TexB, for example, are not used if chromatic correction is not requested.
    /// </summary>
    public struct DistortionVertex
    {
        /// <summary>
        /// The vertex position.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Lerp factor between time-warp matrices. Can be encoded in Position.Z
        /// </summary>
        public float TimeWarpFactor;

        /// <summary>
        /// Vignette fade factor. Can be encoded in Position.W
        /// </summary>
        public float VignetteFactor;

        /// <summary>
        /// Texture R coordinates.
        /// </summary>
        public Vector2 TexR;

        /// <summary>
        /// Texture G coordinates.
        /// </summary>
        public Vector2 TexG;

        /// <summary>
        /// Texture B coordinates.
        /// </summary>
        public Vector2 TexB;
    }

    /// <summary>
    /// Describes a full set of distortion mesh data, filled in by <see cref="OpenTK.OculusRift.CreateDistortionMesh"/>.
    /// Contents of this data structure, if not null, should be freed by <see cref="OpenTK.OculusRift.DestroyDistortionMesh"/>.
    /// </summary>
    public struct DistortionMesh
    {
        /// <summary>
        /// An unmanaged array of <see cref="DistortionVertex"/> structures.
        /// </summary>
        public IntPtr VertexData;

        /// <summary>
        /// An unmanaged array of <see cref="System.UInt16"/> indices.
        /// </summary>
        public IntPtr IndexData;

        /// <summary>
        /// The number of <see cref="DistortionVertex"/> elements in <see cref="DistortionMesh.VertexData"/>.
        /// </summary>
        public int VertexCount;

        /// <summary>
        /// The number of <see cref="System.UInt16"/> elements in <see cref="DistortionMesh.IndexData"/>.
        /// </summary>
        public int IndexCount;
    }

    /// <summary>
    /// Represts frame timing information reported by <see cref="OculusRift.BeginFrameTiming"/>
    /// <para>
    /// It is generally expected that the following hold:
    /// ThisFrameSeconds &lt; TimewarpPointSeconds &lt; NextFrameSeconds &lt; 
    /// EyeScanoutSeconds[EyeOrderFirst] &lt;= ScanoutMidpointSeconds &lt;= EyeScanoutSeconds[EyeOrderSecond]
    /// </para>
    /// </summary>
    public struct FrameTiming
    {
        /// <summary>
        /// The amount of time that has passed since the previous frame returned
        /// BeginFrameSeconds value, usable for movement scaling.
        /// This will be clamped to no more than 0.1 seconds to prevent
        /// excessive movement after pauses for loading or initialization.
        /// </summary>
        public float DeltaSeconds;

        /// <summary>
        /// Absolute time value of when rendering of this frame began or is expected to
        /// begin; generally equal to NextFrameSeconds of the previous frame. Can be used
        /// for animation timing.
        /// </summary>
        public double ThisFrameSeconds;

        /// <summary>
        /// Absolute point when IMU expects to be sampled for this frame.
        /// </summary>
        public double TimewarpPointSeconds;

        /// <summary>
        /// Absolute time when frame Present + GPU Flush will finish, and the next frame starts.
        /// </summary>
        public double NextFrameSeconds;

        /// <summary>
        /// Time when when half of the screen will be scanned out. Can be passes as a prediction
        /// value to <see cref="OculusRift.GetSensorState"/> to get general orientation.
        /// </summary>
        public double ScanoutMidpointSeconds;

        /// <summary>
        /// Timing points when each eye will be scanned out to display. Used for rendering each eye. 
        /// </summary>
        public double EyeScanoutSecondsLeft;

        /// <summary>
        /// Timing points when each eye will be scanned out to display. Used for rendering each eye. 
        /// </summary>
        public double EyeScanoutSecondsRight;
    }

    /// <summary>
    /// A platform-independent rendering configuration, which is
    /// used in <see cref="OculusRift.ConfigureRendering"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderConfiguration
    {
        /// <summary>
        /// The <see cref="RenderApiType"/> for this configuration.
        /// </summary>
        public RenderApiType Api;

        /// <summary>
        /// An <see cref="OculusSize"/> instance representing the size of the render texture.
        /// </summary>
        public OculusSize RTSize;

        /// <summary>
        /// An <see cref="System.Int32"/> representing the antialiasing sample count.
        /// </summary>
        public int Multisample;

        // Platform-specific configuration
        #pragma warning disable 0169
        IntPtr PlatformData0;
        IntPtr PlatformData1;
        IntPtr PlatformData2;
        IntPtr PlatformData3;
        IntPtr PlatformData4;
        IntPtr PlatformData5;
        IntPtr PlatformData6;
        IntPtr PlatformData7;
        #pragma warning restore 0169
    }

    /// <summary>
    /// A platform-independent texture description for
    /// <see cref="OculusRift.EndFrame" />.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OculusTexture
    {
        /// <summary>
        /// The <see cref="RenderApiType"/> for this configuration.
        /// </summary>
        public RenderApiType Api;

        /// <summary>
        /// An <see cref="OculusSize"/> instance representing the size of this texture.
        /// </summary>
        public OculusSize TextureSize;

        /// <summary>
        /// Pixel viewport in texture that holds eye image.
        /// </summary>
        public OculusRectangle RenderViewport;

        // Platform-specific configuration
        #pragma warning disable 0169
        IntPtr PlatformData0;
        IntPtr PlatformData1;
        IntPtr PlatformData2;
        IntPtr PlatformData3;
        IntPtr PlatformData4;
        IntPtr PlatformData5;
        IntPtr PlatformData6;
        IntPtr PlatformData7;
        #pragma warning restore 0169
    }

    #endregion
}

