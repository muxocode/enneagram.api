using System;
using System.Collections.Generic;

namespace entities
{
    public partial class Language: EntityBase<short>
    {
        public Language()
        {
            Translations = new List<Translation>();
        }
        public string KeyName { get; set; }
        public virtual ICollection<Translation> Translations { get; set; }
    }
}
