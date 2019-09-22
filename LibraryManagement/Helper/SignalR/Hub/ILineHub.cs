
namespace Seagull.Doctors.Helper.SignalR.Hub
{
    public interface ISeagullHub
    {
        void CheckNotify();
        void NotifyClients();
    }
}
