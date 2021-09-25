using SteamGameNotes.DTO;
using System.Collections.Generic;

namespace SteamGameNotes.Service
{
    public class GetAppListResponseDto
    {
        public AppList applist { get; set; }
    }

    public class AppList
    {
        public List<SteamAppDto> apps { get; set; }
    }
}
