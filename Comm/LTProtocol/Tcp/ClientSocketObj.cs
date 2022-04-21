using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LTProtocol.Tcp
{
    public class ClientSocketObj
    {
        /// <summary>
        /// ip+端口
        /// </summary>
        public string key;
        /// <summary>
        /// 连接socket
        /// </summary>
        public Socket socket;
    }
}
