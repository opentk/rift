using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTK
{
    public static class OculusRiftExtensions
    {
        #region Public Members

        public static float GetFieldOfView(this OculusRift rift)
        {
            if (rift == null)
                throw new ArgumentNullException();
            if (!rift.IsConnected)
                return 45.0f;

            return 2.0f * (float)Math.Atan(0.5f * rift.VScreenSize / rift.EyeToScreenDistance);
        }

        public static float GetAspectRatio(this OculusRift rift)
        {
            if (rift == null)
                throw new ArgumentNullException();
            if (!rift.IsConnected)
                return 16.0f / 9.0f;

            return 0.5f * rift.HResolution / (float)rift.VResolution;
        }

        public static Matrix4 GetLeftProjection(this OculusRift rift, float near, float far)
        {
            Matrix4 projection;
            GetProjection(rift, near, far, 1, out projection);
            return projection;
        }

        public static void GetLeftProjection(this OculusRift rift, float near, float far,
            out Matrix4 projection)
        {
            GetProjection(rift, near, far, 1, out projection);
        }

        public static Matrix4 GetRightProjection(this OculusRift rift, float near, float far)
        {
            Matrix4 projection;
            GetProjection(rift, near, far, -1, out projection);
            return projection;
        }

        public static void GetRightProjection(this OculusRift rift, float near, float far,
            out Matrix4 projection)
        {
            GetProjection(rift, near, far, -1, out projection);
        }

        public static Matrix4 GetLeftModelview(this OculusRift rift)
        {
            Matrix4 modelview;
            rift.GetLeftModelview(out modelview);
            return modelview;
        }

        public static void GetLeftModelview(this OculusRift rift,
            out Matrix4 modelview)
        {
            GetModelview(rift, 1, out modelview);
        }

        public static void GetRightModelview(this OculusRift rift,
            out Matrix4 modelview)
        {
            GetModelview(rift, -1, out modelview);
        }

        public static Matrix4 GetRightModelview(this OculusRift rift)
        {
            Matrix4 modelview;
            rift.GetRightModelview(out modelview);
            return modelview;
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

        static void GetModelview(OculusRift rift, float eye,
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
