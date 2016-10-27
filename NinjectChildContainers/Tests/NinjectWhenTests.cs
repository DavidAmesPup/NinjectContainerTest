using Moq;
using Ninject;
using Ninject.Extensions.ChildKernel;
using NinjectChildContainers.BusinessLogic;
using NinjectChildContainers.Repository;
using Xunit;

namespace NinjectChildContainers.Tests
{
    public class NinjectWhenTests
    {
        private class ByValueBooleanWrapper
        {
            private bool _flag;

            public ByValueBooleanWrapper(bool flag)
            {
                Flag = flag;
            }

            // ReSharper disable once ConvertToAutoProperty
            public bool Flag
            {
                get { return _flag; }
                set { _flag = value; }
            }
        }

        [Fact]
        public static void CanWhenTakePrecidenceForBusinessLogic()
        {
            //Arrange
            var kernel = new StandardKernel();
            kernel.Bind<IMyRepository>().To<MyRepository>().InTransientScope();
            kernel.Bind<IMyBusinessLogic>().To<MyBusinessLogic>().InTransientScope();

            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock");

            var shouldReturnTestInstance = new ByValueBooleanWrapper(true);
       
            kernel.Bind<IMyRepository>().ToConstant(myRepository.Object).When(x => shouldReturnTestInstance.Flag);

            //Act
            var myBusinessLogic = kernel.Get<IMyBusinessLogic>();

            //Assert
            Assert.Equal("I came from a mock and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));

            shouldReturnTestInstance.Flag = false;

            myBusinessLogic = kernel.Get<IMyBusinessLogic>();

            Assert.Equal("You got: Test from myRepository and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));
        }


        [Fact]
        public static void CanWhenTakePrecidenceForBusinessLogicWhenUsedWithChildKernel()
        {
            //Arrange
            var kernel = new StandardKernel();
            kernel.Bind<IMyRepository>().To<MyRepository>().InTransientScope();
            kernel.Bind<IMyBusinessLogic>().To<MyBusinessLogic>().InTransientScope();

            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock");

            var shouldReturnTestInstance = new ByValueBooleanWrapper(true);

            var childKernel = new ChildKernel(kernel);
            childKernel.Bind<IMyRepository>().ToConstant(myRepository.Object).When(x => shouldReturnTestInstance.Flag);

            //Act
            var myBusinessLogic = childKernel.Get<IMyBusinessLogic>();

            //Assert
            Assert.Equal("I came from a mock and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));

            shouldReturnTestInstance.Flag = false;

            myBusinessLogic = kernel.Get<IMyBusinessLogic>();

            Assert.Equal("You got: Test from myRepository and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));
        }
    }
}