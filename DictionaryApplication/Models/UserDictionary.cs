namespace DictionaryApp.Models
{
    public class UserDictionary
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int StudiedLangId { get; set; }
        public Language StudiedLanguage { get; set; } = null!;
        public int TranslationLangId { get; set; }
        public Language TranslationLanguage { get; set; } = null!;
        public ICollection<DictionaryLexemePair> DictionaryLexemePairs { get; set; } = null!;
    }
}
