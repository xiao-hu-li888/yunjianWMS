using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.RealTime.Data
{
    public class ShelfUnitsList
    {
        /// <summary>
        /// 顶部统计信息
        /// </summary>
        public ShelfUnitsTop Top { get; set; }
        /// <summary>
        /// 货架信息
        /// </summary>
        public List<LTWMSWebMVC.Areas.BasicData.Data.ShelvesModel> Shelves
        {
            get;set;
        } 
    }
}