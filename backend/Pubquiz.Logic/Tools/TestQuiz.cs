using System.Collections.Generic;
using Pubquiz.Domain.Models;

namespace Pubquiz.Logic.Tools
{
    public static class TestQuiz
    {
        public static Question GetMCQuestion()
        {
            var mcQuestion = new Question
            {
                Body = "Question body",
                Title = "Multiple choice",
                MaxScore = 1,
                QuestionType = QuestionType.MultipleChoice
            };

            mcQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "Choose",
                ChoiceOptions = new List<ChoiceOption>
                {
                    new ChoiceOption(1, "Option 1"),
                    new ChoiceOption(2, "Option 2"),
                    new ChoiceOption(3, "Correct option 3"),
                    new ChoiceOption(4, "Option 4")
                },
                Solution = new Solution(new[] {3}),
                InteractionType = InteractionType.MultipleChoice,
                MaxScore = 1
            });

            return mcQuestion;
        }

        public static Question GetMRQuestion()
        {
            var mrQuestion = new Question
            {
                Body = "Question body",
                Title = "Multiple response",
                MaxScore = 1,
                QuestionType = QuestionType.MultipleResponse
            };

            mrQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "Choose",
                ChoiceOptions = new List<ChoiceOption>
                {
                    new ChoiceOption(1, "Option 1"),
                    new ChoiceOption(2, "Option 2"),
                    new ChoiceOption(3, "Correct option 3"),
                    new ChoiceOption(4, "Corrent option 4")
                },
                Solution = new Solution(new[] {3, 4}),
                InteractionType = InteractionType.MultipleResponse,
                MaxScore = 1
            });

            return mrQuestion;
        }

        public static Question GetSAQuestion()
        {
            var mrQuestion = new Question
            {
                Body = "Question body",
                Title = "Short answer with one solution",
                MaxScore = 1,
                QuestionType = QuestionType.ShortAnswer
            };

            mrQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "What is the right answer? (hint: 'answer')",
                Solution = new Solution(new[] {"answer"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            return mrQuestion;
        }
        
        public static Question GetSAWithMultipleSolutionsQuestion()
        {
            var mrQuestion = new Question
            {
                Body = "Question body",
                Title = "Short answer with multiple solutions",
                MaxScore = 1,
                QuestionType = QuestionType.ShortAnswer
            };

            mrQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "What is the right answer? (hint: 'answer')",
                Solution = new Solution(new[] {"answer", "ansver"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            return mrQuestion;
        }

        public static Question GetMultipleSAQuestion()
        {
            var multipleSAQuestion = new Question
            {
                Body = "Question body",
                Title = "Test question",
                MaxScore = 3,
                QuestionType = QuestionType.ShortAnswer
            };

            multipleSAQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "1. What is the right answer? (hint: 'answer1')",
                Solution = new Solution(new[] {"answer1"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 1
            });

            multipleSAQuestion.Interactions.Add(new Interaction(2)
            {
                Text = "2. What is the right answer? (hint: 'answer2')",
                Solution = new Solution(new[] {"answer2"}),
                InteractionType = InteractionType.ShortAnswer,
                MaxScore = 2
            });
            return multipleSAQuestion;
        }
        
        public static Question GetETQuestion()
        {
            var etQuestion = new Question
            {
                Body = "Question body",
                Title = "Extended text (no solution, manually corrected)",
                MaxScore = 1,
                QuestionType = QuestionType.ExtendedText
            };

            etQuestion.Interactions.Add(new Interaction(1)
            {
                Text = "Name all of the right answers.",
                InteractionType = InteractionType.ExtendedText,
                MaxScore = 1
            });

            return etQuestion;
        }


        public static Quiz GetQuiz()
        {
            var quizSection = new QuizSection
            {
                Title = "Main quiz section",
                QuizItems = new List<QuizItem>
                {
                    GetMCQuestion(),
                    GetMRQuestion(),
                    GetSAQuestion(),
                    GetSAWithMultipleSolutionsQuestion(),
                    GetMultipleSAQuestion(),
                    GetETQuestion()
                }
            };

            var quiz = new Quiz {Title = "Testquiz"};
            quiz.QuizSections.Add(quizSection);

            return quiz;
        }
    }
}