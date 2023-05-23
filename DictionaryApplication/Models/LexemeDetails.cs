using DictionaryApplication.Data;
using Newtonsoft.Json.Linq;

namespace DictionaryApplication.Models
{
    public class LexemeDetails
    {
        public Lexeme Lexeme { get; set; }
        public List<Lexeme> Translations { get; set; }
        public string TranslationsRepresentation { get; set; }
        public double TestResults { get; set; }
        public string TestResultsRepresentation { get; set; }

        public LexemeDetails(ApplicationDbContext context, int lexemeId)
        {
            var lexeme = context.Lexemes.FirstOrDefault(x => x.Id == lexemeId);
            if (lexeme == null)
            {
                throw new ArgumentException("Lexeme was not found.");
            } 
            Lexeme = lexeme;

            Translations = context.Lexemes.Where(x =>
                        context.LexemeTranslationPairs
                            .Where(y => y.LexemeId == Lexeme.Id)
                            .Select(y => y.TranslationId).Contains(x.Id))
                        .ToList();

            TranslationsRepresentation = Translations.Count > 1 
                ? string.Join(Environment.NewLine, Translations.Select(x => x.Word).Select((s, i) => $"{i + 1}. {s}"))
                : Translations.Select(x => x.Word).First();

            if (Lexeme.TotalTestAttempts > 0)
            {
                TestResults = (double)Lexeme.CorrectTestAttempts / Lexeme.TotalTestAttempts;
            }
            else
            {
                TestResults = 0;
            }

            if (Lexeme.TotalTestAttempts == 0)
            {
                TestResultsRepresentation = "0 attempts";
            }
            else
            {
                string results = string.Format("{0,6:##0.00; }", TestResults * 100);
                TestResultsRepresentation = $"{results} % — {Lexeme.CorrectTestAttempts} out of {Lexeme.TotalTestAttempts}";
            }
        }

    }
}
