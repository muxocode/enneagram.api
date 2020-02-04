using mxcd.core.entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace entities
{
    public class Configuration : EntityBase<Guid>, IKeyValue
    {
        public string KeyName { get; set; }
        public string Value { get; set; }
    }
}
