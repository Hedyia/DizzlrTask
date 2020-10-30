using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace DizzlrApp.ViewModels
{
    public class OrderManagerVM
    {
        public List<OrderManagerDetailVM> OrderDetails { get; set; }
        public SelectList Statuses { get; set; }
        public int PageCount { get; set; }
        public int CurrentPageIndex { get; set; }
    }
    public class OrderManagerDetailVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalFiles { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public SelectList Statuses { get; set; }
    }

}
