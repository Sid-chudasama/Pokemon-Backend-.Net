using System;

namespace PokemonCore.Domain
{
    public class Pokemon
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public void Update(Pokemon p)
        {
            this.Name = p.Name;
            this.Url = p.Url;
        }
    }
}