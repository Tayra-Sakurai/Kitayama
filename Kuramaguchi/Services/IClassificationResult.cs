// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kuramaguchi.Services
{
    public interface IClassificationResult<TNumber> : IComparable<IClassificationResult<TNumber>>
        where TNumber : INumber<TNumber>, IFloatingPoint<TNumber>
    {
        /// <summary>
        /// The index of the element's original index.
        /// </summary>
        int Index { get; init; }

        /// <summary>
        /// The name of the category.
        /// </summary>
        string Classifier { get; init; }

        /// <summary>
        /// The matching rate.
        /// </summary>
        TNumber MatchRate { get; init; }
    }
}
