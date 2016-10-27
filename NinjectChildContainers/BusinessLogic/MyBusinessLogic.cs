using NinjectChildContainers.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjectChildContainers.BusinessLogic
{
    public class MyBusinessLogic : IMyBusinessLogic
    {
        public MyBusinessLogic(IMyRepository myRepository)
        {
            _MyRepository = myRepository;

        }
        private readonly IMyRepository _MyRepository;

        public string DoSomething(string what)
        {
            return $"{_MyRepository.Get(what)} and then the business layer returned it.";
        }
    }
}
