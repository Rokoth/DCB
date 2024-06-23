
using DCB.DB.Attributes;
using System;

namespace DCB.DB.Model
{
    [TableName("characteristics")]
    public class Characteristics : Entity
    {
        [ColumnName("personid")]
        public Guid PersonId { get; set; }
    }
}