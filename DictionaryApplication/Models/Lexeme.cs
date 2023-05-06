namespace DictionaryApp.Models
{
    public class Lexeme
    {
        public int Id { get; set; }
        public int LangId { get; set; }
        public Language? LexemeLanguage { get; set; }
        public string Word { get; set; } = null!;
        public int TotalTestAttempts { get; set; }
        public int CorrectTestAttempts { get; set; }
        public ICollection<LexemeDefinition>? LexemeDefinitions { get; set; }
        public ICollection<DictionaryLexemePair> DictionaryLexemePairs { get; set; } = null!;
        public ICollection<LexemePair>? Lexeme1Pairs { get; set; }
        public ICollection<LexemePair>? Lexeme2Pairs { get; set; }
    }
}
