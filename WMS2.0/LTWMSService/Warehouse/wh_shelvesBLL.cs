using LTWMSEFModel.Hardware;
using LTWMSEFModel.Warehouse;
using LTWMSService.ApplicationService.Basic;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
    public class wh_shelvesBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_shelves>
    {
        LTWMSService.Warehouse.wh_shelfunitsBLL bll_shelfunits;
        public wh_shelvesBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
            bll_shelfunits = new wh_shelfunitsBLL(dbContext);
        }
        /// <summary>
        /// 通过站台获取对应所有的货架信息
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public List<wh_shelves> GetAllShelvesByStation(wh_wcs_device station)
        {
            if (station == null)
            {
                return null;
            }
            IQueryable<wh_shelves> query = from a in dbcontext.wh_shelves
                                           join b in dbcontext.wh_shelves_dev
                                           on a.guid equals b.shelves_guid
                                           join c in dbcontext.wh_wcs_device
                                           on b.wcs_device_guid equals c.guid
                                           //on new { c.wcs_srv_guid,c.number } equals new { d.shvwcs_srv_guid, d.number } into temp1
                                           where a.state != LTWMSEFModel.EntityStatus.Deleted &&
                                           c.guid == station.guid && a.warehouse_guid == station.warehouse_guid
                                           select a;
            return query.AsNoTracking().Distinct().OrderBy(o => o.rack)
                    .ToList() ?? null;
        }
        /// <summary>
        /// 通过wcs编号获取所有对应的仓库
        /// </summary>
        /// <param name="Wcs_srv_guid"></param>
        /// <returns></returns>
        public List<wh_warehouse> getAllWareHouseByWcsSrvGuid(Guid Wcs_srv_guid)
        {
            IQueryable<wh_warehouse> query = from a in dbcontext.wh_warehouse
                                             join b in dbcontext.wh_shelves
                                             on a.guid equals b.warehouse_guid
                                             where a.state == LTWMSEFModel.EntityStatus.Normal && b.wcs_srv_guid == Wcs_srv_guid
                                             select a;
            return query.AsNoTracking().Distinct().OrderBy(o => o.createdate).ToList();
        }
        /// <summary>
        /// 通过堆垛机获取所有货架
        /// </summary>
        /// <param name="wcsdevice"></param>
        /// <returns></returns>
        public List<wh_shelves> GetShelvesByStacker(List<hdw_plc> wcsdevice)
        {
            if (wcsdevice == null || wcsdevice.Count == 0)
            {
                return null;
            }
            Guid? _warehouseguid = wcsdevice[0].warehouse_guid;
            List<Guid> guidList = ComBLLService.GetBaseBaseGuidList(wcsdevice);

            IQueryable<wh_shelves> query = from a in dbcontext.wh_shelves
                                           join b in dbcontext.wh_shelves_dev
                                           on a.guid equals b.shelves_guid
                                           join c in dbcontext.wh_wcs_device
                                           on b.wcs_device_guid equals c.guid
                                           join d in dbcontext.hdw_plc
                                           on c.u_identification equals d.u_identification
                                           //on new { c.wcs_srv_guid,c.number } equals new { d.shvwcs_srv_guid, d.number } into temp1
                                           where a.state != LTWMSEFModel.EntityStatus.Deleted &&
                                          guidList.Contains(d.guid) && a.warehouse_guid == _warehouseguid
                                           select a;
            return query.AsNoTracking().Distinct().OrderBy(o => o.rack)
                    .ToList() ?? null;
            /* IQueryable<wh_shelves> query = from a in dbcontext.wh_shelves
                                            join b in dbcontext.wh_shelves_dev
                                            on a.guid equals b.shelves_guid
                                            join c in dbcontext.wh_wcs_device
                                            on b.wcs_device_guid equals c.guid
                                            join d in dbcontext.hdw_plc
                                            on c.u_identification equals d.u_identification
                                            //on new { c.wcs_srv_guid,c.number } equals new { d.shvwcs_srv_guid, d.number } into temp1
                                            where a.state != LTWMSEFModel.EntityStatus.Deleted &&
                                            d.guid == wcsdevice.guid && a.warehouse_guid == wcsdevice.warehouse_guid
                                            select a;
             return query.AsNoTracking().Distinct().OrderBy(o => o.rack)
                     .ToList() ?? null;*/
        }
        /// <summary>
        /// 获取相关联的货架（同一台堆垛机可操作的库位）
        /// </summary>
        /// <param name="shelfU"></param>
        /// <returns></returns>
        public List<wh_shelves> GetRelatedShelves(wh_shelfunits shelfU)
        {
            IQueryable<wh_wcs_device> query = from c in dbcontext.wh_wcs_device
                                              join b in dbcontext.wh_shelves_dev
                                               on c.guid equals b.wcs_device_guid
                                              join a in dbcontext.wh_shelves
                                             on b.shelves_guid equals a.guid 
                                              where a.state != LTWMSEFModel.EntityStatus.Deleted &&
                                              a.guid == shelfU.shelves_guid && c.device_type == DeviceTypeEnum.Stacker
                                              &&a.warehouse_guid==shelfU.warehouse_guid
                                              select c;
            return GetAllShelvesByStation(query.AsNoTracking().FirstOrDefault());
        }
        /// <summary>
        /// 初始化货架信息
        /// </summary>
        /// <param name="wh_Shelves"></param>
        /// <returns></returns>
        public LTWMSEFModel.SimpleBackValue InitShelfUnitData(wh_shelves wh_Shelves)
        {
            //初始化货架信息 
            List<wh_shelfunits> shelfunitsList = new List<wh_shelfunits>();
            int _col = wh_Shelves.columns_specs;
            int _row = wh_Shelves.rows_specs;
            int _rack = wh_Shelves.rack;
            for (int i = 1; i <= _col; i++)
            {
                for (int j = 1; j <= _row; j++)
                {
                    wh_shelfunits shelUnit = new wh_shelfunits();
                    shelUnit.guid = Guid.NewGuid();
                    shelUnit.rack = _rack;//排
                    shelUnit.columns = i;//列
                    shelUnit.rows = j;//层        
                    shelUnit.depth = wh_Shelves.depth;//货架纵深 默认0 
                    shelUnit.same_side_mark = wh_Shelves.same_side_mark;//货架同侧标记
                    shelUnit.locktype = ShelfLockType.Normal;
                    shelUnit.shelfunits_pos = _rack + "-" + shelUnit.columns + "-" + shelUnit.rows;
                    shelUnit.warehouse_guid = wh_Shelves.warehouse_guid;
                    shelUnit.shelves_guid = wh_Shelves.guid;
                    shelUnit.state = LTWMSEFModel.EntityStatus.Normal;
                    shelUnit.cellstate = ShelfCellState.CanIn;
                    shelUnit.createdate = DateTime.Now;
                    shelUnit.createuser = wh_Shelves.createuser;
                    shelfunitsList.Add(shelUnit);
                }
            }
            return bll_shelfunits.AddRange(shelfunitsList);
        }
        public LTWMSEFModel.SimpleBackValue DeleteWidthShelfUnits(wh_shelves shelfObj)
        {
            // bll_shelfunits.Delete(w => w.shelves_guid == shelfObj.guid);
            var DelList = bll_shelfunits.GetAllQuery(w => w.shelves_guid == shelfObj.guid);
            if (DelList != null && DelList.Count > 0)
            {
                foreach (var item in DelList)
                {
                    bll_shelfunits.Delete(item);
                }
            }
            return Delete(shelfObj);
        }
    }
}
