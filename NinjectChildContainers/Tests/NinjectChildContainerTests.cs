using 
    System.Reflection;
using Moq;
using Ninject;
using Ninject.Extensions.ChildKernel;
using NinjectChildContainers.BusinessLogic;
using NinjectChildContainers.Repository;
using Xunit;

namespace NinjectChildContainers.Tests
{
    public static class NinjectChildContainerTests
    {
        [Fact]
        public static void CanChildContainerRequestRepositoryFromParentContainer()
        {
            /*
             * Repository in the parent kernel.
             * Business Logic in the child kernel.
             * We expect Ninject to ask the parent kernel for the repository
           */

            //Arrange
            var kernel = new StandardKernel();

            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock");

            kernel.Bind<IMyRepository>().ToConstant(myRepository.Object);

            var childKernel = new ChildKernel(kernel);
            kernel.Bind<IMyBusinessLogic>().To<MyBusinessLogic>();

            //Act
            var myBusinessLogic = childKernel.Get<IMyBusinessLogic>();

            //Assert

            Assert.Contains("I came from a mock and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));
        }


        [Fact]
        public static void CanChildRedefineRepositoryInChildWithoutAffectingParent()
        {
            /*
            * Define repository & BL in the parent kernel.
            * Redefine in the child. 
            */

            //Arrange
            var kernel = new StandardKernel();
            kernel.Bind<IMyRepository>().To<MyRepository>().InTransientScope();
            kernel.Bind<IMyBusinessLogic>().To<MyBusinessLogic>().InTransientScope();

            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock");

            var childKernel = new ChildKernel(kernel);
            childKernel.Bind<IMyRepository>().ToConstant(myRepository.Object).InTransientScope();
          
            //Act
            var myBusinessLogicFromTheChild = childKernel.Get<IMyBusinessLogic>();
            var myBusinessLogicFromTheParent = kernel.Get<IMyBusinessLogic>();

            //Assert

            Assert.Equal("I came from a mock and then the business layer returned it.",
                myBusinessLogicFromTheChild.DoSomething("Test"));

            Assert.Equal("You got: Test from myRepository and then the business layer returned it.",
                myBusinessLogicFromTheParent.DoSomething("Test"));
        }


        [Fact]
        public static void CanChildSupplyRepositoryInstanceToParent()
        {
            /*
            * Repository in the child kernel.
            * Business Logic in the parent kernel.
            * We expect Ninject to construct the BL in the parent with the child kernels
            * repository
            */

            //Arrange
            var kernel = new StandardKernel();
            kernel.Bind<IMyBusinessLogic>().To<MyBusinessLogic>().InTransientScope();

            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock");

            var childKernel = new ChildKernel(kernel);
            childKernel.Bind<IMyRepository>().ToConstant(myRepository.Object).InSingletonScope();
        
            //Act
            var myBusinessLogic = childKernel.Get<IMyBusinessLogic>();
          
            //Assert
            Assert.Equal("I came from a mock and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));
        }

        [Fact]
        public static void CanSecondChildSupplyDifferentInstanceOfBusinessLogicWithCorrectRepository()
        {
            /*
            * Repository in the child kernel.
            * Business Logic in the parent kernel.
            * Dispose the child kernel and try it again!
            */

            //Arrange
            var kernel = new StandardKernel();
            kernel.Bind<IMyRepository>().To<MyRepository>();
            kernel.Bind<IMyBusinessLogic>().To<MyBusinessLogic>().InTransientScope();

            var myRepository1 = new Mock<IMyRepository>();
            myRepository1.Setup(x => x.Get("Test")).Returns("I came from a mock");

            var childKernel1 = new ChildKernel(kernel);
            childKernel1.Bind<IMyRepository>().ToConstant(myRepository1.Object).InSingletonScope();
            
            var businessLogic1 = childKernel1.Get<IMyBusinessLogic>();

            childKernel1.Dispose();

            var myRepository2 = new Mock<IMyRepository>();
            myRepository2.Setup(x => x.Get("Test")).Returns("I came from a mock2");

            var childKernel2 = new ChildKernel(kernel);
            childKernel1.Bind<IMyRepository>().ToConstant(myRepository2.Object).InSingletonScope();
            
            //Act
            var businessLogic2 = childKernel2.Get<IMyBusinessLogic>();

            //Assert
            Assert.NotEqual(businessLogic1, businessLogic2);

            Assert.Equal("I came from a mock and then the business layer returned it.",
            businessLogic1.DoSomething("Test"));

            Assert.Equal("I came from a mock and then the business layer returned it.",
               businessLogic2.DoSomething("Test"));

        }


        [Fact]
        public static void CanNinjectConstructInstanceWithDependencies()
        {
            //Arrange
            var kernel = new StandardKernel();
            //TODO: some sort of policy here to return concrete instances from interfaces
            kernel.Bind<IMyRepository>().To<MyRepository>();
            kernel.Bind<IMyBusinessLogic>().To<MyBusinessLogic>();
         
            //Act
            var myBusinessLogic = kernel.Get<IMyBusinessLogic>();

            //Assert
            Assert.Equal("You got: Test from myRepository and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));
        }
    }
}