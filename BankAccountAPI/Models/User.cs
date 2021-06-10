using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankAccountAPI.Models
{
    //TODO: add column name and table name annotations
    public class User : BaseModel
    {
        [Key] public string Username { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
        public int PostCode { get; set; }
        public string Address { get; set; }

        public List<Account> Accounts { get; set; }
    }
}