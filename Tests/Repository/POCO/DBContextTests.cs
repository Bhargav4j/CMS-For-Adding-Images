using Xunit;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using Repository.POCO;

namespace Tests.Repository.POCO
{
    public class DBContextTests
    {
        [Fact]
        public void DBContext_Constructor_CreatesInstance()
        {
            // Arrange & Act
            DBContext context = null;
            var exception = Record.Exception(() => context = new DBContext());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void DBContext_Pictures_PropertyNotNull()
        {
            // Arrange
            using (var context = new DBContext())
            {
                // Act
                var pictures = context.Pictures;

                // Assert
                Assert.NotNull(pictures);
            }
        }

        [Fact]
        public void DBContext_GalleryTypes_PropertyNotNull()
        {
            // Arrange
            using (var context = new DBContext())
            {
                // Act
                var galleryTypes = context.GalleryTypes;

                // Assert
                Assert.NotNull(galleryTypes);
            }
        }

        [Fact]
        public void DBContext_InheritsFromIdentityDbContext()
        {
            // Arrange & Act
            using (var context = new DBContext())
            {
                // Assert
                Assert.IsAssignableFrom<IdentityDbContext<IdentityUser>>(context);
            }
        }
    }

    public class DBInitializerTests
    {
        [Fact]
        public void DBInitializer_Constructor_CreatesInstance()
        {
            // Arrange & Act
            DBInitializer initializer = null;
            var exception = Record.Exception(() => initializer = new DBInitializer());

            // Assert
            Assert.Null(exception);
            Assert.NotNull(initializer);
        }

        [Fact]
        public void DBInitializer_InheritsFromDropCreateDatabaseIfModelChanges()
        {
            // Arrange & Act
            var initializer = new DBInitializer();

            // Assert
            Assert.IsAssignableFrom<System.Data.Entity.DropCreateDatabaseIfModelChanges<DBContext>>(initializer);
        }
    }
}
