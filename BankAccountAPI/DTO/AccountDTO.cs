using Newtonsoft.Json;

namespace BankAccountAPI.DTO
{
    public class AccountDTO
    {
        [JsonProperty(PropertyName = "account_number")]
        public int AccountNumber { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public long Balance { get; set; }

        [JsonProperty(PropertyName = "name")] public string Name { get; set; }
    }
}