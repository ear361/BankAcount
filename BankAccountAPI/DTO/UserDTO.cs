using Newtonsoft.Json;

namespace BankAccountAPI.DTO
{
    public class UserDTO
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "state")] public string State { get; set; }

        [JsonProperty(PropertyName = "post_code")]
        public int PostCode { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }
    }
}