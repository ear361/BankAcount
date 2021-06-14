using Newtonsoft.Json;

namespace BankAccountAPI.DTO
{
    public class StatePostcodeDTO
    {
        [JsonProperty(PropertyName = "state")] 
        public string State { get; set; }

        [JsonProperty(PropertyName = "post_code")]
        public int PostCode { get; set; }
    }
}