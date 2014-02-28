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
using System.Security;


#endregion

using OpenTK;
using System;
using System.Runtime.InteropServices;

namespace OpenTK.Rift
{
    using OVR_Instance = IntPtr;

    /// <summary>
    /// Provides high-level access to an Oculus Rift device.
    /// </summary>
    public class OVR : IDisposable
    {
        OVR_Instance instance;
        bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenTK.Rift.OVR"/> class.
        /// </summary>
        public OVR()
        {
            instance = NativeMethods.Create();
        }

        #region Public Members

        /// <summary>
        /// Gets a <see cref="OpenTK.Quaternion"/> representing
        /// the current accumulated orientation.
        /// Use <see cref="OpenTK.Quaternion.ToAxisAngle()"/> to
        /// convert this quaternion to an axis-angle representation.
        /// Use <see cref="OpenTK.Matrix4.CreateFromQuaternion(OpenTK.Quaternion)"/>
        /// to convert this quaternion to a rotation matrix.
        /// </summary>
        /// <value>The orientation.</value>
        public Quaternion Orientation
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetOrientation(instance);
            }
        }

        /// <summary>
        /// Gets the last absolute acceleration reading, in m/s^2.
        /// </summary>
        /// <value>The acceleration.</value>
        public Vector3 Acceleration
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetAcceleration(instance);
            }
        }

        /// <summary>
        /// Gets the last angular velocity reading, in rad/s.
        /// </summary>
        /// <value>The angular velocity.</value>
        public Vector3 AngularVelocity
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetAngularVelocity(instance);
            }
        }

        /// <summary>
        /// Horizontal resolution of the entire HMD screen in pixels.
        /// Half the HResolution is used for each eye.
        /// This value is 1280 for the Development Kit.
        /// </summary>
        public int HResolution
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetHResolution(instance);
            }
        }

        /// <summary>
        /// Vertical resolution of the entire HMD screen in pixels.
        /// This value is 800 for the Development Kit.
        /// </summary>
        public int VResolution
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetVResolution(instance);
            }
        }

        /// <summary>
        /// Horizontal dimension of the entire HMD screen in meters. Half
        /// HScreenSize is used for each eye. The current physical screen size is
        /// 149.76 x 93.6mm, which will be reported as 0.14976f x 0.0935f.
        /// </summary>
        public int HScreenSize
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetHScreenSize(instance);
            }
        }

        /// <summary>
        /// Vertical dimension of the entire HMD screen in meters. Half
        /// HScreenSize is used for each eye. The current physical screen size is
        /// 149.76 x 93.6mm, which will be reported as 0.14976f x 0.0935f.
        /// </summary>
        public int VScreenSize
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetVScreenSize(instance);
            }
        }

        /// <summary>
        /// Physical offset from the top of the screen to eye center, in meters.
        /// Currently half VScreenSize.
        /// </summary>
        public int VScreenCenter
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetVScreenCenter(instance);
            }
        }

        /// <summary>
        /// Physical distance between the lens centers, in meters. Lens centers
        /// are the centers of distortion.
        /// </summary>
        public int LensSeparationDistance
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetLensSeparationDistance(instance);
            }
        }

        /// <summary>
        /// Configured distance between eye centers.
        /// </summary>
        public int InterpupillaryDistance
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetInterpulpillaryDistance(instance);
            }
        }

        /// <summary>
        /// Distance from the eye to the screen, in meters. It combines
        /// distances from the eye to the lens, and from the lens to the screen.
        /// This value is needed to compute the fov correctly.
        /// </summary>
        public int EyeToScreenDistance
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetEyeToScreenDistance(instance);
            }
        }

        /// <summary>
        /// Gets the radial distortion correction coefficients.
        /// </summary>
        /// <value>The distortion k.</value>
        public Vector4 DistortionK
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetDistortionK(instance);
            }
        }

        /// <summary>
        /// Gets the chroma AB aberration coefficients.
        /// </summary>
        public Vector4 ChromaAbAberration
        {
            get
            {
                CheckDisposed();
                return NativeMethods.GetChromaAbCorrection(instance);
            }
        }

        #endregion

        #region Private Members

        void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("OVR");
            }
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Releases all resource used by the <see cref="OpenTK.Rift.OVR"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="OpenTK.Rift.OVR"/>. The
        /// <see cref="Dispose()"/> method leaves the <see cref="OpenTK.Rift.OVR"/> in an unusable state. After calling
        /// <see cref="Dispose()"/>, you must release all references to the <see cref="OpenTK.Rift.OVR"/> so the garbage
        /// collector can reclaim the memory that the <see cref="OpenTK.Rift.OVR"/> was occupying.</remarks>
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
                    NativeMethods.Destroy(instance);
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="OpenTK.Rift.OVR"/>
        /// is reclaimed by garbage collection.
        /// </summary>
        ~OVR()
        {
            Dispose(false);
        }

        #endregion

        #region NativeMethods

        static class NativeMethods
        {
            const string lib = "OVR";

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_Create")]
            public static extern OVR_Instance Create();

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_Destroy")]
            public static extern OVR_Instance Destroy(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetOrientation")]
            public static extern Quaternion GetOrientation(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetAcceleration")]
            public static extern Vector3 GetAcceleration(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetAngularVelocity")]
            public static extern Vector3 GetAngularVelocity(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetHScreenSize")]
            public static extern int GetHScreenSize(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetVScreenSize")]
            public static extern int GetVScreenSize(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetVScreenCenter")]
            public static extern int GetVScreenCenter(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetDesktopX")]
            public static extern int GetDesktopX(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetDesktopY")]
            public static extern int GetDesktopY(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetHResolution")]
            public static extern int GetHResolution(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetVResolution")]
            public static extern int GetVResolution(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetEyeToScreenDistance")]
            public static extern int GetEyeToScreenDistance(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetLensSeparationDistance")]
            public static extern int GetLensSeparationDistance(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetInterpupillaryDistance")]
            public static extern int GetInterpulpillaryDistance(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetDistortionK")]
            public static extern Vector4 GetDistortionK(OVR_Instance inst);

            [SuppressUnmanagedCodeSecurity]
            [DllImport(lib, EntryPoint = "OVR_GetChromaAbCorrection")]
            public static extern Vector4 GetChromaAbCorrection(OVR_Instance inst);
        }

        #endregion
    }
}

