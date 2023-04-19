namespace DictionaryApp.Models
{
    public class LexemeDefinition
    {
        public int Id { get; set; }
        public int LexemeId { get; set; }
        public Lexeme Lexeme { get; set; } = null!;
        public string Definition { get; set; } = null!;
        public ICollection<LexemeUsageExample> LexemeUsageExamples { get; set; } = null!;
    }
}
