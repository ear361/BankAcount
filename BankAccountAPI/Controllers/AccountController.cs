using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankAccountAPI.Contracts;
using BankAccountAPI.DTO;
using BankAccountAPI.Exceptions;
using BankAccountAPI.Helpers;
using BankAccountAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankAccountAPI.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BankContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly Random _random;

        public AccountController(BankContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
            _random = new Random();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAccountsByUsername(
            [FromHeader(Name = "Username")] string username)
        {
            await CheckUserExist(username);
            
            return await _context.Accounts.Where(a => a.Username == username).Select(a => a.ToDto()).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromHeader(Name = "Username")] string username,
            [FromBody] AccountDTO accountDto)
        {
            await CheckUserExist(username);
            
            if (string.IsNullOrWhiteSpace(accountDto.Name))
                throw new InvalidInputException("New account's name cannot be empty.");

            var isDuplicateAccountNumber = true;
            int newAccountNumber;
            while (isDuplicateAccountNumber)
            {
                // re-generate the account number when it is duplicated
                newAccountNumber = _random.Next(10000000, 99999999);
                if (_context.Accounts.Any(a => a.AccountNumber == newAccountNumber))
                    continue;
                isDuplicateAccountNumber = false;
                //TODO: add mappers
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

            _logger.LogInformation($"Account created. Username: {username}. Account Number: {accountDto.AccountNumber}");
            return Created(string.Empty, accountDto);
        }

        [HttpPut("withdraw")]
        public async Task<IActionResult> Withdraw([FromHeader(Name = "Username")] string username,
            [FromBody] TransactionContract transactionContract)
        {
            await CheckUserExist(username);

            var account = await GetAccount(username, transactionContract);

            if (account.Balance < transactionContract.Amount)
                throw new InvalidInputException("Insufficient balance.");

            return await SaveTransaction(account, -transactionContract.Amount);
        }

        [HttpPut("deposit")]
        public async Task<IActionResult> Deposit([FromHeader(Name = "Username")] string username,
            [FromBody] TransactionContract transactionContract)
        {
            await CheckUserExist(username);

            return await SaveTransaction(await GetAccount(username, transactionContract), transactionContract.Amount);
        }

        private async Task<Account> GetAccount(string username, TransactionContract transactionContract)
        {
            if (transactionContract == null)
                throw new InvalidInputException("Transaction contract cannot be empty");
            
            if (transactionContract.Amount <= 0)
                throw new InvalidInputException("Transaction amount should be greater than 0");
            
            var account = await _context.Accounts.SingleOrDefaultAsync(a =>
                a.Username == username && a.AccountNumber == transactionContract.AccountNumber);

            if (account == null)
                throw new ItemNotFoundException($"Account not found. Account number : {transactionContract.AccountNumber}.");

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
                throw new Exception("Service is busy, please try again.");
            }
            
            _logger.LogInformation($"{account.AccountNumber}'s balance was updated. Amount: {amount}.");
            return NoContent();
        }
        
        private async Task CheckUserExist(string username)
        {
            username.ValidateUsername();
            
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
                throw new ItemNotFoundException($"User not found. Username: {username}");
        }
    }
}