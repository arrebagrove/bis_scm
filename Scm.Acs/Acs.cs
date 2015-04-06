using System;
using System.Drawing;
using System.Linq;
using Scm.Common;

namespace Scm.Acs
{
    public class Acs
    {
        private const double Alpha = 2;
        private const double Beta = 1;
        private const double Rho = 0.05;
        private const double Q = 1.0;
        private const int AntCount = 3;


        private double[,] _pheromonMatrix;
        private double[,] _distanceMatrix;
        private Point[] _allNodes;
        private int _nodeCount;
        private Random _random;

        public Trail[] Run(Point[] route, Action<Trail[]> drawCallback = null)
        {
            // Initialize
            _random = new Random();
            _allNodes = (Point[])route.Clone();
            _nodeCount = _allNodes.Length;
            _pheromonMatrix = new double[_nodeCount, _nodeCount];
            _distanceMatrix = new double[_nodeCount, _nodeCount];

            // Build distance matrix
            BuildDistanceMatrix();

            // Initialize ants with first random trail
            var ants = InitAntsWithRandomTrail();

            // Initial generation consists of random trail only
            var bestTrail = GetBestTrail(ants);
            var bestCosts = GetAntTotalCosts(bestTrail);
            if (drawCallback != null)
                drawCallback(new[] {GetResultTrail(bestTrail)});

            // Build pheromon matrix
            BuildPheromonMatrix();

            for (var time = 0; time < 5000; time++)
            {
                UpdateAnts(ants);
                UpdatePhermomones(ants);

                var currentBestTrail = GetBestTrail(ants);
                var currentBestCosts = GetAntTotalCosts(currentBestTrail);
                if (currentBestCosts < bestCosts)
                {
                    bestCosts = currentBestCosts;
                    bestTrail = currentBestTrail;
                    if (drawCallback != null)
                        drawCallback(new[] { GetResultTrail(bestTrail) });
                }
            }

            var trail = GetResultTrail(bestTrail);

            return new[] {trail};
        }

        private Trail GetResultTrail(int[] trail)
        {
            var resultTrail = new Trail();
            resultTrail.Points = new Point[trail.Length + 1];
            for (var t = 0; t < trail.Length; t++)
            {
                resultTrail.Points[t] = new Point(_allNodes[trail[t]].X, _allNodes[trail[t]].Y);
            }
            resultTrail.Points[trail.Length] = new Point(_allNodes[0].X, _allNodes[0].Y);

            return resultTrail;
        }

        private void UpdatePhermomones(int[][] ants)
        {
            // All from's
            for (var m = 0; m < _nodeCount; m++)
            {
                // All to's
                for (var n = 0; n < _nodeCount; n++)
                {
                    // All ants
                    for (var a = 0; a < ants.Length; a++)
                    {
                        var costs = GetAntTotalCosts(ants[a]);
                        var decrease = (1.0 - Rho)*_pheromonMatrix[m, n];
                        var increase = 0.0;
                        if (PathInTrail(m, n, ants[a]))
                            increase = (Q/costs);

                        _pheromonMatrix[m, n] = decrease + increase;

                        if (_pheromonMatrix[m, n] < 0.0001)
                            _pheromonMatrix[m, n] = 0.0001;
                        else if (_pheromonMatrix[m, n] > 100000.0)
                            _pheromonMatrix[m, n] = 100000.0;

                        _pheromonMatrix[m, n] = _pheromonMatrix[n, m];
                    }
                }
            }
        }

        private bool PathInTrail(int m, int n, int[] trail)
        {
            var lastIndex = trail.Length - 1;
            var indexM = trail.ToList().IndexOf(m);

            if (indexM == 0 && trail[1] == n)
                return true;

            if (indexM == 0 && trail[lastIndex] == n)
                return true;

            if (indexM == 0)
                return false;

            if (indexM == lastIndex && trail[lastIndex - 1] == n)
                return true;

            if (indexM == lastIndex && trail[0] == n)
                return true;

            if (indexM == lastIndex)
                return false;

            if (trail[indexM - 1] == n)
                return true;

            if (trail[indexM + 1] == n)
                return true;

            return false;
        }

        private void UpdateAnts(int[][] ants)
        {
            for (var a = 0; a < ants.Length; a++)
            {
                var startNode = 0; //_random.Next(_nodeCount);
                ants[a] = BuildTrail(a, startNode);
            }
        }

        private int[] BuildTrail(int a, int startNode)
        {
            var trail = new int[_nodeCount];
            var visited = new bool[_nodeCount];

            trail[0] = startNode;
            visited[0] = true;

            for (var t = 0; t < _nodeCount - 1; t++)
            {
                var node = trail[t];
                var nextNode = NextNode(a, node, visited);
                trail[t + 1] = nextNode;
                visited[nextNode] = true;
            }

            return trail;
        }

        private int NextNode(int a, int node, bool[] visited)
        {
            var randomProbability = _random.NextDouble();
            var probabilities = MoveProbabilities(a, node, visited);
            var cumulations = new double[probabilities.Length + 1];

            for (var p = 0; p < probabilities.Length; p++)
            {
                cumulations[p + 1] = cumulations[p] + probabilities[p];
            }

            for (var p = 0; p < cumulations.Length - 1; p++)
            {
                if (randomProbability >= cumulations[p] && randomProbability < cumulations[p + 1])
                    return p;
            }

            throw new InvalidOperationException("No valid next city found");
        }

        private double[] MoveProbabilities(int a, int node, bool[] visited)
        {
            var decisions = new double[_nodeCount];
            var sum = 0.0;

            for (var d = 0; d < decisions.Length; d++)
            {
                if (d == node || visited[d])
                {
                    decisions[d] = 0.0;
                }
                else
                {
                    decisions[d] = Math.Pow(_pheromonMatrix[node, d], Alpha) * Math.Pow((1.0/_distanceMatrix[node, d]), Beta);

                    if (decisions[d] < 0.0001)
                        decisions[d] = 0.0001;
                    else if (decisions[d] > (double.MaxValue/(_nodeCount*100)))
                    {
                        decisions[d] = double.MaxValue/(_nodeCount*100);
                    }
                }

                sum += decisions[d];
            }

            var probabilities = new double[_nodeCount];

            for (var p = 0; p < probabilities.Length; p++)
            {
                probabilities[p] = decisions[p]/sum;
            }

            return probabilities;
        }

        private void BuildDistanceMatrix()
        {
            // Build Distance Matrix
            for (var from = 0; from < _nodeCount; from++)
            {
                for (var to = 0; to < _nodeCount; to++)
                {
                    if (from == to)
                    {
                        _distanceMatrix[from, to] = 0;
                        continue;
                    }

                    _distanceMatrix[from, to] = GetDistance(_allNodes[from], _allNodes[to]);
                }
            }
        }

        private double GetDistance(Point from, Point to)
        {
            var dX = from.X - to.X;
            var dY = from.Y - to.Y;

            return Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));
        }

        private void BuildPheromonMatrix()
        {
            for (var m = 0; m < _nodeCount; m++)
            {
                for (var n = 0; n < _nodeCount; n++)
                {
                    _pheromonMatrix[m, n] = 0.01;
                }
            }
        }

        private int[][] InitAntsWithRandomTrail()
        {
            var ants = new int[AntCount][];
            for (var k = 0; k < AntCount; k++)
            {
                var startNode = 0; //_random.Next(0, _nodeCount);
                ants[k] = BuildRandomTrail(startNode);
            }

            return ants;
        }

        private int[] BuildRandomTrail(int startNode)
        {
            var trail = Enumerable.Range(0, _nodeCount).ToArray();

            // Shuffle trail to get a random trail (Fisher-Yates Shuffle)
            for (int i = 0; i < _nodeCount; i++)
            {
                var randomIndex = _random.Next(_nodeCount);
                var tmpNode = trail[i];
                trail[i] = trail[randomIndex];
                trail[randomIndex] = tmpNode;
            }

            // Put start node to index 0
            var indexStart = trail.ToList().IndexOf(startNode);
            var tmpStartNode = trail[0];
            trail[0] = trail[indexStart];
            trail[indexStart] = tmpStartNode;

            return trail;
        }

        private int[] GetBestTrail(int[][] ants)
        {
            return ants.OrderByDescending(GetAntTotalCosts).First();
        }

        private double GetAntTotalCosts(int[] trail)
        {
            var costs = 0.0;
            for (var t = 0; t < trail.Length - 1; t++)
            {
                costs += _distanceMatrix[trail[t], trail[t + 1]];
            }

            costs += _distanceMatrix[trail[trail.Length - 1], trail[0]];

            return costs;
        }
    }
}
