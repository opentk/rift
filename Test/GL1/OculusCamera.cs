#region License
//
// OculusCamera.cs
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

using System;
using OpenTK;

namespace OpenTK.Rift.Test
{
    enum CameraType
    {
        Default = 0,
        Left,
        Right
    }

    sealed class OculusCamera
    {
        readonly OculusRift rift;
        Matrix4 mono_frustum = Matrix4.Identity;
        Matrix4 left_frustum = Matrix4.Identity;
        Matrix4 right_frustum = Matrix4.Identity;
        Matrix4 left_modelview = Matrix4.Identity;
        Matrix4 right_modelview = Matrix4.Identity;
        Quaternion nonrift_orientation = Quaternion.Identity;
        float ZNear = 0.3f;
        float ZFar = 1000.0f;

        #region Constructors

        public OculusCamera(OculusRift rift)
            : this(rift, Vector3.Zero, Quaternion.Identity)
        { }

        public OculusCamera(OculusRift rift, Vector3 position, Quaternion orientation)
        {
            if (rift == null)
                throw new ArgumentNullException();
            this.rift = rift;

            Position = position;
            Orientation = orientation;

            EventHandler<EventArgs> update_frustum = (sender, e) =>
            {
                float view_center = rift.HScreenSize * 0.25f;
                float eye_projection_shift = view_center - rift.LensSeparationDistance * 0.5f;
                float projection_center_offset =
                    rift.IsConnected ?
                    4.0f * eye_projection_shift / rift.HScreenSize :
                    0;

                GetProjectionMatrix(out mono_frustum);
                left_frustum = mono_frustum * Matrix4.CreateTranslation(projection_center_offset, 0, 0);
                right_frustum = mono_frustum * Matrix4.CreateTranslation(-projection_center_offset, 0, 0);

                float half_ipd = rift.InterpupillaryDistance * 0.5f;
                if (view_center != 0)
                {
                    left_modelview = Matrix4.CreateTranslation(half_ipd * view_center, 0, 0);
                    right_modelview = Matrix4.CreateTranslation(-half_ipd * view_center, 0, 0);
                }
            };

            //FieldOfViewChanged += update_frustum;
            //AspectRatioChanged += update_frustum;
            //ZNearChanged += update_frustum;
            FusionDistanceChanged += update_frustum;
            InterocularDistanceChanged += update_frustum;

            update_frustum(this, EventArgs.Empty);
        }

        #endregion

        #region Events

        public event EventHandler<EventArgs> InterocularDistanceChanged = delegate { };
        public event EventHandler<EventArgs> FusionDistanceChanged = delegate { };

        #endregion

        #region Properties

        public Vector3 Position { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Quaternion Orientation
        {
            get { return nonrift_orientation * Quaternion.Conjugate(rift.PredictedOrientation); }
            set { nonrift_orientation = value; }
        }
        public Quaternion TargetOrientation { get; set; }

        public float AspectRatio
        {
            get
            {
                if (rift.IsConnected)
                {
                    return 0.5f * rift.HResolution / (float)rift.VResolution;
                }
                else
                {
                    return 16.0f / 9.0f;
                }
            }
        }

        public float FieldOfView
        {
            get
            {
                if (rift.IsConnected)
                {
                    return (float)(
                        2.0f * Math.Atan(
                            0.5f * rift.VScreenSize / rift.EyeToScreenDistance));
                }
                else
                {
                    return MathHelper.PiOver4;
                }
            }
        }

        public float InterocularDistance
        {
            get { return rift.InterpupillaryDistance; }
            #if false
            set
            {
                if (value < 0)
                    interocular_distance = 0;
                interocular_distance = value;

                InterocularDistanceChanged(this, EventArgs.Empty);
            }
            #endif
        }

        #if false
        // Not necessary for the Oculus Rift
        float FusionDistance
        {
            get { return fusion_distance; }
            set
            {
                if (value < ZNear)
                    fusion_distance = ZNear;
                fusion_distance = value;

                FusionDistanceChanged(this, EventArgs.Empty);
            }
        }
        #endif

        #endregion

        #region Methods

        void GetProjectionMatrix(out Matrix4 matrix)
        {
            matrix = Matrix4.CreatePerspectiveFieldOfView(
                FieldOfView,
                AspectRatio,
                ZNear,
                ZFar);
        }

        void GetModelviewMatrix(out Matrix4 matrix)
        {
            var translation_matrix = Matrix4.CreateTranslation(-Position);
            var rotation_matrix = Matrix4.CreateFromQuaternion(Orientation);
            Matrix4.Mult(ref translation_matrix, ref rotation_matrix, out matrix);
        }

        void GetRotationMatrix(out Matrix4 matrix)
        {
            matrix = Matrix4.CreateFromQuaternion(Orientation);
        }

        public void GetProjectionMatrix(CameraType type, out Matrix4 matrix)
        {
            switch (type)
            {
                case CameraType.Default:
                    matrix = mono_frustum;
                    break;

                case CameraType.Left:
                    matrix = left_frustum;
                    break;

                case CameraType.Right:
                    matrix = right_frustum;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public void GetModelviewMatrix(CameraType type, out Matrix4 matrix)
        {
            this.GetModelviewMatrix(out matrix);
            Console.WriteLine(matrix);
            Console.WriteLine(left_modelview);
            Console.WriteLine(right_modelview);

            switch (type)
            {
                case CameraType.Default:
                    // Nothing to do
                    break;

                case CameraType.Left:
                    Matrix4.Mult(ref matrix, ref left_modelview, out matrix);
                    break;

                case CameraType.Right:
                    Matrix4.Mult(ref matrix, ref right_modelview, out matrix);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public void GetModelviewProjectionMatrix(CameraType type, out Matrix4 matrix)
        {
            Matrix4 modelview;
            GetModelviewMatrix(type, out modelview);
            GetProjectionMatrix(type, out matrix);
            Matrix4.Mult(ref modelview, ref matrix, out matrix);
        }

        public void GetRotationMatrix(CameraType type, out Matrix4 matrix)
        {
            this.GetRotationMatrix(out matrix);
        }

        #endregion
    }
}

