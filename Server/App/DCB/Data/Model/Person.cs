
using DCB.DB.Attributes;
using System;

namespace DCB.DB.Model
{
    [TableName("person")]
    public class Person : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }
        [ColumnName("description")]
        public string Description { get; set; }

        [ColumnName("userid")]
        public Guid UserId { get; set; }

        [ColumnName("locationid")]
        public Guid LocationId { get; set; }
    }
}