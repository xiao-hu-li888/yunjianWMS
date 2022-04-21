using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Basic
{
    public class sys_control_dicBLL : LTERPEFModel.ComDao<LTERPEFModel.Basic.sys_control_dic>
    {
        public sys_control_dicBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {

        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        public DateTime getServerDateTime()
        {
            DateTime dateTime = dbcontext.Database.SqlQuery<DateTime>(
              "select now()").FirstOrDefault();
            return dateTime;
        }

        /// <summary>
        /// 根据类型获取字典值(电池条码为空，默认自动添加。。。)
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public string GetValueByType(CommDictType dictType)
        {
            string _keyname = Enum.GetName(typeof(CommDictType), dictType);
            LTERPEFModel.Basic.sys_control_dic _dicObj = GetFirstDefault(w => w.key == _keyname);
            if (_dicObj != null)
            {
                return _dicObj.value;
            }
            //电池条码规则为空默认添加
            if (dictType == CommDictType.BatteryBarCodeRule)
            {//电池条码生成规则(T01-01-01/T02-01-01) M01-1-2  xxx-xxx-xxx
                string _val = @"^[\da-zA-Z]+-[\da-zA-Z]+-[\da-zA-Z]+$";
                Add(new LTERPEFModel.Basic.sys_control_dic()
                {
                    guid = Guid.NewGuid(),
                    key = _keyname,
                    value = _val
                });
                return _val;
            }
            return "";
        }
        /// <summary>
        /// 根据类型设置字典值
        /// </summary>
        /// <param name="dictType"></param>
        /// <param name="value"></param>
        public LTERPEFModel.SimpleBackValue SetValueByType(CommDictType dictType, string value)
        {
            string _keyname = Enum.GetName(typeof(CommDictType), dictType);
            LTERPEFModel.Basic.sys_control_dic _dicObj = GetFirstDefault(w => w.key == _keyname);
            if (_dicObj != null)
            {
                _dicObj.value = value;
                return Update(_dicObj);
            }
            else
            {
                return Add(new LTERPEFModel.Basic.sys_control_dic()
                {
                    guid = Guid.NewGuid(),
                    key = _keyname,
                    value = value
                });
            }
        }
    }
    public enum CommDictType
    {
        /// <summary>
        /// 发送任务至堆垛机:0 不发送， 1发送
        /// </summary>
        SendToStacker = 0,
        /// <summary>
        /// 库位分配方式 
        /// 0=最下层分配完再往上层分配（按层）
        /// 1=最上层分配完再往下层分配（按层）
        /// 2=从左至右先下后上（按列）
        /// 3=从左至右先上后下（按列）
        /// </summary>
        TaskDispatchWay = 1,
        /// <summary>
        ///  重新请求入库任务 0 默认  1重启A入库口线程（获取A入库任务） 2重启B入库口线程（获取B入库任务）
        /// </summary>
        OnExceptionDealingTaskIn = 2,
        /// <summary>
        /// 统一处理数据时间（默认凌晨2-4，不在这个时间不处理）
        /// </summary>
        DealDataTime = 3,
        /*************各种条码生成规则配置,条码前缀不能重复*****************/
        /// <summary>
        /// 物料条码生成规则(M1001/M1002)
        /// </summary>
        MatterBarcodeRule = 4,
        /// <summary>
        /// 物料编码生成规则(W101/W102)
        /// </summary>
        MatterCodeRule = 5,
        /// <summary>
        /// 入库单据编号生成规则(R101/R102)
        /// </summary>
        BillStockInOddRule = 6,
        /// <summary>
        /// 出库单据编号生成规则(C101/C102)
        /// </summary>
        BillStockOutOddRule = 7,
        /// <summary>
        /// 电池条码生成规则(T01-01-01/T02-01-01)
        /// </summary>
        BatteryBarCodeRule = 8,
        /// <summary>
        /// 托盘条码生成规则(01/02)
        /// </summary>
        TrayBarcodeRule = 9,
        /// <summary>
        /// 打印条码生成规则(P101/P102)
        /// </summary>
        PrintBarcodeRule = 10,
        /// <summary>
        /// 批次号生成规则(B101/B102)
        /// </summary>
        BatchNumberRule = 11,
        /// <summary>
        /// 电池订单编号规则(T01/T02)
        /// </summary>
        BatteryOrderNumberRule = 12,
        /*************各种条码生成规则配置*****************/
        /// <summary>
        /// 电池扫码：请求朝向
        /// </summary>
        ScanBatteryBarcode = 13,
        /// <summary>
        /// 电池扫码：响应结果
        /// </summary>
        ScanResultText = 14,
        /// <summary>
        /// Agv取料起点1
        /// </summary>
        AgvStartPos1 = 15,
        /// <summary>
        /// Agv取料起点2
        /// </summary>
        AgvStartPos2 = 16,
        /// <summary>
        /// 组盘超时时间（默认4小时）
        /// </summary>
        GroupTrayTimeOut=17
    }
}
