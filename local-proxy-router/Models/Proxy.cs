using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalProxyRouter.Models
{
    public class Proxy
    {
        public string IP { get; set; }          // Địa chỉ IP của proxy
        public int PORT { get; set; }            // Cổng của proxy
        public string REMOTE_IP { get; set; }    // Địa chỉ IP từ xa
        public int REMOTE_PORT { get; set; }      // Cổng từ xa

        // Constructor
        public Proxy(string ip, int port, string remoteIp, int remotePort)
        {
            IP = ip;
            PORT = port;
            REMOTE_IP = remoteIp;
            REMOTE_PORT = remotePort;
        }

        // Phương thức để hiển thị thông tin proxy
        public override string ToString()
        {
            return $"Proxy: {IP}:{PORT} -> Remote: {REMOTE_IP}:{REMOTE_PORT}";
        }
    }

}
