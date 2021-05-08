using Newtonsoft.Json;
using System;

namespace RaceData.CustomData
{
    public class Stage
    {
        [JsonProperty("id")]

        public Guid Id { get; set; }

        [JsonProperty("name")]

        public string Name { get; set; }

        [JsonProperty("from")]

        public string From { get; set; }

        [JsonProperty("to")]

        public string To { get; set; }

        [JsonProperty("distance")]

        public int Distance { get; set; }
    }
}
