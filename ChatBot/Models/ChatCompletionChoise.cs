namespace ChatBot.Models
{
    public class ChatCompletionChoise
    {
        public int Index { get; set; }
        public ChatCompletionMessage Message { get; set; }
        public string FinishReason { get; set; }
    }
}
