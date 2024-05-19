namespace Gateway.Helpers
{
    public interface IChatHub
    {
        Task SendMessage(string message);
    }
}
