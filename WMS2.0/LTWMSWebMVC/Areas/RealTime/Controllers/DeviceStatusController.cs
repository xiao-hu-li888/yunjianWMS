using LTWMSWebMVC.Areas.RealTime.Data;
using LTWMSEFModel.Hardware;
using LTWMSService.Hardware;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTWMSService.Warehouse;
using LTWMSEFModel.Warehouse;

namespace LTWMSWebMVC.Areas.RealTime.Controllers
{
    /// <summary>
    /// 设备状态控制器（堆垛机、输送线、Agv）
    /// </summary>
    public class DeviceStatusController : BaseController
    {
        //  hdw_agvBLL bll_hdw_agv;
        hdw_plcBLL bll_hdw_plc;
        wh_wcs_srvBLL bll_wh_wcs_srv;
        wh_wcs_deviceBLL bll_wh_wcs_device;
        public DeviceStatusController(hdw_plcBLL bll_hdw_plc, wh_wcs_srvBLL bll_wh_wcs_srv, wh_wcs_deviceBLL bll_wh_wcs_device)
        {
            //this.bll_hdw_agv = bll_hdw_agv;
            this.bll_hdw_plc = bll_hdw_plc;
            this.bll_wh_wcs_srv = bll_wh_wcs_srv;
            this.bll_wh_wcs_device = bll_wh_wcs_device;
        }
        // GET: RealTime/DeviceStatus
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DeviceStatusRealTime()
        {
            //DeviceList Model = new DeviceList();
            //Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            ////Model.ListDevAgv = bll_hdw_agv.GetAllQueryOrderby(o => o.number, w => w.state == LTWMSEFModel.EntityStatus.Normal, true)
            ////    .Select(s=> MapperConfig.Mapper.Map<hdw_agv,Data.DevAgvModel>(s)).ToList();
            //Model.ListDevPlc = bll_hdw_plc.GetAllQueryOrderby(o => o.number, w => w.warehouse_guid== warehouseguid 
            //&& w.state == LTWMSEFModel.EntityStatus.Normal, true)
            //    .Select(s => MapperConfig.Mapper.Map<hdw_plc, Data.DevPlcModel>(s)).ToList();
            //return PartialView("ViewList", Model);
            Guid warehouseguid = GetCurrentLoginUser_WareGuid();
            //通过当前warehouse 查询对应关联的wcs
            List<LTWMSWebMVC.Areas.Setting.Data.WcsSrvModel> Model = bll_wh_wcs_srv.GetAllWcsSrvByWarehouseguid(warehouseguid).Select(s =>
                {
                    List<DevPlcModel> ListPLC = bll_hdw_plc.GetAllQuery(w => w.warehouse_guid == warehouseguid&&w.shvwcs_srv_guid==s.guid).Select(s =>
                    MapperConfig.Mapper.Map<hdw_plc, Data.DevPlcModel>(s)).ToList();
                    var wcsSrvModel = MapperConfig.Mapper.Map<wh_wcs_srv, LTWMSWebMVC.Areas.Setting.Data.WcsSrvModel>(s);
                    wcsSrvModel.List_wcsDeviceModel = bll_wh_wcs_device.GetAllQueryOrderby(w => w.device_type, w => w.state == LTWMSEFModel.EntityStatus.Normal
                    &&w.warehouse_guid== warehouseguid && w.wcs_srv_guid == wcsSrvModel.guid, false).Select(s =>
                    {
                        var wcsDevM = MapperConfig.Mapper.Map<wh_wcs_device, LTWMSWebMVC.Areas.Setting.Data.WcsDeviceModel>(s);
                        wcsDevM.DevPlcModel = ListPLC.Where(w => w.shvwcs_srv_guid == wcsDevM.wcs_srv_guid && w.number == wcsDevM.number).FirstOrDefault();
                        return wcsDevM;
                    }).ToList();
                    return wcsSrvModel;
                }).ToList();

            return PartialView("ViewList", Model);
        }

    }
}