using System;
using System.IO;
using System.Threading.Tasks;

namespace SteamGameNotes.Persistence
{
    public class NotesRepository
    {
        private const string NOTE_FILENAME = "content.txt";
        private string BASE_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SteamGameNotes\\notes";

        public async Task SaveNotes(long appId, string note)
        {
            Directory.CreateDirectory(Path.Combine(BASE_DIRECTORY, appId.ToString()));
            await File.WriteAllTextAsync(Path.Combine(BASE_DIRECTORY, appId.ToString(), NOTE_FILENAME), note);
        }

        public async Task<string> GetNotes(long appId)
        {
            Directory.CreateDirectory(Path.Combine(BASE_DIRECTORY, appId.ToString()));
            return await File.ReadAllTextAsync(Path.Combine(BASE_DIRECTORY, appId.ToString(), NOTE_FILENAME));
        }

        public void Delete(long appId)
        {
            File.Delete(Path.Combine(BASE_DIRECTORY, appId.ToString(), NOTE_FILENAME));
        }
    }
}
