using System;
using System.Collections.Generic;
using System.Text;

namespace entities
{
    public class Triad: EntityTaslationBase<short>
    {
        public Triad()
        {
            this.Enneatypes = new List<Enneatype>();
        }
        public ICollection<Enneatype> Enneatypes { get; set; }
    }
}
