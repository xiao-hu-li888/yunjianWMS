using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Basic
{
    public class sys_control_dicBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Basic.sys_control_dic>
    {
        public sys_control_dicBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {

        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        public DateTime getServerDateTime()
        {
            string _cmdStr;
           if ( dbcontext.Database.Connection.ToString()== "System.Data.SqlClient.SqlConnection")
            {//sqlserver
                _cmdStr = "select GETDATE()";
            }
            else
            {//mysql
                _cmdStr = "select now()";
            }
            //sqlserver select GETDATE()
            //mysql select now()
                DateTime dateTime = dbcontext.Database.SqlQuery<DateTime>(
             _cmdStr).FirstOrDefault();
            return dateTime;
        }

        /// <summary>
        /// 根据类型获取字典值(电池条码为空，默认自动添加。。。)
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public string GetValueByType(CommDictType dictType, Guid WareHouseGuid)
        {
            string _keyname = Enum.GetName(typeof(CommDictType), dictType);
            LTWMSEFModel.Basic.sys_control_dic _dicObj = GetFirstDefault(w => w.key == _keyname && (WareHouseGuid == Guid.Empty || w.warehouse_guid == WareHouseGuid));
            if (_dicObj != null)
            {
                return _dicObj.value;
            }
            ////电池条码规则为空默认添加
            //if (dictType == CommDictType.BatteryBarCodeRule)
            //{//电池条码生成规则(T01-01-01/T02-01-01) M01-1-2  xxx-xxx-xxx
            //    string _val = @"^[\da-zA-Z]+-[\da-zA-Z]+-[\da-zA-Z]+$";
            //    Add(new LTWMSEFModel.Basic.sys_control_dic()
            //    {
            //        guid = Guid.NewGuid(),
            //        key = _keyname,
            //        value = _val
            //    });
            //    return _val;
            //}
            //托盘条码规则 A101
            if (dictType == CommDictType.TrayBarcodeRule)
            {
                string _val = @"^[TB][\d]{3,5}$";

                var a = new LTWMSEFModel.Basic.sys_control_dic();
                if (WareHouseGuid != Guid.Empty)
                {
                    a.warehouse_guid = WareHouseGuid;
                }
                a.createdate = DateTime.Now;
                a.guid = Guid.NewGuid();
                a.key = _keyname;
                a.value = _val;
                Add(a);
                return _val;
            }
            return "";
        }
        /// <summary>
        /// 根据类型设置字典值
        /// </summary>
        /// <param name="dictType"></param>
        /// <param name="value"></param>
        public LTWMSEFModel.SimpleBackValue SetValueByType(CommDictType dictType, string value, Guid WareHouseGuid)
        {
            string _keyname = Enum.GetName(typeof(CommDictType), dictType);
            LTWMSEFModel.Basic.sys_control_dic _dicObj = GetFirstDefault(w => w.key == _keyname && (WareHouseGuid == Guid.Empty || w.warehouse_guid == WareHouseGuid));
            if (_dicObj != null)
            {
                _dicObj.updatedate = DateTime.Now;
                _dicObj.value = value;
                return Update(_dicObj);
            }
            else
            {
                var a = new LTWMSEFModel.Basic.sys_control_dic();
                if (WareHouseGuid != Guid.Empty)
                {
                    a.warehouse_guid = WareHouseGuid;
                }
                a.createdate = DateTime.Now;
                a.guid = Guid.NewGuid();
                a.key = _keyname;
                a.value = value;
                return Add(a);
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
        GroupTrayTimeOut = 17,
        /// <summary>
        /// 发送任务至所有堆垛机
        /// </summary>
        SendTaskToAllStackers = 18,
        /***
         * 后期可以通过配置的形式  配置扫描口 关联对应的入库口 进行预分配入库！！！
         * *********************/
        /// <summary>
        /// 1号Rfid请求预分配站台（每次取到后置空）
        /// </summary>
        BarcodeOfRequest_Rfid1 = 19,
        /// <summary>
        /// 2号Rfid请求预分配站台（每次取到后置空）
        /// </summary>
        BarcodeOfRequest_Rfid2 = 20,
        /// <summary>
        /// 3号Rfid请求预分配站台（每次取到后置空）
        /// </summary>
        BarcodeOfRequest_Rfid3 = 21,
        /// <summary>
        /// (RGV1)扫码入库口1 站台分配
        /// </summary>
        StationDispatch1 = 22,
        /// <summary>
        /// (RGV2)扫码入库口2 站台分配
        /// </summary>
        StationDispatch2 = 23,
        /// <summary>
        /// (RGV3)扫码入库口3 站台分配
        /// </summary>
        StationDispatch3 = 24,
        /// <summary>
        ///  密码过期时间配置（过期每次登录进行提示！）单位:秒
        /// </summary>
        PassWordExpiration = 25,
        /// <summary>
        /// 登录超时时间配置(单位:秒)
        /// </summary>
        LoginTimeOut=26,
        /// <summary>
        /// 临近有效期
        /// </summary>
        NearTerm=27
    }
}
