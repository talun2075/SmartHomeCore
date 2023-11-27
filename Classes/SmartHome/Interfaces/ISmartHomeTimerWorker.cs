using System.Threading.Tasks;

namespace SmartHome.Classes.SmartHome.Interfaces
{
    public interface ISmartHomeTimerWorker
    {
        Task<bool> CheckTimer();
    }
}