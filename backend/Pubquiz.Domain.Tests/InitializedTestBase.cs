using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pubquiz.Domain.Models;
using Pubquiz.Logic.Handlers;
using Pubquiz.Logic.Messages;
using Pubquiz.Logic.Tools;
using Pubquiz.Persistence;
using Pubquiz.Persistence.NoAction;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;

namespace Pubquiz.Domain.Tests
{
    [TestClass]
    public class InitializedTestBase
    {
        protected IUnitOfWork UnitOfWork;
        protected Game Game;
        protected Quiz Quiz;
        protected List<User> Users;
        protected List<Team> Teams;
        protected List<QuizItem> QuestionsInQuiz;
        protected List<QuizItem> OtherQuestions;
        protected IBus Bus;
        protected ILoggerFactory LoggerFactory;
        private InMemorySubscriberStore _inMemorySubscriberStore;

        [TestInitialize]
        public void Initialize()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole()).BuildServiceProvider();
            LoggerFactory = serviceProvider.GetService<ILoggerFactory>();

            ICollectionOptions inMemoryCollectionOptions = new InMemoryDatabaseOptions();
            UnitOfWork = new NoActionUnitOfWork(memoryCache, LoggerFactory, inMemoryCollectionOptions);

            var quizCollection = UnitOfWork.GetCollection<Quiz>();
            var userCollection = UnitOfWork.GetCollection<User>();
            var teamCollection = UnitOfWork.GetCollection<Team>();
            var gameCollection = UnitOfWork.GetCollection<Game>();
            var questionCollection = UnitOfWork.GetCollection<QuizItem>();

            Users = TestUsers.GetUsers();
            Quiz = TestQuiz.GetQuiz();
            Game = TestGame.GetGame(Users.Where(u => u.UserName == "Quiz master 1").Select(u => u.Id), Quiz);
            var gameRef = new GameRef
                {Id = Game.Id, Title = Game.Title, QuizTitle = Game.QuizTitle, InviteCode = Game.InviteCode};
            Users.First(u => u.UserName == "Quiz master 1").GameRefs.Add(gameRef);
            Users.First(u => u.UserName == "Quiz master 1").QuizRefs.Add(new QuizRef
                {Id = Quiz.Id, Title = Quiz.Title, GameRefs = new List<GameRef> {gameRef}});
            Teams = TestTeams.GetTeams(teamCollection, Game.Id);
            Game.QuizId = Quiz.Id;
            Game.TeamIds = Teams.Select(t => t.Id).ToList();
            QuestionsInQuiz = TestQuiz.GetQuizItems();
            OtherQuestions = new List<QuizItem> {new QuizItem(), new QuizItem(), new QuizItem()};
            Task.WaitAll(
                quizCollection.AddAsync(Quiz),
                QuestionsInQuiz.ToAsyncEnumerable().ForEachAsync(q => questionCollection.AddAsync(q)),
                OtherQuestions.ToAsyncEnumerable().ForEachAsync(q => questionCollection.AddAsync(q)),
                Teams.ToAsyncEnumerable().ForEachAsync(t => teamCollection.AddAsync(t)),
                Users.ToAsyncEnumerable().ForEachAsync(u => userCollection.AddAsync(u)),
                gameCollection.AddAsync(Game));

            // set up bus
            var activator = new BuiltinHandlerActivator();
            activator.Register((bus, messageContext) => new ScoringHandler(UnitOfWork, bus, LoggerFactory));
            activator.Register(() => new ClientNotificationHandler(LoggerFactory, null));

            // needed so the inmemory subscription store will be centralized
            _inMemorySubscriberStore = new InMemorySubscriberStore();

            Configure.With(activator).Logging(l => l.ColoredConsole())
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(true), "Messages"))
                .Routing(r => r.TypeBased().MapAssemblyOf<TeamMembersChanged>("Messages"))
                .Subscriptions(s => s.StoreInMemory(_inMemorySubscriberStore))
                .Start();

            Bus = activator.Bus;
            Bus.SubscribeByScanningForHandlers(Assembly.Load("Pubquiz.Logic"));
        }
    }
}