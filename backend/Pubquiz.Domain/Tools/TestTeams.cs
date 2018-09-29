using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;
using Pubquiz.Repository;

namespace Pubquiz.Domain.Tools
{
    public static class TestTeams
    {
        public static List<Team> GetTeams(IRepository<Team> teamRepository, Guid gameId)
        {
            var teams = new List<Team>();

            var team1Name = "Team 1";
            var team2Name = "Team 2";
            var team3Name = "Team 3";

            teams.Add(new Team
            {
                Name = team1Name,
                UserName = team1Name.ReplaceSpaces(),
                NormalizedUserName = team1Name.ReplaceSpaces().ToUpperInvariant(),
                GameId = gameId,
                RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamRepository, gameId),
                MemberNames = new List<string> {"member 1", "member 2", "member 3"}
            });

            teams.Add(new Team
            {
                Name = team2Name,
                UserName = team2Name.ReplaceSpaces(),
                NormalizedUserName = team2Name.ReplaceSpaces().ToUpperInvariant(),
                GameId = gameId,
                RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamRepository, gameId),
                MemberNames = new List<string> {"member 1", "member 2", "member 3"}
            });
            teams.Add(new Team
            {
                Name = team3Name,
                UserName = team3Name.ReplaceSpaces(),
                NormalizedUserName = team3Name.ReplaceSpaces().ToUpperInvariant(),
                GameId = gameId,
                RecoveryCode = Helpers.GenerateSessionRecoveryCode(teamRepository, gameId),
                MemberNames = new List<string> {"member 1", "member 2", "member 3"}
            });

            return teams;
        }

        public static IEnumerable<User> GetUsersFromTeams(IEnumerable<Team> teams)
        {
            foreach (var team in teams)
            {
                yield return new User
                {
                    Id = team.Id,
                    UserName = team.UserName,
                    NormalizedUserName = team.NormalizedUserName,
                    RecoveryCode = team.RecoveryCode
                };
            }
        }
    }
}