using System.Net.Http.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using PokemonCore;
using PokemonFunctionApp.Models;
using System.Collections.Generic;
using PokemonCore.Domain;

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
                var allPokemons = await _pokemonDbContext.Pokemons.ToListAsync(); //Simultaneously load data from DB while external API call is running
                var result = await responseTask;
                if (!result?.IsSuccessStatusCode ?? true)
                {
                    _logger.LogError($"Get pokemons failed with status : {result?.StatusCode}");
                    return;
                }
                var pokemonExt = await result.Content.ReadFromJsonAsync<PokemonExt>();


                //Process data
                pokemonExt.Results.ForEach(async i =>
                {
                    if (allPokemons.Any(p => p.Name == i.Name))
                    {
                        allPokemons.Single(p => p.Name == i.Name).Update(i);
                    }
                    else
                    {
                        await _pokemonDbContext.Pokemons.AddAsync(i);
                    }
                });
                _pokemonDbContext.Pokemons.UpdateRange(allPokemons);
                await _pokemonDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
            }

            _logger.LogInformation($"Next Pokemon function timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
