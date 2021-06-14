namespace BankAccountAPI
{
    public interface IAppConfig
    {
        string ConnectionString { get; set; }
        string APIGateWayEndpoint { get; set; }
    }

    public class AppConfig : IAppConfig
    {
        public string ConnectionString { get; set; }
        public string APIGateWayEndpoint { get; set; }
    }
}