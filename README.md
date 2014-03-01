OpenTK.Rift
===========

OpenTK.Rift is an intuitive, cross-platform C# wrapper for the Oculus Rift SDK.

Home page: http://www.opentk.com

Official repository: https://github.com/opentk/rift


Features
========

- Create great Oculus Rift applications from the comfort of C#, F# and VB.Net
- Increase your reach: support for Windows, Linux and Mac OS X
- Break the limits: leverage the full power of OpenGL 4.4, OpenCL and OpenAL
- Code faster: avoid the pitfalls of C++ memory management


Instructions
============

OpenTK.Rift consists of two parts: a native, platform-specific library (libOVR) and a C#, platform-independent wrapper (OpenTK.Rift.dll). You need both parts in order to run an Oculus Rift application.

0. Clone OpenTK.Rift using a git client.

Windows:

1. Install [CMake](http://www.cmake.org/cmake/resources/software.html) via the Win32 Installer. In the last screen, select the “Add CMake to your PATH” option.
2. Open and build OpenTK.Rift.sln using Visual Studio 2012 or newer.

Mac OS X:

1. Install [CMake](http://www.cmake.org/cmake/resources/software.html).
2. Install the free edition of [Xamarin Studio](https://xamarin.com/download).
3. Install the [NuGet plugin](https://github.com/mrward/monodevelop-nuget-addin) for Xamarin Studio. Alternatively, follow steps 3 to 6 from the Linux instructions below.
3. Open and build OpenTK.Rift.sln using Xamarin Studio.

Ubuntu Linux:

1. Open a terminal and navigate to where you cloned OpenTK.Rift.
2. sudo apt-get install build-essentials cmake mono-devel monodevelop
3. mozroots --import --sync
4. mkdir -p packages && cd packages
5. wget http://build.nuget.org/NuGet.exe
6. wget https://dl.dropboxusercontent.com/u/30682604/Microsoft.Build.dll
7. mono --runtime=v4.0 NuGet.exe install OpenTK
7. Open and build OpenTK.Rift.sln using MonoDevelop.


Contribute
==========

OpenTK.Rift is a very young project. You are welcome to join its development at https://github.com/opentk/rift


Roadmap
=======

The library is tested and fully functional on Mac OS X 10.9.x and Ubuntu Linux 12.04.

Missing features, in order of importance:
1. Test build system on Windows.
2. Find a good distribution method for the native library.
3. Add support for device hotplugging.
4. Add support for more libOVR features.
