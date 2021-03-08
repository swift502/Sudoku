using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace Sudoku_3
{
    //Obecné funkce. Lze je využít pro mnoho různých situací. Většinou jsou několikrát přetížené.
    //Je potřeba přístup z jakéhokoliv místa v kódu, proto jsou statické.

    static class Func
    {
        public static Random rnd = new Random();

        public static PointF exponential(PointF source, PointF destination, float factor)
        {
            return new PointF(
                source.X + ((destination.X - source.X) / factor),
                source.Y + ((destination.Y - source.Y) / factor));
        }

        public static float exponential(float source, float destination, float factor)
        {
            return source + ((destination - source) / factor);
        }

        public static float prumer(float a, float b)
        {
            return (a + b) / 2;
        }

        public static void spring(ref PointF position, ref PointF vel, PointF target, float mass, float damp)
        {
            PointF acc = new PointF(target.X - position.X, target.Y - position.Y);
            acc.X /= mass;
            acc.Y /= mass;
            vel.X += acc.X;
            vel.Y += acc.Y;
            vel.X *= damp;
            vel.Y *= damp;

            position.X += vel.X;
            position.Y += vel.Y;
        }

        public static float spring(float position, ref float vel, float target, float mass, float damp)
        {
            float acc = target - position;
            acc /= mass;
            vel += acc;
            vel *= damp;

            return position + vel;
        }

        public static float vzdalenost(PointF source, PointF dest)
        {
            PointF difference = new PointF(dest.X - source.X, dest.Y - source.Y);

            //c² = a² + b²
            return (float)Math.Sqrt(Math.Pow(difference.X, 2) + Math.Pow(difference.Y, 2));
        }
    }
}
