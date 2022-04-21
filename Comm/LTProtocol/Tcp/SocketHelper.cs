using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTProtocol.Tcp
{
    public class SocketHelper
    {

        /// <summary>
        /// 0x02{}0x03
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] GetWrappedTextArray(string text, SocketClientEncoding encoding, byte[] StartDataArray, byte[] EndDataArray)
        {
            //   Encoding.GetEncoding("BG2312").GetString(new byte[1] { 2 }); 
            byte[] oldbytes;
            if (encoding == SocketClientEncoding.GB2312)
            {
                oldbytes = Encoding.GetEncoding("GB2312").GetBytes(text);
            }
            else
            {
                oldbytes = Encoding.UTF8.GetBytes(text);
            }
            byte[] newbytes = new byte[oldbytes.Length + StartDataArray.Length + EndDataArray.Length];
            for (int i = 0; i < StartDataArray.Length; i++)
            {
                newbytes[i] = StartDataArray[i];
            }
            oldbytes.CopyTo(newbytes, StartDataArray.Length);
            for (int i = 0; i < EndDataArray.Length; i++)
            {
                newbytes[newbytes.Length - EndDataArray.Length + i] = EndDataArray[i];
            }
            return newbytes;
        }


    }
}
