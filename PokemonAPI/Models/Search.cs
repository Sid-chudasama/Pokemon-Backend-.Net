using PokemonCore.Domain;

namespace PokemonAPI.Models
{
    public class Search : PagedQuery
    {
        public string? Query { get; set; }
        public SortField? SortField { get; set; }
        public OrderBy OrderBy { get; set; }
    }

    public class PagedQuery
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class SearchResponse
    {
        public int recordCount { get; set; }
        public List<Pokemon> pokemons { get; set; }
    }
}
