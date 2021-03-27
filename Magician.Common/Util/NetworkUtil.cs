using System.Text.RegularExpressions;

namespace Magician.Common.Util
{
    class NetworkUtil
    {

        public static bool ValidateIpAddress(string ipAddress)
        {
            var regex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            return ipAddress != "" && regex.IsMatch(ipAddress.Trim());
        }
    }
}
