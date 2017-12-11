using System.Threading.Tasks;

namespace NetCoreBoilerplate.Services.Messaging
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}