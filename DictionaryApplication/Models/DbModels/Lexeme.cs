namespace DictionaryApplication.Models
{
    public class Lexeme
    {
        public int Id { get; set; }
        public int LangId { get; set; }
        public Language? LexemeLanguage { get; set; }
        public int DictionaryId { get; set; }
        public UserDictionary? Dictionary { get; set; } = null!;
        public string Word { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalTestAttempts { get; set; }
        public int CorrectTestAttempts { get; set; }
        public ICollection<LexemeTranslationPair>? LexemePairs { get; set; }
        public ICollection<LexemeTranslationPair>? TranslationPairs { get; set; }
    }
}
