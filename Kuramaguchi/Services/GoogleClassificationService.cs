// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Kuramaguchi.Extensions;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuramaguchi.Services
{
    public class GoogleClassificationService : ClassificationService<float>, IClassificationService<float>
    {
        public GoogleClassificationService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : base(embeddingGenerator) { }
        public GoogleClassificationService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, IList<string> categories)
            : base(embeddingGenerator, categories) { }

        public async override Task<IList<IClassificationResult<float>>> GetCategoryAsync(string query)
        {
            EmbeddingGenerationOptions queryOptions = new()
            {
                AdditionalProperties = new()
                {
                    { "task_type", "retrieval_query" }
                }
            };
            EmbeddingGenerationOptions categoryOptions = new()
            {
                AdditionalProperties = new()
                {
                    { "task_type", "calssification" }
                }
            };

            Embedding<float> queryEmbedding = await embeddingGenerator.GenerateAsync(query, queryOptions);

            int i = 0;
            List<IClassificationResult<float>> results = [];

            foreach (
                (string value, Embedding<float> embedding)
                in await embeddingGenerator.GenerateAndZipAsync(Categories, categoryOptions))
            {
                float product = queryEmbedding.Vector.Span.GetInnerProductWith<float, float, float>(embedding.Vector.Span);

                ClassificationResult<float> result = new()
                {
                    Classifier = value,
                    Index = i++,
                    MatchRate = product,
                };

                results.Add(result);
            }

            return results;
        }
    }
}
