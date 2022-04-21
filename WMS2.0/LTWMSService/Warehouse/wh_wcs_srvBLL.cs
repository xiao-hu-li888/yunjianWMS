using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Warehouse
{
    public class wh_wcs_srvBLL : LTWMSEFModel.ComDao<wh_wcs_srv>
    {
        public wh_wcs_srvBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
        /// <summary>
        /// 查找所有wcs配置 且已关联货架
        /// </summary>
        /// <returns></returns>
        public List<wh_wcs_srv> GetAllWcsSrvUnionShelves()
        {
            IQueryable<wh_wcs_srv> query = from a in dbcontext.wh_wcs_srv
                                           join b in dbcontext.wh_shelves
                                              on a.guid equals b.wcs_srv_guid
                                           where a.state == LTWMSEFModel.EntityStatus.Normal
                                       && b.isinitialized == true && b.category == WareHouseCategoriesEnum.AutomatedWarehouse
                                           select a;
            return query.AsNoTracking().Distinct().OrderBy(w => w.createdate).ToList();
        }
        /// <summary>
        /// 通过仓库id获取关联的wcs配置
        /// </summary>
        /// <param name="warehouseguid"></param>
        /// <returns></returns>
        public List<wh_wcs_srv>  GetAllWcsSrvByWarehouseguid(Guid warehouseguid)
        {
            IQueryable<wh_wcs_srv> query = from a in dbcontext.wh_wcs_srv 
                                           where dbcontext.wh_shelves.Any(w=>w.state== LTWMSEFModel.EntityStatus.Normal
                                           &&w.wcs_srv_guid==a.guid&& w.isinitialized == true 
                                           && w.category == WareHouseCategoriesEnum.AutomatedWarehouse
                                           && w.warehouse_guid == warehouseguid)
                                           && a.state == LTWMSEFModel.EntityStatus.Normal 
                                           select a;
            return query.AsNoTracking().OrderBy(w => w.createdate).ToList();
        }
    }
}
