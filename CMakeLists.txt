cmake_minimum_required(VERSION 2.8)

project (OVR)
set(CMAKE_BUILD_TYPE Release)

#FILE (GLOB_RECURSE source ${CMAKE_CURRENT_SOURCE_DIR}/*.cpp)
#message(“source files: ${source}”)

include_directories(${CMAKE_CURRENT_SOURCE_DIR}/LibOVR)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/LibOVR/Include)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/LibOVR/Src)

set (SRC ${SRC}
  LibOVR/Src/OVR_DeviceHandle.cpp
  LibOVR/Src/OVR_DeviceImpl.cpp
  LibOVR/Src/OVR_JSON.cpp
  LibOVR/Src/OVR_LatencyTestImpl.cpp
  LibOVR/Src/OVR_Profile.cpp
  LibOVR/Src/OVR_SensorFilter.cpp
  LibOVR/Src/OVR_SensorFusion.cpp
  LibOVR/Src/OVR_SensorImpl.cpp
  LibOVR/Src/OVR_ThreadCommandQueue.cpp
  LibOVR/Src/Kernel/OVR_Alg.cpp
  LibOVR/Src/Kernel/OVR_Allocator.cpp
  LibOVR/Src/Kernel/OVR_Atomic.cpp
  LibOVR/Src/Kernel/OVR_File.cpp
  LibOVR/Src/Kernel/OVR_FileFILE.cpp
  LibOVR/Src/Kernel/OVR_Log.cpp
  LibOVR/Src/Kernel/OVR_Math.cpp
  LibOVR/Src/Kernel/OVR_RefCount.cpp
  LibOVR/Src/Kernel/OVR_Std.cpp
  LibOVR/Src/Kernel/OVR_String_FormatUtil.cpp
  LibOVR/Src/Kernel/OVR_String_PathUtil.cpp
  LibOVR/Src/Kernel/OVR_String.cpp
  LibOVR/Src/Kernel/OVR_SysFile.cpp
  LibOVR/Src/Kernel/OVR_System.cpp
  LibOVR/Src/Kernel/OVR_Timer.cpp
  LibOVR/Src/Kernel/OVR_UTF8Util.cpp)

if (WIN32)
  set (SRC ${SRC}
    LibOVR/Src/OVR_Win32_DeviceManager.cpp
    LibOVR/Src/OVR_Win32_DeviceStatus.cpp
    LibOVR/Src/OVR_Win32_HIDDevice.cpp
    LibOVR/Src/OVR_Win32_HMDDevice.cpp
    LibOVR/Src/OVR_Win32_SensorDevice.cpp
    LibOVR/Src/Kernel/OVR_ThreadsWinAPI.cpp)
endif ()

if (LINUX)
  set (SRC ${SRC}
    LibOVR/Src/OVR_Linux_DeviceManager.cpp
    LibOVR/Src/OVR_Linux_HIDDevice.cpp
    LibOVR/Src/OVR_Linux_HMDDevice.cpp
    LibOVR/Src/OVR_Linux_SensorDevice.cpp
    LibOVR/Src/Kernel/OVR_ThreadsPthread.cpp)
endif()

if (APPLE)
  set (SRC ${SRC}
    LibOVR/Src/OVR_OSX_DeviceManager.cpp
    LibOVR/Src/OVR_OSX_HIDDevice.cpp
    LibOVR/Src/OVR_OSX_HMDDevice.cpp
    LibOVR/Src/OVR_OSX_SensorDevice.cpp
    LibOVR/Src/Kernel/OVR_ThreadsPthread.cpp)
endif ()

add_library(${PROJECT_NAME} SHARED ${SRC})

if (APPLE)
  find_library(MAC_APPSERVICES NAMES ApplicationServices)
  find_library(MAC_CF NAMES CoreFoundation)
  find_library(MAC_IOKIT NAMES IOKit)
  target_link_libraries(${PROJECT_NAME}
    ${MAC_APPSERVICES}
    ${MAC_CF}
    ${MAC_IOKIT})
endif ()
