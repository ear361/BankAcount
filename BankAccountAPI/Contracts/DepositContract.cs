using Newtonsoft.Json;

namespace BankAccountAPI.Contracts
{
    public class TransactionContract
    {
        [JsonProperty(PropertyName = "account_number")]
        public int AccountNumber { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public long Amount { get; set; }
    }
}