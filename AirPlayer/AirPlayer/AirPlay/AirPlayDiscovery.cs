using System.Linq;
using Zeroconf;

namespace AirPlayer.AirPlay
{
    public class AirPlayDiscovery
    {
        public delegate void AirplayServiceFoundDelegate(IZeroconfHost item);
        public event AirplayServiceFoundDelegate AirplayServiceFound;

        /// <summary>
        /// Initializes a new instance of the <see cref="AirPlayDiscovery"/> class.
        /// </summary>
        public AirPlayDiscovery()
        {
            ProbeForNetworkAirplayDevices();
        }

        public async void ProbeForNetworkAirplayDevices()
        {
            var domains = await ZeroconfResolver.BrowseDomainsAsync();
            
            var results = await ZeroconfResolver.ResolveAsync(domains.Where(x => x.Key.Contains("airplay")).Select(g => g.Key));

            //var results = await ZeroconfResolver.ResolveAsync("_airplay._tcp.local");
            foreach (var result in results)
            {
                AirplayServiceFound?.Invoke(result);
            }
        }
    }
}
