using System;

namespace C__23_part2
{
    class Program
    {
        static void Main(string[] args)
        {
            long a = 1, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0;
            goto line1;
        line1:
            b = 99;
            goto line2;
        line2:
            c = b;
            goto line3;
        line3:
            if (a == 0)
            {
                goto line4;
            }
            else
            {
                goto line5;
            }
        line4:
            if (1 == 0)
            {
                goto line5;
            }
            else
            {
                goto line9;
            }
        line5:
            b *= 100;
            goto line6;
        line6:
            b -= -100000;
            goto line7;
        line7:
            c = b;
            goto line8;
        line8:
            c -= -17000;
            goto line9;
        line9:
            f = 1;
            goto line10;
        line10:
            d = 2;
            goto line11;
        line11:
            e = 2;
            goto line12;
        line12:
            g = d;
            goto line13;
        line13:
            g *= e;
            goto line14;
        line14:
            g -= b;
            goto line15;
        line15:
            if (g == 0)
            {
                goto line16;
            }
            else
            {
                goto line17;
            }
        line16:
            f = 0;
            goto line17;
        line17:
            e -= -1;
            goto line18;
        line18:
            g = e;
            goto line19;
        line19:
            g -= b;
            goto line20;
        line20:
            if (g == 0)
            {
                goto line21;
            }
            else
            {
                goto line12;
            }
        line21:
            d -= -1;
            goto line22;
        line22:
            g = d;
            goto line23;
        line23:
            g -= b;
            goto line24;
        line24:
            if (g == 0)
            {
                goto line25;
            }
            else
            {
                goto line11;
            }
        line25:
            if (f == 0)
            {
                goto line26;
            }
            else
            {
                goto line27;
            }
        line26:
            h -= -1;
            Console.WriteLine("h changed to " + h);
            goto line27;
        line27:
            g = b;
            goto line28;
        line28:
            g -= c;
            goto line29;
        line29:
            if (g == 0)
            {
                goto line30;
            }
            else
            {
                goto line31;
            }
        line30:
            if (1 == 0)
            {
                goto line31;
            }
            else
            {
                goto finished;
            }
        line31:
            b -= -17;
            goto line32;
        line32:
            if (1 == 0)
            {
                goto finished;
            }
            else
            {
                goto line9;
            }
        finished:
            Console.WriteLine("Value of h: " + h);
        }
    }
}
