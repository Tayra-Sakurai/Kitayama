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
    public class MediumCategory :ICategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LargeCategoryId { get; set; }
        public LargeCategory LargeCategory { get; set; } = null!;
        public ICollection<SmallCategory> SmallCategories { get; } = new List<SmallCategory>();
    }
}
