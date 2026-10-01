using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using Moq;

namespace MTAoarsGeneral.Tests.TestHelpers
{
    /// <summary>
    /// Helper class for creating mock ObjectSets for testing Entity Framework repositories
    /// </summary>
    public static class MockObjectSetHelper
    {
        /// <summary>
        /// Creates a mock ObjectSet that can be used with LINQ queries
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="data">The test data to populate the mock set with</param>
        /// <returns>A mocked ObjectSet configured for testing</returns>
        public static Mock<ObjectSet<T>> CreateMockObjectSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<ObjectSet<T>>();

            // Setup IQueryable interface for LINQ support
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            // Setup AddObject to add to the list
            mockSet.Setup(m => m.AddObject(It.IsAny<T>())).Callback<T>(entity => data.Add(entity));

            // Setup DeleteObject to remove from the list
            mockSet.Setup(m => m.DeleteObject(It.IsAny<T>())).Callback<T>(entity => data.Remove(entity));

            // Setup Attach to add to the list (similar to AddObject for testing purposes)
            mockSet.Setup(m => m.Attach(It.IsAny<T>())).Callback<T>(entity =>
            {
                if (!data.Contains(entity))
                {
                    data.Add(entity);
                }
            });

            return mockSet;
        }

        /// <summary>
        /// Creates an empty mock ObjectSet
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <returns>A mocked ObjectSet with no data</returns>
        public static Mock<ObjectSet<T>> CreateMockObjectSet<T>() where T : class
        {
            return CreateMockObjectSet(new List<T>());
        }
    }
}

