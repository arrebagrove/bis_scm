using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Scm.Common;
using Scm.Ga;

namespace Scm.Visualizer
{
    public partial class Form1 : Form
    {
        private Instance _instance;
        private InstanceSolution _instanceSolution;
        private Trail[] _trails;
        private int _colorIndex;
        private Color[] _colors = { Color.Green, Color.Blue, Color.Orange, Color.Brown, Color.Violet, Color.Yellow, Color.Lime, Color.Magenta, Color.Indigo };

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

                        if (from == default(Point) || to == default(Point))
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

                foreach (var trail in _trails)
                {
                    //var trail = _trails.OrderBy(t => t.Costs).First();
                    for (var i = 0; i < trail.Points.Length; i++)
                    {
                        if (i + 1 >= trail.Points.Length)
                            break;

                        var from = trail.Points[i];
                        var to = trail.Points[i + 1];

                        if (from == default(Point) || to == default(Point))
                            continue;

                        var fromPoint = GetScaledPoint(new Point(maxX, maxY), from);
                        var toPoint = GetScaledPoint(new Point(maxX, maxY), to);

                        e.Graphics.DrawLine(new Pen(Brushes.Red, 1f), fromPoint.X + 5, fromPoint.Y + 5, toPoint.X + 5, toPoint.Y + 5);
                         
                        e.Graphics.DrawString((i + 1).ToString(), new Font(Font.FontFamily, 10), Brushes.Red, toPoint.X + 10, toPoint.Y - 10);
                    }

                    // Connect last with first
                    //var lastPoint = trail.Points.Length - 1;
                    //var fromLastPoint = GetScaledPoint(new Point(maxX, maxY), trail.Points[lastPoint]);
                    //var toLastPoint = GetScaledPoint(new Point(maxX, maxY), trail.Points[0]);
                    //e.Graphics.DrawLine(new Pen(Brushes.Red, 1f), fromLastPoint.X + 5, fromLastPoint.Y + 5, toLastPoint.X + 5, toLastPoint.Y + 5);

                    e.Graphics.DrawString(trail.Costs.ToString("n2"), new Font(Font.FontFamily, 12), Brushes.Green, 10, 10);
                }
            }
        }

        private void ButtonLoadInstanceClick(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            _instanceSolution = null;
            _instance = InstanceHelper.LoadInstance(openFileDialog.FileName);
            panel.Refresh();
        }

        private void ButtonLoadSolutionClick(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
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
            buttonSolveTsp.Enabled = false;

            _trails = null;

            var acs = new Acs.Acs();
            for (var i = 0; i < 50; i++)
            {
                var trails = acs.Run(_instance.CustomerList.Select(p => p.Point).ToArray(), DrawCallback);

                //labelCosts.Text = trails[0].Costs.ToString("n2");
                //
            }

            buttonSolveTsp.Enabled = true;
        }

        private void DrawCallback(Trail[] trails)
        {
            if (_trails == null || trails[0].Costs < _trails[0].Costs)
            {
                _trails = trails;
                panel.Refresh();
                Thread.Sleep(200);
            }
        }
    }
}
