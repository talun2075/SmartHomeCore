using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SmartHome.Classes
{
    public class ColoClient
    {
        private const string COMMAND_PREFIX = "535a30300000000000";
        private const string COMMAND_CONFIG = "20000000000000000000000000000000000100000000000000000004010301c";
        private const string COMMAND_EFFECT = "23000000000000000000000000000000000100000000000000000004010602ff";
        private const int port = 8900;

        #region Properties
        public IPAddress Host { get; set; }

        public enum Effects
        {
            Club80s,
            CherryBlossom,
            CocktailParade,
            Instagrammer,
            Pensieve,
            Savasana,
            Sunrise,
            TheCircus,
            Unicorns,
            Christmas,
            RainbowFlow,
            MusicMode
        }

        private readonly NameValueCollection EffectsLookup = new() { { "Club80s", "049a0000" }, { "CherryBlossom", "04940800" }, { "CocktailParade", "05bd0690" }, { "Instagrammer", "03bc0190" }, { "Pensieve", "04c40600" }, { "Savasana", "04970400" }, { "Sunrise", "01c10a00" }, { "TheCircus", "04810130" }, { "Unicorns", "049a0e00" }, { "Christmas", "068b0900" }, { "RainbowFlow", "03810690" }, { "MusicMode", "07bd0990" } };
        #endregion Properties
        #region Ctor
        public ColoClient()
        {
        }
        public ColoClient(string IP)
        {
            if (IPAddress.TryParse(IP, out IPAddress parsed))
            {
                Host = parsed;
            }
            else
            {
                throw new ArgumentException("IP can´t parse", IP);
            }
        }
        public ColoClient(IPAddress IP)
        {
            Host = IP;
        }
        #endregion Ctor
        #region Public Methods
        public async Task<Boolean> TurnOn()
        {
            string command = string.Format("{0}{1}{2}", COMMAND_PREFIX, COMMAND_CONFIG, "f35");

            return await SendMessage(command);
        }

        public async Task<Boolean> TurnOff()
        {
            string command = string.Format("{0}{1}{2}", COMMAND_PREFIX, COMMAND_CONFIG, "e1e");

            return await SendMessage(command);
        }
        public async Task<Boolean> SetEffect(Effects effect)
        {
            string effectValue = EffectsLookup.Get(Enum.GetName(typeof(Effects), effect));

            string command = string.Format("{0}{1}{2}", COMMAND_PREFIX, COMMAND_EFFECT, effectValue);

            return await SendMessage(command);
        }

        public async Task<Boolean> SetBrightness(int brightness)
        {
            if (brightness < 0)
                brightness = 0;
            else if (brightness > 100)
                brightness = 100;

            string prefix = "f";
            string command = string.Format("{0}{1}{2}{3}", COMMAND_PREFIX, COMMAND_CONFIG, prefix, brightness.ToString("X2"));

            return await SendMessage(command);
        }

        public async Task<Boolean> SetColour(System.Drawing.Color colour)
        {
            string hexColour = colour.R.ToString("X2") + colour.G.ToString("X2") + colour.B.ToString("X2");

            return await SetColour(hexColour);
        }

        public async Task<Boolean> SetColour(string colour)
        {
            string prefix = "00";
            string command = string.Format("{0}{1}{2}{3}", COMMAND_PREFIX, COMMAND_EFFECT, prefix, colour);

            return await SendMessage(command);
        }
        #endregion Public Methods
        #region Private Methods
        private async Task<Boolean> SendMessage(string message)
        {
            if (PingHost(this.Host.ToString()))
            {
                UdpClient udpClient = new();
                IPEndPoint ipEndPoint = new(this.Host, port);

                try
                {
                    udpClient.Connect(ipEndPoint);

                    byte[] dgram = StringToByteArray(message);

                    if (dgram.Length > 0)
                    {
                        await udpClient.SendAsync(dgram, dgram.Length);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
                finally
                {
                    udpClient.Close();
                }
            }

            return false;
        }
        private static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {

                pinger?.Dispose();
            }

            return pingable;
        }
        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;

            if (NumberChars % 2 == 0)
            {
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                return bytes;
            }
            else
            {
                return Array.Empty<byte>();
            }
        }
        #endregion Private Methods
    }
}
