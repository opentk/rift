#ifndef OPENTK_RIFT_OVR_H
#define OPENTK_RIFT_OVR_H

extern "C"
{
    struct OVR_Instance;
    typedef struct
    {
        float x, y, z, w;
    } OVR_Quaternion;

    typedef struct
    {
        float x, y, z;
    } OVR_Vector3;

    typedef struct
    {
        float x, y, z, w;
    } OVR_Vector4;

    OVR_Instance* OVR_Create();
    void OVR_Destroy(OVR_Instance *inst);
    OVR_Quaternion OVR_GetOrientation(OVR_Instance *inst);
    OVR_Vector3 OVR_GetAcceleration(OVR_Instance *inst);
    OVR_Vector3 OVR_GetAngularVelocity(OVR_Instance *inst);
    int OVR_GetHScreenSize(OVR_Instance *inst);
    int OVR_GetVScreenSize(OVR_Instance *inst);
    int OVR_GetVScreenCenter(OVR_Instance *inst);
    int OVR_GetDesktopX(OVR_Instance *inst);
    int OVR_GetDesktopY(OVR_Instance *inst);
    int OVR_GetHResolution(OVR_Instance *inst);
    int OVR_GetVResolution(OVR_Instance *inst);
    int OVR_GetEyeToScreenDistance(OVR_Instance *inst);
    int OVR_GetLensSeparationDistance(OVR_Instance *inst);
    int OVR_GetInterpupillaryDistance(OVR_Instance *inst);
    OVR_Vector4 OVR_GetDistortionK(OVR_Instance *inst);
    OVR_Vector4 OVR_GetChromaAbCorrection(OVR_Instance *inst);
}

#endif
