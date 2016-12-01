using System.Linq;
using Zeroconf;

namespace AirPlayer.AirPlay
{
    public class AirPlayDiscovery
    {
        public delegate void AirplayServiceFoundDelegate(IZeroconfHost item);

        /// <summary>
        ///     Used for getting all AirPlay devices on the local network
        /// </summary>
        public AirPlayDiscovery()
        {
            ProbeForNetworkAirplayDevices();
        }

        public event AirplayServiceFoundDelegate AirplayServiceFound;

        public async void ProbeForNetworkAirplayDevices()
        {
            var domains = await ZeroconfResolver.BrowseDomainsAsync();

            var results =
                await ZeroconfResolver.ResolveAsync(domains.Where(x => x.Key.Contains("airplay")).Select(g => g.Key));

            foreach (var result in results)
            {
                AirplayServiceFound?.Invoke(result);
            }
        }
    }
}