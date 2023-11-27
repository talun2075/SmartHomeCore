using SmartHome.Classes.Shelly.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHome.Classes.Shelly
{
    public interface IShellyWorker
    {
        Task<bool> PowerGuestRoom(bool PowerOn = false);
        Task<bool> PowerGuestRoomLeft(bool PowerOn = false);
        Task<bool> PowerGuestRoomRight(bool PowerOn = false);
        Task<bool> PowerRoom(string room, bool PowerOn = false);
        Task<bool> PowerRooms(List<string> shellys, bool PowerOn = false);
        Task<List<Shelly1>> Read();
        Task<bool> Switch();
    }
}