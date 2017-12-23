using System;

namespace C__23_part2
{
    class Program
    {
        static void Main(string[] args)
        {
            int b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0;
            b = 109900;
            c = 126900;

            while (true)
            {
                d = 2;
                while (g != 0)
                {
                    e = 2;
                    while (g != 0)
                    {
                        g = d;
                        g *= e;
                        g -= b;
                        if (g == 0)
                        {
                            f = 0;
                        }
                        e -= -1;
                        g = e;
                        g -= b;
                    }
                    d -= -1;
                    g = d;
                    g -= b;
                }
                if (f == 0)
                {
                    h -= -1;
                    Console.WriteLine("h changed to " + h);
                }
                g = b;
                g -= c;
                if (g == 0)
                {
                    break;
                }
                b -= -17;
            }
            Console.WriteLine("Done! Final value of h is: " + h);
        }
    }
}
