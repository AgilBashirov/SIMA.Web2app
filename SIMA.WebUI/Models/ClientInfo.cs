namespace SIMA.WebUI.Models
{
    public class ClientInfo
    {
        public int ClientId { get; set; }
        public string SecretKey { get; set; }
        public string HostName { get; set; }
        public string IconURI { get; set; }
        public string Callback { get; set; }
    }
}
