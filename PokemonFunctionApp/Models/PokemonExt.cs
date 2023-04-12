using PokemonCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonFunctionApp.Models
{
    public class PokemonExt
    {
        public int Count { get; set; }
        public string Next { get; set; }
        public object Previous { get; set; }
        public List<PokemonCore.Domain.Pokemon> Results { get; set; }
    }
}

