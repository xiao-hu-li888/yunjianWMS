using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTERPWebMVC
{
    public class PagingModel
    {
        /// <summary>
        /// 每页显示大小
        /// </summary>
        public int PageSize { get { return 100; }}
        private int _totalsize;
        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalSize { get { return _totalsize; } set { _totalsize = value; } }
       
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get
            {
                int mod = TotalSize % PageSize;
                if (mod > 0)
                {
                    return TotalSize / PageSize + 1;
                }
                return TotalSize / PageSize;
            }
        }
        private int _currpage;
        /// <summary>
        /// 当前页(获取参数用)
        /// </summary>
        public int paging_curr_page
        {
            get
            {
                _currpage=LTLibrary.ConvertUtility.ToInt(HttpContext.Current.Request.Params["paging_curr_page"]);
                if (_currpage <= 0)
                {
                    return 1;
                }
                return _currpage;
            }
            //set { _currpage = value; }
        }
        /// <summary>
        /// 当前页（绑定view）
        /// </summary>
        public int CurrPage
        {
            get
            {
                int _rtcurr = 0;
                if(TotalPages < paging_curr_page)
                {
                    _rtcurr = TotalPages;
                }
                else
                {
                    _rtcurr = paging_curr_page;
                }
                if(_rtcurr<=0)
                {
                    _rtcurr = 1;
                }
                return _rtcurr;
            }
        }
    }
}