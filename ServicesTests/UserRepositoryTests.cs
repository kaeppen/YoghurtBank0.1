using Xunit;
using YoghurtBank.Services;
using YoghurtBank.Infrastructure;
using YoghurtBank.Data.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Net;

namespace ServicesTests {

    public class UserRepositoryTests : IDisposable
    {
        private readonly UserRepository _repository;
        private readonly YoghurtContext _context;
        private bool disposed;

        public UserRepositoryTests()
        {
            //copy-pasta fra rasmus github:
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<YoghurtContext>();
            builder.UseSqlite(connection);
            var context = new YoghurtContext(builder.Options);
            context.Database.EnsureCreated();

            User Mette = new Student {Id = 1, UserName = "Mette", CollaborationRequests = new List<CollaborationRequest>{}}; 
            User Sofia = new Student {Id = 2, UserName = "Sofia", CollaborationRequests = new List<CollaborationRequest>{}};
            User Jens = new Supervisor {Id = 3, UserName = "Jens", CollaborationRequests = new List<CollaborationRequest>{}};
            User Line = new Supervisor {Id = 4, UserName = "Line", CollaborationRequests = new List<CollaborationRequest>{}};

            context.Users.AddRange(
                Mette, Sofia, Jens, Line
            );
            context.SaveChanges(); 

            _context = context;
            _repository = new UserRepository(_context);
        }


        [Fact]
        public async Task FindUserByIdAsync_given_invalid_Id_returns_notFound_and_null() {
            
            int userId = 10;
            var actualUser = await _repository.FindUserByIdAsync(userId);

            Assert.Equal((HttpStatusCode.NotFound, null), actualUser);
        }


        [Fact]
        public async Task FindUserByIdAsync_given_id_2_returns_UserDetailsDTO() 
        {

            int userId = 2;
            var user = await _repository.FindUserByIdAsync(userId);

            var expected = new UserDetailsDTO
            {
                Id = 2, 
                UserName = "Sofia",
                UserType = "Student"
            };

            Assert.Equal(2, user.details.Id);
            Assert.Equal("Sofia", user.details.UserName);
            Assert.Equal("Student", user.details.UserType);

        }

        [Fact]
        public async Task DeleteAsync_given_valid_id_deletes_it_and_returns_id()
        {
            #region Arrange
            var id = 1;
            #endregion

            #region Act
            var result = await _repository.DeleteAsync(id);
            var entityDeleted = await _context.Users.FindAsync(id);
            #endregion

            #region Assert
            Assert.Equal(1, result);
            Assert.Null(entityDeleted);
            #endregion
        }

        //change this when return value changes
        [Fact]
        public async Task DeleteAsync_given_invalid_id_returns_minusone()
        {
            #region Arrange
            var id = 500;
            var entityNoExist = await _context.Users.FindAsync(id);
            #endregion

            #region Act
            var result = await _repository.DeleteAsync(id);
            #endregion

            #region Assert
            Assert.Null(entityNoExist);
            Assert.Equal(-1, result);
            #endregion
        }
        
        [Fact]
        public async Task CreateAsync_given_user_with_type_student_Returns_UserDetailsDTO()
        {
            var user = new UserCreateDTO{UserName = "Hanne", UserType = "Student"};
            
            var result = await _repository.CreateAsync(user);

            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
            Assert.Equal("Hanne", result.UserName);
            Assert.Equal("Student", result.UserType);
        }

        [Fact]
        public async Task CreateAsync_given_user_with_type_supervisor_Returns_UserDetailsDTO()
        {
            var user = new UserCreateDTO{UserName = "Hanne-Birgitte", UserType = "Supervisor"};
            
            var result = await _repository.CreateAsync(user);

            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
            Assert.Equal("Hanne-Birgitte", result.UserName);
            Assert.Equal("Supervisor", result.UserType);
        }


        //change this if method is changed to return something else in case of no type...
        [Fact]
        public async Task CreateAsync_given_invalid_usertype_returns_null()
        {
            var user = new UserCreateDTO{UserName = "Mikki", UserType = "invalid"};
            var result = await _repository.CreateAsync(user);
            Assert.Null(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                // TODO: dispose managed state (managed objects)
                }
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}