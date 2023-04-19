namespace DictionaryApp.Models
{
    public class DictionaryLexemePair
    {
        public int UserDictionaryId { get; set; }
        public UserDictionary UserDictionary { get; set; } = null!;
        public int LexemeId { get; set; }
        public Lexeme Lexeme { get; set; } = null!;
    }
}
