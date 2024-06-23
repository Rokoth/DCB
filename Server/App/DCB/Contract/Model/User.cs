using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Model
{
    public class User : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public Role Role { get; set; }

        public List<Person> Persons { get; set; }
    }
}
