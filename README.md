#Spike#
## Independent Ninject Configuration Between Tests ##

##Quick Start##
Look at the tests in the folder TestsWithSingletonKernelContainer.


##Purpose##
The goal of this spike is to find a way to share a Ninject kernel between unit tests whilst allow each test to redefine how object graphs are created.
The most common example of this is a controller dependent on a business logic which is dependent on a repository.
For a given unit test we want to return a mock of that repository instead of the concrete implementation.
That mock repository should only exist for the life of the unit test.

##Approaches##
3 approaches were tried.
* Ninject child containers.
* Structuremap Nested containers.
* Ninject Named Bindings.


### Ninject child containers ###
You will find those unit tests in the class: NinjectChildContainerTests.
4 of the unit tests fail which indicate this method is not suitable.

### Structuremap Nested containers ###
You will find those unit tests in the class: StructmapContainerTests.
All tests pass indicating the Structuremap is suitable for this task however would require migration from Ninject to Structuremap which will be a long progress.

### Named Bindings  ###
You will find those unit tests in the class: WhenInCurrentTestTests and the folder TestsWithSingletonKernelContainer.
The downside to this approach is that it is dependent on TestContext.CurrentContext.Test.FullName which is a nUnit concept.  
There does not appear to be a XUnit equivalent.

### TODO ###
*Test parallel execution using NUnit command line, probably in a tight loop to look for multithreaded issues.
*Example for StoryQ



