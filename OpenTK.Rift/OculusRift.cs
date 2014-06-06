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
    using OVR_Instance = IntPtr;
    using ovrHmd = IntPtr;

    /// <summary>
    /// Provides high-level access to an Oculus Rift device.
    /// </summary>
    public class OculusRift : IDisposable
    {
        static int instance_count;
        static Toolkit opentk;
        bool disposed;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTK.OculusRift"/> class.
        /// </summary>
        public OculusRift()
        {
            if (Interlocked.Increment(ref instance_count) == 1)
            {
                opentk = Toolkit.Init();
                if (!NativeMethods.Initialize())
                {
                    throw new NotSupportedException("OculusRift failed to initialize");
                }
            }
        }

        #endregion

        #region Public Members

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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            set
            {
                CheckDisposed();
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            set
            {
                CheckDisposed();
                throw new NotImplementedException();
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
                        NativeMethods.Shutdown();
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

        #region NativeMethods

        static class NativeMethods
        {
            const string lib = "OVR";

            static NativeMethods()
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

            [DllImport("kernel32.dll")]
            static extern IntPtr LoadLibrary(string filename);

            #region DistortionRenderer

            [DllImport(lib, EntryPoint = "ovr_Initialize", CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool Initialize();

            [DllImport(lib, EntryPoint = "ovr_Shutdown", CallingConvention = CallingConvention.Winapi)]
            public static extern void Shutdown();

            // Detects or re-detects HMDs and reports the total number detected.
            // Users can get information about each HMD by calling ovrHmd_Create with an index.
            [DllImport(lib, EntryPoint = "ovrHmd_Detect", CallingConvention = CallingConvention.Winapi)]
            public static extern int Detect();

            // Creates a handle to an HMD and optionally fills in data about it.
            // Index can [0 .. ovrHmd_Detect()-1]; index mappings can cange after each ovrHmd_Detect call.
            // If not null, returned handle must be freed with ovrHmd_Destroy.
            [DllImport(lib, EntryPoint = "ovrHmd_Create", CallingConvention = CallingConvention.Winapi)]
            public static extern ovrHmd Create(int index);

            [DllImport(lib, EntryPoint = "ovrHmd_Destroy", CallingConvention = CallingConvention.Winapi)]
            public static extern void Destroy(ovrHmd hmd);

            // Creates a "fake" HMD used for debugging only. This is not tied to specific hardware,
            // but may be used to debug some of the related rendering.
            [DllImport(lib, EntryPoint = "ovrHmd_CreateDebug", CallingConvention = CallingConvention.Winapi)]
            public static extern ovrHmd CreateDebug(HMDisplayType type);

            // Returns last error for HMD state. Returns null for no error.
            // String is valid until next call or GetLastError or HMD is destroyed.
            // Pass null hmd to get global error (for create, etc).
            [DllImport(lib, EntryPoint = "ovrHmd_GetLastError", CallingConvention = CallingConvention.Winapi)]
            static extern IntPtr GetLastError(ovrHmd hmd);

            // Returns capability bits that are enabled at this time; described by ovrHmdCaps.
            // Note that this value is different font ovrHmdDesc::HmdCaps, which describes what
            // capabilities are available.
            [DllImport(lib, EntryPoint = "ovrHmd_GetEnabledCaps", CallingConvention = CallingConvention.Winapi)]
            public static extern HMDisplayCaps GetEnabledCaps(ovrHmd hmd);

            // Modifies capability bits described by ovrHmdCaps that can be modified,
            // such as ovrHmd_LowPersistance.
            [DllImport(lib, EntryPoint = "ovrHmd_SetEnabledCaps", CallingConvention = CallingConvention.Winapi)]
            public static extern void SetEnabledCaps(ovrHmd hmd, HMDisplayCaps hmdCaps);

            // All sensor interface functions are thread-safe, allowing sensor state to be sampled
            // from different threads.
            // Starts sensor sampling, enabling specified capabilities, described by ovrSensorCaps.
            //  - supportedSensorCaps specifies support that is requested. The function will succeed 
            //    even if these caps are not available (i.e. sensor or camera is unplugged). Support
            //    will automatically be enabled if such device is plugged in later. Software should
            //    check ovrSensorState.StatusFlags for real-time status.
            //  - requiredSensorCaps specify sensor capabilities required at the time of the call.
            //    If they are not available, the function will fail. Pass 0 if only specifying
            //    supportedSensorCaps.
            [DllImport(lib, EntryPoint = "ovrHmd_StartSensor", CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool StartSensor(ovrHmd hmd,
                HMDisplayCaps supportedSensorCaps,
                HMDisplayCaps requiredSensorCaps);

            // Stops sensor sampling, shutting down internal resources.
            [DllImport(lib, EntryPoint = "ovrHmd_StopSensor", CallingConvention = CallingConvention.Winapi)]
            public static extern void ovrHmd_StopSensor(ovrHmd hmd);

            // Resets sensor orientation.
            [DllImport(lib, EntryPoint = "ovrHmd_ResetSensor", CallingConvention = CallingConvention.Winapi)]
            public static extern void ovrHmd_ResetSensor(ovrHmd hmd);

            // Returns sensor state reading based on the specified absolute system time.
            // Pass absTime value of 0.0 to request the most recent sensor reading; in this case
            // both PredictedPose and SamplePose will have the same value.
            // ovrHmd_GetEyePredictedSensorState relies on this internally.
            // This may also be used for more refined timing of FrontBuffer rendering logic, etc.
            [DllImport(lib, EntryPoint = "ovrHmd_GetSensorState", CallingConvention = CallingConvention.Winapi)]
            public static extern SensorState ovrHmd_GetSensorState(ovrHmd hmd, double absTime);

            // Returns information about a sensor.
            // Only valid after StartSensor.
            [DllImport(lib, EntryPoint = "ovrHmd_GetSensorDesc", CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool ovrHmd_GetSensorDesc(ovrHmd hmd, StringBuilder descOut);

            #endregion
        }

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

    // Distortion capability bits reported by device.
    // Used with ovrHmd_ConfigureRendering and ovrHmd_CreateDistortionMesh.
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
        /// HMD Display is available & connected.
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

    [StructLayout(LayoutKind.Sequential)]
    public struct OculusPoint
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OculusSize
    {
        public int Width;
        public int Height;
    }

    /// <summary>
    /// Position and orientation together.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Pose
    {
        Quaternion Orientation;
        Vector3 Position;
    }

    /// <summary>
    /// Full pose (rigid body) configuration with first and second derivatives.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PoseState
    {
        public Pose Pose;
        public Vector3 AngularVelocity;
        public Vector3 LinearVelocity;
        public Vector3 AngularAcceleration;
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
        public float UpTan;
        public float DownTan;
        public float LeftTan;
        public float RightTan;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct HMDisplayDescription
    {
        /// <summary>
        /// Handle of this HMD.
        /// </summary>
        public ovrHmd Handle;
        public HMDisplayType Type;

        IntPtr ProductNamePtr;
        IntPtr ManufacturerPtr;

        /// <summary>
        /// Name string describing the product: "Oculus Rift DK1", etc.
        /// </summary>
        /// <value>The name of the product.</value>
        public string ProductName
        {
            get { return Marshal.PtrToStringAnsi(ProductNamePtr); }
        }

        public string Manufacturer
        {
            get { return Marshal.PtrToStringAnsi(ManufacturerPtr); }
        }

        /// <summary>
        /// Capability bits described by <see cref="HMDisplayCaps"/>.
        /// </summary>
        public HMDisplayCaps HMDisplayCaps;

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
        public OculusPoint WindowsPos;

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

        // Windows: "\\\\.\\DISPLAY3", etc. Can be used in EnumDisplaySettings/CreateDC.
        IntPtr DisplayDeviceNamePtr;
        // MacOS
        int DisplayId;
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

    #endregion
}

