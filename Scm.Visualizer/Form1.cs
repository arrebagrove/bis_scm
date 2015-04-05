using Scm.Common;
using Scm.Ga;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scm.Visualizer
{
    public partial class Form1 : Form
    {
        private Instance _instance;
        private InstanceSolution _instanceSolution;
        private Trail[] _trails;
        private int _colorIndex = 0;
        private Color[] _colors = new[] { Color.Green, Color.Blue, Color.Orange, Color.Brown, Color.Violet, Color.Yellow, Color.Lime, Color.Magenta, Color.Indigo };

        public Form1()
        {
            InitializeComponent();
        }

        private Point GetScaledPoint(Point max, Point current)
        {
            var gSize = new Point(panel.Size);
            var xScaleFactor = (double)gSize.X / max.X;
            var yScaleFactor = (double)gSize.Y / max.Y;

            return new Point((int)(current.X * xScaleFactor), (int)(current.Y * yScaleFactor));
        }

        private void PanelPaint(object sender, PaintEventArgs e)
        {
            if (_instance != null)
            {

                var maxX = Math.Max(_instance.DepotList.Max(i => i.Point.X), _instance.CustomerList.Max(i => i.Point.X)) + 5;
                var maxY = Math.Max(_instance.DepotList.Max(i => i.Point.Y), _instance.CustomerList.Max(i => i.Point.Y)) + 5;

                foreach (var depot in _instance.DepotList)
                {
                    var point = GetScaledPoint(new Point(maxX, maxY), depot.Point);
                    e.Graphics.FillEllipse(Brushes.Red, new RectangleF(point, new SizeF(10, 10)));
                }

                foreach (var customer in _instance.CustomerList)
                {
                    var point = GetScaledPoint(new Point(maxX, maxY), customer.Point);
                    e.Graphics.FillEllipse(Brushes.Black, new RectangleF(point, new SizeF(10, 10)));
                }
            }

            if (_instance != null && _instanceSolution != null)
            {
                var maxX = Math.Max(_instance.DepotList.Max(i => i.Point.X), _instance.CustomerList.Max(i => i.Point.X)) + 5;
                var maxY = Math.Max(_instance.DepotList.Max(i => i.Point.Y), _instance.CustomerList.Max(i => i.Point.Y)) + 5;

                foreach (var tour in _instanceSolution.Tour)
                {
                    for (var i=0; i<tour.Route.Count; i++)
                    {
                        if (i + 1 >= tour.Route.Count)
                            break;

                        var from = _instance.CustomerList.Where(l => l.Name == tour.Route[i]).Select(l => l.Point).SingleOrDefault();
                        if (from == default(Point))
                            from = _instance.DepotList[int.Parse(tour.DepotName) - 1].Point;

                        var to = _instance.CustomerList.Where(l => l.Name == tour.Route[i+1]).Select(l => l.Point).SingleOrDefault();
                        if (to == default(Point))
                            to = _instance.DepotList[int.Parse(tour.DepotName) - 1].Point;

                        if (from == null || to == null)
                            continue;

                        var fromPoint = GetScaledPoint(new Point(maxX, maxY), from);
                        var toPoint = GetScaledPoint(new Point(maxX, maxY), to);

                        e.Graphics.DrawLine(new Pen(_colors[_colorIndex % _colors.Length], 2f), fromPoint.X + 5, fromPoint.Y + 5, toPoint.X + 5, toPoint.Y + 5);
                    }

                    _colorIndex++;
                }
            }

            if (_instance != null && _trails != null)
            {
                var maxX = Math.Max(_instance.DepotList.Max(i => i.Point.X), _instance.CustomerList.Max(i => i.Point.X)) + 5;
                var maxY = Math.Max(_instance.DepotList.Max(i => i.Point.Y), _instance.CustomerList.Max(i => i.Point.Y)) + 5;

                //foreach (var trail in _trails)
                //{
                var trail = _trails.OrderBy(t => t.Costs).First();
                    for (var i = 0; i < trail.Points.Length; i++)
                    {
                        if (i + 1 >= trail.Points.Length)
                            break;

                        var from = trail.Points[i];
                        var to = trail.Points[i + 1];

                        if (from == null || to == null)
                            continue;

                        var fromPoint = GetScaledPoint(new Point(maxX, maxY), from);
                        var toPoint = GetScaledPoint(new Point(maxX, maxY), to);

                        e.Graphics.DrawLine(new Pen(Brushes.Red, 1f), fromPoint.X + 5, fromPoint.Y + 5, toPoint.X + 5, toPoint.Y + 5);
                    }
                //}
            }
        }

        private void ButtonLoadInstanceClick(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            _instanceSolution = null;
            _instance = InstanceHelper.LoadInstance(openFileDialog.FileName);
            panel.Refresh();
        }

        private void ButtonLoadSolutionClick(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            _instanceSolution = InstanceHelper.LoadInstanceSolution(openFileDialog.FileName);
            panel.Refresh();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            panel.Invalidate();
        }

        private void ButtonSolveTspClick(object sender, EventArgs e)
        {
            var replaceFactor = 0.3;
            var trailCount = 50;
            var mutationCount = 100000;

            var random = new Random();
            _trails = new Trail[trailCount];

            // Build Trails
            for (var t=0; t<trailCount; t++)
            {
                var trail = new Trail();
                trail.Points = _instance.CustomerList.Select(i => i.Point).Take(10).ToArray();

                // Shuffle trail
                for (var p=0; p<trail.Points.Length; p++)
                {
                    var from = random.Next(trail.Points.Length);
                    var to = random.Next(trail.Points.Length);
                    var tmpTrail = trail.Points[from];
                    trail.Points[from] = trail.Points[to];
                    trail.Points[to] = tmpTrail;
                }

                _trails[t] = trail;
            }

            for (var m=0; m<mutationCount; m++)
            {
                // Replace lower (more costly) trails
                var orderedTrails = _trails.OrderBy(t => t.Costs).ToArray();

                var lowerIndex = (int)(_trails.Length * replaceFactor);
                for (var t=0; t<lowerIndex; t++)
                {
                    _trails[trailCount - 1 - t] = _trails[random.Next(trailCount - lowerIndex)].Mutation();
                }
            }

            panel.Refresh();
        }
    }
}
