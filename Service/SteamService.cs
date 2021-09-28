using log4net;
using SteamGameNotes.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamGameNotes.Service
{
    public class SteamService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SteamService));

        private const string STEAM_API_URL = "https://api.steampowered.com/";
        private const string CACHE_FILENAME = "steamapps.json";
        private string CACHE_DIRECTORY = Path.GetTempPath() + "SteamGameNotes";

        private HttpClient _httpClient;

        public SteamService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<SteamAppDto> GetSteamApp(string name)
        {
            await CreateSteamAppsCache();
            var appList = await _fetchCachedSteamApps();

            return appList.Find((app) =>
            {
                return app.name.ToLower().Equals(name.ToLower());
            });
        }

        public async Task CreateSteamAppsCache()
        {
            if (!_isSteamAppsCached())
            {
                log.Info("Steam apps not cached");
                await _createSteamAppsCache();
            }
        }

        public void InvalidateCache()
        {
            log.Info("Removing steam apps cache file");
            File.Delete(Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME));
        }

        private async Task<List<SteamAppDto>> _fetchCachedSteamApps()
        {
            log.Debug("Reading cached games from " + Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME));

            using FileStream jsonStream = File.OpenRead(Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME));

            var cache = await JsonSerializer.DeserializeAsync<GetAppListResponseDto>(jsonStream);

            return cache.applist.apps;
        }

        private async Task _createSteamAppsCache()
        {
            string path = "ISteamApps/GetAppList/v2/";
            _httpClient.BaseAddress = new Uri(STEAM_API_URL);

            log.Info("Creating steam apps cache. Enpoint url: " + STEAM_API_URL + path);

            string appsJson = null;
            HttpResponseMessage response = await _httpClient.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                appsJson = await response.Content.ReadAsStringAsync();

                log.Info("Writing cache file to " + Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME));

                Directory.CreateDirectory(CACHE_DIRECTORY);
                await File.WriteAllTextAsync(Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME), appsJson);
            }
            else
            {
                log.Warn("Unsuccessful response from steam api: " + response.StatusCode + " " + response.ReasonPhrase);
            }
        }

        private bool _isSteamAppsCached ()
        {
            return File.Exists(Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME));
        }
    }
}
