using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTK
{
    /// <summary>
    /// Contains extension methods to simplify interaction with
    /// <see cref="OpenTK.OculusRift"/>.
    /// </summary>
    public static class OculusRiftExtensions
    {
        #region Public Members

        /// <summary>
        /// Gets the <see cref="OpenTK.DisplayDevice"/> that
        /// corresponds to the specified <see cref="OpenTK.OculusRift"/>
        /// device.
        /// </summary>
        /// <returns>
        /// The <see cref="OpenTK.DisplayDevice"/> that corresponds
        /// to an Oculus Rift, or <see cref="OpenTK.DisplayDevice.Default"/>
        /// if no Oculus Rift device is connected.
        /// </returns>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        public static DisplayDevice GetDisplay(this OculusRift rift)
        {
            if (rift == null)
                throw new ArgumentNullException();

            if (rift.IsConnected)
            {
                bool is_rift = false;
                for (var i = DisplayIndex.First; i < DisplayIndex.Sixth; i++)
                {
                    var display = DisplayDevice.GetDisplay(i);
                    is_rift = display != null;
                    is_rift &= display.Width == rift.HResolution;
                    is_rift &= display.Height == rift.VResolution;
                    if (is_rift)
                    {
                        return display;
                    }
                }
            }
            return DisplayDevice.Default;
        }

        /// <summary>
        /// Gets the field of view of the specified <see cref="OpenTK.OculusRift"/>, in radians.
        /// </summary>
        /// <returns>
        /// The field of view of an <see cref="OpenTK.OculusRift"/>, in radians.
        /// </returns>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        public static float GetFieldOfView(this OculusRift rift)
        {
            if (rift == null)
                throw new ArgumentNullException();
            if (!rift.IsConnected)
                return 45.0f;

            return 2.0f * (float)Math.Atan(0.5f * rift.VScreenSize / rift.EyeToScreenDistance);
        }

        /// <summary>
        /// Gets the aspect ratio of the specified <see cref="OpenTK.OculusRift"/>,
        /// for a single eye.
        /// </summary>
        /// <returns>The aspect ratio of an <see cref="OpenTK.OculusRift"/>.</returns>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        public static float GetAspectRatio(this OculusRift rift)
        {
            if (rift == null)
                throw new ArgumentNullException();
            if (!rift.IsConnected)
                return 16.0f / 9.0f;

            return 0.5f * rift.HResolution / (float)rift.VResolution;
        }

        /// <summary>
        /// Gets the left projection matrix for the specified <see cref="OpenTK.OculusRift"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="OpenTK.Matrix4"/> representing the projection matrix for the left eye.
        /// </returns>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        /// <param name="near">The near clipping plane of the view frustum.</param>
        /// <param name="far">The far clipping plane of the view frustum.</param>
        public static Matrix4 GetLeftProjection(this OculusRift rift, float near, float far)
        {
            Matrix4 projection;
            GetProjection(rift, near, far, 1, out projection);
            return projection;
        }

        /// <summary>
        /// Gets the left projection matrix for the specified <see cref="OpenTK.OculusRift"/>.
        /// </summary>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        /// <param name="near">The near clipping plane of the view frustum.</param>
        /// <param name="far">FThe far clipping plane of the view frustumar.</param>
        /// <param name="projection">
        /// A <see cref="OpenTK.Matrix4"/> representing the projection matrix for the left eye.
        /// </param>
        public static void GetLeftProjection(this OculusRift rift, float near, float far,
            out Matrix4 projection)
        {
            GetProjection(rift, near, far, 1, out projection);
        }

        /// <summary>
        /// Gets the right projection matrix for the specified <see cref="OpenTK.OculusRift"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="OpenTK.Matrix4"/> representing the projection matrix for the right eye.
        /// </returns>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        /// <param name="near">The near clipping plane of the view frustum.</param>
        /// <param name="far">The far clipping plane of the view frustum.</param>
        public static Matrix4 GetRightProjection(this OculusRift rift, float near, float far)
        {
            Matrix4 projection;
            GetProjection(rift, near, far, -1, out projection);
            return projection;
        }

        /// <summary>
        /// Gets the right projection matrix for the specified <see cref="OpenTK.OculusRift"/>.
        /// </summary>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        /// <param name="near">The near clipping plane of the view frustum.</param>
        /// <param name="far">The far clipping plane of the view frustum.</param>
        /// <param name="projection">
        /// A <see cref="OpenTK.Matrix4"/> representing the projection matrix for the right eye.
        /// </param>
        public static void GetRightProjection(this OculusRift rift, float near, float far,
            out Matrix4 projection)
        {
            GetProjection(rift, near, far, -1, out projection);
        }

        /// <summary>
        /// Gets the left translation matrix for the specified <see cref="OpenTK.OculusRift"/> instance.
        /// </summary>
        /// <returns>
        /// A <see cref="OpenTK.Matrix4"/> representing the translation matrix for the left eye.
        /// </returns>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        public static Matrix4 GetLeftTranslation(this OculusRift rift)
        {
            Matrix4 modelview;
            rift.GetLeftTranslation(out modelview);
            return modelview;
        }

        /// <summary>
        /// Gets the left translation matrix for the specified <see cref="OpenTK.OculusRift"/> instance.
        /// </summary>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        /// <param name="modelview">
        /// A <see cref="OpenTK.Matrix4"/> representing the translation matrix for the left eye.
        /// </param>
        public static void GetLeftTranslation(this OculusRift rift,
            out Matrix4 modelview)
        {
            GetTranslation(rift, 1, out modelview);
        }

        /// <summary>
        /// Gets the right translation matrix for the specified <see cref="OpenTK.OculusRift"/> instance.
        /// </summary>
        /// <returns>
        /// A <see cref="OpenTK.Matrix4"/> representing the translation matrix for the right eye.
        /// </returns>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        public static Matrix4 GetRightTranslation(this OculusRift rift)
        {
            Matrix4 modelview;
            rift.GetRightTranslation(out modelview);
            return modelview;
        }

        /// <summary>
        /// Gets the right translation matrix for the specified <see cref="OpenTK.OculusRift"/> instance.
        /// </summary>
        /// <param name="rift">An <see cref="OpenTK.OculusRift"/> instance.</param>
        /// <param name="modelview">
        /// A <see cref="OpenTK.Matrix4"/> representing the translation matrix for the right eye.
        /// </param>
        public static void GetRightTranslation(this OculusRift rift,
            out Matrix4 modelview)
        {
            GetTranslation(rift, -1, out modelview);
        }

        #endregion

        #region Private Members

        static void GetProjection(OculusRift rift, float near, float far, float eye,
            out Matrix4 projection)
        {
            if (rift == null)
                throw new ArgumentNullException();

            Matrix4.CreatePerspectiveFieldOfView(
                rift.GetFieldOfView(), rift.GetAspectRatio(), near, far,
                out projection);

            if (rift.IsConnected)
            {
                float view_center = rift.HScreenSize * 0.25f;
                float eye_projection_shift =
                    view_center - rift.LensSeparationDistance * 0.5f;
                float projection_center_offset =
                    eye * 4.0f * eye_projection_shift / rift.HScreenSize;
                float half_ipd = rift.InterpupillaryDistance * 0.5f;

                Matrix4 translation;
                Matrix4.CreateTranslation(
                    projection_center_offset, 0, 0,
                    out translation);

                Matrix4.Mult(ref projection, ref translation, out projection);
            }
        }

        static void GetTranslation(OculusRift rift, float eye,
            out Matrix4 modelview)
        {
            float view_center = rift.HScreenSize * 0.25f;
            float half_ipd = rift.InterpupillaryDistance * 0.5f;
            float translation = eye * half_ipd * view_center;
            Matrix4.CreateTranslation(translation, 0, 0,
                out modelview);
        }

        #endregion
    }
}
