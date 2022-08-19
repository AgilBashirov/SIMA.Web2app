namespace SimaTest.Models
{
    public class ContractModel
    {
        public ContractModel()
        {
            SignableContainer = new SignableContainer();
            Header = new Header();
        }
        public SignableContainer SignableContainer { get; set; }
        public Header Header { get; set; }
    }
}
