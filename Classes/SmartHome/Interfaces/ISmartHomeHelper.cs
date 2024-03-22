using InnerCore.Api.DeConz.Models;
using SmartHome.Classes.Deconz;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SmartHome.Classes.SmartHome.Interfaces
{
    public interface ISmartHomeHelper
    {
        Task<DeConzResults> DeconzAllLightsOff();
        Task<DeConzResults> DeconzEating(DeconzSwitch hs);
        Task<DeConzResults> DeconzFirstFloorOff();
        Task<string> DeconzGardenOff();
        Task<string> DeconzGardenOn();
        Task<DeConzResults> DeconzGroundFloorOff();
        Task<DeConzResults> DeconzGroundFloorOn();
        Task<DeConzResults> DeconzLightsPower(List<string> ids, bool PowerOn = true);
        Task<DeConzResults> DeconzLightsPower(string id, bool PowerOn = true);
        Task<bool> PowerOffAurora(string selectedaurora);
        Task<bool> PowerOffAuroras();
        Task<bool> PowerOffDenon(bool ignoreinput = false);
        Task<bool> PowerOnAurora(string selectedaurora);
        Task<bool> PowerOnAuroras(string room);
        Task<bool> PowerOffAuroras(string room);
        Task<bool> PowerOnDenon();
        Task<bool> PowerShellysGuestRoom(bool powerOn);
        Task<bool> PowerShellysGuestRoomRight(bool powerOn);
        Exception ReturnWebError(string message, HttpStatusCode hsc = HttpStatusCode.BadRequest);
        Task<bool> SonosGroundFloorOff();
        Task<bool> SonosGroundFloorOn(string playlist);
        Task<bool> SonosGuestRoom(string playlist, int volume = 0);
        Task<bool> SonosWorkRoom(string playlist, int volume = 0);
        Task<bool> SonosWorkRoomOff();
        Task<bool> CupeLiving(int id);
        Task<bool> SonosGuestRoomAudioInOff();
        Task<bool> SonosGuestRoomAudioInOn();
        Task<bool> SonosGuestRoomOff();
        Task<bool> SonosGuestRoomVolume(bool vol);
        Task<bool> SonosIanRoomOff();
        Task<bool> SonosIanRoomRandom2();
        Task<bool> SonosIanRoomRandom3();
        Task<bool> SonosFinnRoomRandom1();
        Task<bool> SonosFinnRoomOff();
        Task<bool> SonosFinn(string playlist);
        Task<bool> SonosLivingRoomSpezial();
        Task<bool> SonosLivingRoomVolume(bool vol);
        Task<bool> SonosStoppAllPlayer();
        string Test();
    }
}