using System.Threading.Tasks;

namespace CarAuctionSystem.Console.Commands
{
    public interface ICommandHandler
    {
        Task HandleCommandAsync(string command);
    }
}