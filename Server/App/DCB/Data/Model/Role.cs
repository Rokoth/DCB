
using DCB.DB.Attributes;

namespace DCB.DB.Model
{
    [TableName("role")]
    public class Role : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }
        [ColumnName("description")]
        public string Description { get; set; }
        [ColumnName("isadmin")]
        public bool IsAdmin { get; set; }
    }
}