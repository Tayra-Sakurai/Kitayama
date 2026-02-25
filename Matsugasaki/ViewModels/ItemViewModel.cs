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

        public ItemViewModel()
        {
            context = new();

            item = new()
            {
                Amount = 0,
                Name = string.Empty,
            };
        }

        public void InitializeForExistingrecord(Item item)
        {
            this.item = item;

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
