//
// OVR_wrapper.cpp
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

#include <assert.h>
#include <stdbool.h>
#include <stdint.h>

#include "OVR.h"
#include "OVR_wrapper.h"

using namespace OVR;

struct OVR_Instance
{
    DeviceManager *Manager;
    HMDDevice     *Device;
    SensorDevice  *Sensor;
    SensorFusion  *Fusion;
    HMDInfo       Info;
};

namespace
{
    inline OVR_Quaternion quat_to_quat(Quatf q)
    {
        OVR_Quaternion ret;
        ret.x = q.x;
        ret.y = q.y;
        ret.z = q.z;
        ret.w = q.w;
    }

    inline OVR_Vector3 vec3_to_vec3(Vector3f v)
    {
        OVR_Vector3 ret;
        ret.x = v.x;
        ret.y = v.y;
        ret.z = v.z;
    }

    inline OVR_Vector4 float4_to_vec4(float v[4])
    {
        OVR_Vector4 ret;
        ret.x = v[0];
        ret.y = v[1];
        ret.z = v[2];
        ret.w = v[3];
        return ret;
    }
}

OVR_Instance* OVR_Create()
{
    OVR_Instance *inst = new OVR_Instance();
    assert(inst);
    memset(inst, 0, sizeof(OVR_Instance));

    System::Init();

    inst->Fusion = new SensorFusion();
    inst->Manager = DeviceManager::Create();

    inst->Device = inst->Manager->EnumerateDevices<HMDDevice>().CreateDevice();

    if (inst->Device)
    {
       bool loaded = inst->Device->GetDeviceInfo(&inst->Info);
       inst->Sensor = inst->Device->GetSensor();
    }
    else
    {
       inst->Sensor = inst->Manager->EnumerateDevices<SensorDevice>().CreateDevice();
    }

    if (inst->Sensor)
    {
       inst->Fusion->AttachToSensor(inst->Sensor);
    }

    return inst;
}

void OVR_Destroy(OVR_Instance *inst)
{
    assert(inst);
    //inst->Sensor->Clear();
    //inst->Device->Clear();
    //inst->Manager->Clear();
    delete inst->Fusion;
    System::Destroy();
}

OVR_Quaternion OVR_GetOrientation(OVR_Instance *inst)
{
    return quat_to_quat(inst->Fusion->GetOrientation());
}

OVR_Vector3 OVR_GetAcceleration(OVR_Instance *inst)
{
    return vec3_to_vec3(inst->Fusion->GetAcceleration());
}

OVR_Vector3 OVR_GetAngularVelocity(OVR_Instance *inst)
{
    return vec3_to_vec3(inst->Fusion->GetAngularVelocity());
}

int OVR_GetHScreenSize(OVR_Instance *inst)
{
    return inst->Info.HScreenSize;
}

int OVR_GetVScreenSize(OVR_Instance *inst)
{
    return inst->Info.VScreenSize;
}

int OVR_GetVScreenCenter(OVR_Instance *inst)
{
    return inst->Info.VScreenCenter;
}

int OVR_GetDesktopX(OVR_Instance *inst)
{
    return inst->Info.DesktopX;
}

int OVR_GetDesktopY(OVR_Instance *inst)
{
    return inst->Info.DesktopY;
}

int OVR_GetHResolution(OVR_Instance *inst)
{
    return inst->Info.HResolution;
}

int OVR_GetVResolution(OVR_Instance *inst)
{
    return inst->Info.VResolution;
}

int OVR_GetEyeToScreenDistance(OVR_Instance *inst)
{
    return inst->Info.EyeToScreenDistance;
}

int OVR_GetLensSeparationDistance(OVR_Instance *inst)
{
    return inst->Info.LensSeparationDistance;
}

int OVR_GetInterpupillaryDistance(OVR_Instance *inst)
{
    return inst->Info.InterpupillaryDistance;
}

OVR_Vector4 OVR_GetDistortionK(OVR_Instance *inst)
{
    return float4_to_vec4(inst->Info.DistortionK);
}

OVR_Vector4 OVR_GetChromaAbCorrection(OVR_Instance *inst)
{
    return float4_to_vec4(inst->Info.ChromaAbCorrection);
}
