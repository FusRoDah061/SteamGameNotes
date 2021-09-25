using System;

namespace SteamGameNotes.DTO
{
    public class SteamAppDto
    {

        public long appid { get; set; }
        public string name { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SteamAppDto dto &&
                   appid == dto.appid &&
                   name == dto.name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(appid, name);
        }
    }
}
