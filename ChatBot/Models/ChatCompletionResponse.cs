namespace ChatBot.Models
{
    public class ChatCompletionResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public List<ChatCompletionChoise> Choices { get; set; }
        public ChatCompletionUsage Usage { get; set; }
    }
}
