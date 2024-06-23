using System;

namespace Contract.Model
{
    public class Person : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }
                
        public Inventory Inventory { get; set; }

        public Characteristics Characteristics { get; set; }

        public Location Location { get; set; }

        public User User { get; set; }

        public DateTime? LastEnterDate { get; set; }

        public DateTime CreateDate { get; set;}

    }
}
