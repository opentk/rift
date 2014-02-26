#ifndef OPENTK_RIFT_OVR_H
#define OPENTK_RIFT_OVR_H

extern "C"
{
    struct OVR_Instance;
    typedef struct
    {
        float x, y, z, w;
    } Quaternion;

    OVR_Instance* OVR_Create();
    void OVR_Destroy(OVR_Instance *inst);

}

#endif
