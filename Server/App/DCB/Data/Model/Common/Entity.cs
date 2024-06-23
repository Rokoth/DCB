using DCB.DB.Attributes;
using System;

namespace DCB.DB.Model.Common
{
    public class Entity
    {
        [ColumnName("id")]
        public Guid Id { get; set; }
        public DateTimeOffset VersionDate { get; set; }
    }
}