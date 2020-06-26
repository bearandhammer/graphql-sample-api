using PizzaOrder.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaOrder.Business.Models
{
    public class PageRequest
    {
        public int? First { get; set; }
        public int? Last { get; set; }
        public string After { get; set; }
        public string Before { get; set; }
        public SortingDetails<CompletedOrdersSortingFields> OrderBy { get; set; }
    }
}
