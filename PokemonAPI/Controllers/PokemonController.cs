using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonAPI.Models;
using PokemonCore;
using PokemonCore.Domain;

namespace PokemonAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PokemonController : ControllerBase
    {

        private readonly ILogger<PokemonController> _logger;
        private readonly PokemonDbContext _pokemonDbContext;

        public PokemonController(ILogger<PokemonController> logger, PokemonDbContext pokemonDbContext)
        {
            _logger = logger;
            _pokemonDbContext = pokemonDbContext;
        }

        #region Public
        [HttpGet(Name = "Search")]
        [Produces("application/json", Type = typeof(SearchResponse))]
        [ProducesResponseType(typeof(SearchResponse), (int)StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Search([FromQuery]Search criteria)
        {
            try
            {
                var query = _pokemonDbContext.Pokemons
                            .Include(p => p.PokemonDetail)
                            .AsQueryable();

                if (!string.IsNullOrEmpty(criteria.Query))
                {
                    query = query.Where(p => p.Name.Contains(criteria.Query));
                }

                int skip = (criteria.Page ?? 1) - 1;
                int pageSize = criteria.PageSize ?? 20;

                query = SortAndOrder(criteria.SortField, criteria.OrderBy, query);

                var pokemons = query.Skip(skip * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

                var matchingRecordsCount = query.CountAsync();

                return Ok(new SearchResponse
                {
                    recordCount = await matchingRecordsCount,
                    pokemons = await pokemons
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpGet(Name = "GetAll")]
        [Produces("application/json", Type = typeof(List<Pokemon>))]
        [ProducesResponseType(typeof(List<Pokemon>), (int)StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _pokemonDbContext.Pokemons.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }

        [HttpGet(Name = "GetByName/{name}")]
        [Produces("application/json", Type = typeof(List<Pokemon>))]
        [ProducesResponseType(typeof(List<Pokemon>), (int)StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var result = await _pokemonDbContext.Pokemons.FirstOrDefaultAsync(p => p.Name == name);

                return string.IsNullOrEmpty(result?.Name) ? NotFound("No match found") : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Problem(ex.Message);
            }
        }
        #endregion

        #region Private
        private IQueryable<Pokemon> SortAndOrder(SortField? sortBy, OrderBy orderby, IQueryable<Pokemon> query) => sortBy switch
        {
            SortField.Name => orderby.Equals(OrderBy.Asc) ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
            SortField.Height => orderby.Equals(OrderBy.Asc) ? query.OrderBy(p => p.PokemonDetail.Height) : query.OrderByDescending(p => p.PokemonDetail.Height),
            SortField.Weight => orderby.Equals(OrderBy.Asc) ? query.OrderBy(p => p.PokemonDetail.Weight) : query.OrderByDescending(p => p.PokemonDetail.Weight),
            SortField.Experience => orderby.Equals(OrderBy.Asc) ? query.OrderBy(p => p.PokemonDetail.Experience) : query.OrderByDescending(p => p.PokemonDetail.Experience),
            _ => orderby.Equals(OrderBy.Asc) ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id),
        };
        #endregion
    }
}