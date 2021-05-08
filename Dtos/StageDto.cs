using NPoco;
using System;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace RaceData.CustomData
{
    [TableName("appStages")]
    [PrimaryKey("Id", AutoIncrement = false)]
    public class StageDto
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public int Distance { get; set; }
    }
}
