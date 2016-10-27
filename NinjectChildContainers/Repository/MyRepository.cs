using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjectChildContainers.Repository
{
    public class MyRepository : IMyRepository
    {
        public string Get(string what)
        {
            return $"You got: {what} from myRepository";
        }
    }
}
