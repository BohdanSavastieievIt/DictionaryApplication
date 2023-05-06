using DictionaryApp.Data;
using DictionaryApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DictionaryApplication.Data
{
    public class KnowledgeTestManager
    {
        private readonly ApplicationDbContext _context;

        public KnowledgeTestManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ContainAnyLexemes(IEnumerable<int> idsOfDictionaries)
        {
            return _context.DictionaryLexemePairs
                .Where(x => idsOfDictionaries.Contains(x.UserDictionaryId))
                .Count() != 0;
        }
        public int GetTotalLexemesAmount(IEnumerable<int> idsOfDictionaries)
        {
            return GetAllLexemesFromDictionaries(idsOfDictionaries).Count;
        }
        public bool IsValid(KnowledgeTestModel testModel)
        {
            return testModel != null &&
                testModel.SelectedDictionaryIds != null
                && testModel.SelectedDictionaryIds.Count > 0
                && ContainAnyLexemes(testModel.SelectedDictionaryIds)
                && testModel.NumberOfWords > 0;
        }
        private List<Lexeme> GetTestLexemes(KnowledgeTestModel testModel)
        {
            Random random = new Random();
            var lexemes = GetAllLexemesFromDictionaries(testModel.SelectedDictionaryIds);
            switch (testModel.TestType)
            {
                case TestType.AllWords:
                    return lexemes.OrderBy(l => random.Next())
                        .Take(testModel.NumberOfWords)
                        .ToList();
                case TestType.LastWords:
                    return lexemes.TakeLast(testModel.NumberOfWords)
                        .ToList();
                case TestType.WordsWithWorstResults:
                    return lexemes.OrderBy(x =>
                        x.TotalTestAttempts == 0
                        ? random.Next()
                        : x.CorrectTestAttempts / x.TotalTestAttempts)
                        .ThenBy(x => random.Next())
                        .Take(testModel.NumberOfWords)
                        .ToList();
                default:
                    return lexemes;
            }

            // implement language direction
        }
        public (List<(int LexemeId, string Lexeme)>, List<(int LexemeId, string Translation)>) GetTestLexemesAndTranslations(KnowledgeTestModel testModel)
        {
            var lexemesRes = new List<(int, string)>();
            var translationsRes = new List<(int, string)>();
            var lexemes = GetTestLexemes(testModel);

            foreach (var lexeme in lexemes)
            {
                lexemesRes.Add((lexeme.Id, lexeme.Word));

                //var currentTranslations = lexeme.Lexeme1Pairs?.Select(x => _context.Lexemes.FirstOrDefault(y => y.Id == x.Lexeme1Id));
                var lexemePairs = _context.LexemePairs.Where(x => x.Lexeme2Id == lexeme.Id
                                    && x.LexemeRelationType == LexemeRelationType.Translations)
                        .Select(x=> x.Lexeme1Id).ToList();
                var currentTranslations = _context.Lexemes.Where(x => lexemePairs.Contains(x.Id)).ToList();

                if (currentTranslations != null && currentTranslations.Any())
                {
                    foreach (var translation in currentTranslations)
                    {
                        translationsRes.Add((lexeme.Id, translation.Word));
                    }
                }
            }

            return testModel.IsTranslationFromMainLanguage
                ? (lexemesRes, translationsRes)
                : (translationsRes, lexemesRes);
        }
        public List<Lexeme> GetAllLexemesFromDictionaries(IEnumerable<int> idsOfDictionaries)
        {
            return _context.Lexemes
                .Where(x => idsOfDictionaries
                    .Contains(x.DictionaryLexemePairs
                        .FirstOrDefault(y => y.IsStudiedLexeme) != null
                        ? x.DictionaryLexemePairs
                        .FirstOrDefault(y => y.IsStudiedLexeme)
                        .UserDictionaryId
                        : -1))
                .ToList();
        }
        public List<(int LexemeId, string Answer)> GetWrongAnswers(
            List<(int LexemeId, string Lexeme)> testLexemes,
            List<(int LexemeId, string Lexeme)> testTranslations,
            List<(int LexemeId, string Answer)> answers)
        {
            var wrongAnswers = new List<(int LexemeId, string Answer)>();
            var correctAnswers = new List<(int LexemeId, string Answer)>();

            for (int i = 0; i < answers.Count; i++)
            {
                bool isCorrect = false;
                var currentWordTranslations = testTranslations
                    .Where(x => x.LexemeId == testLexemes[i].LexemeId)
                    .Select(x => x.Lexeme); 
                if (currentWordTranslations == null)
                {
                    continue;
                }

                foreach(var translation in currentWordTranslations)
                {
                    if (IsCorrectAnswer(answers[i].Answer, translation))
                    {
                        isCorrect = true;
                        break;
                    }
                }

                if (isCorrect)
                {
                    correctAnswers.Add(answers[i]);
                }
                else
                {
                    wrongAnswers.Add(answers[i]);
                }
            }

            foreach (var answer in correctAnswers)
            {
                var lexeme = _context.Lexemes.First(x => answer.LexemeId == x.Id);
                lexeme.TotalTestAttempts++;
                lexeme.CorrectTestAttempts++;
                _context.SaveChanges();
            }

            return wrongAnswers;
        }

        public bool IsCorrectAnswer(string correctAnswer, string userAnswer)
        {
            // Преобразование строк в нижний регистр и удаление пробелов в начале и конце
            correctAnswer = correctAnswer.ToLower().Trim();
            userAnswer = userAnswer.ToLower().Trim();

            // Удаление всех запятых и пробелов
            correctAnswer = correctAnswer.Replace(",", "").Replace(" ", "");
            userAnswer = userAnswer.Replace(",", "").Replace(" ", "");

            // Сравнение строк без учета регистра и возможных опечаток
            if (correctAnswer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Попытка исправления опечаток и сравнение исправленных строк
            var levensteinDistance = LevenshteinDistance(correctAnswer, userAnswer);
            if (levensteinDistance <= 2)
            {
                return true;
            }

            // Если не удалось сравнить строки, то возвращаем false
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

    }
}
