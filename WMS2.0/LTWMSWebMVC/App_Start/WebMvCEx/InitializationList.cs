using LTWMSEFModel;
using LTWMSEFModel.Basic;
using LTWMSEFModel.Bills;
using LTWMSEFModel.BillsAihua;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.query_model;
using LTWMSEFModel.Warehouse;
using LTWMSWebMVC.Areas.BasicData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LTWMSWebMVC.App_Start.WebMvCEx
{
    public class InitializationList
    {
        public InitializationList()
        {

        }
        /// <summary>
        /// 设置字典下拉框
        /// </summary>
        /// <param name="typename">字典类型</param>
        /// <param name="ListName">设置下拉框数据源名称</param>
        public static void SetDictDrpdownList(string DictType, string ListName)
        {
            SetDictDrpdownList(DictType, ListName, false);
        }
        /// <summary>
        /// 设置字典下拉框
        /// </summary>
        /// <param name="typename">字典类型</param>
        /// <param name="ListName">设置下拉框数据源名称</param>
        /// <param name="ValueIsName">值名称是否为同一内容</param>
        public static void SetDictDrpdownList(string DictType, string ListName, bool ValueIsName)
        {
            List<ListItem> list = new List<ListItem>();
            var d = App_Start.AutofacConfig.GetFromFac<LTWMSService.Basic.sys_dictionaryBLL>();
            IList<LTWMSEFModel.Basic.sys_dictionary> cmplist = d.GetListByParentID(DictType);
            if (ValueIsName)
                SetListToContext(cmplist, b => b.key, c => c.key, ListName);
            else
                SetListToContext(cmplist, b => b.guid.ToString(), c => c.key, ListName);
        }
        /// <summary>
        /// 登录用户
        /// </summary>
        public static void setEmployDropdownlist()
        {
            var d = App_Start.AutofacConfig.GetFromFac<LTWMSService.Basic.sys_loginBLL>();
            var EmployList = d.GetAllQuery();
            SetListToContext(EmployList, b => b.guid, c => c.loginname, "ChooseCheckLoginIDList");
        }
        /// <summary>
        /// 登录用户
        /// </summary>
        public static void setEmployDropdownlist(List<LTWMSEFModel.Basic.sys_login> emps)
        {
            SetListToContext(emps, b => b.guid, c => c.loginname, "ChooseCheckLoginIDList");
        }

        public static void SetListToContext<T, TValue, TText>(IEnumerable<T> list, Expression<Func<T, TValue>> value, Expression<Func<T, TText>> text, string name, bool isstatic = false)
        {
            if (list == null) return;
            List<ListItem> items = new List<ListItem>();
            foreach (T d in list)
            {
                items.Add(new ListItem(text.Compile().Invoke(d).ToString(), value.Compile().Invoke(d).ToString()));
            }
            if (isstatic)
                ListProvider.AddList(name, items, null);
            else
                ListProvider.AddList(name, items);
        }
        public static void SetListToContext<T, TValue, TText, TColor>(IEnumerable<T> list, Expression<Func<T, TValue>> value, Expression<Func<T, TText>> text, Expression<Func<T, TColor>> color, string name, bool isstatic = false)
        {
            if (list == null) return;
            List<ListItem> items = new List<ListItem>();
            foreach (T d in list)
            {
                items.Add(new ListItem(text.Compile().Invoke(d).ToString(), value.Compile().Invoke(d).ToString(), color.Compile().Invoke(d).ToString()));
            }
            if (isstatic)
                ListProvider.AddList(name, items, null);
            else
                ListProvider.AddList(name, items);
        }
        public string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }

        public void StaticList()
        {
            ListProvider.AddList("Active", new ListItem[] {new ListItem("启用","True","label label-sm label-success"),
            new ListItem("已禁用","False","label label-sm label-danger")}, null);

            ListProvider.AddList("gender", new ListItem[] { new ListItem("男", "True"), new ListItem("女", "False") }, null);
            ListProvider.AddList("YesNoState", new ListItem[] {new ListItem("是","True","label label-sm label-success"),
            new ListItem("否","False","label label-sm label-danger")}, null);
            ListProvider.AddList("State", new ListItem[] {
                new ListItem("启用","1","label label-sm label-success"),
                new ListItem("已禁用","0","label label-sm label-warning"),
                new ListItem("已删除","3","label label-sm label-danger")
            }, null);

            //MatterType_list  物料分类 电池/其它物料
            ListProvider.AddList("MatterType_list", new ListItem[] {
                new ListItem("电池","1"),
                new ListItem("其它物料","2")
            }, null);
            //////////////////
            List<ListItem> List_WcsTaskStatus = new List<ListItem>();
            foreach (WcsTaskStatus _e in Enum.GetValues(typeof(WcsTaskStatus)))
            {
                List_WcsTaskStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            //任务完成状态 
            ListProvider.AddList("WcsTaskStatus", List_WcsTaskStatus, null);
            /*************完成历史*****************/
            List<ListItem> List_WcsTaskStatusHis = new List<ListItem>();
            foreach (WcsTaskStatus _e in Enum.GetValues(typeof(WcsTaskStatus)))
            {
                if (_e == WcsTaskStatus.Canceled || _e == WcsTaskStatus.Finished
                    || _e == WcsTaskStatus.ForceComplete
                 )
                {
                    List_WcsTaskStatusHis.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
                }
            }
            //任务完成状态 
            ListProvider.AddList("WcsTaskStatusHis", List_WcsTaskStatusHis, null);
            /***************************/
            //任务类型 
            List<ListItem> List_WcsTaskType = new List<ListItem>();
            foreach (WcsTaskType _e in Enum.GetValues(typeof(WcsTaskType)))
            {
                List_WcsTaskType.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            //任务完成状态 
            ListProvider.AddList("WcsTaskType", List_WcsTaskType, null);

            //库位状态 
            List<ListItem> List_ShelfCellState = new List<ListItem>();
            foreach (ShelfCellState _e in Enum.GetValues(typeof(ShelfCellState)))
            {
                List_ShelfCellState.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("ShelfCellState", List_ShelfCellState, null);
            //锁类型 
            List<ListItem> List_ShelfLockType = new List<ListItem>();
            foreach (ShelfLockType _e in Enum.GetValues(typeof(ShelfLockType)))
            {
                List_ShelfLockType.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("ShelfLockType", List_ShelfLockType, null);
            //TrayStatus
            //托盘状态 
            List<ListItem> List_TrayStatus = new List<ListItem>();
            foreach (TrayStatus _e in Enum.GetValues(typeof(TrayStatus)))
            {
                if (_e == TrayStatus.OnShelf)
                {
                    List_TrayStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-success"));
                }
                else
                {
                    List_TrayStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-warning"));
                }

            }
            ListProvider.AddList("TrayStatus", List_TrayStatus, null);
            //单据进行状态 BillsStatus
            List<ListItem> List_BillsStatus = new List<ListItem>();
            foreach (BillsStatus _e in Enum.GetValues(typeof(BillsStatus)))
            {
                List_BillsStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("BillsStatus", List_BillsStatus, null);
            //BillsStatus_Out
            List<ListItem> List_BillsStatus_Out = new List<ListItem>();
            foreach (BillsStatus_Out _e in Enum.GetValues(typeof(BillsStatus_Out)))
            {
                List_BillsStatus_Out.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("BillsStatus_Out", List_BillsStatus_Out, null);

            //接收状态 
            //List<ListItem> List_AgvReceiveStatus = new List<ListItem>();
            //foreach (AgvReceiveStatus _e in Enum.GetValues(typeof(AgvReceiveStatus)))
            //{
            //    if (_e == AgvReceiveStatus.SendedError)
            //    {
            //        //label label-sm label-danger
            //        List_AgvReceiveStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-danger"));
            //    }
            //    else
            //    {
            //        List_AgvReceiveStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            //    }
            //} 
            //ListProvider.AddList("AgvReceiveStatus", List_AgvReceiveStatus, null);
            //运行状态 AgvTaskExecuteStatus
            //List<ListItem> List_AgvTaskExecuteStatus = new List<ListItem>();
            //foreach (AgvTaskExecuteStatus _e in Enum.GetValues(typeof(AgvTaskExecuteStatus)))
            //{
            //    List_AgvTaskExecuteStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            //} 
            //ListProvider.AddList("AgvTaskExecuteStatus", List_AgvTaskExecuteStatus, null);
            ////完成状态 AgvTaskStatus
            //List<ListItem> List_AgvTaskStatus = new List<ListItem>();
            //foreach (AgvTaskStatus _e in Enum.GetValues(typeof(AgvTaskStatus)))
            //{
            //    List_AgvTaskStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            //}
            //ListProvider.AddList("AgvTaskStatus", List_AgvTaskStatus, null);
            //报警来源 AlarmFrom
            //List<ListItem> List_AlarmFrom = new List<ListItem>();
            //foreach (AlarmFrom _e in Enum.GetValues(typeof(AlarmFrom)))
            //{
            //    List_AlarmFrom.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            //}
            //ListProvider.AddList("AlarmFrom", List_AlarmFrom, null);
            //PLCRunStatus 堆垛机、输送线状态 
            List<ListItem> List_PLCRunStatus = new List<ListItem>();
            foreach (PLCRunStatus _e in Enum.GetValues(typeof(PLCRunStatus)))
            {
                List_PLCRunStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("PLCRunStatus", List_PLCRunStatus, null);
            ////PLCType  plc类型（堆垛机、输送线） 
            //List<ListItem> List_PLCType = new List<ListItem>();
            //foreach (PLCType _e in Enum.GetValues(typeof(PLCType)))
            //{
            //    List_PLCType.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            //}
            //ListProvider.AddList("PLCType", List_PLCType, null);
            //WareHouseCategoriesEnum 仓库类型
            List<ListItem> List_WareHouseCategoriesEnum = new List<ListItem>();
            foreach (WareHouseCategoriesEnum _e in Enum.GetValues(typeof(WareHouseCategoriesEnum)))
            {
                List_WareHouseCategoriesEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("WareHouseCategoriesEnum", List_WareHouseCategoriesEnum, null);


            //DistributeWayEnum 存货策略
            List<ListItem> List_DistributeWayEnum = new List<ListItem>();
            foreach (DistributeWayEnum _e in Enum.GetValues(typeof(DistributeWayEnum)))
            {
                List_DistributeWayEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("DistributeWayEnum", List_DistributeWayEnum, null);


            //WcsServerType wcs服务类型
            List<ListItem> List_WcsServerType = new List<ListItem>();
            foreach (WcsServerType _e in Enum.GetValues(typeof(WcsServerType)))
            {
                List_WcsServerType.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("WcsServerType", List_WcsServerType, null);
            //设备类型 DeviceTypeEnum
            List<ListItem> List_DeviceTypeEnum = new List<ListItem>();
            foreach (DeviceTypeEnum _e in Enum.GetValues(typeof(DeviceTypeEnum)))
            {
                List_DeviceTypeEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("DeviceTypeEnum", List_DeviceTypeEnum, null);
            //出入库模式 StationModeEnum
            List<ListItem> List_StationModeEnum = new List<ListItem>();
            foreach (StationModeEnum _e in Enum.GetValues(typeof(StationModeEnum)))
            {
                List_StationModeEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("StationModeEnum", List_StationModeEnum, null);
            //出库处理逻辑 OutLogicEnum
            List<ListItem> List_OutLogicEnum = new List<ListItem>();
            foreach (OutLogicEnum _e in Enum.GetValues(typeof(OutLogicEnum)))
            {
                List_OutLogicEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("OutLogicEnum", List_OutLogicEnum, null);
            //预留单出库状态ReserveBillOutStatus 
            List<ListItem> List_ReserveBillOutStatus = new List<ListItem>();
            foreach (ReserveBillOutStatus _e in Enum.GetValues(typeof(ReserveBillOutStatus)))
            {
                //  List_ReserveBillOutStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
                if (_e == ReserveBillOutStatus.Finished)
                {
                    List_ReserveBillOutStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-success"));
                }
                else if (_e == ReserveBillOutStatus.Running)
                {
                    List_ReserveBillOutStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-warning"));
                }
                else
                {
                    List_ReserveBillOutStatus.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-default"));
                }
            }
            ListProvider.AddList("ReserveBillOutStatus", List_ReserveBillOutStatus, null);
            //单据任务完成状态（通用）TaskStatusEnum
            List<ListItem> List_TaskStatusEnum = new List<ListItem>();
            foreach (TaskStatusEnum _e in Enum.GetValues(typeof(TaskStatusEnum)))
            {
                List_TaskStatusEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("TaskStatusEnum", List_TaskStatusEnum, null);
            //库位分配方式 StockDistributeEnum
            List<ListItem> List_StockDistributeEnum = new List<ListItem>();
            foreach (StockDistributeEnum _e in Enum.GetValues(typeof(StockDistributeEnum)))
            {
                List_StockDistributeEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("StockDistributeEnum", List_StockDistributeEnum, null);
            //货架同侧标记 SameSideMarkEnum
            List<ListItem> List_SameSideMarkEnum = new List<ListItem>();
            foreach (SameSideMarkEnum _e in Enum.GetValues(typeof(SameSideMarkEnum)))
            {
                // List_SameSideMarkEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString())); 
                if ((int)_e % 2 == 0)
                {
                    List_SameSideMarkEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-success"));
                }
                else
                {
                    List_SameSideMarkEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-warning"));
                }
            }
            ListProvider.AddList("SameSideMarkEnum", List_SameSideMarkEnum, null);
            //StockInType
            List<ListItem> List_StockInType = new List<ListItem>();
            foreach (StockInType _e in Enum.GetValues(typeof(StockInType)))
            {
                List_StockInType.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("StockInType", List_StockInType, null);

            //TestStatusEnum
            List<ListItem> List_TestStatusEnum = new List<ListItem>();
            foreach (TestStatusEnum _e in Enum.GetValues(typeof(TestStatusEnum)))
            {
                // List_TestStatusEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
                if (_e == TestStatusEnum.TestOk)
                {
                    List_TestStatusEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-success"));
                }
                else if (_e == TestStatusEnum.TestFail)
                {
                    List_TestStatusEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-warning"));
                }
                else
                {
                    List_TestStatusEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-default"));
                }
            }
            ListProvider.AddList("TestStatusEnum", List_TestStatusEnum, null);
            //StockOutType
            List<ListItem> List_StockOutType = new List<ListItem>();
            foreach (StockOutType _e in Enum.GetValues(typeof(StockOutType)))
            {
                List_StockOutType.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("StockOutType", List_StockOutType, null);
            // SpecialLockTypeEnum
            List<ListItem> List_SpecialLockTypeEnum = new List<ListItem>();
            foreach (SpecialLockTypeEnum _e in Enum.GetValues(typeof(SpecialLockTypeEnum)))
            {
                List_SpecialLockTypeEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("SpecialLockTypeEnum", List_SpecialLockTypeEnum, null);
            //MatterOrderEnum
            List<ListItem> List_MatterOrderEnum = new List<ListItem>();
            foreach (MatterOrderEnum _e in Enum.GetValues(typeof(MatterOrderEnum)))
            {
                List_MatterOrderEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("MatterOrderEnum", List_MatterOrderEnum, null);
            //LockUnLockEnum
            List<ListItem> List_LockUnLockEnum = new List<ListItem>();
            foreach (LockUnLockEnum _e in Enum.GetValues(typeof(LockUnLockEnum)))
            {
                List_LockUnLockEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("LockUnLockEnum", List_LockUnLockEnum, null);
            //TrayInStockStatusEnum
            List<ListItem> List_TrayInStockStatusEnum = new List<ListItem>();
            foreach (TrayInStockStatusEnum _e in Enum.GetValues(typeof(TrayInStockStatusEnum)))
            {
                List_TrayInStockStatusEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("TrayInStockStatusEnum", List_TrayInStockStatusEnum, null);
            //TrayOutStockStatusEnum
            List<ListItem> List_TrayOutStockStatusEnum = new List<ListItem>();
            foreach (TrayOutStockStatusEnum _e in Enum.GetValues(typeof(TrayOutStockStatusEnum)))
            {
                List_TrayOutStockStatusEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("TrayOutStockStatusEnum", List_TrayOutStockStatusEnum, null);
            // SearchBillsStatus_In
            List<ListItem> List_SearchBillsStatus_In = new List<ListItem>();
            foreach (SearchBillsStatus_In _e in Enum.GetValues(typeof(SearchBillsStatus_In)))
            {
                List_SearchBillsStatus_In.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("SearchBillsStatus_In", List_SearchBillsStatus_In, null);
            //  SearchBillsStatus_Out
            List<ListItem> List_SearchBillsStatus_Out = new List<ListItem>();
            foreach (SearchBillsStatus_Out _e in Enum.GetValues(typeof(SearchBillsStatus_Out)))
            {
                List_SearchBillsStatus_Out.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString()));
            }
            ListProvider.AddList("SearchBillsStatus_Out", List_SearchBillsStatus_Out, null);

            //BillsInOutEnum
            List<ListItem> List_BillsInOutEnum = new List<ListItem>();
            foreach (BillsInOutEnum _e in Enum.GetValues(typeof(BillsInOutEnum)))
            {  
                if (_e == BillsInOutEnum.BillsIn)
                {
                    List_BillsInOutEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-success"));
                }
                else
                {
                    List_BillsInOutEnum.Add(new ListItem(GetEnumDescription(_e), ((int)_e).ToString(), "label label-sm label-warning"));
                }
            }
            ListProvider.AddList("BillsInOutEnum", List_BillsInOutEnum, null);
        }
    }
}