// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kuramaguchi.Services;
using Matsugasaki.Contexts;
using Matsugasaki.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsugasaki.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        private readonly MatsugasakiContext context;
        private readonly ISearchService<float> searchService;

        /// <summary>
        /// Represents the items registered in the database.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Item> items;

        /// <summary>
        /// The search query string.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        private string searchQuery = "";

        /// <summary>
        /// Initializes the view model.
        /// </summary>
        /// <param name="searchService">The search service.</param>
        public ItemsViewModel(ISearchService<float> searchService)
        {
            context = new();
            this.searchService = searchService;

            Items = [];
        }

        /// <summary>
        /// Loads entities asynchronously.
        /// </summary>
        /// <returns>The task.</returns>
        [RelayCommand]
        public async Task LoadAsync()
        {
            await context.SaveChangesAsync();
            Items.Clear();

            List<Item> ilist = await context.Items.ToListAsync();
            ilist.Sort();

            foreach (var item in ilist)
                Items.Add(item);
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <returns>No value is returned.</returns>
        [RelayCommand]
        public async Task AddAsync()
        {
            await context.SaveChangesAsync();

            Item item = new();

            await context.AddAsync(item);

            await LoadAsync();
        }

        /// <summary>
        /// Judges if search query is valid.
        /// </summary>
        /// <returns><see cref="true"/> if the search query is valid; otherwise, returns <see cref="false"/>.</returns>
        public bool CanSearch()
        {
            return string.IsNullOrEmpty(SearchQuery);
        }

        /// <summary>
        /// Searches the query and reorders the items.
        /// </summary>
        /// <returns>No returns.</returns>
        [RelayCommand(CanExecute = nameof(CanSearch))]
        public async Task SearchAsync()
        {
            IEnumerable<string> titles =
                from item in Items
                select item.Name;

            IEnumerable<string> contents =
                from item in Items
                select item.Description;

            // List of the results.
            List<float> results = [];

            await foreach (float result in searchService.SearchAsync(SearchQuery, contents, titles))
                results.Add(result);

            IEnumerable<Item> orderedItems =
                from item in Items
                from r in results
                orderby r descending
                select item;

            Items.Clear();

            foreach (var item in orderedItems)
                Items.Add(item);
        }

        /// <summary>
        /// Removes the selected item.
        /// </summary>
        /// <param name="item">The selected item.</param>
        /// <returns>No value.</returns>
        [RelayCommand(CanExecute = nameof(CanRemove))]
        public async Task RemoveAsync(Item item)
        {
            context.Remove(item);

            await LoadAsync();
        }

        /// <summary>
        /// Judges is the record can be removed.
        /// </summary>
        /// <param name="item">The selected item.</param>
        /// <returns>The value indicates if the record can be removed.</returns>
        public bool CanRemove(Item item)
        {
            return context.Items.Contains(item);
        }
    }
}
