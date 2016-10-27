using Moq;
using Ninject;
using NinjectChildContainers.BusinessLogic;
using NinjectChildContainers.Repository;
using NinjectChildContainers.TestScope;
using NUnit.Framework;

namespace NinjectChildContainers.Tests
{
    [TestFixture]
    public class WhenInCurrentTestTests
    {
        private IKernel _Kernel;

        [OneTimeSetUp]
        public void SetupKernel()
        {
            _Kernel = new StandardKernel();
            _Kernel.Bind<IMyRepository>().To<MyRepository>().InTransientScope();
            _Kernel.Bind<IMyBusinessLogic>().To<MyBusinessLogic>().InTransientScope();
        }

        [OneTimeTearDown]
        public void DisposeKernel()
        {
            _Kernel?.Dispose();
        }

        [Test]
        public void CanGetItemsFromGlobalKernel()
        {
            //Arrange

            //Act
            var myBusinessLogic = _Kernel.Get<IMyBusinessLogic>();

            //Assert
            Assert.AreEqual("You got: Test from myRepository and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));
        }

        [Test]
        public void CanOverrideRepositoryInIndividualTest()
        {
            //Arrange
            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock");
            _Kernel.Bind<IMyRepository>().ToConstant(myRepository.Object).WhenInCurrentTest();

            //Act
            var myBusinessLogic = _Kernel.Get<IMyBusinessLogic>();

            //Assert
            Assert.AreEqual("I came from a mock and then the business layer returned it.",
                myBusinessLogic.DoSomething("Test"));
        }

    }
}