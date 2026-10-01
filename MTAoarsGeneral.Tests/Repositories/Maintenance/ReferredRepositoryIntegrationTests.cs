using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MTAoarsGeneral.Repositories.Maintenance;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Tests.Repositories.Maintenance
{
    /// <summary>
    /// Integration tests for ReferredRepository using a real database connection
    /// These tests require the MTAOARS database to be accessible
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    public class ReferredRepositoryIntegrationTests
    {
        private EntityContext _context;
        private ReferredRepository _repository;
        private List<ReferredHeader> _testHeaders;
        private List<ReferredDetail> _testDetails;

        [SetUp]
        public void SetUp()
        {
            // Create a real EntityContext connected to the database
            _context = new EntityContext();
            _repository = new ReferredRepository(_context);

            // Track test data for cleanup
            _testHeaders = new List<ReferredHeader>();
            _testDetails = new List<ReferredDetail>();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up any test data created during the test
            try
            {
                foreach (var detail in _testDetails)
                {
                    var existing = _context.ReferredDetails.FirstOrDefault(d => d.ID == detail.ID);
                    if (existing != null)
                    {
                        _context.ReferredDetails.DeleteObject(existing);
                    }
                }

                foreach (var header in _testHeaders)
                {
                    var existing = _context.ReferredHeaders.FirstOrDefault(h => h.ID == header.ID);
                    if (existing != null)
                    {
                        _context.ReferredHeaders.DeleteObject(existing);
                    }
                }

                _context.SaveChanges();
            }
            catch
            {
                // Ignore cleanup errors
            }
            finally
            {
                if (_context != null)
                {
                    _context.Dispose();
                }
                _repository = null;
                _testHeaders = null;
                _testDetails = null;
            }
        }

        #region Helper Methods

        private string GenerateUniqueICNumber()
        {
            return $"TEST{DateTime.Now.Ticks.ToString().Substring(0, 8)}";
        }

        #endregion

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidContext_ShouldCreateInstance()
        {
            // Arrange & Act
            var context = new EntityContext();
            var repository = new ReferredRepository(context);

            // Assert
            Assert.IsNotNull(repository);
            Assert.IsInstanceOf<ReferredRepository>(repository);

            // Cleanup
            context.Dispose();
        }

        [Test]
        public void Constructor_WithNullContext_ShouldThrowNullReferenceException()
        {
            // Arrange, Act & Assert
            Assert.Throws<NullReferenceException>(() => new ReferredRepository(null));
        }

        #endregion

        #region GetDetail Tests



        [Test]
        public void GetDetail_WithNonExistingICNumber_ShouldReturnNull()
        {
            // Arrange
            var nonExistingIcNumber = "NONEXIST999999";

            // Act
            var result = _repository.GetDetail(nonExistingIcNumber);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetDetail_WithEmptyICNumber_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetDetail(string.Empty);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetDetail_WithNullICNumber_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetDetail(null);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetDetailByCompany Tests



        [Test]
        public void GetDetailByCompany_WithNonExistingICNumber_ShouldReturnNull()
        {
            // Arrange - use an existing header if possible, or create one
            var headers = _context.ReferredHeaders.Take(1).ToList();
            if (headers.Count == 0)
            {
                // Create a test header if none exist
                var header = ReferredHeader.CreateReferredHeader(0, 1, 1, true, new byte[] { 1 });
                header.CreatedBy = 1; // User ID
                header.CreatedDate = DateTime.Now;
                _context.ReferredHeaders.AddObject(header);
                _context.SaveChanges();
                _testHeaders.Add(header);
                headers.Add(header);
            }

            var testHeaderId = headers[0].ID;
            var nonExistingIcNumber = "NONEXIST999999";

            // Act
            var result = _repository.GetDetailByCompany(nonExistingIcNumber, testHeaderId, 1);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Database Connectivity Tests

        [Test]
        public void Repository_ShouldConnectToDatabase()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Try to query the database
                var count = _context.ReferredDetails.Count();
                Assert.GreaterOrEqual(count, 0);
            });
        }

        [Test]
        public void GetDetail_ShouldExecuteQueryAgainstRealDatabase()
        {
            // Arrange
            var testIcNumber = GenerateUniqueICNumber();

            // Act - query should execute without error even if no results
            var result = _repository.GetDetail(testIcNumber);

            // Assert - either null or a result is fine, just verify it executes
            Assert.That(result == null || result is ReferredDetail);
        }

        #endregion
    }
}
