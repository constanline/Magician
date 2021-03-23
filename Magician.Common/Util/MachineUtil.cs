using System;
using System.Collections.Generic;
using System.Management;

namespace Magician.Common.Util
{
    public class MachineUtil
    {
        public static string GetSerialNumber()
        {
            string result;
            try
            {
                var managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                string text = null;
                using (var enumerator = managementObjectSearcher.Get().GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        var managementObject = (ManagementObject) enumerator.Current;
                        text = managementObject["SerialNumber"].ToString().Trim();
                    }
                }

                result = text;
            }
            catch
            {
                result = "";
            }

            return result;
        }

        public static string GetMacAddress()
        {
            string result;
            try
            {
                var text = "";
                var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var instances = managementClass.GetInstances();
                foreach (var managementBaseObject in instances)
                {
                    var managementObject = (ManagementObject) managementBaseObject;
                    if ((bool) managementObject["IPEnabled"]) text += managementObject["MACAddress"].ToString();
                }

                result = text;
            }
            catch
            {
                result = "";
            }

            return result;
        }

        public static List<CpuInfo> GetCpuInfo()
        {
            var managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            var managementObjectCollection = managementObjectSearcher.Get();
            var num = managementObjectCollection.Count;

            var list = new List<CpuInfo>();
            foreach (var managementBaseObject in managementObjectCollection)
            {
                var managementObject2 = (ManagementObject) managementBaseObject;
                list.Add(new CpuInfo(managementObject2.GetPropertyValue("ProcessorId").ToString(),
                    managementObject2.GetPropertyValue("Name").ToString(),
                    (uint) managementObject2.GetPropertyValue("CurrentClockSpeed"),
                    (uint) (Environment.ProcessorCount / num)));
            }

            return list;
        }
    }

    public struct CpuInfo
    {
        public CpuInfo(string processorId, string name, uint currentClockSpeed, uint coreNum)
        {
            ProcessorId = processorId;
            Name = name;
            CurrentClockSpeed = currentClockSpeed;
            CoreNum = coreNum;
        }

        public string ProcessorId;

        public string Name;

        public uint CurrentClockSpeed;

        public uint CoreNum;
    }
}