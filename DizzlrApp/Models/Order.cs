using DizzlrApp.Areas.Identity.Data;
using System;
using System.Collections.Generic;

namespace DizzlrApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderFileItem> Files { get; set; }
        public int StatusId { get; set; }
        public virtual Status Status { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
