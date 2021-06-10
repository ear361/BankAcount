using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountAPI.Models
{
    //TODO: add column name and table name annotations
    public class Account : BaseModel
    {
        [Key] public int AccountNumber { get; set; }

        public string Name { get; set; }
        public long Balance { get; set; }

        [ForeignKey(nameof(User))] public string Username { get; set; }

        [Required] public virtual User User { get; set; }
    }
}