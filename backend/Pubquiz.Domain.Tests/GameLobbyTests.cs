using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Requests;
using Pubquiz.Logic.Requests.Notifications;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Persistence.Extensions;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class GameLobbyTests : InitializedTestBase
    {
        [TestMethod]
        public void TestGame_TeamLobby_ShouldntShowOwnTeam()
        {
            // arrange
            var firstTeam = Teams[0];
            var query = new TeamLobbyViewModelQuery(UnitOfWork) {TeamId = firstTeam.Id};

            // act
            var model = query.Execute().Result;

            // assert
            CollectionAssert.DoesNotContain(model.OtherTeamsInGame, firstTeam.Id);
        }

        [TestMethod]
        public void TestGame_TeamLobbyWithInvalidTeamId_ThrowsException()
        {
            // arrange
            var query = new TeamLobbyViewModelQuery(UnitOfWork) {TeamId = Guid.Empty.ToShortGuidString()};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => query.Execute()).Result;
            Assert.AreEqual(ResultCode.InvalidTeamId, exception.ResultCode);
            Assert.AreEqual("Invalid TeamId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }


        [TestMethod]
        public void TestGame_TeamLobbyWithNonOpenGame_ThrowsException()
        {
            // arrange
            var game = UnitOfWork.GetCollection<Game>().GetAsync(Game.Id).Result;
            game.SetState(GameState.Closed);
            UnitOfWork.GetCollection<Game>().UpdateAsync(game);
            var firstTeam = Teams[0];
            var query = new TeamLobbyViewModelQuery(UnitOfWork) {TeamId = firstTeam.Id};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => query.Execute()).Result;
            Assert.AreEqual(ResultCode.LobbyUnavailableBecauseOfGameState, exception.ResultCode);
            Assert.AreEqual("The lobby for this game is not open.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_QuizMasterSelectsValidGame_ReturnsUser()
        {
            // arrange
            var actorId = Users.First(u => u.UserName == "Quiz master 1").Id;
            var notification = new SelectGameNotification(UnitOfWork, Bus) {GameId = Game.Id, ActorId = actorId};

            // act
            notification.Execute().Wait();

            // assert
            var updatedUser = UnitOfWork.GetCollection<User>().GetAsync(actorId).Result;
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(Game.Id, updatedUser.CurrentGameId);
        }

        [TestMethod]
        public void TestGame_QuizMasterSelectsInvalidGame_ThrowsException()
        {
            // arrange
            var actorId = Users.First(u => u.UserName == "Quiz master 1").Id;
            var command = new SelectGameNotification(UnitOfWork, Bus)
                {GameId = Guid.Empty.ToShortGuidString(), ActorId = actorId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ResultCode.InvalidGameId, exception.ResultCode);
            Assert.AreEqual("Invalid GameId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_InvalidQuizMasterSelectsGame_ThrowsException()
        {
            // arrange
            var actorId = Guid.Empty.ToShortGuidString();
            var command = new SelectGameNotification(UnitOfWork, Bus) {GameId = Game.Id, ActorId = actorId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ResultCode.InvalidUserId, exception.ResultCode);
            Assert.AreEqual("Invalid ActorId.", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }

        [TestMethod]
        public void TestGame_UnauthorizedQuizMasterSelectsGame_ThrowsException()
        {
            // arrange
            var actorId = Users.First(u => u.UserName == "Quiz master 2").Id;
            var command = new SelectGameNotification(UnitOfWork, Bus) {GameId = Game.Id, ActorId = actorId};

            // act & assert
            var exception = Assert.ThrowsExceptionAsync<DomainException>(() => command.Execute()).Result;
            Assert.AreEqual(ResultCode.QuizMasterUnauthorizedForGame, exception.ResultCode);
            Assert.AreEqual($"Actor with id {actorId} is not authorized for game '{Game.Id}'", exception.Message);
            Assert.IsTrue(exception.IsBadRequest);
        }
    }
}