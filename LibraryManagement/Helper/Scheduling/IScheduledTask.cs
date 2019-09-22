using System.Threading;
using System.Threading.Tasks;

namespace Seagull.Tqweemco.Scheduling
{
    public interface IScheduledTask
    {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}