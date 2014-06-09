#region License
//
// OculusRift.cs
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

    /// <summary>
    /// Provides high-level access to an Oculus Rift device.
    /// </summary>
    [Obsolete("Use the VR class to access the official libOVR C API")]
    public class OculusRift : IDisposable
    {
        static int instance_count;
        static Toolkit opentk;
        bool disposed;

        // For compatibility with the old 0.2.x API
        HMDisplay instance;
        HMDisplayDescription description;
        float prediction_delta;
        bool prediction_enabled;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTK.OculusRift"/> class.
        /// </summary>
        [Obsolete("Use the VR class to access the official libOVR C API")]
        public OculusRift()
        {
            if (Interlocked.Increment(ref instance_count) == 1)
            {
                opentk = Toolkit.Init();
                if (!VR.Initialize())
                {
                    throw new NotSupportedException("OculusRift failed to initialize");
                }
            }

            int count = VR.Detect();
            if (count > 0)
            {
                instance = VR.Create(0);
                if (instance != HMDisplay.Zero)
                {
                    VR.GetDescription(instance, out description);
                }
                else
                {
                    throw new NotSupportedException("Failed to open the VR device");
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
                return instance != HMDisplay.Zero;
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
                if (IsConnected)
                {
                    var state = VR.GetSensorState(instance, IsPredictionEnabled ? PredictionDelta : 0);
                    return state.Recorded.Pose.Orientation;
                }
                else
                {
                    return Quaternion.Identity;
                }
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
                if (IsConnected)
                {
                    var state = VR.GetSensorState(instance, IsPredictionEnabled ? PredictionDelta : 0);
                    return state.Predicted.Pose.Orientation;
                }
                else
                {
                    return Quaternion.Identity;
                }
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
                if (IsConnected)
                {
                    var state = VR.GetSensorState(instance, IsPredictionEnabled ? PredictionDelta : 0);
                    return state.Recorded.AngularAcceleration;
                }
                else
                {
                    return Vector3.Zero;
                }
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
                if (IsConnected)
                {
                    var state = VR.GetSensorState(instance, IsPredictionEnabled ? PredictionDelta : 0);
                    return state.Recorded.AngularVelocity;
                }
                else
                {
                    return Vector3.Zero;
                }
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
                        VR.Shutdown();
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
    }
}
