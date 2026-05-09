using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.CPK
{
    public class CPKCalculator
    {
        /// <summary>
        /// 计算过程能力指数CPK
        /// </summary>
        /// <param name="data">数据集合</param>
        /// <param name="lowerSpecLimit">规格下限</param>
        /// <param name="upperSpecLimit">规格上限</param>
        /// <returns>CPK值</returns>
        public static double CalculateCPK(IEnumerable<double> data, double lowerSpecLimit, double upperSpecLimit)
        {
            if (data == null || !data.Any())
                throw new ArgumentException("数据集合不能为空");

            if (lowerSpecLimit >= upperSpecLimit)
                throw new ArgumentException("规格下限必须小于规格上限");

            double mean = data.Average();
            double stdDev = CalculateStandardDeviation(data);

            double cpu = (upperSpecLimit - mean) / (3 * stdDev);
            double cpl = (mean - lowerSpecLimit) / (3 * stdDev);

            return Math.Min(cpu, cpl);
        }

        /// <summary>
        /// 计算标准差
        /// </summary>
        private static double CalculateStandardDeviation(IEnumerable<double> data)
        {
            double mean = data.Average();
            double sumOfSquares = data.Sum(value => Math.Pow(value - mean, 2));
            double variance = sumOfSquares / (data.Count() - 1); // 样本标准差使用n-1
            return Math.Sqrt(variance);
        }

        /// <summary>
        /// 计算CP
        /// </summary>
        public static double CalculateCP(double lowerSpecLimit, double upperSpecLimit, double standardDeviation)
        {
            return (upperSpecLimit - lowerSpecLimit) / (6 * standardDeviation);
        }

        /// <summary>
        /// 计算PPK（性能指数）
        /// </summary>
        public static double CalculatePPK(IEnumerable<double> data, double lowerSpecLimit, double upperSpecLimit)
        {
            if (data == null || !data.Any())
                throw new ArgumentException("数据集合不能为空");

            double mean = data.Average();
            double stdDev = CalculateOverallStandardDeviation(data);

            double ppu = (upperSpecLimit - mean) / (3 * stdDev);
            double ppl = (mean - lowerSpecLimit) / (3 * stdDev);

            return Math.Min(ppu, ppl);
        }

        /// <summary>
        /// 计算总体标准差
        /// </summary>
        private static double CalculateOverallStandardDeviation(IEnumerable<double> data)
        {
            double mean = data.Average();
            double sumOfSquares = data.Sum(value => Math.Pow(value - mean, 2));
            double variance = sumOfSquares / data.Count(); // 总体标准差使用n
            return Math.Sqrt(variance);
        }

        /// <summary>
        /// 计算PP
        /// </summary>
        public static double CalculatePP(double lowerSpecLimit, double upperSpecLimit, double overallStandardDeviation)
        {
            return (upperSpecLimit - lowerSpecLimit) / (6 * overallStandardDeviation);
        }

        /// <summary>
        /// 计算CGK值
        /// </summary>
        /// <param name="referenceValues">参考值数组</param>
        /// <param name="measuredValues">测量值数组</param>
        /// <param name="tolerance">公差范围</param>
        /// <returns>CGK值</returns>
        public static double CalculateCGK(double[] referenceValues, double[] measuredValues, double tolerance)
        {
            if (referenceValues.Length != measuredValues.Length)
            {
                throw new ArgumentException("参考值和测量值的数量必须相同");
            }

            if (tolerance <= 0)
            {
                throw new ArgumentException("公差必须大于0");
            }

            int n = referenceValues.Length;

            // 计算偏倚(bias)
            double sumBias = 0;
            for (int i = 0; i < n; i++)
            {
                sumBias += (measuredValues[i] - referenceValues[i]);
            }
            double bias = sumBias / n;

            // 计算实际过程变异(σ_actual)
            double sumSqActual = 0;
            double meanReference = referenceValues.Average();
            foreach (var val in referenceValues)
            {
                sumSqActual += Math.Pow(val - meanReference, 2);
            }
            double sigmaActual = Math.Sqrt(sumSqActual / (n - 1));

            // 计算观测总变异(σ_observed)
            double sumSqObserved = 0;
            double meanMeasured = measuredValues.Average();
            foreach (var val in measuredValues)
            {
                sumSqObserved += Math.Pow(val - meanMeasured, 2);
            }
            double sigmaObserved = Math.Sqrt(sumSqObserved / (n - 1));

            // 计算CGK
            double cgk = (1 - Math.Abs(bias) / tolerance) * (sigmaActual / sigmaObserved);

            return cgk;
        }
    }
}
