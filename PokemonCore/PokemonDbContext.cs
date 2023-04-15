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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.PokemonDetail)
                .WithOne(p => p.Pokemon)
                .HasForeignKey<PokemonDetail>("PokemonId")
                .OnDelete(DeleteBehavior.ClientCascade);
        }

        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<PokemonDetail> PokemonDetails { get; set; }
    }
}
