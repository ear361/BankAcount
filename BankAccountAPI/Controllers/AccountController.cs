using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankAccountAPI.Contracts;
using BankAccountAPI.DTO;
using BankAccountAPI.Helpers;
using BankAccountAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankAccountAPI.Controllers
{
    //todo: break the logic to Repository-Service pattern
    //todo: add logger
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BankContext _context;
        private readonly Random _random;

        public AccountController(BankContext context)
        {
            _context = context;
            _random = new Random();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAccountsByUsername(
            [FromHeader(Name = "Username")] string username)
        {
            username.ValidateUsername();
            return await _context.Accounts.Where(a => a.Username == username).Select(a => a.ToDto()).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromHeader(Name = "Username")] string username,
            [FromBody] AccountDTO accountDto)
        {
            username.ValidateUsername();

            var isDuplicateAccountNumber = true;
            var newAccountNumber = 0;
            while (isDuplicateAccountNumber)
            {
                newAccountNumber = _random.Next(10000000, 99999999);
                if (_context.Accounts.Any(a => a.AccountNumber == newAccountNumber))
                    continue;
                isDuplicateAccountNumber = false;
                var newAccount = new Account
                {
                    AccountNumber = newAccountNumber,
                    Name = accountDto.Name,
                    Balance = 0,
                    Username = username,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };
                accountDto = newAccount.ToDto();
                await _context.Accounts.AddAsync(newAccount);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(
                nameof(Post),
                new {id = newAccountNumber},
                accountDto);
        }

        [HttpPut("withdraw")]
        public async Task<IActionResult> Withdraw([FromHeader(Name = "Username")] string username,
            [FromBody] TransactionContract transactionContract)
        {
            username.ValidateUsername();

            var account = await GetAccount(username, transactionContract);

            if (account.Balance < transactionContract.Amount)
                //TODO: Create custom exception
                throw new Exception("Insufficient balance.");

            return await SaveTransaction(account, -transactionContract.Amount);
        }

        [HttpPut("deposit")]
        public async Task<IActionResult> Deposit([FromHeader(Name = "Username")] string username,
            [FromBody] TransactionContract transactionContract)
        {
            username.ValidateUsername();

            return await SaveTransaction(await GetAccount(username, transactionContract), transactionContract.Amount);
        }

        private async Task<Account> GetAccount(string username, TransactionContract transactionContract)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a =>
                a.Username == username && a.AccountNumber == transactionContract.AccountNumber);

            if (account == null)
                //TODO: Create custom exception
                throw new Exception("Account not found.");

            return account;
        }

        private async Task<IActionResult> SaveTransaction(Account account, long amount)
        {
            try
            {
                account.Balance += amount;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                //TODO: Create custom exception
                throw new Exception("Service is busy, please try again.");
            }

            return NoContent();
        }
    }
}