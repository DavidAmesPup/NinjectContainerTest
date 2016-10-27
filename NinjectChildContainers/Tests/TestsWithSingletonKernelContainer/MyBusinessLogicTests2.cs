using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ninject;
using NinjectChildContainers.BusinessLogic;
using NinjectChildContainers.Repository;
using NinjectChildContainers.SingletonKernel;
using NinjectChildContainers.TestScope;
using NUnit.Framework;

using static  NinjectChildContainers.SingletonKernel.GlobalKernelContainer;
namespace NinjectChildContainers.Tests.TestsWithSingletonKernelContainer
{
    [TestFixture]
    public class MyBusinessLogicTests2
    {
        [Test]
        public void CanAMockRepositoryBeReturnedWhenAskingForBusinessLogicTest1_1()
        {
            
            //Arrange
            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock - Test 1");
            GlobalKernel.Bind<IMyRepository>().ToConstant(myRepository.Object).WhenInCurrentTest();

            //Act
            var myBusinessLogic = GlobalKernel.Get<IMyBusinessLogic>();

            //Assert
            Assert.AreEqual("I came from a mock - Test 1 and then the business layer returned it.",
                    myBusinessLogic.DoSomething("Test"));

        }


        [Test]
        public void CanAMockRepositoryBeReturnedWhenAskingForBusinessLogicTest1_2()
        {

            //Arrange
            var myRepository = new Mock<IMyRepository>();
            myRepository.Setup(x => x.Get("Test")).Returns("I came from a mock - Test 2");
            GlobalKernel.Bind<IMyRepository>().ToConstant(myRepository.Object).WhenInCurrentTest();

            //Act
            var myBusinessLogic = GlobalKernel.Get<IMyBusinessLogic>();

            //Assert
            Assert.AreEqual("I came from a mock - Test 2 and then the business layer returned it.",
                    myBusinessLogic.DoSomething("Test"));

        }





    }
}
