//
// OVR_wrapper.h
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

    void OVR_Init();
    void OVR_Shutdown();
    OVR_Instance* OVR_Create();
    void OVR_Destroy(OVR_Instance *inst);
    int OVR_IsConnected(OVR_Instance *inst);
    OVR_Vector4 OVR_GetDistortionK(OVR_Instance *inst);
    OVR_Vector4 OVR_GetChromaAbCorrection(OVR_Instance *inst);
    int OVR_GetDesktopX(OVR_Instance *inst);
    int OVR_GetDesktopY(OVR_Instance *inst);
    int OVR_GetHResolution(OVR_Instance *inst);
    int OVR_GetVResolution(OVR_Instance *inst);
    float OVR_GetHScreenSize(OVR_Instance *inst);
    float OVR_GetVScreenSize(OVR_Instance *inst);
    float OVR_GetVScreenCenter(OVR_Instance *inst);
    float OVR_GetEyeToScreenDistance(OVR_Instance *inst);
    float OVR_GetLensSeparationDistance(OVR_Instance *inst);
    float OVR_GetInterpupillaryDistance(OVR_Instance *inst);

    // Sensor Fusion
    OVR_Quaternion OVR_GetOrientation(OVR_Instance *inst);
    OVR_Quaternion OVR_GetPredictedOrientation(OVR_Instance *inst);
    OVR_Vector3 OVR_GetAcceleration(OVR_Instance *inst);
    OVR_Vector3 OVR_GetAngularVelocity(OVR_Instance *inst);
    float OVR_GetPredictionDelta(OVR_Instance *inst);
    void OVR_SetPrediction(OVR_Instance *inst, float dt, int enable);
    void OVR_SetPredictionEnabled(OVR_Instance *inst, int enable);
    int OVR_IsPredictionEnabled(OVR_Instance *inst);
}

#endif
