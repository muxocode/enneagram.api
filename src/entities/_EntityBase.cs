
using System;

namespace entities
{
    public abstract class EntityBase
    {
        public abstract object GetKey();
    }
    public abstract class EntityBase<T>
    {
        public T Id { get; set; }
        public object GetKey()
        {
            return Id;
        }
    }

    public abstract class EntityTaslationBase<T>: EntityBase<T>
    {
        public string KeyName { get; set; }
    }
}
