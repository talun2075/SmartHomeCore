using Microsoft.Extensions.Options;
using SharpModbus;
using SmartHome.Classes.ModbusWrapper.Config;
using SmartHome.Classes.ModbusWrapper.Enums;
using SmartHome.Classes.ModbusWrapper.Model;
using System;

namespace SmartHome.Classes.ModbusWrapper
{

    public class DeyeModbus : IDeyeModbus
    {
        private readonly IOptionsMonitor<ModbusOptions> _options;

        public DeyeModbus(IOptionsMonitor<ModbusOptions> options)
        {
            _options = options;
        }

        public DeyeDto ReadData()
        {
            DeyeDto retval = new DeyeDto();
            try
            {
                using (var master = ModbusMaster.TCP(_options.CurrentValue.Server, _options.CurrentValue.Port))
                {
                    #region Serial
                    //SerialNumber
                    var data = master.ReadHoldingRegisters(1, 3, 5);
                    string serial = string.Empty;
                    byte[] byteArray;
                    foreach (var reg in data)
                    {
                        byteArray = BitConverter.GetBytes(reg);
                        serial += string.Format("{0:d}{1:d}", Convert.ToChar(byteArray[1]).ToString(), Convert.ToChar(byteArray[0]).ToString());
                    }
                    retval.Serial = serial;
                    #endregion Serial
                    #region Battery
                    //Battery %
                    var batterydatat = master.ReadHoldingRegisters(1, 586, 5);
                    var batteryper = batterydatat[2];
                    retval.Battery.FilledInPercent = batteryper.ToString();
                    retval.Battery.CurrentVoltage = batterydatat[1].ToString();
                    //Batterie Ussage
                    var batterycurrentpower = batterydatat[4];
                    // var hex = singledata2.ToString("X");//todo: nötig bei minus?
                    //Console.WriteLine("Batterie Ladestatus (Hex convert): " + Convert.ToInt16(hex, 16));
                    int intValue = batterycurrentpower - (batterycurrentpower > 32767 ? 65536 : 0);//convert to signed int
                    retval.Battery.CurrentPower = intValue;
                    var batterytemp = batterydatat[0];
                    retval.Battery.Temperatur = (Convert.ToDecimal(batterytemp - 1000) / 10).ToString();
                    #endregion Battery
                    #region Inverter
                    var deyestate = master.ReadHoldingRegister(1, 500);
                    if (Enum.TryParse(deyestate.ToString(), out DeyeState ds))
                    {
                        retval.Deye.State = ds;
                    }
                    var deyestempac = master.ReadHoldingRegister(1, 541);
                    retval.Deye.InverterACTemperature = (Convert.ToDecimal(deyestempac - 1000) / 10).ToString();
                    var powerstate = master.ReadHoldingRegister(1, 551);
                    retval.Deye.PowerOn = powerstate == 1;
                    #endregion Inverter
                    #region Home
                    retval.HomeUse.Daily = master.ReadHoldingRegister(1, 526);
                    retval.HomeUse.Total = master.ReadHoldingRegister(1, 527);
                    retval.HomeUse.Current = master.ReadHoldingRegister(1, 643);
                    #endregion Home
                    #region Grid


                    //Grid
                    var gridcurrent = master.ReadHoldingRegister(1, 607);
                    retval.Grid.GridCurrent = gridcurrent - (gridcurrent > 32767 ? 65536 : 0);//convert to signed int

                    var giddata = master.ReadHoldingRegisters(1, 520, 5);
                    retval.Grid.DailyBuy = giddata[0];
                    retval.Grid.DailySell = giddata[1];
                    retval.Grid.TotalBuy = giddata[2];
                    retval.Grid.TotalSell = giddata[4];
                    #endregion Grid
                    #region Photovoltaics
                    //PV Produktion
                    var pv = master.ReadHoldingRegisters(1, 672, 4);
                    retval.Photovoltaics.PV1CurrentPower = pv[0];
                    retval.Photovoltaics.PV2CurrentPower = pv[1];
                    retval.Photovoltaics.PV3CurrentPower = pv[2];
                    retval.Photovoltaics.PV4CurrentPower = pv[3];
                    retval.Photovoltaics.Daily = master.ReadHoldingRegister(1, 529);
                    retval.Photovoltaics.Total = master.ReadHoldingRegister(1, 534);
                    #endregion Photovoltaics

                    //test
                    var k = master.ReadHoldingRegister(1, 62);

                }
            }
            catch (Exception ex)
            {
                retval.CommunicationErrors = ex.Message;
            }
            return retval;
        }
    }
}