using System;
using System.Drawing;
using System.Linq;
using Scm.Common;

namespace Scm.Ga
{
    public class Ga
    {
        public const double ReplaceFactor = 0.2;
        public const int Generations = 50;
        public const int Mutations = 1000;

        private Trail[] _trails;

        public void Run(Point[] points)
        {
            var random = new Random();
            _trails = new Trail[Generations];

            // Build Trails
            for (var t = 0; t < Generations; t++)
            {
                var trail = new Trail {Points = (Point[]) points.Clone()};

                // Shuffle trail
                for (var p = 0; p < trail.Points.Length; p++)
                {
                    var from = random.Next(trail.Points.Length);
                    var to = random.Next(trail.Points.Length);
                    var tmpTrail = trail.Points[from];
                    trail.Points[from] = trail.Points[to];
                    trail.Points[to] = tmpTrail;
                }

                _trails[t] = trail;
            }

            for (var m = 0; m < Mutations; m++)
            {
                // Replace lower (more costly) trails
                _trails = _trails.OrderBy(t => t.Costs).ToArray();

                var lowerIndex = (int)(_trails.Length * ReplaceFactor);
                for (var t = 0; t < lowerIndex; t++)
                {
                    _trails[Generations - 1 - t] = _trails[random.Next(Generations - lowerIndex)].Mutation();
                }

                //// Reset costs
                //for (var t = 0; t < _trails.Length; t++)
                //{
                //    _trails[t].ResetCosts();
                //}
            }
        }
    }
}
