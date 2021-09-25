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
        private NotesRepository _notesRepository = new NotesRepository();

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

        public async Task Delete(long appId)
        {
            await _gamesRepository.Delete(appId);
            _notesRepository.Delete(appId);
        }

        public async Task SaveNotes(long appid, string note)
        {
            await _notesRepository.SaveNotes(appid, note);
        }
        public async Task<string> GetNotes(long appid)
        {
            return await _notesRepository.GetNotes(appid);
        }
    }
}
