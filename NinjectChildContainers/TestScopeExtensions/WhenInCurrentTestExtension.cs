using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NinjectChildContainers.TestScope
{
    public static class WhenInCurrentTestExtension
    {
        public static IBindingInNamedWithOrOnSyntax<T> WhenInCurrentTest<T>(this IBindingWhenSyntax<T> binding)
        {
            var testName = TestContext.CurrentContext.Test.FullName;
            return binding.When(w => TestContext.CurrentContext.Test.FullName == testName);
        }   
    }
   
}
