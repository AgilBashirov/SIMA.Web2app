namespace SimaTest.Models
{
    public class SignableContainer
    {
        public SignableContainer()
        {
            ProtoInfo = new ProtoInfo();
            OperationInfo = new OperationInfo();
            ClientInfo = new ClientInfo();
        }
        public ProtoInfo ProtoInfo { get; set; }
        public OperationInfo OperationInfo { get; set; }
        public ClientInfo ClientInfo { get; set; }
    }
}
