using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWCSService.ApplicationService.WcsServer.Model
{
    public class ReceiveTaskCMD
    {
        public SendCMDEnum cmd;////  101=入库  102=出库  103=移库
        public int seq;//:1,		
        public int task_id;//:888,		//任务id，系统生命周期内唯一编码，从系统部署开始，从100开始一直累加。
        public int src_station;//起点站台
        public int dest_station;//目标站台
        public int src_rack;//1排,2纵深  :1,	//按排列层顺序	面朝仓库入口，按照从左至右(排),从前至后(列),从下至上(层)	
        public int src_col;//:2,//列
        public int src_row;//:3,//行
        public int dest_rack;//1排,2纵深    :4,//货架排
        public int dest_col;//:5,//列
        public int dest_row;//:6,//行 
        public double weight;//:999,		//1空托盘 0主机
        public string barcode;//"0000000000"//料箱编码 
    }
    public enum SendCMDEnum
    {
        /// <summary>
        /// 101=入库
        /// </summary>
        CMDIn = 101,
        /// <summary>
        /// 102=出库
        /// </summary>
        CMDOut = 102,
        /// <summary>
        ///  103=移库
        /// </summary>
        CMDMove = 103
    }
}
