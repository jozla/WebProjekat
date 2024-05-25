namespace Gateway.Helpers.ChatHub
{
    public interface IChatHub
    {
        Task SendMessage(string message);
    }
}
