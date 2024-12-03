using SmartHome.Classes.SmartHome.Data;
using System.Threading.Tasks;

namespace SmartHome.Classes.SmartHome.Interfaces
{
    public interface ISmartHomeWrapper
    {
        Task<bool> CupeLiving(int id);
        Task<bool> Double(string mac);
        Task<bool> Long(string mac);
        Task<bool> PowerShellysGuestRoom(bool powerOn = false);
        Task<bool> Single(string mac);
        Task<bool> Touch(string mac, ButtonRequest br = null);
    }
}