using ChatBot.Services;

namespace ChatBot.Models
{
    public class OpenAIService
    {
        readonly HttpClient _httpClient;
        readonly string _apiKey;
        readonly ILogger<OpenAIService> _logger;
        readonly JsonSettingsStore _settingsStore;
        readonly IHttpContextAccessor _httpContextAccessor;

        public OpenAIService(HttpClient httpClient, string apiKey, ILogger<OpenAIService> logger, JsonSettingsStore settingsStore, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _logger = logger;
            _settingsStore = settingsStore;
            _httpContextAccessor = httpContextAccessor;
        }

        public string ConcatenateSettings(Settings settings)
        {
            string settingsPrompt = $"Styl długości odpowiedzi: {settings.MessageLength}, Ton: {settings.Tone}, ";
            if (!String.IsNullOrWhiteSpace(settings.DefaultProgrammingLanguage))
                settingsPrompt += $"Domyślny język programowania: {settings.DefaultProgrammingLanguage}, ";
            if (!String.IsNullOrWhiteSpace(settings.DefaultFramework))
                settingsPrompt += $"Domyślny framework: {settings.DefaultFramework}, ";
            if (settings.UsePolishAsDefaultLanguage)
                settingsPrompt += "Odpowiadaj po polsku. ";
            if (settings.IncludeShortcutSources)
                settingsPrompt += "Pisz źródła skrótów, np. h jak heading w HTML. ";
            if (settings.UseShortCSharpSyntax)
                settingsPrompt += "Używaj skróconej składni C#, np. używaj primary constructor, uproszczonej składni new, body expressions, embedded statements. ";
            if (settings.AvoidVar)
                settingsPrompt += "Unikaj używania słowa kluczowego var. ";
            if (settings.UseTechnicalFileNames)
                settingsPrompt += "Nazywaj technicznie pliki, np. resmon.exe zamiast monitor zasobów. ";
            if (settings.CheckForeignLanguageAccuracy)
                settingsPrompt += "Jeśli piszę w obcym języku, sprawdź poprawność i powiadom mnie jeśli cokolwiek napisałem błędnie. ";
            if (settings.ProvideAllPolishTranslations)
                settingsPrompt += "Tłumacząc słówko lub idiom na polski, podaj wszystkie tłumaczenia. ";
            if (settings.ForeignTranslationWithProperPartOfSpeech)
                settingsPrompt += "Tłumaczenie na język obcy podaj w odpowiedniej części mowy. ";
            if (settings.IncludeLegalReferences)
                settingsPrompt += "Do każdej porady prawnej dodaj podstawę prawną (artykuł, przepis). ";
            if (settings.InstructionsInEnglishUI)
                settingsPrompt += "Korzystam z oprogramowania w języku angielskim. Uwzględnij to w swoich radach. ";
            if (settings.UseNumericRegex)
                settingsPrompt += "Używaj [0-9] zamiast \\d w regex. ";
            if (settings.AnswerOnlyIfCertain)
                settingsPrompt += "Jeśli nie jesteś pewien odpowiedzi, to jej nie wymyślaj, tylko napisz, że nie wiesz. ";
            if (settings.UseRichVocabulary)
                settingsPrompt += "Używaj bogatego zasobu słów. ";
            if (settings.ProvideWordAssociations)
                settingsPrompt += "Podawaj skojarzenia słówek lub idiomów z języka obcego, np. vehicle można skojarzyć z wehikułem. ";
            if (settings.NoImmediatePuzzleSolutions)
                settingsPrompt += "Jeśli podajesz zadanie lub zagadkę do rozwiązania, to pozwól mi pomyśleć - nie podawaj odpowiedzi razem z pytaniem. ";
            if (settings.UseEnvironmentVariablesForPaths)
                settingsPrompt += "Używaj zmiennych środowiskowych do ścieżek, np. pisz %tmp% zamiast C:\\Users\\Admin\\AppData\\Local\\Temp. ";
            if (settings.WriteOnlyChanges)
                settingsPrompt += "Jeśli zmieniasz coś w moim kodzie, to nie wklejaj całego kodu, tylko to, co zmieniłeś. ";
            if (settings.CustomProperties is not null)
                settingsPrompt += settings.CustomProperties;

            return settingsPrompt;
        }
        public async Task<string> GetAnswerAsync(string userPrompt, ChatMemory memory)
        {
            string? userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            Settings currentSettings = null;
            if (userId is not null)
                currentSettings = _settingsStore.Deserialize(userId);
            if (currentSettings is null)
                currentSettings = new(); // domyślne ustawienia
            try
            {
                memory.Messages.Add(new { role = "user", content = userPrompt });
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                string systemPrompt = "Bądź pomocnym asystentem. " + ConcatenateSettings(currentSettings);

                List<object> messages = new() { new { role = "system", content = systemPrompt } };
                messages.AddRange(memory.Messages);

                var requestBody = new
                {
                    model = "gpt-4o-mini",
                    messages = messages.ToArray(),
                    max_tokens = currentSettings.MaxTokens,
                    temperature = currentSettings.Creativity
                };
                using var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("OpenAI API error: {StatusCode}, Content: {ErrorContent}",
                        response.StatusCode, errorContent);
                    return "Przepraszam, nie udało się uzyskać odpowiedzi od AI.";
                }

                var jsonResponse = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();
                var answer = jsonResponse?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
                if (!string.IsNullOrEmpty(answer))
                    memory.Messages.Add(new { role = "assistant", content = answer });
                return string.IsNullOrEmpty(answer)
                    ? "Przepraszam, nie udało się uzyskać odpowiedzi od AI."
                    : answer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas komunikacji z OpenAI API.");
                return "Przepraszam, wystąpił błąd podczas komunikacji z AI.";
            }
        }
    }
}
