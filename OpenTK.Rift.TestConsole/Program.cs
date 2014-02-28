using System;

namespace OpenTK.Rift.TestConsole
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                OVR ovr = new OVR();

                do
                {
                    int left = Console.CursorLeft;
                    int top = Console.CursorTop;

                    Console.WriteLine("Orientation (quaternion):\t\t{0}", ovr.Orientation);
                    Console.WriteLine("Orientation (axis-angle):\t\t{0}", ovr.Orientation.ToAxisAngle());
                    Console.WriteLine("Acceleration (m/s^2):\t\t{0}", ovr.Acceleration);
                    Console.WriteLine("Angular velocity (rad/s):\t\t{0}", ovr.AngularVelocity);
                    Console.WriteLine("Chroma aberration:\t\t{0}", ovr.ChromaAbAberration);
                    Console.WriteLine("Distortion K:\t\t{0}", ovr.DistortionK);
                    Console.WriteLine("Eye-screen distance (m):\t\t{0}", ovr.EyeToScreenDistance);
                    Console.WriteLine("Interpupillary distance (m):\t\t{0}", ovr.InterpupillaryDistance);
                    Console.WriteLine("Lens separation distance (m):\t\t{0}", ovr.LensSeparationDistance);
                    Console.WriteLine("Horizonal resolution (px):\t\t{0}", ovr.HResolution);
                    Console.WriteLine("Vertical resolution (px):\t\t{0}", ovr.VResolution);
                    Console.WriteLine("Horizontal size (mm):\t\t{0}", ovr.HScreenSize);
                    Console.WriteLine("Vertical size (mm):\t\t{0}", ovr.VScreenSize);
                    Console.WriteLine("Vertical center (mm):\t\t{0}", ovr.VScreenCenter);

                    Console.WriteLine("Press any key to exit . . .");
                    Console.SetCursorPosition(left, top);
                }
                while (!Console.KeyAvailable);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
