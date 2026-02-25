// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kuramaguchi.Services
{
    public interface ISearchService<TNumber>
        where TNumber : INumber<TNumber>, IFloatingPoint<TNumber>
    {
        /// <summary>
        /// Conducts AI search by query onto the titled or untitled documents.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="contents">The document contents.</param>
        /// <param name="titles">Document titles.</param>
        /// <returns>An <see cref="IAsyncEnumerable{float}"/> instance each of which represents the match rate.</returns>
        /// <exception cref="ArgumentException">When the numbers of given titles and given contents are not same.</exception>
        IAsyncEnumerable<float> SearchAsync(string query, IEnumerable<string> contents, IEnumerable<string> titles = null);
    }
}
