using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using BrowApp;

namespace GSP
{
    /// <summary>
    /// XY坐标系误差补偿器（.NET Framework 4.6.2兼容版本）
    /// </summary>
    public class CoordinateCompensator : IDisposable
    {
        #region 内部类和枚举

        public class CompensationPoint
        {
            public double X { get; }
            public double Y { get; }
            public double OffsetX { get; }
            public double OffsetY { get; }

            public CompensationPoint(double x, double y, double offsetX, double offsetY)
            {
                X = x;
                Y = y;
                OffsetX = offsetX;
                OffsetY = offsetY;
            }
        }

        public enum CompensationMethod
        {
            /// <summary>最近邻补偿</summary>
            NearestNeighbor,
            /// <summary>线性插值补偿</summary>
            LinearInterpolation,
            /// <summary>双线性插值补偿</summary>
            BilinearInterpolation,
            /// <summary>反距离加权补偿</summary>
            InverseDistanceWeighting
        }

        #endregion

        #region 私有字段

        private readonly List<CompensationPoint> _points = new List<CompensationPoint>();
        private List<CompensationPoint> _xSorted = new List<CompensationPoint>();
        private List<CompensationPoint> _ySorted = new List<CompensationPoint>();
        private readonly object _lock = new object();
        private bool _disposed = false;

        // 反距离加权参数
        public int IdwPower { get; set; } = 2;
        public int IdwNeighbors { get; set; } = 8;

        #endregion

        #region 公共属性

        /// <summary>获取补偿点数量</summary>
        public int Count
        {
            get { lock (_lock) { return _points.Count; } }
        }

        public List<CompensationPoint> Points
        {
            get { lock (_lock) { return _points; } }
        }
        #endregion

        #region 公共方法

        public double MaxSearchDistanceSq { get; set; } = 20; // 可配置
        public void AddPoint(double x, double y, double offsetX, double offsetY)
        {
            CheckDisposed();
            lock (_lock)
            {
                var point = new CompensationPoint(x, y, offsetX, offsetY);
                _points.Add(point);
                InsertSorted(_xSorted, point, (a, b) => a.X.CompareTo(b.X));
                InsertSorted(_ySorted, point, (a, b) => a.Y.CompareTo(b.Y));
            }
        }

        private void InsertSorted(List<CompensationPoint> list, CompensationPoint item,
            Func<CompensationPoint, CompensationPoint, int> comparer)
        {
            int index = list.BinarySearch(item, Comparer<CompensationPoint>.Create((x, y) => comparer(x, y)));
            if (index < 0) index = ~index;
            list.Insert(index, item);
        }
        public Tuple<double, double> GetCompensatedPosition(double x, double y,
            CompensationMethod method = CompensationMethod.NearestNeighbor)
        {
            CheckDisposed();

            lock (_lock)
            {
                if (_points.Count == 0) return Tuple.Create(x, y);

                switch (method)
                {
                    case CompensationMethod.NearestNeighbor:
                        return NearestNeighbor(x, y);
                    case CompensationMethod.LinearInterpolation:
                        return LinearInterpolation(x, y);
                    case CompensationMethod.BilinearInterpolation:
                        return BilinearInterpolation(x, y);
                    case CompensationMethod.InverseDistanceWeighting:
                        return InverseDistanceWeighting(x, y);
                    default:
                        throw new ArgumentException("无效的补偿方法");
                }
            }
        }

        public void Clear()
        {
            CheckDisposed();
            lock (_lock)
            {
                _points.Clear();
                _xSorted.Clear();
                _ySorted.Clear();
            }
        }

        public void SaveToCsv(string filePath)
        {
            CheckDisposed();
            lock (_lock)
            {
                var lines = _points.Select(p =>
                    $"{p.X.ToString("F6", CultureInfo.InvariantCulture)}," +
                    $"{p.Y.ToString("F6", CultureInfo.InvariantCulture)}," +
                    $"{p.OffsetX.ToString("F6", CultureInfo.InvariantCulture)}," +
                    $"{p.OffsetY.ToString("F6", CultureInfo.InvariantCulture)}");
                File.WriteAllLines(filePath, lines);
            }
        }

        public void LoadFromCsv(string filePath)
        {
            try
            {


                CheckDisposed();
                lock (_lock)
                {
                    _points.Clear();
                    _xSorted.Clear();
                    _ySorted.Clear();
                    int lineNumber = 0;
                    var errors = new List<string>();
                    foreach (var line in File.ReadLines(filePath))
                    {
                        lineNumber++;
                        var parts = line.Split(',');
                        if (parts.Length != 4)
                        {
                            errors.Add($"第 {lineNumber} 行无效：列数错误");
                            continue;
                        }

                        if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) ||
                            !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y) ||
                            !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double dx) ||
                            !double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double dy))
                        {
                            errors.Add($"第 {lineNumber} 行包含无效数值");
                            continue;
                        }

                        var point = new CompensationPoint(x, y, dx, dy);
                        _points.Add(point);
                        InsertSorted(_xSorted, point, (a, b) => a.X.CompareTo(b.X));
                        InsertSorted(_ySorted, point, (a, b) => a.Y.CompareTo(b.Y));
                    }

                    if (errors.Count > 0)
                        throw new FormatException($"CSV加载错误：\n{string.Join("\n", errors)}");
                }
            }
            catch { }
        }

        public void Dispose()
        {
            if (_disposed) return;
            lock (_lock)
            {
                _points.Clear();
                _xSorted.Clear();
                _ySorted.Clear();
                _disposed = true;
            }
        }

        #endregion

        #region 私有方法

        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private Tuple<double, double> NearestNeighbor(double x, double y)
        {
            if (_points.Count == 0)
                return Tuple.Create(x, y);

            CompensationPoint nearest = null;
            double minDistSq=double.MaxValue;

            foreach (var p in _points)
            {
                double distSq = DistanceSquared(p.X, p.Y, x, y);
                if (distSq > MaxSearchDistanceSq* MaxSearchDistanceSq) continue; // 跳过过远的点
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    nearest = p;
                }
            }
            // 处理未找到有效点的情况
            if (nearest == null)
            {
                // 可返回原坐标、抛出异常或记录日志
                return Tuple.Create(x, y);
            }
            APP.Log.I_Log("补偿X:" + nearest.OffsetX.ToString()+"补偿Y"+ nearest.OffsetY.ToString());
            return Tuple.Create(x + nearest.OffsetX, y + nearest.OffsetY);
        }

        private Tuple<double, double> LinearInterpolation(double x, double y)
        {
            CompensationPoint first = null, second = null;
            double firstDistSq = double.MaxValue;
            double secondDistSq = double.MaxValue;

            foreach (var p in _points)
            {
                double distSq = DistanceSquared(p.X, p.Y, x, y);
                if (distSq < firstDistSq)
                {
                    second = first;
                    secondDistSq = firstDistSq;
                    first = p;
                    firstDistSq = distSq;
                }
                else if (distSq < secondDistSq)
                {
                    second = p;
                    secondDistSq = distSq;
                }
            }

            if (first == null)
                return Tuple.Create(x, y);
            if (second == null)
                return Tuple.Create(x + first.OffsetX, y + first.OffsetY);

            double dist = Distance(first.X, first.Y, second.X, second.Y);
            if (dist < 1e-9)
                return Tuple.Create(x + first.OffsetX, y + first.OffsetY);

            double weight1 = Distance(second.X, second.Y, x, y);
            double weight2 = Distance(first.X, first.Y, x, y);
            double totalWeight = weight1 + weight2;

            if (totalWeight < 1e-9)
                return Tuple.Create(x + first.OffsetX, y + first.OffsetY);

            double dx = (first.OffsetX * weight1 + second.OffsetX * weight2) / totalWeight;
            double dy = (first.OffsetY * weight1 + second.OffsetY * weight2) / totalWeight;

            return Tuple.Create(x + dx, y + dy);
        }

        private Tuple<double, double> BilinearInterpolation(double x, double y)
        {
            var xOrdered = _xSorted;
            var yOrdered = _ySorted;

            int xIndex = FindInsertIndex(xOrdered, x, (p, val) => p.X.CompareTo(val));
            if (xIndex == 0 || xIndex == xOrdered.Count)
                return LinearInterpolation(x, y);

            int yIndex = FindInsertIndex(yOrdered, y, (p, val) => p.Y.CompareTo(val));
            if (yIndex == 0 || yIndex == yOrdered.Count)
                return LinearInterpolation(x, y);

            CompensationPoint left = xOrdered[xIndex - 1];
            CompensationPoint right = xOrdered[xIndex];
            CompensationPoint bottom = yOrdered[yIndex - 1];
            CompensationPoint top = yOrdered[yIndex];

            var candidates = new List<CompensationPoint> { left, right, bottom, top };
            var p11 = FindClosestPoint(candidates, left.X, bottom.Y);
            var p12 = FindClosestPoint(candidates, left.X, top.Y);
            var p21 = FindClosestPoint(candidates, right.X, bottom.Y);
            var p22 = FindClosestPoint(candidates, right.X, top.Y);

            if (!IsValidGrid(p11, p12, p21, p22))
                return LinearInterpolation(x, y);

            try
            {
                double dx = BilinearCalculate(x, y, p11, p12, p21, p22, p => p.OffsetX);
                double dy = BilinearCalculate(x, y, p11, p12, p21, p22, p => p.OffsetY);
                return Tuple.Create(x + dx, y + dy);
            }
            catch
            {
                return LinearInterpolation(x, y);
            }
        }

        private int FindInsertIndex(List<CompensationPoint> sortedList, double target,
            Func<CompensationPoint, double, int> comparer)
        {
            int low = 0, high = sortedList.Count - 1;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                int comp = comparer(sortedList[mid], target);
                if (comp < 0)
                    low = mid + 1;
                else
                    high = mid - 1;
            }
            return low;
        }

        private CompensationPoint FindClosestPoint(IEnumerable<CompensationPoint> points,
            double targetX, double targetY, double tolerance = 1e-6)
        {
            return points
                .Where(p => Math.Abs(p.X - targetX) < tolerance && Math.Abs(p.Y - targetY) < tolerance)
                .OrderBy(p => DistanceSquared(p.X, p.Y, targetX, targetY))
                .FirstOrDefault();
        }

        private bool IsValidGrid(CompensationPoint p11, CompensationPoint p12,
            CompensationPoint p21, CompensationPoint p22)
        {
            if (p11 == null || p12 == null || p21 == null || p22 == null)
                return false;

            bool xAligned = Math.Abs(p11.X - p12.X) < 1e-9 &&
                            Math.Abs(p21.X - p22.X) < 1e-9;

            bool yAligned = Math.Abs(p11.Y - p21.Y) < 1e-9 &&
                            Math.Abs(p12.Y - p22.Y) < 1e-9;

            return xAligned && yAligned;
        }

        private double BilinearCalculate(double x, double y,
            CompensationPoint p11, CompensationPoint p12,
            CompensationPoint p21, CompensationPoint p22,
            Func<CompensationPoint, double> selector)
        {
            double xMin = p11.X;
            double xMax = p21.X;
            double yMin = p11.Y;
            double yMax = p12.Y;

            if (x < xMin || x > xMax || y < yMin || y > yMax)
                return LinearInterpolation(x, y).Item1 - x;

            double tx = (x - xMin) / (xMax - xMin);
            double ty = (y - yMin) / (yMax - yMin);

            return selector(p11) * (1 - tx) * (1 - ty) +
                   selector(p21) * tx * (1 - ty) +
                   selector(p12) * (1 - tx) * ty +
                   selector(p22) * tx * ty;
        }

        private Tuple<double, double> InverseDistanceWeighting(double x, double y)
        {
            var nearest = _points
                .OrderBy(p => DistanceSquared(p.X, p.Y, x, y))
                .Take(IdwNeighbors)
                .ToList();

            if (nearest.Count == 0)
                return Tuple.Create(x, y);
            if (nearest.Count == 1)
                return Tuple.Create(x + nearest[0].OffsetX, y + nearest[0].OffsetY);

            double sumWeights = 0;
            double sumDx = 0;
            double sumDy = 0;

            foreach (var point in nearest)
            {
                double dist = Distance(point.X, point.Y, x, y);
                if (dist < 1e-9)
                    return Tuple.Create(x + point.OffsetX, y + point.OffsetY);

                double weight = 1.0 / Math.Pow(dist, IdwPower);
                sumWeights += weight;
                sumDx += point.OffsetX * weight;
                sumDy += point.OffsetY * weight;
            }
            return Tuple.Create(x + sumDx / sumWeights, y + sumDy / sumWeights);
        }
        private static double DistanceSquared(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return dx * dx + dy * dy;
        }

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(DistanceSquared(x1, y1, x2, y2));
        }
        #endregion
    }
}
