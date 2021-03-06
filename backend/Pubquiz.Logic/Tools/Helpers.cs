using System;
using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Tools
{
    public static class Helpers
    {
        private static readonly List<string> Wordlist = new List<string>
        {
            "flatulent",
            "stroef",
            "extra",
            "dof",
            "peuzel",
            "aap",
            "noot",
            "mies",
            "wim",
            "zus",
            "jet",
            "teun",
            "vuur",
            "gijs",
            "lam",
            "kees",
            "weide",
            "does",
            "hok",
            "duif",
            "schapen"
        };

        public static string GenerateSessionRecoveryCode(Persistence.ICollection<Team> teamCollection, string gameId)
        {
            string result;
            do
            {
                var words = new List<string>();
                var random = new Random();
                var wordlistLength = Wordlist.Count;
                for (int i = 0; i < 3; i++)
                {
                    var nextIndex = random.Next(0, wordlistLength);
                    words.Add(Wordlist[nextIndex]);
                }

                result = string.Join(" ", words);
            } while (teamCollection.AnyAsync(t => t.RecoveryCode == result && t.CurrentGameId == gameId).Result);

            return result;
        }

        public static string GetTeamsGroupId(string gameId)
        {
            return $"teams-{gameId}";
        }

        public static string GetQuizMasterGroupId(string gameId)
        {
            return $"quizmaster-{gameId}";
        }

        public static string GetAdminGroupId()
        {
            return "admin";
        }
    }
}