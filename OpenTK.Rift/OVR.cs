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

    public class OVR : IDisposable
    {
        OVR_Instance instance;

        public OVR()
        {
            instance = NativeMethods.Create();
        }

        #region Public Members

        public Quaternion Orientation
        {
            get
            {
                return NativeMethods.GetOrientation(instance);
            }
        }

        public Vector3 Acceleration
        {
            get
            {
                return NativeMethods.GetAcceleration(instance);
            }
        }

        public Vector3 AngularVelocity
        {
            get
            {
                return NativeMethods.GetAngularVelocity(instance);
            }
        }

        public int HResolution
        {
            get
            {
                return NativeMethods.GetHResolution(instance);
            }
        }

        public int VResolution
        {
            get
            {
                return NativeMethods.GetVResolution(instance);
            }
        }

        public int HScreenSize
        {
            get
            {
                return NativeMethods.GetHScreenSize(instance);
            }
        }

        public int VScreenSize
        {
            get
            {
                return NativeMethods.GetVScreenSize(instance);
            }
        }

        public int VScreenCenter
        {
            get
            {
                return NativeMethods.GetVScreenCenter(instance);
            }
        }

        public int LensSeparationDistance
        {
            get
            {
                return NativeMethods.GetLensSeparationDistance(instance);
            }
        }

        public int InterpupillaryDistance
        {
            get
            {
                return NativeMethods.GetInterpulpillaryDistance(instance);
            }
        }

        public int EyeToScreenDistance
        {
            get
            {
                return NativeMethods.GetEyeToScreenDistance(instance);
            }
        }

        public Vector4 DistortionK
        {
            get
            {
                return NativeMethods.GetDistortionK(instance);
            }
        }

        public Vector4 ChromaAbAberration
        {
            get
            {
                return NativeMethods.GetChromaAbCorrection(instance);
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool manual)
        {
            if (manual)
            {
                NativeMethods.Destroy(instance);
            }
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

