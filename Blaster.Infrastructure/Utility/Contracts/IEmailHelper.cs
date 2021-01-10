namespace Blaster.Infrastructure.Utility.Contracts
{
    public interface IEmailHelper
    {
        void Render(string to, string subject, string template, object data);

        void Send(string to, string subject, string body);
    }
}
