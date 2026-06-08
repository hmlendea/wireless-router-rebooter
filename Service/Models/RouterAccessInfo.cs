namespace WirelessRouterRebooter.Service.Models
{
    public sealed class RouterAccessInfo
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string IpAddress { get; set; }

        public string GetIpAddressOrDefault(string defaultIpAddress)
            => string.IsNullOrWhiteSpace(IpAddress)
                ? defaultIpAddress
                : IpAddress;
    }
}
