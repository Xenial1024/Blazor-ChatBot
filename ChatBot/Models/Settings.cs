namespace ChatBot.Models
{
    public class Settings
    {
        public string UserId { get; set; }
        public string MessageLength { get; set; } = "normal"; // wartości: minimalist, concise, normal, wordy
        public string Tone { get; set; } = "neutral"; //wartości: friendly, formal, humorous, neutral
        public string? DefaultProgrammingLanguage { get; set; }
        public string? DefaultFramework { get; set; }
        public float Creativity { get; set; } = 0.7f;
        public int MaxTokens { get; set; } = 1000;
        public bool SuggestPrompts { get; set; } = true;
        public bool UsePolishAsDefaultLanguage { get; set; } = true;
        public bool IncludeShortcutSources { get; set; } = false;
        public bool UseShortCSharpSyntax { get; set; } = false;
        public bool AvoidVar { get; set; } = false;
        public bool UseTechnicalFileNames { get; set; } = false;
        public bool CheckForeignLanguageAccuracy { get; set; } = false;
        public bool ProvideAllPolishTranslations { get; set; } = false;
        public bool ForeignTranslationWithProperPartOfSpeech { get; set; } = false;
        public bool IncludeLegalReferences { get; set; } = false;
        public bool InstructionsInEnglishUI { get; set; } = false;
        public bool UseNumericRegex { get; set; } = false;
        public bool AnswerOnlyIfCertain { get; set; } = false;
        public bool UseRichVocabulary { get; set; } = false;
        public bool ProvideWordAssociations { get; set; } = false;
        public bool NoImmediatePuzzleSolutions { get; set; } = false;
        public bool UseEnvironmentVariablesForPaths { get; set; } = false;
        public bool WriteOnlyChanges { get; set; } = false;
        public string? CustomProperties { get; set; }
    }
}
