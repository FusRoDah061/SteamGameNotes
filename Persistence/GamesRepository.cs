using log4net;
using SteamGameNotes.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamGameNotes.Persistence
{
    public class GamesRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GamesRepository));

        private const string JSON_NAME = "games.json";
        private string BASE_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SteamGameNotes";

        public GamesRepository()
        {
            Directory.CreateDirectory(BASE_DIRECTORY);

            log.Info("Games will be saved to " + Path.Combine(BASE_DIRECTORY, JSON_NAME));
        }

        public async Task Create (SteamAppDto game)
        {
            var games = await ListGames();
            games.Add(game);

            _writeFile(games);
        }

        public async Task<List<SteamAppDto>> ListGames()
        {
            if (File.Exists(Path.Combine(BASE_DIRECTORY, JSON_NAME)))
            {
                using FileStream jsonStream = File.OpenRead(Path.Combine(BASE_DIRECTORY, JSON_NAME));
               return await JsonSerializer.DeserializeAsync<List<SteamAppDto>>(jsonStream);
            }
            else
            {
                log.Debug("Games json does not exist");
            }

            return new List<SteamAppDto>();
        }

        public async Task Delete(long appId)
        {
            var games = await ListGames();
            games.RemoveAll((game) => { return game.appid == appId; });

            _writeFile(games);
        }

        private void _writeFile(List<SteamAppDto> games)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(games, options);
            File.WriteAllBytes(Path.Combine(BASE_DIRECTORY, JSON_NAME), jsonBytes);
        }
    }
}
