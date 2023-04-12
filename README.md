# Backend

PokemonAPI
  - Contains endpoints consumed by fronted to search and get list of matching pokemons and URI to details endpoint
  - Also has a Sqlite database that has the pokemon name and URI data
  
Function App
  - Function app is set up to run every night at 1 am to pull pokemons name and the URI to details endpoint
  - It add's the new record to Sqlite DB used in API and updates existing pokemon's URI if there is any change
  
PokemonCore
  - This project contains Dbcontext and Entities to setup and query Sqlite Database
  - Database is setup using code first migration
