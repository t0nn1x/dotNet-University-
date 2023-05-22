namespace AspDotNetLab3
{
    public static class Utils
    {
        public static string GetClientIpAddress(HttpContext context)
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress;
            string result = string.Empty;
            if (remoteIpAddress is not null)
            {
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                result = remoteIpAddress.ToString();
            }
            return result;
        }
        public static string GetWrappedByTag(string tagName, string? text)
        {
            return $"<{tagName}>" + text + $"</{tagName}>";
        }
    }
}
