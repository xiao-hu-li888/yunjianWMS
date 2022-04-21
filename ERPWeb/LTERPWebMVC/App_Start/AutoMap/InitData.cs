using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTERPWebMVC.App_Start.AutoMap
{
    /// <summary>
    /// 初始化测试数据。。。
    /// </summary>
    public class InitData
    {
        LTERPService.Basic.sys_table_idBLL bll_sys_table_id;
        LTERPService.Basic.sys_loginBLL bll_sys_login; 
        public InitData(LTERPService.Basic.sys_loginBLL bll_sys_login, LTERPService.Basic.sys_table_idBLL bll_sys_table_id)
        {
            this.bll_sys_login = bll_sys_login; 
            this.bll_sys_table_id = bll_sys_table_id;
        } 
        /// <summary>
        /// 添加admin账号
        /// </summary>
        public void AddLoginAdmin()
        {
            bll_sys_login.AddLogin(new LTERPEFModel.Basic.sys_login()
            {
                createdate = DateTime.Now,
                createuser = "sys",
                gender = true,
                guid = Guid.NewGuid(),
                issuperadmin = true,
                username = "管理员",
                loginname = "admin",
                rowversion = 1,
                state = LTERPEFModel.EntityStatus.Normal
            }, "123456");
        }
    
    }
}