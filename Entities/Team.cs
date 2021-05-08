using Newtonsoft.Json;
using System;

namespace RaceData.CustomData
{
    public class Team
    {
        [JsonProperty("id")]

        public Guid Id { get; set; }

        [JsonProperty("name")]

        public string Name { get; set; }
    }
}
