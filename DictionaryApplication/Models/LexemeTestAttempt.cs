namespace DictionaryApplication.Models
{
    public class LexemeTestAttempt
    {
        public Lexeme Lexeme { get; set; } = null!;
        public List<Lexeme> Translations { get; set; } = null!;
        public string? LexemeTestRepresentation { get; set; } = null!;
        public string? CorrectAnswerRepresentation { get; set; } = null!;
        public string? TestAnswer { get; set; }
        public bool IsCorrectAnswer { get; set; }
    }
}
