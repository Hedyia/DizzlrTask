namespace DizzlrApp.Models
{
    public class OrderFileItem
    {
        public int OrderFileItemId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int FileId { get; set; }
        public File File { get; set; }
    }
}
