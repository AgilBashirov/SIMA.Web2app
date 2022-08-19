namespace SIMA.WebUI.Models
{
    public class OperationInfo
    {
        public string Type { get; set; }
        public string OperationId { get; set; }
        public DateTime NbfUTC { get; set; }
        public DateTime ExpUTC { get; set; }
        public string Assignee { get; set; }
    }
}
