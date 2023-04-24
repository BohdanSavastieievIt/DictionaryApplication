namespace DictionaryApp.Models
{
    public class LexemePair
    {
        public int Lexeme1Id { get; set; }
        public Lexeme? Lexeme1 { get; set; }
        public int Lexeme2Id { get; set; }
        public Lexeme? Lexeme2 { get; set; }
        public LexemeRelationType LexemeRelationType { get; set; }
    }

    public enum LexemeRelationType
    {
        Translations,
        Synonyms,
        Antonyms
    }
}
