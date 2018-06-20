using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasketManagerWebApi.Models;

namespace BasketManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly BasketContext _context;

        public BasketsController(BasketContext context)
        {
            _context = context;
        }

        // GET: api/Baskets
        [HttpGet]
        public IEnumerable<Basket> GetBaskets()
        {
            return _context.Baskets;
        }

        // GET: api/Baskets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasket([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var basket = await _context.Baskets.FindAsync(id);

            if (basket == null)
            {
                return NotFound();
            }

            return Ok(basket);
        }
                     
        // DELETE: api/Baskets/5
        [HttpDelete("{id}")]
        public IActionResult DeleteBasket([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =  _context.DeleteBasketAndAllElements(id);

            if(result == Enums.BasketDeleteResult.NotFound)
            {
                return NotFound(string.Format("basket with id {0} not found", id));
            }
            return Ok();
        }

        private bool BasketExists(int id)
        {
            return _context.Baskets.Any(e => e.Id == id);
        }
    }
}