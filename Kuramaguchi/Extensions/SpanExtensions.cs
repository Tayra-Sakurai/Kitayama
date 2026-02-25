// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kuramaguchi.Extensions
{
    public static class SpanExtensions
    {
        /// <summary>
        /// Gets the inner product with <paramref name="otherSpan"/>.
        /// </summary>
        /// <typeparam name="TSelf">The number type of the value.</typeparam>
        /// <typeparam name="TOther">The type of the number of the multiplier.</typeparam>
        /// <typeparam name="TResult">The inner product type.</typeparam>
        /// <param name="span">The span itself.</param>
        /// <param name="otherSpan">The multiplier.</param>
        /// <returns>The inner product.</returns>
        /// <exception cref="ArgumentException">When the inner product is uncalculatable.</exception>
        public static TResult GetInnerProductWith<TSelf, TOther, TResult>(this ReadOnlySpan<TSelf> span, ReadOnlySpan<TOther> otherSpan)
            where TSelf : IAdditionOperators<TSelf, TOther, TResult>, IMultiplyOperators<TSelf, TOther, TResult>
            where TResult : INumberBase<TResult>
        {
            if (span.Length != otherSpan.Length)
                throw new ArgumentException("The spans must be have the same length", nameof(otherSpan));

            TResult result = TResult.Zero;

            TSelf[] values = span.ToArray();
            TOther[] otherValues = otherSpan.ToArray();

            foreach (
                (TSelf value, TOther otherValue)
                in values.Zip(otherValues))
                result += value * otherValue;

            return result;
        }
    }
}
