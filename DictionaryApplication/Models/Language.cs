namespace DictionaryApp.Models
{
    public class Language
    {
        public int Id { get; set; }
        public string LangCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int IsNativeCount { get; set; }
        public int IsStudiedCount { get; set; }
        public ICollection<UserDictionary> StudiedUserDictionaries { get; set; } = null!;
        public ICollection<UserDictionary> TranslationUserDictionaries { get; set; } = null!;
        public ICollection<Lexeme> Lexemes { get; set; } = null!;
    }
}
