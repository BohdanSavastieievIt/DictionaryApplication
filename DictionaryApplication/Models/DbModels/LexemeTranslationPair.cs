namespace DictionaryApplication.Models
{
    public class LexemeTranslationPair
    {
        public int LexemeId { get; set; }
        public Lexeme? Lexeme { get; set; }
        public int TranslationId { get; set; }
        public Lexeme? Translation { get; set; }
    }
}
