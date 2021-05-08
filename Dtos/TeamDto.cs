using NPoco;
using System;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace RaceData.CustomData
{
    [TableName("appTeams")]
    [PrimaryKey("Id", AutoIncrement = false)]
    public class TeamDto
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
