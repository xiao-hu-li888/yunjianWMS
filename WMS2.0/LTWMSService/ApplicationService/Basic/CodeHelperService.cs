using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTWMSService.ApplicationService.Basic
{
    /// <summary>
    /// 通用编码类（01-01-01）
    /// </summary>
    public class CodeHelperService : BaseService
    {
        Warehouse.wh_warehouse_typeBLL bll_wh_warehouse_type;
        Stock.stk_mattertypeBLL bll_stk_mattertype;
        public CodeHelperService(LTWMSEFModel.LTModel dbcontext, Warehouse.wh_warehouse_typeBLL bll_wh_warehouse_type, Stock.stk_mattertypeBLL bll_stk_mattertype) : base(dbcontext)
        {
            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            this.bll_stk_mattertype = bll_stk_mattertype;
        }
        /// <summary>
        /// 获取编码
        /// </summary>
        /// <param name="codeTable"></param>
        /// <param name="parentcode"></param>
        /// <returns></returns>
        public ComServiceReturn GetNewCodeByTablePrentCode(CodeTableEnum codeTable, string parentcode)
        {
            parentcode = parentcode ?? "";
            string text = "";
            int _numbCode = 0;
            if (codeTable == CodeTableEnum.wh_warehouse_type)
            {
                var lstCode = bll_wh_warehouse_type.GetAllQueryOrderby(o=>o.code_num,w =>(w.parent_code??"")==parentcode,false);
                if (lstCode != null && lstCode.Count > 0)
                { 
                    text = (lstCode[0].parent_code??"")+LTLibrary.ConvertUtility.getNumberWidthString(lstCode[0].code_num+1,2);
                    _numbCode = lstCode[0].code_num + 1;
                }
                else
                {//数据库不存在记录
                    if (string.IsNullOrWhiteSpace(parentcode))
                    {
                        text = "01";
                        _numbCode = 1;
                    }
                    else
                    {
                        text = parentcode + "01";
                        _numbCode = 1;
                    }
                }
            }
            if (codeTable == CodeTableEnum.stk_mattertype)
            {
                var lstCode = bll_stk_mattertype.GetAllQueryOrderby(o => o.code_num, w => (w.parent_code ?? "") == parentcode, false);
                if (lstCode != null && lstCode.Count > 0)
                {
                    text = (lstCode[0].parent_code ?? "") + LTLibrary.ConvertUtility.getNumberWidthString(lstCode[0].code_num + 1, 2);
                    _numbCode = lstCode[0].code_num + 1;
                }
                else
                {//数据库不存在记录
                    if (string.IsNullOrWhiteSpace(parentcode))
                    {
                        text = "01";
                        _numbCode = 1;
                    }
                    else
                    {
                        text = parentcode + "01";
                        _numbCode = 1;
                    }
                }
            }
            return new ComServiceReturn() { success = true, result = text, data= _numbCode };
        }
    }
    /// <summary>
    /// 需要编码管理的表
    /// </summary>
    public enum CodeTableEnum
    {
        /// <summary>
        /// 物料分区表
        /// </summary>
        wh_warehouse_type = 0,
        stk_mattertype=1
    }
}
