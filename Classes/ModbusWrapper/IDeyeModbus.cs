using SmartHome.Classes.ModbusWrapper.Model;

namespace SmartHome.Classes.ModbusWrapper
{
    public interface IDeyeModbus
    {
        DeyeDto ReadData();
    }
}