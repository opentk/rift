cmake_minimum_required(VERSION 2.8)

project(${PROJECT_NAME})
set(CMAKE_INSTALL_PREFIX ${CMAKE_BINARY_DIR})

include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../LibOVR)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../LibOVR/Include)
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/../LibOVR/Src)
include_directories(${CMAKE_CURRENT_SOURCE_DIR})

set (SRC ${SRC}
  #OVR_wrapper.cpp # we are going to use the upstream CAPI instead
  ../LibOVR/Src/OVR_CAPI.cpp
  #../LibOVR/Src/OVR_Common_HMDDevice.cpp # not included in upstream project files
  ../LibOVR/Src/OVR_DeviceHandle.cpp
  ../LibOVR/Src/OVR_DeviceImpl.cpp
  ../LibOVR/Src/OVR_JSON.cpp
  ../LibOVR/Src/OVR_LatencyTestImpl.cpp
  ../LibOVR/Src/OVR_Profile.cpp
  ../LibOVR/Src/OVR_Recording.cpp
  ../LibOVR/Src/OVR_Sensor2Impl.cpp
  ../LibOVR/Src/OVR_SensorCalibration.cpp
  ../LibOVR/Src/OVR_SensorFilter.cpp
  ../LibOVR/Src/OVR_SensorFusion.cpp
  ../LibOVR/Src/OVR_SensorImpl.cpp
  ../LibOVR/Src/OVR_SensorImpl_Common.cpp
  ../LibOVR/Src/OVR_SensorTimeFilter.cpp
  ../LibOVR/Src/OVR_Stereo.cpp
  ../LibOVR/Src/OVR_ThreadCommandQueue.cpp
  ../LibOVR/Src/CAPI/CAPI_DistortionRenderer.cpp
  ../LibOVR/Src/CAPI/CAPI_FrameTimeManager.cpp
  ../LibOVR/Src/CAPI/CAPI_GlobalState.cpp
  ../LibOVR/Src/CAPI/CAPI_HMDRenderState.cpp
  ../LibOVR/Src/CAPI/CAPI_HMDState.cpp
  ../LibOVR/Src/CAPI/GL/CAPI_GL_DistortionRenderer.cpp
  ../LibOVR/Src/CAPI/GL/CAPI_GL_Util.cpp
  ../LibOVR/Src/Kernel/OVR_Alg.cpp
  ../LibOVR/Src/Kernel/OVR_Allocator.cpp
  ../LibOVR/Src/Kernel/OVR_Atomic.cpp
  ../LibOVR/Src/Kernel/OVR_File.cpp
  ../LibOVR/Src/Kernel/OVR_FileFILE.cpp
  ../LibOVR/Src/Kernel/OVR_Lockless.cpp
  ../LibOVR/Src/Kernel/OVR_Log.cpp
  ../LibOVR/Src/Kernel/OVR_Math.cpp
  ../LibOVR/Src/Kernel/OVR_RefCount.cpp
  ../LibOVR/Src/Kernel/OVR_Std.cpp
  ../LibOVR/Src/Kernel/OVR_String.cpp
  ../LibOVR/Src/Kernel/OVR_String_FormatUtil.cpp
  ../LibOVR/Src/Kernel/OVR_String_PathUtil.cpp
  ../LibOVR/Src/Kernel/OVR_SysFile.cpp
  ../LibOVR/Src/Kernel/OVR_System.cpp
  ../LibOVR/Src/Kernel/OVR_Timer.cpp
  ../LibOVR/Src/Kernel/OVR_UTF8Util.cpp
)

if (WIN32)
  add_definitions(-DUNICODE -D_UNICODE)
  set (SRC ${SRC}
    ../LibOVR/Src/OVR_Win32_DeviceManager.cpp
    ../LibOVR/Src/OVR_Win32_DeviceStatus.cpp
    ../LibOVR/Src/OVR_Win32_HIDDevice.cpp
    ../LibOVR/Src/OVR_Win32_HMDDevice.cpp
    ../LibOVR/Src/OVR_Win32_SensorDevice.cpp
    ../LibOVR/Src/Kernel/OVR_ThreadsWinAPI.cpp)
endif ()

if (LINUX)
  set (SRC ${SRC}
    ../LibOVR/Src/OVR_Linux_DeviceManager.cpp
    ../LibOVR/Src/OVR_Linux_HIDDevice.cpp
    ../LibOVR/Src/OVR_Linux_HMDDevice.cpp
    ../LibOVR/Src/OVR_Linux_SensorDevice.cpp
    ../LibOVR/Src/Kernel/OVR_ThreadsPthread.cpp)
endif()

if (APPLE)
  set (CMAKE_OSX_ARCHITECTURES "i386;x86_64")
  set (SRC ${SRC}
    ../LibOVR/Src/OVR_OSX_DeviceManager.cpp
    ../LibOVR/Src/OVR_OSX_HIDDevice.cpp
    ../LibOVR/Src/OVR_OSX_HMDDevice.cpp
    ../LibOVR/Src/OVR_OSX_SensorDevice.cpp
    ../LibOVR/Src/Kernel/OVR_ThreadsPthread.cpp)
endif ()

add_library(${PROJECT_NAME} SHARED ${SRC})
message("Installing to: ${INSTALL_DIR}")
install(
  TARGETS ${PROJECT_NAME}
  ARCHIVE DESTINATION ${INSTALL_DIR}
  LIBRARY DESTINATION ${INSTALL_DIR}
  RUNTIME DESTINATION ${INSTALL_DIR}
  COMPONENT Library)

if (WIN32)
  target_link_libraries(${PROJECT_NAME} setupapi.lib)
  target_link_libraries(${PROJECT_NAME} winmm.lib)
endif ()

if (APPLE)
  find_library(MAC_APPSERVICES NAMES ApplicationServices)
  find_library(MAC_CF NAMES CoreFoundation)
  find_library(MAC_IOKIT NAMES IOKit)
  find_library(MAC_OPENGL OpenGL)
  target_link_libraries(${PROJECT_NAME}
    ${MAC_APPSERVICES}
    ${MAC_CF}
    ${MAC_IOKIT}
    ${MAC_OPENGL})
endif ()
