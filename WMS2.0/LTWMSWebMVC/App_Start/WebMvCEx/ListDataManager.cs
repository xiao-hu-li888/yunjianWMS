using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSService.Basic;
using LTWMSService.Stock;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LTWMSEFModel.Warehouse;

namespace LTWMSWebMVC
{
    public class ListDataManager
    {
        /// <summary>
        /// 查询设置所有可用角色
        /// </summary>
        public static void setWcsSrvGuidList(wh_wcs_srvBLL bll_wh_wcs_srv)
        {

            //if (warehouse_guid == null || warehouse_guid == Guid.Empty)
            //{
            List<wh_wcs_srv> d = bll_wh_wcs_srv.GetAllQuery(w
              => w.state == LTWMSEFModel.EntityStatus.Normal);
            //}
            //else
            //{//根据仓库查询对应的wcs
            // //d = bll_wh_wcs_srv.GetAllQuery(w
            // // => w.state == LTWMSEFModel.EntityStatus.Normal&&w.warehouse_guid==warehouse_guid);
            //    d = bll_wh_wcs_srv.GetAllQuery(w
            //     => w.state == LTWMSEFModel.EntityStatus.Normal );
            //}
            List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in d)
            {
                list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = s.name, Value = s.guid.ToString() });
            }
            ListProvider.AddList("WCSSrvGuidList", list);
        }

        /// <summary>
        /// 查询设置所有仓库列表
        /// </summary>
        public static void setWareHouseGuidList(wh_warehouseBLL bll_warehouse)
        {
            var d = bll_warehouse.GetAllQuery(w
                => w.state == LTWMSEFModel.EntityStatus.Normal);
            List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in d)
            {
                list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = s.name, Value = s.guid.ToString() });
            }
            ListProvider.AddList("WareHouseGuidList", list);
        }
        /// <summary>
        /// 查询设置所有仓库列表2
        /// </summary>
        public static void setWareHouseGuidList2(wh_warehouseBLL bll_warehouse, wh_warehouse_typeBLL bll_warehouse_type)
        {
            var ListWareType = GetWareHouseTypeGuidList(bll_warehouse_type);
            var d = bll_warehouse.GetAllQuery(w
                => w.state == LTWMSEFModel.EntityStatus.Normal);
            List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in d)
            {
                //获取分区名称
                var wtM = ListWareType.FirstOrDefault(w => w.Value == Convert.ToString(s.warehouse_type_guid));
                if (wtM != null)
                {
                    list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = wtM.Text + "/" + s.name, Value = s.guid.ToString() });
                }
                else
                {
                    list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = s.name, Value = s.guid.ToString() });
                }

            }
            ListProvider.AddList("WareHouseGuidList2", list);
        }
        /// <summary>
        ///通过登录用户权限 查询设置所有仓库列表
        /// </summary>
        public static void setWareHouseGuidListByPermission(wh_warehouseBLL bll_warehouse, wh_warehouse_typeBLL bll_warehouse_type, List<Guid> warehouseGuids)
        {
            var ListWareType = GetWareHouseTypeGuidList(bll_warehouse_type);
            if (warehouseGuids != null && warehouseGuids.Count > 0)
            {
                var d = bll_warehouse.GetAllQuery(w
                    => w.state == LTWMSEFModel.EntityStatus.Normal && warehouseGuids.Contains(w.guid));

                List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
                foreach (var s in d)
                {
                    //获取分区名称
                    var wtM = ListWareType.FirstOrDefault(w => w.Value == Convert.ToString(s.warehouse_type_guid));
                    if (wtM != null)
                    {
                        list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = wtM.Text + "/" + s.name, Value = s.guid.ToString() });
                    }
                    else
                    {
                        list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = s.name, Value = s.guid.ToString() });
                    }

                }
                ListProvider.AddList("WareHouseGuidList2", list);
            }
        }
        /// <summary>
        /// 查询设置所有仓库列表
        /// </summary>
        public static void setWareHouseTypeGuidList(wh_warehouse_typeBLL bll_warehouse_type)
        {
            ListProvider.AddList("WareHouseTypeGuidList", GetWareHouseTypeGuidList(bll_warehouse_type));
        }
        public static List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> GetWareHouseTypeGuidList(wh_warehouse_typeBLL bll_warehouse_type)
        {
            var d = bll_warehouse_type.GetAllQueryOrderby(o => o.code, w
                   => w.state == LTWMSEFModel.EntityStatus.Normal, true);
            List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
            if (d != null && d.Count > 0)
            {
                RecursionNames(list, d, "", "");
            }
            return list;
        }
        private static void RecursionNames(List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list, List<wh_warehouse_type> source, string parentcode, string parent_node)
        {
            var data = source.Where(w => (w.parent_code ?? "") == parentcode).ToList();
            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    string _curr_name = string.IsNullOrWhiteSpace(parent_node) ? item.name : parent_node + "/" + item.name;
                    list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem
                    {
                        Text = _curr_name,
                        Value = item.guid.ToString()
                    });
                    RecursionNames(list, source, item.code, _curr_name);
                }
            }
        }
        /// <summary>
        /// 查询设置所有可用角色
        /// </summary>
        public static void setRoleListData(sys_roleBLL bll_sys_role)
        {
            var d = bll_sys_role.GetAllQuery(w
                => w.state == LTWMSEFModel.EntityStatus.Normal);
            List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in d)
            {
                list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = s.rolename, Value = s.guid.ToString() });
            }
            ListProvider.AddList("RoleList", list);
        }


        /// <summary>
        /// 查询设置物料类型
        /// </summary>
        public static void setMatterTypeGuidList(stk_mattertypeBLL bll_mattertype)
        {
            var d = bll_mattertype.GetAllQuery(w
                => w.state == LTWMSEFModel.EntityStatus.Normal);
            List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in d)
            {
                list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = s.name, Value = s.guid.ToString() });
            }
            ListProvider.AddList("MatterTypeGuidList", list);
        }

        ///// <summary>
        ///// 查询设置入库单
        ///// </summary>
        //public static void setBillInOddNumberGuidList(LTWMSService.Bills.bill_stockinBLL bll_bill_stockin)
        //{
        //    var d = bll_bill_stockin.GetBillStockInList();
        //    List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
        //    foreach (var s in d)
        //    {
        //        list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = s.odd_numbers, Value = s.odd_numbers });
        //    }
        //    ListProvider.AddList("BillInOddNumberGuidList", list);
        //}

        /// <summary>
        /// 查询设置Agv终点
        /// </summary>
        public static void setAgvDestinationList()
        {
            List<string> lstDest = new List<string>();
            string ListAgvDest = WMSFactory.Config.AgvDestinationList;
            if (!string.IsNullOrWhiteSpace(ListAgvDest))
            {
                string[] arrList = ListAgvDest.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                lstDest.AddRange(arrList);
            }
            List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in lstDest)
            {
                list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text = s, Value = s });
            }
            ListProvider.AddList("AgvDestiNation", list);
        }
        /// <summary>
        /// 获取设置所有物料信息
        /// </summary>
        public static void SetALLMatterList(LTWMSService.Stock.stk_matterBLL bll_stk_matter)
        {
            var d = bll_stk_matter.GetAllQuery();
            List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem> list = new List<LTWMSWebMVC.App_Start.WebMvCEx.ListItem>();
            foreach (var s in d)
            {
                list.Add(new LTWMSWebMVC.App_Start.WebMvCEx.ListItem { Text =s.code+"/"+s.name+"/"+s.specs, Value = s.guid.ToString() });
            }
            ListProvider.AddList("StockMatter_List", list);
        }
    }
}