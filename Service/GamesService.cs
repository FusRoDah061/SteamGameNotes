using SteamGameNotes.DTO;
using SteamGameNotes.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamGameNotes.Service
{
    public class GamesService
    {
        private GamesRepository _gamesRepository = new GamesRepository();

        public async Task Create(SteamAppDto game)
        {
            var games = await _gamesRepository.ListGames();

            if(games.Contains(game))
            {
                throw new InvalidOperationException("Game already exists.");
            }

            await _gamesRepository.Create(game);
        }

        public async Task<List<SteamAppDto>> ListGames()
        {
            return await _gamesRepository.ListGames();
        }
    }
}
