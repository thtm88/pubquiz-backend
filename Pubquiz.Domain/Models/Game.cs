using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    /// <summary>
    /// A planned instance of a game that uses a certain quiz.
    /// Changes in time throughout the game, e.g. has state.
    /// </summary>
    public class Game
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public GameState State { get; set; }
        public Quiz Quiz { get; set; }
        public List<Team> Teams { get; set; }

        public Question CurrentQuestion { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public int CurrentQuestionSetIndex { get; set; }

        public Game()
        {
            Id = Guid.NewGuid();
            State = GameState.Closed;
            Teams = new List<Team>();
        }

        public void SetState(GameState newGameState)
        {
            switch (newGameState)
            {
                case GameState.Closed:
                    if (State != GameState.Open)
                    {
                        throw new DomainException("Can only close the game from the open state.", true);
                    }

                    break;
                case GameState.Open:
                    if (State != GameState.Closed)
                    {
                        throw new DomainException("Can only open the game from the closed state.", true);
                    }

                    if (State == GameState.Closed && (Quiz == null || string.IsNullOrWhiteSpace(Title)))
                    {
                        throw new DomainException("Can't open the game without a quiz and/or a title.", true);
                    }

                    break;
                case GameState.Running:
                    if (State != GameState.Open && State != GameState.Paused)
                    {
                        throw new DomainException("Can only start the game from the open and paused states.", true);
                    }

                    if (!Teams.Any())
                    {
                        throw new DomainException("Can't start the game without teams.", true);
                    }

                    break;
                case GameState.Paused:
                    if (State != GameState.Running)
                    {
                        throw new DomainException("Can only pause the game from the running state.", true);
                    }

                    break;

                case GameState.Finished:
                    if (State != GameState.Running && State != GameState.Paused)
                    {
                        throw new DomainException("Can only finish the game from the running and paused states.", true);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
            }

            State = newGameState;
        }
    }

    public enum GameState
    {
        Closed,
        Open, // e.g. open for registration
        Running, // InSession? Started?
        Paused,
        Finished // Ended?
    }
}