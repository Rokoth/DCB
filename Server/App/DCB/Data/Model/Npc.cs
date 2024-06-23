
using DCB.DB.Attributes;
using System;

namespace DCB.DB.Model
{
    [TableName("npc")]
    public class Npc : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }
        [ColumnName("description")]
        public string Description { get; set; }

        [ColumnName("npctypeid")]
        public Guid NpcTypeId { get; set; }

        [ColumnName("locationid")]
        public Guid LocationId { get; set; }
    }
}