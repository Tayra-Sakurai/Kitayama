// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Matsugasaki.Contexts;
using Matsugasaki.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsugasaki.ViewModels
{
    public partial class ItemViewModel : ObservableObject
    {
        private Item item;
        private readonly MatsugasakiContext context;

        /// <summary>
        /// Sets the date and time of the item.
        /// </summary>
        /// <param name="model">The model object to be updated.</param>
        /// <param name="date">The date of the new time.</param>
        /// <param name="time">The time of the new time.</param>
        private static void SetDateTimeValue(Item model, DateTimeOffset date, TimeSpan time)
        {
            DateTime dateTime = date.Date;
            model.DateTime = dateTime.Add(time);
        }

        /// <inheritdoc cref="SetDateTimeValue(Item, DateTimeOffset, TimeSpan)"/>
        private void SetDateTimeValue(Item model, DateTimeOffset date)
        {
            SetDateTimeValue(model, date, model.DateTime.TimeOfDay);
        }

        /// <inheritdoc cref="SetDateTimeValue(Item, DateTimeOffset, TimeSpan)"/>
        private void SetDateTimeValue(Item model, TimeSpan time)
        {
            SetDateTimeValue(model, model.DateTime.Date, time);
        }

        private Method FindMethod(string methodName)
        {
            foreach (var method in context.Methods.AsEnumerable())
                if(method.Name == methodName)
                    return method;
            
            throw new ArgumentException("Invalid Name was given.", nameof(methodName));
        }

        private void SetExpenseMethod(Item model, string methodName)
        {
            model.ExpenseMethod = FindMethod(methodName);
        }

        private void SetIncomeMethod(Item model, string methodName)
        {
            model.IncomeMethod = FindMethod(methodName);
        }

        private LargeCategory FindLargeCategory(string name)
        {
            return context
                .LargeCategories
                .Include(e => e.MediumCategories)
                .ThenInclude(e => e.SmallCategories)
                .Single(c => c.Name == name);
        }

        /// <summary>
        /// Updates the collections.
        /// </summary>
        /// <param name="model">The data model.</param>
        private void UpdateCollections(Item model)
        {
            LargeCategory category = model.SmallCategory.MediumCategory.LargeCategory;

            IQueryable<MediumCategory> entry = context.Entry(category)
                .Collection(e => e.MediumCategories)
                .Query();

            MediumCategories.Clear();

            foreach (MediumCategory medium
                in entry)
                MediumCategories.Add(medium.Name);

            SmallCategories.Clear();

            MediumCategory mediumCategory = model.SmallCategory.MediumCategory;

            IQueryable<SmallCategory> smallCategories =
                context
                .Entry(mediumCategory)
                .Collection(e => e.SmallCategories)
                .Query();

            foreach (
                SmallCategory smallCategory in smallCategories)
                SmallCategories.Add(smallCategory.Name);
        }

        /// <inheritdoc cref="UpdateCollections(Item)"/>
        [RelayCommand]
        public void UpdateCollections()
        {
            UpdateCollections(item);
        }

        private MediumCategory FindMediumCategory(Item model, string name)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);

            ICollection<MediumCategory> mediumCategories =
                context
                .LargeCategories
                .Include(e => e.MediumCategories)
                .ThenInclude(e => e.SmallCategories)
                .Single(c => c.Id == model.SmallCategory.MediumCategory.LargeCategoryId)
                .MediumCategories;

            return mediumCategories.FirstOrDefault(c => c.Name == name);
        }

        private SmallCategory FindSmallCategory(Item model, string name)
        {
            ICollection<SmallCategory> smallCategories =
                context
                .MediumCategories
                .Include(e => e.SmallCategories)
                .Single(c => c.Id == model.SmallCategory.MediumCategoryId)
                .SmallCategories;

            return smallCategories.FirstOrDefault(c => c.Name == name);
        }

        private void SetLargeCategory(Item model, string name)
        {
            model.SmallCategory = FindLargeCategory(name).MediumCategories.First().SmallCategories.First();
        }

        private void SetMediumCategory(Item model, string name)
        {
            model.SmallCategory = FindMediumCategory(model, name).SmallCategories.First();

            UpdateCollections(model);
        }

        private void SetSmallCategory(Item model, string name)
        {
            model.SmallCategory = FindSmallCategory(model, name);

            UpdateCollections(model);
        }

        public DateTimeOffset Date
        {
            get => item.DateTime.Date;
            set => SetProperty(new DateTimeOffset(item.DateTime.Date), value, item, SetDateTimeValue);
        }

        public TimeSpan Time
        {
            get => item.DateTime.TimeOfDay;
            set => SetProperty(item.DateTime.TimeOfDay, value, item, SetDateTimeValue);
        }

        public string ExpenseMethod
        {
            get => item.ExpenseMethod.Name;
            set => SetProperty(item.ExpenseMethod.Name, value, item, SetExpenseMethod);
        }

        public string IncomeMethod
        {
            get => item.IncomeMethod.Name;
            set => SetProperty(item.IncomeMethod.Name, value, item, SetIncomeMethod);
        }

        public string Name
        {
            get => item.Name;
            set => SetProperty(item.Name, value, item, (m, v) => m.Name = v);
        }

        public double Amount
        {
            get => item.Amount;
            set => SetProperty(item.Amount, value, item, (m, v) => m.Amount = (int)v);
        }

        public string LargeCategory
        {
            get => item.SmallCategory.MediumCategory.LargeCategory.Name;
            set => SetProperty(item.SmallCategory.MediumCategory.LargeCategory.Name, value, item, SetLargeCategory);
        }

        public string MediumCategory
        {
            get => item.SmallCategory.MediumCategory.Name;
            set => SetProperty(item.SmallCategory.MediumCategory.Name, value, item, SetMediumCategory);
        }

        public string SmallCategory
        {
            get => item.SmallCategory.Name;
            set => SetProperty(item.SmallCategory.Name, value, item, SetSmallCategory);
        }

        public ObservableCollection<string> Methods
        {
            get
            {
                ObservableCollection<string> result = [];

                foreach (var method in context.Methods.AsEnumerable())
                    result.Add(method.Name);

                return result;
            }
        }

        public ObservableCollection<string> LargeCategories
        {
            get
            {
                IQueryable<string> strings =
                    from LargeCategory in context.LargeCategories
                    orderby LargeCategory.Id
                    select LargeCategory.Name;
                return new ObservableCollection<string>(strings);
            }
        }

        [ObservableProperty]
        private ObservableCollection<string> mediumCategories;

        [ObservableProperty]
        private ObservableCollection<string> smallCategories;

        public ItemViewModel()
        {
            context = new();

            item = new()
            {
                Amount = 0,
                Name = string.Empty,
            };

            MediumCategories = [];
            SmallCategories = [];

            foreach (
                MediumCategory mediumCategory
                in context
                .LargeCategories
                .Include(e => e.MediumCategories)
                .First()
                .MediumCategories)
                MediumCategories.Add(mediumCategory.Name);

            foreach (
                SmallCategory small
                in context
                .LargeCategories
                .Include(e => e.MediumCategories)
                .ThenInclude(e => e.SmallCategories)
                .First()
                .MediumCategories
                .First()
                .SmallCategories)
                SmallCategories.Add(small.Name);
        }

        public void InitializeForExistingrecord(Item item)
        {
            this.item = item;

            SmallCategories.Clear();
            MediumCategories.Clear();



            OnPropertyChanged();
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            EntityEntry<Item> entry = context.Entry(item);

            if (entry.State == EntityState.Detached)
            {
                context.Update(item);
            }

            await context.SaveChangesAsync();
        }
    }
}
