using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LTWMSWebMVC.Areas.BasicData.Data
{
    public class MatterTypeModel:BaseModel
    {
        ///// <summary>
        ///// 物料分类编码（2位编码组合、每一级最大允许数量为99个，足够满足分类需求 如：一级：01 二级01-01 三级 01-01-01 依次类推）
        ///// </summary>
        //[Display(Name = "物料编码")]
        //public string code { get; set; }
        ///// <summary>
        ///// 分类名称
        ///// </summary>
        //[Required(ErrorMessage = "分类名称不能为空"), Display(Name = "分类名称")]
        //public string name { get; set; }
        ///// <summary>
        ///// 排序
        ///// </summary>
        //[Display(Name = "排序")]
        //public int sort { get; set; }
        ///// <summary>
        ///// 备注
        ///// </summary>
        //[Display(Name = "备注")]
        //public string memo { get; set; }

        //----------------------
        /// <summary>
        /// 仓库分区名称
        /// </summary>
        [Required(ErrorMessage = "物料分类不能为空"), Display(Name = "物料分类名称")]
        public string name { get; set; }
        /// <summary>
        /// 物料分类编码 （2位编码组合、每一级最大允许数量为99个，足够满足分类需求 如：一级：01 二级01-01 三级 01-01-01 依次类推）
        /// </summary>
        [Display(Name = "编码")]
        public string code { get; set; }
        /// <summary>
        /// 数字编码（code=code_num+parent_code）
        /// </summary>
        [Display(Name = "数字编码")]
        public int code_num { get; set; }
        /// <summary>
        /// 父级编码（可以为空）
        /// </summary>
        [Display(Name = "父级编码")]
        public string parent_code { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int sort { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string memo { get; set; }
        public List<MatterTypeModel> SubItems { get; set; }
        /// <summary>
        /// 物料列表
        /// </summary>
        public List<MatterModel> ListMatterModel { get; set; }

    }
}