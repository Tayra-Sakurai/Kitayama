using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kuramaguchi.Services
{
    public class ClassificationResult<TNumber> : IClassificationResult<TNumber>
        where TNumber : INumber<TNumber>, IFloatingPoint<TNumber>
    {
        public int Index { get; init; }

        public string Classifier { get; init; }

        public TNumber MatchRate { get; init; }

        public int CompareTo(IClassificationResult<TNumber> other)
        {
            int res1 = MatchRate.CompareTo(other.MatchRate);
            return res1 == 0 ? -Index.CompareTo(other.Index) : res1;
        }
    }
}
