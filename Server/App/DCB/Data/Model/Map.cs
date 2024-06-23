
using DCB.DB.Attributes;

namespace DCB.DB.Model
{
    [TableName("map")]
    public class Map : Entity
    {
        public int Level { get; set; }
        public int Y { get; set; }
        public int X { get; set; }
    }
}