using System;
using Contract.Interface;

namespace Contract.Model
{
    public class Entity : IEntity
    {
        public Guid Id { get; set; }
    }
}
