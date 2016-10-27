using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NinjectChildContainers.BusinessLogic;
using NinjectChildContainers.Repository;

namespace NinjectChildContainers.SingletonKernel
{
    /// <summary>
    /// I contain a singleton reference of the IKernel used for unit testing. 
    /// </summary>
    public static class GlobalKernelContainer
    {
        private static readonly Lazy<IKernel> _lazyKernel = new Lazy<IKernel>(() => CreateKernel());

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IMyRepository>().To<MyRepository>().InTransientScope();
            kernel.Bind<IMyBusinessLogic>().To<MyBusinessLogic>().InTransientScope();

            return kernel;

        }

       
        public static IKernel GlobalKernel => _lazyKernel.Value;
        
    }
}
