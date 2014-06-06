using System;

namespace OpenTK.Rift.TestConsole
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                using (OculusRift ovr = new OculusRift())
                {
                    do
                    {
                        int left = Console.CursorLeft;
                        int top = Console.CursorTop;

                        Console.WriteLine("Obsolete API:");
                        Console.WriteLine("Orientation (quaternion):     {0}", ovr.Orientation);
                        Console.WriteLine("Orientation (axis-angle):     {0}", ovr.Orientation.ToAxisAngle());
                        Console.WriteLine("Acceleration (m/s^2):         {0}", ovr.Acceleration);
                        Console.WriteLine("Angular velocity (rad/s):     {0}", ovr.AngularVelocity);
                        Console.WriteLine("Chroma aberration:            {0}", ovr.ChromaAbAberration);
                        Console.WriteLine("Distortion K:                 {0}", ovr.DistortionK);
                        Console.WriteLine("Desktop x-position (px):      {0}", ovr.DesktopX);
                        Console.WriteLine("Desktop y-position (px):      {0}", ovr.DesktopY);
                        Console.WriteLine("Horizonal resolution (px):    {0}", ovr.HResolution);
                        Console.WriteLine("Vertical resolution (px):     {0}", ovr.VResolution);
                        Console.WriteLine("Eye-screen distance (m):      {0}", ovr.EyeToScreenDistance);
                        Console.WriteLine("Interpupillary distance (m):  {0}", ovr.InterpupillaryDistance);
                        Console.WriteLine("Lens separation distance (m): {0}", ovr.LensSeparationDistance);
                        Console.WriteLine("Horizontal size (m):          {0}", ovr.HScreenSize);
                        Console.WriteLine("Vertical size (m):            {0}", ovr.VScreenSize);
                        Console.WriteLine("Vertical center (m):          {0}", ovr.VScreenCenter);

                        Console.WriteLine("Press any key to exit . . .");
                        Console.SetCursorPosition(left, top);
                    }
                    while (!Console.KeyAvailable);

                    for (int i = 0; i < 16; i++)
                        Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
