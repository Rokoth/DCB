
using DCB.DB.Attributes;
using System;

namespace DCB.DB.Model
{
    [TableName("location")]
    public class Location : Entity
    {
        public Guid MapId { get; set; }
        public bool IsEnter { get; set; }
        public int Level { get; set; }
        public int Y { get; set; }
        public int X { get; set; }
    }
}