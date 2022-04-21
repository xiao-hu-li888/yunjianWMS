using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Basic
{
    public class sys_number_ruleBLL : LTERPEFModel.ComDao<LTERPEFModel.Basic.sys_number_rule>
    {
        public sys_number_ruleBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {

        }
        /// <summary>
        /// 获取入库编号
        /// </summary>
        /// <returns></returns>
        public string GetBillStockInNum()
        {
            var model = GetFirstDefault(w => w.rule_name == LTERPEFModel.Basic.NumberRuleName.BillStockIn);
            if (model == null)
            {//如果为空，则默认插入一条记录
                var newmod = new LTERPEFModel.Basic.sys_number_rule();
                newmod.rule_name = LTERPEFModel.Basic.NumberRuleName.BillStockIn;
                newmod.createdate = DateTime.Now;
                newmod.createuser = "sys_admin";
                newmod.date_format = "yyyyMMdd";
                newmod.guid = Guid.NewGuid();
                newmod.number = 2;
                newmod.prefix = "IN";
                newmod.split_str = "-";
                newmod.state = LTERPEFModel.EntityStatus.Normal;
                LTERPEFModel.SimpleBackValue v = AddIfNotExists(newmod, w => w.rule_name);
                if (v == LTERPEFModel.SimpleBackValue.True ||
                    v == LTERPEFModel.SimpleBackValue.ExistsOfAdd)
                {//添加成功 //添加数据的时候已存在，并发
                    model = GetFirstDefault(w => w.rule_name == LTERPEFModel.Basic.NumberRuleName.BillStockIn);
                }
            }
            //解析数据并返回单号
            if (model != null)
            {
                return getNumberValue(model);
            }
            return string.Empty;
        }
        private string getNumberValue(LTERPEFModel.Basic.sys_number_rule model)
        {
            if (string.IsNullOrWhiteSpace(model.date_format))
            {
                model.date_format = "yyyyMMdd";
            }
            int _lastnum = 0;
            if (!string.IsNullOrWhiteSpace(model.last_value))
            {//提取最后一个值
                _lastnum = model.last_increase;
            }
            model.split_str = string.IsNullOrWhiteSpace(model.split_str) ? "" : model.split_str;
            string _returnStr = model.prefix + model.split_str +
                DateTime.Now.ToString(model.date_format) + model.split_str + getNumberSpace(model.number, _lastnum + 1);
            //保存最后生成的值
            model.last_date = DateTime.Now;
            model.last_increase = (_lastnum + 1);
            model.last_value = _returnStr;
            model.updatedate = DateTime.Now;
            model.updateuser = "sys_admin";
            Update(model);
            return _returnStr;
        }
        /// <summary>
        /// 返回对应字符宽度的数字，不足用0补齐
        /// </summary>
        /// <param name="num"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private string getNumberSpace(int width, int num)
        {
            if (width <= 0)
            {
                return num.ToString();
            }
            int _len=num.ToString().Length;//字符串的长度 
            if(width<=_len)
            {//宽度小于或等于字符串的长度
                return num.ToString();
            }
            //补齐字符串
            int _sps_len = (width - _len);//空格的长度
            string _space = "";
            for (int i = 0; i < _sps_len; i++)
            {
                _space += "0";
            }
            return _space + num.ToString();
        }
        /// <summary>
        /// 获取出库编号
        /// </summary>
        /// <returns></returns>
        public string GetBillStockOutNum()
        {
            var model = GetFirstDefault(w => w.rule_name == LTERPEFModel.Basic.NumberRuleName.BillStockOut);
            if (model == null)
            {//如果为空，则默认插入一条记录
                var newmod = new LTERPEFModel.Basic.sys_number_rule();
                newmod.rule_name = LTERPEFModel.Basic.NumberRuleName.BillStockOut;
                newmod.createdate = DateTime.Now;
                newmod.createuser = "sys_admin";
                newmod.date_format = "yyyyMMdd";
                newmod.guid = Guid.NewGuid();
                newmod.number = 2;
                newmod.prefix = "OT";
                newmod.split_str = "-";
                newmod.state = LTERPEFModel.EntityStatus.Normal;
                LTERPEFModel.SimpleBackValue v = AddIfNotExists(newmod, w => w.rule_name);
                if (v == LTERPEFModel.SimpleBackValue.True)
                {//添加成功
                    model = newmod;
                }
                else if (v == LTERPEFModel.SimpleBackValue.ExistsOfAdd)
                {//添加数据的时候已存在，并发
                    model = GetFirstDefault(w => w.rule_name == LTERPEFModel.Basic.NumberRuleName.BillStockOut);
                }
            }
            //解析数据并返回单号
            if (model != null)
            {
                return getNumberValue(model);
            }
            return string.Empty;
        }
    }
}
