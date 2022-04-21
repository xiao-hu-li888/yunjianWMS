using LTWMSEFModel.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace LTWMSService.Warehouse
{
    public class wh_wcs_deviceBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Warehouse.wh_wcs_device>
    {
        public wh_wcs_deviceBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {
        }
        /// <summary>
        /// 根据堆垛机查找对应绑定的站台列表
        /// </summary>
        /// <param name="listShelves"></param>
        /// <returns></returns>
        public List<wh_wcs_device> GetALLStationByStacker(List<wh_shelves> listShelves)
        {
            List<Guid> shelvesGuid = new List<Guid>();
            if (listShelves != null && listShelves.Count > 0)
            {
                shelvesGuid = listShelves.Select(w => w.guid).Distinct().ToList();
            }
            IQueryable<wh_wcs_device> query = from a in dbcontext.wh_wcs_device
                                              join b in dbcontext.wh_shelves_dev
                                              on a.guid equals b.wcs_device_guid
                                              //join c in dbcontext.hdw_plc
                                              //on a.u_identification equals c.u_identification
                                              where shelvesGuid.Contains(b.shelves_guid) && a.state == LTWMSEFModel.EntityStatus.Normal
                                              &&a.device_type== DeviceTypeEnum.Station
                                              //&& c.run_status == LTWMSEFModel.Hardware.PLCRunStatus.Ready
                                              select a;  
            return query.AsNoTracking().Distinct().OrderBy(w =>w.number).ToList();
        }
    }
}
