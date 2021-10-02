using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiFe.Data;
using DiFe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiFe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoinController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoinController(AppDbContext context) => _context = context;

        [HttpGet("Wallet/{wallet}")]
        public async Task<ActionResult<List<CoinInfo>>> GetByWallet(string wallet)
        {
            try
            {
                var tokens = await _context.Coins.Where(x => x.Wallet.ToUpper() == wallet.ToUpper()).OrderBy(x => x.Name).ToListAsync();
                return tokens;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CoinInfo>> Post([FromBody] CoinInfo value)
        {
            try
            {
                if (value is null)
                {
                    return BadRequest();
                }
                var token = new CoinInfo
                {
                    Address = value.Address ?? string.Empty,
                    Countdown = value.Countdown ?? string.Empty,
                    Name = value.Name ?? string.Empty,
                    Wallet = value.Wallet ?? string.Empty,
                    Website = value.Website ?? string.Empty,
                };
                await _context.Coins.AddAsync(token);
                await _context.SaveChangesAsync();
                return token;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CoinInfo>> Put(string id, [FromBody] CoinInfo value)
        {
            try
            {
                if (value is null)
                {
                    return BadRequest();
                }
                var token = await _context.Coins.FirstOrDefaultAsync(x => x.Id == id);
                if (token is null)
                {
                    return BadRequest(new ExceptionInfo("That token doesn't exist."));
                }
                token.Address = value.Address ?? string.Empty;
                token.Countdown = value.Countdown ?? string.Empty;
                token.Name = value.Name ?? string.Empty;
                token.Wallet = value.Wallet ?? string.Empty;
                token.Website = value.Website ?? string.Empty;
                await _context.SaveChangesAsync();
                return token;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut]
        public async Task<ActionResult<List<CoinInfo>>> PutForLastPrice([FromBody] List<CoinInfo> value)
        {
            try
            {
                value.ForEach(x =>
                {
                    var coin = _context.Coins.FirstOrDefault(y => y.Id == x.Id);
                    if (coin is null)
                    {
                        return;
                    }
                    coin.LastPrice = x.Price;
                    coin.LastValue = x.Value;
                });
                await _context.SaveChangesAsync();
                return value;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CoinInfo>> Delete(string id)
        {
            try
            {
                var token = await _context.Coins.FirstOrDefaultAsync(x => x.Id == id);
                if (token is null)
                {
                    return BadRequest(new ExceptionInfo("That token doesn't exist."));
                }
                _context.Coins.Remove(token);
                await _context.SaveChangesAsync();
                return token;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}