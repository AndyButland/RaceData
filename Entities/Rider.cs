using Newtonsoft.Json;
using System;

namespace RaceData.CustomData
{
    public class Rider
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]

        public string Name { get; set; }

        [JsonProperty("teame")]

        public Team Team { get; set; }
    }
}   
