using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTLibrary.Wms
{
    public class WmsHelper
    {
        /// <summary>
        /// 获取命令值
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static int getCmd(string json)
        {
            //  System.Text.RegularExpressions.Match matcoll = System.Text.RegularExpressions.Regex.Match(json, @"[,{]\s*""\s*cmd\s*""\s*[:]\s*(?<cmd>[\d]+)\s*[,}]", System.Text.RegularExpressions.RegexOptions.Multiline);
            // System.Text.RegularExpressions.Match matcoll = System.Text.RegularExpressions.Regex.Match(json, @"[,{]\s*""\s*cmd\s*""\s*[:]\s*""(?<cmd>[\d]+)""\s*[,}]", System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Text.RegularExpressions.Match matcoll = System.Text.RegularExpressions.Regex.Match(json, @"[,{]\s*""\s*cmd\s*""\s*[:]\s*""(?<cmd>[\d]+)""\s*[,}]|[,{]\s*""\s*cmd\s*""\s*[:]\s*(?<cmd>[\d]+)\s*[,}]", System.Text.RegularExpressions.RegexOptions.Multiline);
            if (matcoll.Success)
            {
                return Convert.ToInt32(matcoll.Groups["cmd"].Value);
            }
            return -1;
        }
    }
}
