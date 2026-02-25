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
    public class GoogleSearchService : SearchService, ISearchService<float>
    {
        public GoogleSearchService(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
            : base(embeddingGenerator) { }

        public async override IAsyncEnumerable<float> SearchAsync(string query, IEnumerable<string> contents, IEnumerable<string> titles = null)
        {
            AdditionalPropertiesDictionary queryAddtionalOptions = new()
            {
                { "task_type", "retrieval_query" }
            };

            // Embedding of the query.
            Embedding<float> embedding1 = await embeddingGenerator.GenerateAsync(
                query,
                new EmbeddingGenerationOptions()
                {
                    AdditionalProperties = queryAddtionalOptions
                });

            if (titles is not null)
            {
                if (titles.Count() != contents.Count())
                    throw new ArgumentException("The numbers of the titles and contents must be same.", nameof(titles));
                IEnumerable<AdditionalPropertiesDictionary> pairs =
                    from string title in titles
                    select new AdditionalPropertiesDictionary()
                    {
                        { "task_type", "retrieval_document" },
                        { "title", title }
                    };
                IEnumerable<EmbeddingGenerationOptions> options =
                    from AdditionalPropertiesDictionary pair in pairs
                    select new EmbeddingGenerationOptions()
                    {
                        AdditionalProperties = pair
                    };
                for (int i = 0;i < contents.Count(); i++)
                {
                    Embedding<float> embedding = await embeddingGenerator.GenerateAsync(
                        contents.ToList()[i],
                        options.ToList()[i]);

                    List<float> floats = [];

                    for (int index = 0; i < embedding.Dimensions; i++)
                        floats.Add(embedding.Vector.Span[index] * embedding1.Vector.Span[index]);

                    // Returning element.
                    float result = 0;
                    foreach (float f in floats)
                        result += f;

                    yield return result;
                }
            }
            else
            {
                EmbeddingGenerationOptions embeddingGenerationOptions = new()
                {
                    AdditionalProperties = new()
                    {
                        { "task_type", "retrieval_document" }
                    }
                };

                // Document vectors data.
                IEnumerable<Embedding<float>> embeddings = await embeddingGenerator.GenerateAsync(contents, embeddingGenerationOptions);
                
                foreach (Embedding<float> embedding in embeddings)
                {
                    List<float> floats = [];

                    for (int index = 0; index < embedding.Dimensions; index++)
                        floats.Add(embedding.Vector.Span[index] * embedding1.Vector.Span[index]);

                    yield return floats.Sum();
                }
            }
        }
    }
}
