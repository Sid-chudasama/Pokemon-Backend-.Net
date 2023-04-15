using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PokemonCore.Domain
{
    public class PokemonDetail
    {
        public long Id { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Experience { get; set; }
        [JsonIgnore]
        public Pokemon Pokemon { get; set; }
    }
}
