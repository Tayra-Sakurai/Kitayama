// SPDX-FileCopyrightText: 2026 Tayra Sakurai <taira_salurai@outlook.jp>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matsugasaki.Models
{
    public class Item : IComparable<Item>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public int Amount { get; set; }
        public int? ExpenseMethodId { get; set; }
        public int? IncomeMethodId { get; set; }
        public int SmallCategoryId { get; set; }
        public Method ExpenseMethod { get; set; } = null;
        public Method IncomeMethod { get; set; } = null;
        public SmallCategory SmallCategory { get; set; } = null!;

        public int CompareTo(Item other)
        {
            if (other == null) return 1;
            if (DateTime != other.DateTime) return DateTime.CompareTo(other.DateTime);
            return Id.CompareTo(other.Id);
        }
    }
}
