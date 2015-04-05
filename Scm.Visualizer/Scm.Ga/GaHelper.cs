using Scm.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Ga
{
    public static class GaHelper
    {
        private static Random _random = new Random();

        public static Trail Mutation(this Trail trail)
        {
            var from = _random.Next(trail.Points.Length);
            var to = _random.Next(trail.Points.Length);

            var newTrail = new Trail();
            newTrail.Points = (Point[])trail.Points.Clone();

            var tmpPoint = newTrail.Points[from];
            newTrail.Points[from] = newTrail.Points[to];
            newTrail.Points[to] = tmpPoint;

            return newTrail;
        }
    }
}
