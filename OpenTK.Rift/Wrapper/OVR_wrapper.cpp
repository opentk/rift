//
// OVR.cpp
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
} ;

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

Quaternion OVR_GetOrientation(OVR_Instance *inst)
{
    Quatf q = inst->Fusion->GetOrientation();
    return *(Quaternion*)&q;
}
