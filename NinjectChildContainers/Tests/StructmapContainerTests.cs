using Moq;
using NinjectChildContainers.BusinessLogic;
using NinjectChildContainers.Repository;
using StructureMap;
using Xunit;

namespace NinjectChildContainers.Tests
{
    public static class ContainerTests
    {
        [Fact]
        public static void CanChildContainerRequestInsanceFromParentContainer()
        {
            /*
             * Repository in the parent kernel.
             * Business Logic in the child kernel.
             * We expect Ninject to ask the parent kernel for the repository
           */

            //Arrange

            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock");


            var container = new Container(x => { x.For<IMyRepository>().Use(myRepository.Object); });

            using (var nestedContainer = container.GetNestedContainer())
            {
                nestedContainer.Configure(x => x.For<IMyBusinessLogic>().Use<MyBusinessLogic>());

                //Act

                var myBusinessLogic = nestedContainer.GetInstance<IMyBusinessLogic>();

                //Assert
                Assert.Contains("I came from a mock and then the business layer returned it.",
                    myBusinessLogic.DoSomething("Test"));
            }
        }


        [Fact]
        public static void CanChildRedefineRepositoryInChildWithoutAffectingParent()
        {
            /*
            * Define repository & BL in the parent kernel.
            * Redefine in the child. 
            */

            //Arrange
            var container = new Container(x => x.Scan(s =>
            {
                s.TheCallingAssembly();
                s.WithDefaultConventions();
            }));

            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock");

            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Configure(x => x.For<IMyRepository>().Use(myRepository.Object));

            //Act
            var myBusinessLogicFromTheChild = nestedContainer.GetInstance<IMyBusinessLogic>();
            var myBusinessLogicFromTheParent = container.GetInstance<IMyBusinessLogic>();

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
            var container = new Container(x => { x.For<IMyBusinessLogic>().Use<MyBusinessLogic>(); });
            
            using (var nestedContainer = container.GetNestedContainer())
            {
                var myRepository = new Mock<IMyRepository>();
                myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock");

                nestedContainer.Configure(x => x.For<IMyRepository>().Use(myRepository.Object));
                
                //Act
                var myBusinessLogic = nestedContainer.GetInstance<IMyBusinessLogic>();

                //Assert

                Assert.Equal("I came from a mock and then the business layer returned it.",
                    myBusinessLogic.DoSomething("Test"));

            }
            
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

            var container = new Container(x =>
            {
                x.For<IMyBusinessLogic>().Use<MyBusinessLogic>();
                x.For<IMyRepository>().Use<MyRepository>();
            });



            var myRepository1 = new Mock<IMyRepository>();
            myRepository1.Setup(x => x.Get("Test")).Returns("I came from a mock");

            var myRepository2 = new Mock<IMyRepository>();
            myRepository2.Setup(x => x.Get("Test")).Returns("I came from a mock2");

            var nestedContainer1 = container.GetNestedContainer();
            var nestedContainer2 = container.GetNestedContainer();

            nestedContainer1.Configure(x => x.For<IMyRepository>().Use(myRepository1.Object));
            nestedContainer2.Configure(x => x.For<IMyRepository>().Use(myRepository2.Object));
            

            //Act
            var businessLogic1 = nestedContainer1.GetInstance<IMyBusinessLogic>();
            var businessLogic2 = nestedContainer2.GetInstance<IMyBusinessLogic>();

            //Assert
            Assert.NotEqual(businessLogic1, businessLogic2);

            Assert.Equal("I came from a mock and then the business layer returned it.",
               businessLogic1.DoSomething("Test"));

            Assert.Equal("I came from a mock2 and then the business layer returned it.",
               businessLogic2.DoSomething("Test"));

        }


        [Fact]
        public static void CanConstructInstanceWithDependencies()
        {
            //Arrange
            var container = new Container(x => x.Scan(s =>
            {
                s.TheCallingAssembly();
                s.WithDefaultConventions();
            }));

            //Act
            var myBusinessLogic = container.GetInstance<IMyBusinessLogic>();

            //Assert
            Assert.Equal("You got: Test from myRepository and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));
        }
    }
}