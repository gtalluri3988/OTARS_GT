using System;
using System.Data.Objects;
using MTAoarsGeneral.DomainModels;
using Moq;

namespace MTAoarsGeneral.Tests.TestHelpers
{
    /// <summary>
    /// Test helper class for creating mock EntityContext instances for testing
    /// </summary>
    public static class MockContextHelper
    {
        /// <summary>
        /// Creates a mock EntityContext for testing
        /// This is useful when you need to mock the context without setting up specific ObjectSets
        /// </summary>
        /// <returns>A mocked EntityContext</returns>
        public static Mock<EntityContext> CreateMockContext()
        {
            var mockContext = new Mock<EntityContext>();
            
            // Setup default behavior for common operations
            mockContext.Setup(c => c.SaveChanges(It.IsAny<SaveOptions>())).Returns(1);
            mockContext.Setup(c => c.AcceptAllChanges()).Verifiable();
            
            return mockContext;
        }

        /// <summary>
        /// Creates a mock EntityContext with CommandTimeout property support
        /// </summary>
        /// <returns>A mocked EntityContext with timeout configuration</returns>
        public static Mock<EntityContext> CreateMockContextWithTimeout()
        {
            var mockContext = CreateMockContext();
            
            // Setup CommandTimeout property
            mockContext.SetupProperty(c => c.CommandTimeout);
            
            return mockContext;
        }

        /// <summary>
        /// Helper method to verify that SaveChanges was called on the mock context
        /// </summary>
        /// <param name="mockContext">The mock context to verify</param>
        /// <param name="times">Expected number of times SaveChanges should be called (default: once)</param>
        public static void VerifySaveChanges(Mock<EntityContext> mockContext, Times? times = null)
        {
            mockContext.Verify(c => c.SaveChanges(It.IsAny<SaveOptions>()), times ?? Times.Once());
        }

        /// <summary>
        /// Helper method to verify that AcceptAllChanges was called on the mock context
        /// </summary>
        /// <param name="mockContext">The mock context to verify</param>
        /// <param name="times">Expected number of times AcceptAllChanges should be called (default: once)</param>
        public static void VerifyAcceptAllChanges(Mock<EntityContext> mockContext, Times? times = null)
        {
            mockContext.Verify(c => c.AcceptAllChanges(), times ?? Times.Once());
        }
    }
}

