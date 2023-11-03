# Backend

PokemonAPI
  - Contains endpoints consumed by [fronted](https://github.com/Sid-chudasama/Headversity-Frontend) to search and get list of matching pokemons and URI to details endpoint
  - Also has a Sqlite database that has the pokemon name and URI data
  
Function App
  - Function app is set up to run every night at 1 am to pull pokemons name and the URI to details endpoint
  - It add's the new record to Sqlite DB used in API and updates existing pokemon's URI if there is any change
  - Calls [PokeAPI](https://pokeapi.co/docs/v2#pokemon) to get Pokemon name and details URI data
  - Please make sure to update the ConnectionStrings in localsettings to point to the location of Sqlite DB
  - DB file can be found under PokemonAPI project - pokemon.db
  
PokemonCore
  - This project contains Dbcontext and Entities to setup and query Sqlite Database
  - Database is setup using code first migration
Note : Make sure to update the path to DB in appsettings, if the db file has been moved.
