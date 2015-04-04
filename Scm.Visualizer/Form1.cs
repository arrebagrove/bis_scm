using Scm.Common;
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

                foreach (var trail in _instanceSolution.Trails)
                {
                    for (var i=0; i<trail.Route.Count; i++)
                    {
                        if (i + 1 >= trail.Route.Count)
                            break;

                        var from = _instance.CustomerList.Where(l => l.Name == trail.Route[i]).Select(l => l.Point).SingleOrDefault();
                        if (from == default(Point))
                            from = _instance.DepotList[int.Parse(trail.DepotName) - 1].Point;

                        var to = _instance.CustomerList.Where(l => l.Name == trail.Route[i+1]).Select(l => l.Point).SingleOrDefault();
                        if (to == default(Point))
                            to = _instance.DepotList[int.Parse(trail.DepotName) - 1].Point;

                        if (from == null || to == null)
                            continue;

                        var fromPoint = GetScaledPoint(new Point(maxX, maxY), from);
                        var toPoint = GetScaledPoint(new Point(maxX, maxY), to);

                        e.Graphics.DrawLine(new Pen(_colors[_colorIndex % _colors.Length], 2f), fromPoint.X + 5, fromPoint.Y + 5, toPoint.X + 5, toPoint.Y + 5);
                    }

                    _colorIndex++;
                }
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
    }
}
