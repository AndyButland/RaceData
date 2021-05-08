using NPoco;
using System;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace RaceData.CustomData
{
    [TableName("appRiders")]
    [PrimaryKey("Id", AutoIncrement = false)]
    public class RiderDto
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid TeamId { get; set; }
    }
}
