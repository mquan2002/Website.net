using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Final.Data;
using Final.Models;
using Final.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AccountService _accountService;

        public AccountController(ApplicationDbContext context, AccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        // POST: api/Account
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            // validation data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // check duplicates Username
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == account.Username);
            if (existingAccount != null)
            {
                return Conflict(new
                {
                    message = "Username đã tồn tại."
                });
            }

            // check duplicates Email
            existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == account.Email);
            if (existingAccount != null)
            {
                return Conflict(new { message = "Email đã tồn tại." });
            }
            
            // check existing Role
            var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == account.Role.Name);
            if (existingRole == null)
            {
                return Conflict(new { message = "Vai trò không tồn tại." });
            }
            // account.RoleId = existingRole.Id;

            // hash password
            account.Password = _accountService.HashPassword(account.Password);


            _context.Accounts.Add(account);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.Id }, account);
        }



        // GET: api/Account
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

    }
}