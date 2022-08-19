namespace SimaTest.VmModels
{
    public class ContractVm
    {
        public string Version { get; set; }
        public string Type { get; set; }
        public string OperationId { get; set; }
        public DateTime NbfUTC { get; set; }
        public DateTime ExpUTC { get; set; }
        public string Assignee { get; set; }
        public int ClientId { get; set; }
        public string SecretKey { get; set; }
        public string HostName { get; set; }
        public string IconURI { get; set; }
        public string Callback { get; set; }
    }
}
