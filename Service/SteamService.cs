using SteamGameNotes.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamGameNotes.Service
{
    class SteamService
    {
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
                await _createSteamAppsCache();
            }
        }

        public void InvalidateCache()
        {
            File.Delete(Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME));
        }

        private async Task<List<SteamAppDto>> _fetchCachedSteamApps()
        {
            using FileStream jsonStream = File.OpenRead(Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME));

            var cache = await JsonSerializer.DeserializeAsync<GetAppListResponseDto>(jsonStream);

            return cache.applist.apps;
        }

        private async Task _createSteamAppsCache()
        {
            string path = "ISteamApps/GetAppList/v2/";
            _httpClient.BaseAddress = new Uri(STEAM_API_URL);

            string appsJson = null;
            HttpResponseMessage response = await _httpClient.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                appsJson = await response.Content.ReadAsStringAsync();
            }

            Directory.CreateDirectory(CACHE_DIRECTORY);
            await File.WriteAllTextAsync(Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME), appsJson);
        }

        private bool _isSteamAppsCached ()
        {
            return File.Exists(Path.Combine(CACHE_DIRECTORY, CACHE_FILENAME));
        }
    }
}
