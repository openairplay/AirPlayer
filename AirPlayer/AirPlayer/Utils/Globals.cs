using System.Linq;
using System.Management;
using System.Threading;

namespace AirPlayer.Utils
{
    static internal class Globals
    {
        static Globals()
        {
            // Create a semaphore with the amount of physical cores of the computer
            var coreCount = new ManagementObjectSearcher("Select * from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["NumberOfCores"].ToString()));
            Semaphore = new Semaphore(coreCount,coreCount);
        }

        internal static Semaphore Semaphore;
    }
}