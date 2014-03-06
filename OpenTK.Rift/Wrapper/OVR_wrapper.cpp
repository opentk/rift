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

struct OVR_SystemInstance
{
    DeviceManager *Manager;

    OVR_SystemInstance()
    {
        System::Init();
        Manager = DeviceManager::Create();
        assert(Manager);
    }

    ~OVR_SystemInstance()
    {
        Manager->Release();
        Manager = NULL;
        System::Destroy();
    }
};

struct OVR_Instance
{
    OVR_SystemInstance *System;
    HMDDevice     *Device;
    SensorDevice  *Sensor;
    SensorFusion  *Fusion;
    HMDInfo       *Info;

    OVR_Instance() :
        System(NULL),
        Device(NULL),
        Sensor(NULL),
        Fusion(NULL),
        Info(NULL)
    {
    }
};

namespace
{
    static OVR_SystemInstance *SystemInstance = NULL;

    inline OVR_Quaternion quat_to_quat(Quatf q)
    {
        OVR_Quaternion ret;
        ret.x = q.x;
        ret.y = q.y;
        ret.z = q.z;
        ret.w = q.w;
        return ret;
    }

    inline OVR_Vector3 vec3_to_vec3(Vector3f v)
    {
        OVR_Vector3 ret;
        ret.x = v.x;
        ret.y = v.y;
        ret.z = v.z;
        return ret;
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

    inline OVR_Vector4 vec4()
    {
        OVR_Vector4 v = { 0 };
        return v;
    }

    inline OVR_Vector3 vec3()
    {
        OVR_Vector3 v = { 0 };
        return v;
    }

    inline OVR_Quaternion quat()
    {
        OVR_Quaternion q = { 0 };
        return q;
    }

    inline OVR_Quaternion unit_quat()
    {
        OVR_Quaternion q = { 0.0f, 0.0f, 0.0f, 1.0f };
        return q;
    }
}

void OVR_Init()
{
    assert(!SystemInstance);
    SystemInstance = new OVR_SystemInstance();
}

void OVR_Shutdown()
{
    assert(SystemInstance);
    delete SystemInstance;
    SystemInstance = NULL;
}

OVR_Instance* OVR_Create()
{
    assert(SystemInstance);

    OVR_Instance *inst = new OVR_Instance();
    assert(inst);

    inst->System = SystemInstance;
    inst->Fusion = new SensorFusion();
    inst->Device = inst->System->Manager->EnumerateDevices<HMDDevice>().CreateDevice();

    if (inst->Device)
    {
        inst->Sensor = inst->Device->GetSensor();

        HMDInfo info;
        if (inst->Device->GetDeviceInfo(&info))
        {
            inst->Info = new HMDInfo(info);
        }
    }
    else
    {
       inst->Sensor = inst->System->Manager->EnumerateDevices<SensorDevice>().CreateDevice();
    }

    if (inst->Sensor)
    {
       inst->Fusion->AttachToSensor(inst->Sensor);
    }

    return inst;
}

void OVR_Destroy(OVR_Instance *inst)
{
    if (inst)
    {
        delete inst->Fusion;
        delete inst->Sensor;
        delete inst->Device;
        delete inst->Info;
        inst->Fusion = NULL;
        inst->Sensor = NULL;
        inst->Device = NULL;
        inst->Info = NULL;
    }
    delete inst;
    inst = NULL;
}

int OVR_IsConnected(OVR_Instance *inst)
{
    return inst && inst->Info;
}

float OVR_GetHScreenSize(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->HScreenSize :
        0;
}

float OVR_GetVScreenSize(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->VScreenSize :
        0;
}

float OVR_GetVScreenCenter(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->VScreenCenter :
        0;
}

int OVR_GetDesktopX(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->DesktopX :
        0;
}

int OVR_GetDesktopY(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->DesktopY :
        0;
}

int OVR_GetHResolution(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->HResolution :
        0;
}

int OVR_GetVResolution(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->VResolution :
        0;
}

float OVR_GetEyeToScreenDistance(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->EyeToScreenDistance :
        0;
}

float OVR_GetLensSeparationDistance(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->LensSeparationDistance :
        0;
}

float OVR_GetInterpupillaryDistance(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        inst->Info->InterpupillaryDistance :
        0;
}

OVR_Vector4 OVR_GetDistortionK(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        float4_to_vec4(inst->Info->DistortionK) :
        vec4();
}

OVR_Vector4 OVR_GetChromaAbCorrection(OVR_Instance *inst)
{
    return
        inst && inst->Info ?
        float4_to_vec4(inst->Info->ChromaAbCorrection) :
        vec4();
}


// Sensor Fusion
OVR_Quaternion OVR_GetOrientation(OVR_Instance *inst)
{
    return
        inst && inst->Fusion ?
        quat_to_quat(inst->Fusion->GetOrientation()) :
        unit_quat();
}

OVR_Quaternion OVR_GetPredictedOrientation(OVR_Instance *inst)
{
    return
        inst && inst->Fusion ?
        quat_to_quat(inst->Fusion->GetPredictedOrientation()) :
        unit_quat();
}

OVR_Vector3 OVR_GetAcceleration(OVR_Instance *inst)
{
    return
        inst && inst->Fusion ?
        vec3_to_vec3(inst->Fusion->GetAcceleration()) :
        vec3();
}

OVR_Vector3 OVR_GetAngularVelocity(OVR_Instance *inst)
{
    return
        inst && inst->Fusion ?
        vec3_to_vec3(inst->Fusion->GetAngularVelocity()) :
        vec3();
}

float OVR_GetPredictionDelta(OVR_Instance *inst)
{
    return
        inst && inst->Fusion ?
        inst->Fusion->GetPredictionDelta() :
        0;
}

void OVR_SetPrediction(OVR_Instance *inst, float dt, int enable)
{
    if (inst && inst->Fusion)
    {
        inst->Fusion->SetPrediction(dt, enable != 0);
    }
}

void OVR_SetPredictionEnabled(OVR_Instance *inst, int enable)
{
    if (inst && inst->Fusion)
    {
        inst->Fusion->SetPredictionEnabled(enable != 0);
    }
}

int OVR_IsPredictionEnabled(OVR_Instance *inst)
{
    return
        inst && inst->Fusion ?
        inst->Fusion->IsPredictionEnabled() :
        0;
}
