using System;
using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Pubquiz.Domain.Models
{
    /// <summary>
    /// An instance of a quiz (a composition of questions) that can be held at some time.
    /// Contains question sets which contain questions.
    /// </summary>
    public class Quiz
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<QuizSection> QuizSections { get; set; }

        public Quiz()
        {
            Id = Guid.NewGuid();
            QuizSections = new List<QuizSection>();
        }
    }
}