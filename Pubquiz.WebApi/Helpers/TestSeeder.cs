using System.Linq;
using Microsoft.Extensions.Logging;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Domain.Tools;
using Pubquiz.Repository;

namespace Pubquiz.WebApi.Helpers
{
    public class TestSeeder
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ILogger _logger;

        public TestSeeder(IRepositoryFactory repositoryFactory, ILoggerFactory loggerFactory)
        {
            _repositoryFactory = repositoryFactory;
            _logger = loggerFactory.CreateLogger<TestSeeder>();
        }

        public void SeedTestSet()
        {
            _logger.LogInformation("Seeding test set.");
            var quizRepo = _repositoryFactory.GetRepository<Quiz>();
            var teamRepo = _repositoryFactory.GetRepository<Team>();
            var gameRepo = _repositoryFactory.GetRepository<Game>();
            var userRepo = _repositoryFactory.GetRepository<User>();
            var game = TestGame.GetGame();
            var quiz = TestQuiz.GetQuiz();
            var teams = TestTeams.GetTeams(teamRepo, game.Id);
            game.QuizId = quiz.Id;
            game.TeamIds = teams.Select(t => t.Id).ToList();
            var users = TestTeams.GetUsersFromTeams(teams).ToList();
            quizRepo.AddAsync(quiz).Wait();
            teams.ForEach(t => teamRepo.AddAsync(t).Wait());
            users.ForEach(u => userRepo.AddAsync(u).Wait());
            gameRepo.AddAsync(game).Wait();
        }
    }
}