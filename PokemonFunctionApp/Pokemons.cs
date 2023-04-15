using System.Net.Http.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using PokemonCore;
using PokemonFunctionApp.Models;
using System.Collections.Generic;
using PokemonCore.Domain;
using Microsoft.Azure.Functions.Worker.Http;

namespace PokemonFunctionApp
{
    public class Pokemons
    {
        private readonly ILogger _logger;
        private readonly PokemonDbContext _pokemonDbContext;

        public Pokemons(ILoggerFactory loggerFactory, PokemonDbContext pokemonDbContext)
        {
            _logger = loggerFactory.CreateLogger<Pokemons>();
            _pokemonDbContext = pokemonDbContext;
            _pokemonDbContext = pokemonDbContext;
        }

        [Function("Pokemons")]
        public async Task Run([TimerTrigger("0 0 1 * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger Pokemon function executed at: {DateTime.Now}");

            try
            {
                //Initial call to API
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("PokemonEndPoint"));
                var response = await client.GetAsync("");
                if (!response?.IsSuccessStatusCode ?? true)
                {
                    _logger.LogError($"Get pokemons failed with status : {response?.StatusCode}");
                    return;
                }
                var initialCall = await response.Content.ReadFromJsonAsync<PokemonExt>();


                //Get all records
                var responseTask = client.GetAsync($"?offset=0&limit={initialCall?.Count}");
                var allPokemons = await _pokemonDbContext.Pokemons.ToListAsync();
                var result = await responseTask;
                if (!result?.IsSuccessStatusCode ?? true)
                {
                    _logger.LogError($"Get pokemons failed with status : {result?.StatusCode}");
                    return;
                }
                var pokemonExt = await result.Content.ReadFromJsonAsync<PokemonExt>();
                List<Pokemon> newPokemons = new List<Pokemon>();

                //Process data
                pokemonExt.Results.ForEach(async i =>
                {
                    if (allPokemons.Any(p => p.Name == i.Name))
                    {
                        allPokemons.Single(p => p.Name == i.Name).Update(i);
                    }
                    else
                    {
                        newPokemons.Add(i);
                    }
                });

                await ManageDetailsAsync(newPokemons); //Get details of new pokemons
                await ManageDetailsAsync(allPokemons); //Update details of existing pokemons

                await _pokemonDbContext.Pokemons.AddRangeAsync(newPokemons);
                _pokemonDbContext.Pokemons.UpdateRange(allPokemons);

                await _pokemonDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
            }

            _logger.LogInformation($"Next Pokemon function timer schedule at: {myTimer.ScheduleStatus.Next}");
        }

        #region Private
        private async Task ManageDetailsAsync(List<Pokemon> pokemons)
        {
            int count = 1;
            List<PokemonDetail> details = new List<PokemonDetail>();

            foreach(var p in pokemons)
            {
                //Call details API for 200 pokemons simultaneously
                List<Task> allDetailTasks = new List<Task>();
                allDetailTasks.Add(GetDetails(p));

                if (count % 200 == 0)
                {
                    await Task.WhenAll(allDetailTasks);
                }
                else if (count == pokemons.Count)
                {
                    await Task.WhenAll(allDetailTasks);
                }
                count++;
            }
        }

        private async Task GetDetails(Pokemon pokemon)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(pokemon.Url);
            var response = await client.GetAsync("");

            if (!response?.IsSuccessStatusCode ?? true)
            {
                _logger.LogError($"Get pokemon detail failed with status : {response?.StatusCode}");
                return;
            }
            var result = await response.Content.ReadFromJsonAsync<PokemonDetailExt>();

            pokemon.PokemonDetail = new PokemonDetail
            {
                Experience = result.Base_experience ?? 0,
                Height = result.Height,
                Weight = result.Weight
            };
        }
        #endregion
    }
}
