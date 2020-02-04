using entities;
using mxcd.core.services;
using mxcd.core.unitOfWork;
using System.Collections.Generic;
using System.Linq;

namespace controllers
{
    public class EnneatypeController : EnneaControllerBase<Enneatype, short>
    {
        public EnneatypeController(IService<Enneatype> Service, IErrorHandler errorHandler, IUnitOfWork unitOfWork) :base(Service, unitOfWork, errorHandler)
        {

        }

        public override IEnumerable<Enneatype> GetById(IEnumerable<Enneatype> set, short key)
        {
            return set.Where(x => x.Id == key);
        }
    }
}
