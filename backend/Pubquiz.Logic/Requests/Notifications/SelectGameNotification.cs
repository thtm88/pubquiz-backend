﻿using System.Linq;
using System.Threading.Tasks;
using Pubquiz.Domain;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Requests.Queries;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Rebus.Bus;

namespace Pubquiz.Logic.Requests.Notifications
{
    /// <summary>
    /// Command to select a current <see cref="Game"/>.
    /// </summary>
    [ValidateEntity(EntityType = typeof(User), IdPropertyName = "ActorId")]
    [ValidateEntity(EntityType = typeof(Game), IdPropertyName = "GameId")]
    public class SelectGameNotification : Notification
    {
        public string ActorId { get; set; }
        public string GameId { get; set; }

        public SelectGameNotification(IUnitOfWork unitOfWork, IBus bus) : base(unitOfWork, bus)
        {
        }

        protected override async Task DoExecute()
        {
            var userCollection = UnitOfWork.GetCollection<User>();
            var user = await userCollection.GetAsync(ActorId);
            if (user.UserRole != UserRole.QuizMaster)
            {
                throw new DomainException(ResultCode.UnauthorizedRole, "You can't do that with this role.", true);
            }

            if (user.GameRefs.All(r => r.Id != GameId))
            {
                throw new DomainException(ResultCode.QuizMasterUnauthorizedForGame,
                    $"Actor with id {ActorId} is not authorized for game '{GameId}'", true);
            }

            var oldGameId = user.CurrentGameId;
            user.CurrentGameId = GameId;
            await userCollection.UpdateAsync(user);

            var query = new QmLobbyViewModelQuery(UnitOfWork)
            {
                UserId = ActorId
            };
            var viewModel = await query.Execute();
            await Bus.Publish(new GameSelected(oldGameId, GameId, viewModel));
        }
    }
}