using Newtonsoft.Json;

namespace BankAccountAPI.DTO
{
    public class UserDTO : StatePostcodeDTO
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }
    }
}