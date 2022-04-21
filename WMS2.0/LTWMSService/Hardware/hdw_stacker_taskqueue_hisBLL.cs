using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.Hardware
{
    public class hdw_stacker_taskqueue_hisBLL : LTWMSEFModel.ComDao<LTWMSEFModel.Hardware.hdw_stacker_taskqueue_his>
    {
        public hdw_stacker_taskqueue_hisBLL(LTWMSEFModel.LTModel dbContext) : base(dbContext)
        {

        }
        /// <summary>
        /// 获取最近一个月的出入库统计
        /// </summary>
        public void GetStockInoutCount()
        {

        }

        /// <summary>
        /// 出入库统计，按月份
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<chartData> GetStockInOutCount(StockInOutEnum type,Guid warehouseguid)
        {
            DateTime _begindt = DateTime.Now.AddDays(-30);//查询最近30天的统计数据 
            if (type == StockInOutEnum.StockIn)
            {
                //入库
                IQueryable<chartData> query = from a in dbcontext.hdw_stacker_taskqueue_his
                                              where a.warehouse_guid== warehouseguid && a.createdate > _begindt && a.state == LTWMSEFModel.EntityStatus.Normal && a.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockIn
                                               && (a.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.Finished|| a.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.ForceComplete)
                                              //  orderby a.createdate
                                              group a by new { a.createdate.Year, a.createdate.Month, a.createdate.Day } into g
                                            //  orderby g.Key.Year descending, g.Key.Month descending, g.Key.Day descending
                                              select new chartData
                                              {
                                                  year = g.FirstOrDefault().createdate.Year,
                                                  month = g.FirstOrDefault().createdate.Month,
                                                  day = g.FirstOrDefault().createdate.Day,
                                                  counts = g.Count()
                                              };

                return query.AsNoTracking().Take(30).OrderBy(a => a.year).ThenBy(b => b.month).ThenBy(c => c.day).ToList();
            }
            else
            {
                //出库
                IQueryable<chartData> query = from a in dbcontext.hdw_stacker_taskqueue_his
                                              where a.warehouse_guid== warehouseguid && a.createdate > _begindt && a.state == LTWMSEFModel.EntityStatus.Normal && a.tasktype == LTWMSEFModel.Hardware.WcsTaskType.StockOut
                                               && (a.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.Finished|| a.taskstatus == LTWMSEFModel.Hardware.WcsTaskStatus.ForceComplete)
                                              //  orderby a.createdate
                                              group a by new { a.createdate.Year, a.createdate.Month, a.createdate.Day } into g
                                          //  orderby g.Key.Year descending, g.Key.Month descending, g.Key.Day descending
                                              select new chartData
                                              {
                                                  year = g.FirstOrDefault().createdate.Year,
                                                  month = g.FirstOrDefault().createdate.Month,
                                                  day = g.FirstOrDefault().createdate.Day,
                                                  counts = g.Count()
                                              };

                return query.AsNoTracking().Take(30).OrderBy(a => a.year).ThenBy(b => b.month).ThenBy(c => c.day).ToList();
            }
        }

    }
    public enum StockInOutEnum
    {
        /// <summary>
        /// 入库
        /// </summary>
        StockIn=0,
        /// <summary>
        /// 出库
        /// </summary>
        StockOut=1
    }
    public class chartData
    {
        public int year;
        public int month;
        public int day;
        public int counts;
    }


}
