namespace DictionaryApplication.Models
{
    public class UserDictionary
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int StudiedLangId { get; set; }
        public Language? StudiedLanguage { get; set; }
        public int TranslationLangId { get; set; }
        public Language? TranslationLanguage { get; set; }
        public ICollection<Lexeme>? Lexemes { get; set; }
    }
}
