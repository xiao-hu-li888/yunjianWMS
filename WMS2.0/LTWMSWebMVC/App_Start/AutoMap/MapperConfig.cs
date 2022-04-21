using AutoMapper;
using LTWMSWebMVC.Areas.Admin.Models;
using LTWMSWebMVC.Areas.BasicData.Data;
using LTWMSWebMVC.Areas.Bills.Data;
using LTWMSWebMVC.Areas.History.Data;
using LTWMSWebMVC.Areas.RealTime.Data;
using LTWMSWebMVC.Areas.System.Data;
using LTWMSEFModel.Basic;
using LTWMSEFModel.Bills;
using LTWMSEFModel.Hardware;
using LTWMSEFModel.Logs;
using LTWMSEFModel.Stock;
using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LTWMSWebMVC.Areas.Setting.Data;
using LTWMSEFModel.BillsAihua;
using LTWMSEFModel.query_model;

namespace LTWMSWebMVC
{
    public class MapperConfig
    {
        private static IMapper mapper;
        public static void Config()
        {
            var config = new MapperConfiguration(cfg =>
            {
                //  cfg.AddProfile<AppProfile>();
                cfg.CreateMap<sys_login, sysloginModel>().ReverseMap();
                cfg.CreateMap<hdw_stacker_taskqueue, StackerTaskQueueModel>().ReverseMap();
                cfg.CreateMap<wh_shelves, ShelvesModel>().ReverseMap();
                cfg.CreateMap<wh_warehouse, WareHouseModel>().ReverseMap();
                cfg.CreateMap<sys_role, sysRoleModel>().ReverseMap();
                cfg.CreateMap<wh_shelfunits, ShelfUnitsModel>().ReverseMap();
                cfg.CreateMap<log_sys_alarm, AlarmLogModel>().ReverseMap();
                cfg.CreateMap<hdw_stacker_taskqueue_his, StackerTaskQueueHisModel>().ReverseMap();
                cfg.CreateMap<log_sys_useroperation, UserOperationLogModel>().ReverseMap();
                cfg.CreateMap<log_sys_execute, ExecuteLogModel>().ReverseMap();
                cfg.CreateMap<wh_tray, TrayModel>().ReverseMap();
                cfg.CreateMap<stk_matter, MatterModel>().ReverseMap();
                cfg.CreateMap<stk_mattertype, MatterTypeModel>().ReverseMap();
                cfg.CreateMap<bill_stockout, StockOutModel>().ReverseMap();
                cfg.CreateMap<bill_stockin, StockInModel>().ReverseMap();
                cfg.CreateMap<wh_traymatter, TrayMatterModel>().ReverseMap();
                //cfg.CreateMap<hdw_agv_task_main, AgvTaskMainModel>().ReverseMap();
                //cfg.CreateMap<hdw_agv_taskqueue, AgvTaskQueueModel>().ReverseMap();
                cfg.CreateMap<wh_service_status, ServiceStatusModel>().ReverseMap();
                // cfg.CreateMap<hdw_agv_taskqueue, hdw_agv_taskqueue_his>().ReverseMap();

                cfg.CreateMap<wh_wcs_device, WcsDeviceModel>().ReverseMap();
                cfg.CreateMap<wh_warehouse_type, WarehouseTypeModel>().ReverseMap();
                cfg.CreateMap<hdw_plc, DevPlcModel>().ReverseMap();
                cfg.CreateMap<wh_wcs_srv, WcsSrvModel>().ReverseMap();
                //cfg.CreateMap<hdw_agv_taskqueue_his, AgvTaskQueueHisModel>().ReverseMap();
                cfg.CreateMap<billah_reserved_order, ReservedOrderModel>().ReverseMap();
                cfg.CreateMap<billah_reserved_order_detail, ReservedOrderDetailModel>().ReverseMap();

                cfg.CreateMap<StockMatter, MatterModel>().ReverseMap();
                cfg.CreateMap<bill_stockin_detail, StockInDetailModel>().ReverseMap();
                cfg.CreateMap<bill_stockout_detail, StockOutDetailModel>().ReverseMap();

                cfg.CreateMap<bill_stockin_detail_traymatter, bill_stockin_detail_traymatterModel > ().ReverseMap();
                cfg.CreateMap<bill_stockout_detail_traymatter, bill_stockout_detail_traymatterModel > ().ReverseMap();

                cfg.CreateMap<BillsGather, BillsGatherModel>().ReverseMap(); 
            });
            mapper = config.CreateMapper();
        }
        public static IMapper Mapper
        {
            get
            {
                return mapper;
            }
        }
    }
}