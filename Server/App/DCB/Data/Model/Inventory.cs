
using DCB.DB.Attributes;
using System;

namespace DCB.DB.Model
{
    [TableName("inventory")]
    public class Inventory : Entity
    {
        [ColumnName("personid")]
        public Guid PersonId { get; set; }
    }
}