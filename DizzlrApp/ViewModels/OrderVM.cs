using System;
using System.Collections.Generic;

namespace DizzlrApp.ViewModels
{
    public class OrderVM
    {
        public List<OrderDetailVM> OrderDetails { get; set; }
    }

    public class OrderDetailVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int TotalFiles { get; set; }
        public string Status { get; set; }
    }


}
