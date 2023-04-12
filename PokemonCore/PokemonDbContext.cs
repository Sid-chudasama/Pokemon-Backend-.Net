using Microsoft.EntityFrameworkCore;
using PokemonCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonCore
{
    public class PokemonDbContext : DbContext
    {
        public PokemonDbContext(DbContextOptions<PokemonDbContext> options) : base(options) {
            Database.EnsureCreated();
        }

        public DbSet<Pokemon> Pokemons { get; set; }
    }
}
