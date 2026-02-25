// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace Kuramaguchi.Services
{
    public class ClassificationService<TNumber> : IClassificationService<TNumber>
        where TNumber : INumber<TNumber>, IFloatingPoint<TNumber>
    {
        protected readonly IEmbeddingGenerator<string, Embedding<TNumber>> embeddingGenerator;

        public IList<string> Categories { get; init; }

        /// <summary>
        /// Initialize the service with an <see cref="IEmbeddingGenerator{String, Embedding{TNumber}}"/> instance.
        /// </summary>
        /// <param name="embeddingGenerator">An <see cref="IEmbeddingGenerator{string, Embedding{TNumber}}"/> instance to be used.</param>
        public ClassificationService(IEmbeddingGenerator<string, Embedding<TNumber>> embeddingGenerator = null!)
        {
            ArgumentNullException.ThrowIfNull(embeddingGenerator);

            Categories ??= new List<string>();

            this.embeddingGenerator = embeddingGenerator;
        }

        public ClassificationService(IEmbeddingGenerator<string, Embedding<TNumber>> embeddingGenerator, IList<string> categories)
        {
            this.embeddingGenerator = embeddingGenerator;
            Categories = categories;
        }
        
        public async virtual Task<IList<IClassificationResult<TNumber>>> GetCategoryAsync(string query)
        {
            Embedding<TNumber> embedding = await embeddingGenerator.GenerateAsync(query);

            TNumber[] queryVector = embedding.Vector.ToArray();

            List<IClassificationResult<TNumber>> results = [];

            for (int i = 0;i < Categories.Count;i++)
            {
                Embedding<TNumber> embedding1 = await embeddingGenerator.GenerateAsync(Categories[i]);

                TNumber[] numbers = embedding1.Vector.ToArray();

                TNumber matchRate = TNumber.Zero;

                foreach (
                    (TNumber queryElement, TNumber categoryElement)
                    in queryVector.Zip(numbers))
                    matchRate += queryElement * categoryElement;

                ClassificationResult<TNumber> classificationResult = new()
                {
                    Classifier = Categories[i],
                    Index = i,
                    MatchRate = matchRate,
                };

                results.Add(classificationResult);
            }

            return results;
        }
    }
}
