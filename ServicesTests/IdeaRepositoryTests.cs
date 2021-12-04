using YoghurtBank.Data.Model;
using YoghurtBank.Infrastructure;
using Xunit;
using YoghurtBank.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace YoghurtBank.ServicesTests {

    public class IdeaRepositoryTests : IDisposable
    {
        private readonly YoghurtContext _context;
        private readonly IdeaRepository _repo;

        public IdeaRepositoryTests() 
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<YoghurtContext>();
            builder.UseSqlite(connection);
            var context = new YoghurtContext(builder.Options);
            context.Database.EnsureCreated();

            var supervisor1 = new Supervisor{Id = 1, UserName = "Torben", CollaborationRequests = new List<CollaborationRequest>(), Ideas = new List<Idea>()};
            var supervisor2 = new Supervisor{Id = 2, UserName = "Preben", CollaborationRequests = new List<CollaborationRequest>(), Ideas = new List<Idea>()};
            
            var idea1 = new Idea{Id = 1, Creator = supervisor2, Posted = DateTime.Now, Subject = "Big Data", Title = "Big data is good", Description = "Big data gives value", AmountOfCollaborators = 3, Open = true, TimeToComplete = DateTime.Now-DateTime.Today, StartDate = DateTime.Now, Type = IdeaType.Bachelor};
            var idea2 = new Idea{Id = 2, Creator = supervisor1, Posted = DateTime.Now, Subject = "Data Intelligence", Title = "Data Intelligence is good", Description = "Data Intelligence gives value", AmountOfCollaborators = 1, Open = true, TimeToComplete = DateTime.Now-DateTime.Today, StartDate = DateTime.Now, Type = IdeaType.PhD};
            var idea3 = new Idea{Id = 3, Creator = supervisor2, Posted = DateTime.Now, Subject = "DevOps", Title = "DevOps is good", Description = "DevOps gives value", AmountOfCollaborators = 2, Open = true, TimeToComplete = DateTime.Now-DateTime.Today, StartDate = DateTime.Now, Type = IdeaType.Project};
            var idea4 = new Idea{Id = 4, Creator = supervisor1, Posted = DateTime.Now, Subject = "Requirements Elicitation", Title = "Requirements Elicitation is good", Description = "Requirements Elicitation gives value", AmountOfCollaborators = 5, Open = true, TimeToComplete = DateTime.Now-DateTime.Today, StartDate = DateTime.Now, Type = IdeaType.Masters};

            supervisor1.Ideas.Add(idea2);
            supervisor1.Ideas.Add(idea4);
            supervisor2.Ideas.Add(idea1);
            supervisor2.Ideas.Add(idea3);

            context.Ideas.AddRange(idea1, idea2, idea3, idea4);
            context.Users.AddRange(supervisor1, supervisor2);

            context.SaveChanges();
            _context = context;
            _repo = new IdeaRepository(_context);
            
        }


        /*[Fact]
        public void AddIdeaAsyncReturnsSomething(){

            TimeSpan ts1 = TimeSpan. FromDays(12);
            Type bach = IdeaType.Bachelor;
            var idea = new IdeaCreateDTO{CreatorId = 1, Title = "BigDataHandling", Subject =  "Database Managment", Description = "Interesting topic with the best supervisor (B-dog)", AmountOfCollaborators = 3, Open = true, TimeToComplete = null, StartDate = 2022-01-03, Type = null};
            var output = _repo.AddIdea(idea);

            Assert.Equal(HttpStatusCode.Created, output);
           
        }*/

        [Fact]
        public async Task FindIdeaDetailsAsync_given_valid_id_returns_details()
        {
            #region Arrange
            var id = 1; 
            #endregion

            #region Act
            var result = await _repo.FindIdeaDetailsAsync(id);
            #endregion

            #region Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Big Data", result.Subject);
            Assert.Equal("Big data is good", result.Title);
            Assert.Equal("Big data gives value", result.Description);
            Assert.Equal(3, result.AmountOfCollaborators);
            Assert.Equal(2, result.CreatorId);
            Assert.True(result.Open);
            //datetime properties also?
            #endregion
        }

        [Fact]
        public async Task FindIdeaDetailsAsync_given_invalid_id_returns_null()
        {
            #region Arrange
            var id = 500; 
            #endregion
            
            #region Act
            var result = await _repo.FindIdeaDetailsAsync(id);
            #endregion

            #region Assert
            //needs to be changed when return value of method is changed
            Assert.Null(result);
            #endregion
            
        }

        [Fact]
        public async Task DeleteAsync_given_valid_id_deletes_it()
        {
            #region Arrange
            var id = 1;
            #endregion
            
            #region Act
            var result = await _repo.DeleteAsync(id);
            var entity = await _context.Ideas.FindAsync(id);
            #endregion

            #region Assert
            Assert.Equal(1, result);
            Assert.Null(entity);
            #endregion
        }

        //TODO change this when return value of delete is changed
        [Fact]
        public async Task DeleteAsync_given_invalid_id_returns_minusone()
        {
            var id = 500;
            //make sure it doesnt exists initally
            var entity = await _context.Ideas.FindAsync(id);
            Assert.Null(entity);

            var result = await _repo.DeleteAsync(id);
            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task UpdateAsync_given_existing_entity_updates_it()
        {
            #region Arrange
            var id = 1; 
            //get intital 
            var entity = await _context.Ideas.FindAsync(id);

            var update = new IdeaUpdateDTO
            {
                Title = "NewTitle",
                Subject = "NewSubject",
                Description = "NewDescription",
                AmountOfCollaborators = 400,
                Open = false
            };
            #endregion

            #region Act
            var result = await _repo.UpdateAsync(id, update);
            //get the actual entity to verify the updates

            var updatedEntity = await _context.Ideas.FindAsync(id);

            #endregion
            #region Assert
            Assert.NotNull(result);
            Assert.Equal("NewTitle", result.Title);
            Assert.Equal("NewSubject", result.Subject);
            Assert.Equal("NewDescription", result.Description);
            Assert.Equal(400, result.AmountOfCollaborators);
            Assert.False(result.Open);
            

            Assert.NotNull(updatedEntity);
            Assert.Equal("NewTitle", updatedEntity.Title);
            Assert.Equal("NewSubject", updatedEntity.Subject);
            Assert.Equal("NewDescription", updatedEntity.Description);
            Assert.Equal(400, updatedEntity.AmountOfCollaborators);
            Assert.False(updatedEntity.Open);
            Assert.Equal(entity.TimeToComplete, updatedEntity.TimeToComplete);
            Assert.Equal(entity.StartDate, updatedEntity.StartDate);
            Assert.Equal(entity.Type, updatedEntity.Type);
            #endregion
        }

        [Fact]
        public async Task CreateAsync_given_idea_returns_it()
        {
            #region Arrange
            var idea = new IdeaCreateDTO
            {
                CreatorId = 2,
                Title = "GADDAG's and their use in ScrabbleBots",
                Subject = "Scrabble",
                Description = "Heya Sweden",
                AmountOfCollaborators = 2, 
                Open = true,
                TimeToComplete = DateTime.Now-DateTime.Now,
                StartDate = DateTime.Now,
                Type = IdeaType.Bachelor
            };
            #endregion

            #region Act
            var result = await _repo.CreateAsync(idea);
            #endregion

            #region Assert
            Assert.NotNull(result);
            //needs more assertions
            #endregion
        }

        [Fact]
        public async Task FIndIdeaBySupervisor_given_invalid_SupervisorId_returns_null()
        {
            int supervisorid = 5;
            var ideasFromSupervisorId = await _repo.FindIdeasBySupervisorIdAsync(supervisorid);
            
            Assert.Equal((HttpStatusCode.NotFound, null), ideasFromSupervisorId);

        }
        
        [Fact]
       public async Task FIndIdeaBySupervisor_given_valid_UserId_returns_list_with_two_elements()
        {
            int supervisorid = 2;
            var ideasFromSupervisorId = await _repo.FindIdeasBySupervisorIdAsync(supervisorid);
            List<IdeaDTO> expectedIdeas = new List<IdeaDTO>();

            var ideaDTO1 = new IdeaDTO {
                Id = 1,
                Title = "Big data is good",
                Subject = "Big Data",
                Type = IdeaType.Bachelor
            };

            var ideaDTO2 = new IdeaDTO {
                Id = 3,
                Title = "DevOps is good",
                Subject = "DevOps",
                Type = IdeaType.Project
            };

            expectedIdeas.Add(ideaDTO1);
            expectedIdeas.Add(ideaDTO2);
            
            Assert.Equal(HttpStatusCode.Accepted, ideasFromSupervisorId.code);
            Assert.Collection(expectedIdeas,
            idea => Assert.Equal(ideaDTO1, idea),
            idea => Assert.Equal(ideaDTO2, idea)
            );
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}