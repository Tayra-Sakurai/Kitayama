// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuramaguchi.Services
{
    public class SearchService : ISearchService<float>
    {
        protected readonly IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator;
        protected EmbeddingGenerationOptions options; 

        public SearchService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
        {
            this.embeddingGenerator = embeddingGenerator;
        }

        public async virtual IAsyncEnumerable<float> SearchAsync(string query, IEnumerable<string> contents, IEnumerable<string> titles = null)
        {
            Embedding<float> embedding = await embeddingGenerator.GenerateAsync(query);
            IEnumerable<float> floats1 =
                from element in embedding.Vector.ToArray()
                select element;

            // Calculates inner products for each item.

            foreach (string content in contents)
            {
                Embedding<float> embedding1 = await embeddingGenerator.GenerateAsync(content);

                ICollection<float> floats =
                    new List<float>();

                for (int i = 0;i < embedding.Dimensions;i++)
                    floats.Add(embedding.Vector.Span[i] * embedding1.Vector.Span[i]);

                float result = 0;

                foreach (float f in floats)
                    result += f;

                yield return result;
            }
        }
    }
}
