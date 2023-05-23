using DictionaryApplication.Data;
using DictionaryApplication.Models;
using DictionaryApplication.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace DictionaryApplication.Services
{
    public class KnowledgeTestService
    {
        private readonly ILexemeTestAttemptRepository _lexemeTestAttemptRepository;

        public KnowledgeTestService(ILexemeTestAttemptRepository lexemeTestAttemptRepository)
        {
            _lexemeTestAttemptRepository = lexemeTestAttemptRepository;
        }

        public async Task<bool> ContainAnyLexemesAsync(IEnumerable<int> userDictionaryIds)
        {
            var lexemes = await _lexemeTestAttemptRepository.GetAllAsync(userDictionaryIds.ToArray());
            return lexemes.Any();
        }
        public async Task<int> GetTotalLexemesAmount(IEnumerable<int> userDictionaryIds)
        {
            var lexemes = await _lexemeTestAttemptRepository.GetAllAsync(userDictionaryIds.ToArray());
            return lexemes.Count;
        }
        public async Task<bool> AreParametersValid(KnowledgeTestParameters testParameters)
        {
            return testParameters != null 
                && testParameters.SelectedDictionaryIds != null
                && testParameters.SelectedDictionaryIds.Count > 0
                && await ContainAnyLexemesAsync(testParameters.SelectedDictionaryIds)
                && testParameters.NumberOfWords > 0;
        }

        public async Task<List<LexemeTestAttempt>> GetLexemeTestAttemptsAsync(KnowledgeTestParameters testParameters)
        {
            Random random = new Random();
            var lexemeTestAttempts = await _lexemeTestAttemptRepository.GetAllAsync(testParameters.SelectedDictionaryIds.ToArray());

            foreach (var lexemeTestAttempt in lexemeTestAttempts)
            {
                var translations = lexemeTestAttempt.Translations;
                var translationsRepresentation = translations.Count > 1
                        ? string.Join(Environment.NewLine, translations.Select(l => l.Word).Select((s, i) => $"{i + 1}. {s}"))
                        : translations.First().Word;

                if (testParameters.IsUserTranslatesStudiedLanguage)
                {
                    lexemeTestAttempt.LexemeTestRepresentation = lexemeTestAttempt.Lexeme.Word;
                    lexemeTestAttempt.CorrectAnswerRepresentation = translationsRepresentation;
                }
                else 
                {
                    lexemeTestAttempt.LexemeTestRepresentation = translationsRepresentation;
                    lexemeTestAttempt.CorrectAnswerRepresentation = lexemeTestAttempt.Lexeme.Word;
                }
            }

            switch (testParameters.TestType)
            {
                case TestType.AllWords:
                    return lexemeTestAttempts.OrderBy(l => random.Next())
                        .Take(testParameters.NumberOfWords)
                        .ToList();
                case TestType.LastWords:
                    return lexemeTestAttempts.TakeLast(testParameters.NumberOfWords)
                        .ToList();
                case TestType.WordsWithWorstResults:
                    return lexemeTestAttempts.OrderBy(x =>
                        x.Lexeme.TotalTestAttempts == 0
                        ? random.Next()
                        : x.Lexeme.CorrectTestAttempts / x.Lexeme.TotalTestAttempts)
                        .ThenBy(x => random.Next())
                        .Take(testParameters.NumberOfWords)
                        .ToList();
                default:
                    return lexemeTestAttempts;
            }
        }

        public void CheckAnswers(ref List<LexemeTestAttempt> lexemes, KnowledgeTestParameters knowledgeTestParameters)
        {
            foreach (var lexeme in lexemes)
            {
                if (lexeme.TestAnswer == null)
                {
                    continue;
                }

                lexeme.IsCorrectAnswer = knowledgeTestParameters.IsUserTranslatesStudiedLanguage
                    ? IsCorrectAnswer(lexeme.TestAnswer, lexeme.Translations.Select(t => t.Word).ToArray())
                    : IsCorrectAnswer(lexeme.TestAnswer, lexeme.Lexeme.Word);
            }
        }
        
        public bool IsCorrectAnswer(string userAnswer, params string[] correctTranslations)
        {
            userAnswer = userAnswer.ToLower().Trim().Replace(",", "");

            for (int i = 0; i < correctTranslations.Length; i++)
            {
                correctTranslations[i] = correctTranslations[i].ToLower().Trim().Replace(",", "");

                if (correctTranslations[i].Equals(userAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                var levensteinDistance = LevenshteinDistance(correctTranslations[i], userAnswer);
                if (levensteinDistance <= 1)
                {
                    return true;
                }
            }

            return false;
        }

        // Метод для вычисления расстояния Левенштейна между двумя строками
        private int LevenshteinDistance(string s1, string s2)
        {
            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= s2.Length; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j <= s2.Length; j++)
            {
                for (int i = 1; i <= s1.Length; i++)
                {
                    if (s1[i - 1] == s2[j - 1])
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        d[i, j] = Math.Min(d[i - 1, j], Math.Min(d[i, j - 1], d[i - 1, j - 1])) + 1;
                    }
                }
            }

            return d[s1.Length, s2.Length];
        }

        public async Task SetResults(List<LexemeTestAttempt> lexemes)
        {
            foreach (var lexeme in lexemes)
            {
                lexeme.Lexeme.TotalTestAttempts++;

                if (lexeme.IsCorrectAnswer)
                {
                    lexeme.Lexeme.CorrectTestAttempts++;
                }

                await _lexemeTestAttemptRepository.UpdateTestResultAsync(lexeme);
            }
        }

    }
}
