using mxcd.core.entities;
using System;
using System.Collections.Generic;

namespace entities
{
    public partial class Translation : EntityBase<Guid>, IKeyValue
    {
        public string KeyName { get; set; }
        public string Value { get; set; }
        public short LanguageId { get; set; }
        public Language Language { get; set; }
    }
}
