namespace DictionaryApp.Models
{
    public class LexemeUsageExample
    {
        public int Id { get; set; }
        public int LexemeDefinitionId { get; set; }
        public LexemeDefinition LexemeDefinition { get; set; } = null!;
        public string UsageExample { get; set; } = null!;
    }
}
