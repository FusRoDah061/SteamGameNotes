using log4net;
using SteamGameNotes.DTO;
using SteamGameNotes.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamGameNotes.Service
{
    public class GamesService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GamesService));

        private GamesRepository _gamesRepository = new GamesRepository();
        private NotesRepository _notesRepository = new NotesRepository();

        public async Task Create(SteamAppDto game)
        {
            log.Info("Adding new game: " + game.ToString());

            var games = await _gamesRepository.ListGames();

            if(games.Contains(game))
            {
                throw new InvalidOperationException("Game already exists.");
            }

            await _gamesRepository.Create(game);
        }

        public async Task<List<SteamAppDto>> ListGames()
        {
            log.Debug("Getting games list.");
            return await _gamesRepository.ListGames();
        }

        public async Task Delete(long appId)
        {
            log.Info("Removing game: " + appId.ToString());

            await _gamesRepository.Delete(appId);
            _notesRepository.Delete(appId);
        }

        public async Task SaveNotes(long appId, string note)
        {
            log.Info("Saving notes for game: " + appId.ToString());

            await _notesRepository.SaveNotes(appId, note);
        }
        public async Task<string> GetNotes(long appId)
        {
            log.Debug("Getting notes for game: " + appId.ToString());
            return await _notesRepository.GetNotes(appId);
        }
    }
}
