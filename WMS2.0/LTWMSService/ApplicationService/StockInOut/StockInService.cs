using LTWMSEFModel.Warehouse;
using LTWMSService.ApplicationService.Model;
using LTWMSService.Basic;
using LTWMSService.Stock;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.StockInOut
{
    /// <summary>
    /// 入库逻辑（复用）
    /// </summary>
    public class StockInService : BaseService
    {
        stk_matterBLL bll_stk_matter;
        sys_control_dicBLL bll_sys_control_dic;
        wh_traymatterBLL bll_wh_traymatter;
        wh_trayBLL bll_wh_tray;
        public StockInService(LTWMSEFModel.LTModel dbcontext, sys_control_dicBLL bll_sys_control_dic, stk_matterBLL bll_stk_matter, wh_traymatterBLL bll_wh_traymatter, wh_trayBLL bll_wh_tray) : base(dbcontext)
        {
            this.bll_sys_control_dic = bll_sys_control_dic;
            this.bll_stk_matter = bll_stk_matter;
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_wh_tray = bll_wh_tray;
        }
        /// <summary>
        /// 检查条码并验证
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public ComServiceReturn CheckBarcodeType(string barcode)
        {
            int type = 0;
            string text = "";
            string traybarcode = bll_sys_control_dic.GetValueByType(CommDictType.TrayBarcodeRule, Guid.Empty);
            if (Regex.IsMatch(barcode, traybarcode))
            {//托盘条码
                //料箱条码B101... 托盘条码T101 指令C101
                //正常简短条码：^B[\d]{3,4}$  >>BOX-101
                //蓝天遗留条码规则：^LT-TP-[\d]+|LTZN[\d]+$
                text = barcode + "料箱条码";
                type = 1;
            }
            else
            {
                var MatterObj = bll_stk_matter.GetFirstDefault(w => w.code == barcode);
                if (MatterObj != null && MatterObj.guid != Guid.Empty)
                {//系统中存在物料条码，即将绑定数据
                    type = 2;
                }
                else
                {//错误或不存在
                    type = 3;
                    text = "条码错误";
                }
                /*  //从物料中查询是否存在条码
                  for (int i = 0; i < 4; i++)
                  {
                      var aaaaa = new LTWMSEFModel.Stock.stk_matter();
                      aaaaa.brand_name = "测试品牌";
                      aaaaa.code = "code1-" + i;
                      aaaaa.createdate = DateTime.Now;
                      aaaaa.createuser = "admin";
                      aaaaa.description = "型号";
                      aaaaa.guid = Guid.NewGuid();
                      aaaaa.mattertype_name = "分类";
                      aaaaa.memo = "备注";
                      aaaaa.name = "物料11";
                      aaaaa.specs = "规格";
                      aaaaa.state = LTWMSEFModel.EntityStatus.Normal;
                      aaaaa.unit_measurement = "单位"; 
                      bll_stk_matter.Add(aaaaa);
                  }*/

            }
            //检查条码是否是托盘条码
            //检查条码是否是物料条码/包装袋条码/箱包装条码 
            //检查条码是否合法，不合法请重扫
            return new ComServiceReturn() { success = true, result = text, data = type };
        }
        /// <summary>
        /// 绑定托盘和物料信息（组盘）
        /// </summary>
        /// <param name="traybarcode"></param>
        /// <param name="dataArr"></param>
        /// <returns></returns>
        public ComServiceReturn SaveBind(string traybarcode, List<MatterBarcode> dataArr, wh_tray trayAddition = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(traybarcode))
                {
                    return JsonReturn(false, "托盘条码为空");
                }
                if (dataArr == null || dataArr.Count() == 0)
                {//空托盘组盘
                    return SaveTrayBind(traybarcode, dataArr, trayAddition);
                }
                //数据去重,条码为空也去除
                List<MatterBarcode> lstD = new List<MatterBarcode>();
                foreach (var item in dataArr)
                {
                    if (!string.IsNullOrWhiteSpace(item.matter_code) && !lstD.Exists(w => w.matter_code == item.matter_code))
                    {
                        lstD.Add(item);
                    }
                }
                dataArr = lstD;
                /*
                //判断包装条码是否与其它托盘有绑定关系？有则提示 
                string[] _mArr = dataArr.Select(s => s.barcode).ToArray();
                var traymatterlist = bll_wh_traymatter.GetAllQuery(w => _mArr.Contains(w.matter_barcode));
                if (traymatterlist != null && traymatterlist.Count > 0)
                {
                    bool flag = false;
                    foreach (var item in traymatterlist)
                    {
                        var _trayMd = bll_wh_tray.GetFirstDefault(w => w.guid == item.tray_guid);
                        if (_trayMd != null && _trayMd.traybarcode != Model.traybarcode)
                        {
                            flag = true;
                            AddJsonError("绑定失败，电池条码：" + item.matter_barcode + " 与托盘：" + _trayMd.traybarcode
                                + " 已存在绑定关系！托盘在库状态：" + LTLibrary.EnumHelper.GetEnumDescription(_trayMd.status) + "");
                        }
                    }
                    if (flag)
                    {
                        return JsonError();
                    }
                }
                */
                return SaveTrayBind(traybarcode, dataArr, trayAddition);

            }
            catch (Exception ex)
            {
                throw new Exception("保存数据出错！异常信息：" + ex.Message);
                // AddJsonError("保存数据出错！异常信息：" + ex.Message);
                ///AddUserOperationLog("[PDA]保存托盘电池条码绑定关系失败！异常：=>>" + ex.ToString());
            }
            //return JsonError();
            // return new ComServiceReturn() { success = true, result = text, data = type };
        }

        private ComServiceReturn SaveTrayBind(string traybarcode, List<MatterBarcode> dataArr, wh_tray trayAddition = null)
        {
            string _mess = "";
            bool empty = true;
            if (dataArr != null && dataArr.Count() > 0)
            {//非空托盘
                empty = false;
            }
            var _trayM = bll_wh_tray.GetFirstDefault(w => w.traybarcode == traybarcode);
            LTWMSEFModel.SimpleBackValue rtv1;
            if (_trayM == null|| _trayM.guid==Guid.Empty)
            {//系统没有则新增一条记录
                _trayM = new LTWMSEFModel.Warehouse.wh_tray();
                _trayM.traybarcode = traybarcode;
                _trayM.createdate = DateTime.Now;
                _trayM.createuser = "system";
                _trayM.emptypallet = empty;
                _trayM.guid = Guid.NewGuid();
                _trayM.state = LTWMSEFModel.EntityStatus.Normal;
                _trayM.status = LTWMSEFModel.Warehouse.TrayStatus.OffShelf;
                _trayM.isscan = true;
                _trayM.scandate = bll_sys_control_dic.getServerDateTime();
                if (trayAddition != null)
                {
                    _trayM.dispatch_shelfunits_guid = trayAddition.dispatch_shelfunits_guid;
                    _trayM.dispatch_shelfunits_pos = trayAddition.dispatch_shelfunits_pos;
                }
                rtv1 = bll_wh_tray.Add(_trayM);
            }
            else
            {
                //立库中存在且在库状态，不能修改
                if (_trayM.status == TrayStatus.OnShelf || _trayM.shelfunits_guid != null)
                {//在库状态不能组盘
                    return JsonReturn(false,"托盘【"+ traybarcode + "】在库状态不能组盘！");
                }
                ////////////////////////////
                _trayM.updatedate = DateTime.Now;
                _trayM.emptypallet = empty;
                _trayM.isscan = true;
                _trayM.scandate = bll_sys_control_dic.getServerDateTime();
                if (trayAddition != null)
                {
                    _trayM.dispatch_shelfunits_guid = trayAddition.dispatch_shelfunits_guid;
                    _trayM.dispatch_shelfunits_pos = trayAddition.dispatch_shelfunits_pos;
                }
                else
                {
                    _trayM.dispatch_shelfunits_guid = null;
                    _trayM.dispatch_shelfunits_pos = "";
                }
                rtv1 = bll_wh_tray.Update(_trayM);
            }
            //托盘电池绑定表
            // 删除托盘与电池条码的绑定关系
            //bll_wh_traymatter.Delete(w => w.tray_guid == _trayM.guid);
            var dellist = bll_wh_traymatter.GetAllQuery(w => w.tray_guid == _trayM.guid);
            if (dellist != null && dellist.Count > 0)
            {
                foreach (var item in dellist)
                {
                    bll_wh_traymatter.Delete(item);
                }
            }
            List<wh_traymatter> lstTrayMatt = new List<wh_traymatter>();
            if (dataArr != null && dataArr.Count > 0)
            {
                foreach (var item in dataArr)
                {
                    wh_traymatter _tray = new wh_traymatter();
                    _tray.createdate = DateTime.Now;
                    _tray.createuser = "system";
                    _tray.guid = Guid.NewGuid();
                    _tray.state = LTWMSEFModel.EntityStatus.Normal;
                    _tray.tray_guid = _trayM.guid;
                    //_tray.batch =; 
                    _tray.x_barcode = item.matter_code;
                    //   _tray.is_check_ok = item.is_check_ok;
                    _tray.memo = item.memo;
                    // _tray.x_barcode = item.barcode;
                    _tray.barcodetype = BarcodeStoredTypeEnum.MatterCode;
                    // _tray.name = "";
                    _tray.number = item.number;
                    //_tray.lot_number =item.lot;// traybarcode;
                    _tray.single_weight = 0;
                    _tray.traybarcode = traybarcode;
                    _tray.project_no = item.project_no;
                    _tray.project_name = item.project_name;
                    _tray.customer_name = item.customer_name;
                    _tray.name_list = item.matter_name;
                    /************/
                    _tray.effective_date = item.effective_date;
                    _tray.lot_number = item.lot_number;
                    _tray.odd_numbers_in = item.odd_numbers_in;
                    _tray.producedate = item.producedate;
                    _tray.test_status = item.test_status;

                    lstTrayMatt.Add(_tray);
                }
            }
            //保存新数据 
            if (lstTrayMatt.Count > 0)
            {//添加电池与托盘的绑定关系
                var rtv2 = bll_wh_traymatter.AddRange(lstTrayMatt);
                if (rtv1 == LTWMSEFModel.SimpleBackValue.True && rtv2 == LTWMSEFModel.SimpleBackValue.True)
                {
                    string[] _mArr = dataArr.Select(s => s.matter_code).ToArray();
                    // AddUserOperationLog("[PDA]已成功保存托盘[" + _trayM.guid + "][" + _trayM.traybarcode + "],电池条码[" + string.Join(",", _mArr) + "]绑定关系");
                    return JsonReturn(true,"物料组盘");
                }
                else
                {
                    _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据保存失败！请重试...";
                    // AddJsonError("数据保存失败！请重试...");
                    //return JsonReturn(false);
                }
            }
            else
            {
                //空托盘组盘
                if (rtv1 == LTWMSEFModel.SimpleBackValue.True)
                {
                    // AddUserOperationLog("[PDA]已成功保存托盘[" + _trayM.guid + "][" + _trayM.traybarcode + "]信息：空托盘！");
                    return JsonReturn(true,"空托盘组盘");
                }
                else
                {
                    _mess += (string.IsNullOrWhiteSpace(_mess) ? "" : ";") + "数据保存失败！请重试...";
                    //AddJsonError("数据保存失败！请重试...");
                    //return JsonReturn(false);
                }
            }
            return JsonReturn(false, _mess);
        }

    }

}
