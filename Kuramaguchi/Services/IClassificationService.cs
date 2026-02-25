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
    /// <summary>
    /// Represents the category searching service with AI.
    /// </summary>
    /// <typeparam name="TNumber">The type of number used by the AI service.</typeparam>
    public interface IClassificationService<TNumber>
        where TNumber : INumber<TNumber>, IFloatingPoint<TNumber>
    {
        /// <summary>
        /// The list of the category names.
        /// </summary>
        IList<string> Categories { get; init; }

        /// <summary>
        /// Gets the category from the given query.
        /// </summary>
        /// <param name="query">The query string.</param>
        /// <returns>The query's match rate for each category.</returns>
        Task<IList<IClassificationResult<TNumber>>> GetCategoryAsync(string query);
    }
}
