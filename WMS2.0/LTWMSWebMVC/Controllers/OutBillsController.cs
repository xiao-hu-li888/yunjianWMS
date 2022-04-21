using LTWMSService.BillsAihua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace LTWMSWebMVC.Controllers
{
    public class OutBillsController : ApiBaseController
    {
        public billah_reserved_orderBLL bll_billah_reserved_order;
        public billah_reserved_order_detailBLL bll_billah_reserved_order_detail;
        public OutBillsController(billah_reserved_orderBLL bll_billah_reserved_order, billah_reserved_order_detailBLL bll_billah_reserved_order_detail)
        {
            this.bll_billah_reserved_order = bll_billah_reserved_order;
            this.bll_billah_reserved_order_detail = bll_billah_reserved_order_detail;
        }
        /// <summary>
        /// 接收预留单信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Rec()
        {
            int rtvState = 2;
            string _mess = "接收失败";
            try
            {
                //接收post传入的数据 
                var request = System.Web.HttpContext.Current.Request;
                byte[] requestData = new byte[request.InputStream.Length];
                request.InputStream.Read(requestData, 0, (int)request.InputStream.Length);
                var jsonData = Encoding.UTF8.GetString(requestData);
                ReservationForm recvTaskStatus = (ReservationForm)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonData, typeof(ReservationForm));
                //_mess ="ylid:"+ recvTaskStatus.yl_id+">>data-lenght:"+ recvTaskStatus.data.Count+">>vv:"+ recvTaskStatus.data[0].qty;
                AddUserOperationLog("接收预留单数据:json>>"+ jsonData,"");
                if (recvTaskStatus != null && recvTaskStatus.data.Count > 0)
                {
                    //查找系统是否已存在预留单号
                    var reservObj = bll_billah_reserved_order.GetFirstDefault(w => w.yl_id == recvTaskStatus.yl_id);
                    if (reservObj != null && reservObj.guid != Guid.Empty)
                    {
                        rtvState = 0;
                        _mess = "预留单【" + reservObj.yl_id + "】已存在。总数量：" + reservObj.total_record + "，添加时间：" + reservObj.createdate;
                    }
                    else
                    {
                        using (var _tran = bll_billah_reserved_order.BeginTransaction())
                        {
                            //保存预留单信息
                            var orderMain = new LTWMSEFModel.BillsAihua.billah_reserved_order();
                            orderMain.bill_out_status = LTWMSEFModel.BillsAihua.ReserveBillOutStatus.None;
                            orderMain.guid = Guid.NewGuid();
                            orderMain.total_record = recvTaskStatus.data.Count;
                            orderMain.yl_id = recvTaskStatus.yl_id;
                            orderMain.createdate = DateTime.Now;
                            orderMain.createuser = "接口";
                            var rtv1 = bll_billah_reserved_order.Add(orderMain);
                            //保存子信息
                            var lstDetails = new List<LTWMSEFModel.BillsAihua.billah_reserved_order_detail>();
                            foreach (var item in recvTaskStatus.data)
                            {
                                var mdd = new LTWMSEFModel.BillsAihua.billah_reserved_order_detail();
                                mdd.guid = Guid.NewGuid();
                                mdd.goods_id = item.goods_id;
                                mdd.reserved_order_guid = orderMain.guid;
                                mdd.qty = item.qty;
                                mdd.spec_id = item.spec_id;
                                lstDetails.Add(mdd);
                            }
                            var rtv2 = bll_billah_reserved_order_detail.AddRange(lstDetails);
                            if (rtv1 == LTWMSEFModel.SimpleBackValue.True && rtv2 == LTWMSEFModel.SimpleBackValue.True)
                            {//数据保存成功
                                rtvState = 0;
                                _mess = "接收成功>>预留单号:["+ orderMain.yl_id+"]，总记录数:" + orderMain.total_record;
                                _tran.Commit();
                            }
                            else
                            {//数据保存失败
                                _mess = "数据保存失败";
                            }
                        }
                    }
                }
                else
                {
                    _mess = "数据为空或解析失败";
                }

            }
            catch (Exception ex)
            {
                _mess = "处理异常：>>>" + ex.Message;
                WMSFactory.Log.v("接收出库预留单参数异常：" + ex);
            }
            //返回接收状态。。。
            AddUserOperationLog("处理预留单返回状态(0:成功)："+ rtvState + "，返回消息："+ _mess,"");
            Hashtable hashtable = new Hashtable();
            hashtable.Add("state", rtvState);
            hashtable.Add("msg", _mess);
            var resp = new HttpResponseMessage { Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(hashtable), System.Text.Encoding.UTF8, "application/json") };
            return resp;
        }
    }
    /// <summary>
    /// 预留单
    /// </summary>
    public class ReservationForm

    {
        ///预留单号 
        public string yl_id { get; set; }
        public List<ReservationMatterDetail> data { get; set; }
    }
    /// <summary>
    /// 预留单明细
    /// </summary>
    public class ReservationMatterDetail
    {
        /// <summary>
        /// sap料号
        /// </summary>
        public string goods_id { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string spec_id { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal qty { get; set; }
    }
}
