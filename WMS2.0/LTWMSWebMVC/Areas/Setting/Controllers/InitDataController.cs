using DbBackUpService.bak;
using DbBackUpService.bakBLL;
using LTWMSEFModel;
using LTWMSEFModel.Basic;
using LTWMSEFModel.Stock;
using LTWMSEFModel.Warehouse;
using LTWMSService.Basic;
using LTWMSService.Stock;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.Setting.Controllers
{
    //public static class Extensions
    //{
    //    public static List<PropertyInfo> GetDbSetProperties(this DbContext context)
    //    {
    //        var dbSetProperties = new List<PropertyInfo>();
    //        var properties = context.GetType().GetProperties();
    //        foreach (var property in properties)
    //        {
    //            var setType = property.PropertyType;

    //            //#if EF5 || EF6
    //            var isDbSet = setType.IsGenericType && (typeof(IDbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()) || setType.GetInterface(typeof(IDbSet<>).FullName) != null);
    //            //#elif EF7
    //            //            var isDbSet = setType.IsGenericType && (typeof (DbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()));
    //            //#endif
    //            if (isDbSet)
    //            {
    //                dbSetProperties.Add(property);
    //            }
    //        }
    //        return dbSetProperties;
    //    }
    //}

    public class InitDataController : BaseController
    {
        /*****************/
        bak_dataBLL bll_bak_data;
        /************配置信息备份类库************/
        wh_wcs_srvBLL bll_wh_wcs_srv;
        wh_wcs_deviceBLL bll_wh_wcs_device;

        wh_warehouse_typeBLL bll_wh_warehouse_type;
        wh_warehouseBLL bll_wh_warehouse;
        wh_shelves_devBLL bll_wh_shelves_dev;
        wh_shelvesBLL bll_wh_shelves;
        wh_shelfunitsBLL bll_wh_shelfunits;
        wh_service_statusBLL bll_wh_service_status;
        stk_matterBLL bll_stk_matter;
        sys_control_dicBLL bll_sys_control_dic;
        sys_loginBLL bll_sys_login;
        sys_loginroleBLL bll_sys_loginrole;
        sys_roleBLL bll_sys_role;
        wh_trayBLL bll_wh_tray;
        wh_traymatterBLL bll_wh_traymatter;
        LTModel ltdbcontext;
        public InitDataController(LTModel ltdbcontext, bak_dataBLL bll_bak_data,
             wh_wcs_srvBLL bll_wh_wcs_srv, wh_wcs_deviceBLL bll_wh_wcs_device, wh_warehouse_typeBLL bll_wh_warehouse_type
            , wh_warehouseBLL bll_wh_warehouse, wh_shelves_devBLL bll_wh_shelves_dev, wh_shelvesBLL bll_wh_shelves,
            wh_shelfunitsBLL bll_wh_shelfunits, wh_service_statusBLL bll_wh_service_status, stk_matterBLL bll_stk_matter,
           sys_control_dicBLL bll_sys_control_dic, sys_loginBLL bll_sys_login, sys_loginroleBLL bll_sys_loginrole,
           sys_roleBLL bll_sys_role, wh_trayBLL bll_wh_tray, wh_traymatterBLL bll_wh_traymatter)
        {
            this.ltdbcontext = ltdbcontext;
            this.bll_wh_traymatter = bll_wh_traymatter;
            this.bll_wh_tray = bll_wh_tray;
            this.bll_bak_data = bll_bak_data;
            this.bll_wh_wcs_srv = bll_wh_wcs_srv;
            this.bll_wh_wcs_device = bll_wh_wcs_device;

            this.bll_wh_warehouse_type = bll_wh_warehouse_type;
            this.bll_wh_warehouse = bll_wh_warehouse;
            this.bll_wh_shelves_dev = bll_wh_shelves_dev;
            this.bll_wh_shelves = bll_wh_shelves;
            this.bll_wh_shelfunits = bll_wh_shelfunits;
            this.bll_wh_service_status = bll_wh_service_status;
            this.bll_stk_matter = bll_stk_matter;
            this.bll_sys_control_dic = bll_sys_control_dic;
            this.bll_sys_login = bll_sys_login;
            this.bll_sys_loginrole = bll_sys_loginrole;
            this.bll_sys_role = bll_sys_role;

        }
        // GET: Setting/InitData
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取所有表列表
        /// </summary>
        /// <returns></returns>
        private List<string> getAllTables()
        {
            //查询所有表
            List<string> nameList = new List<string>();
            var listpropertyies = ltdbcontext.GetType().GetProperties();
            foreach (var property in listpropertyies)
            {
                var t = property.PropertyType;
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    var args = t.GetGenericArguments();
                    if (args.Length > 0)
                    {
                        nameList.Add(args[0].FullName);
                        //   AddJsonError(args[0].Name);
                        // var bb = property.GetValue(ltdbcontext);
                    }
                }
            }
            return nameList;
        }
        /// <summary>
        /// 备份配置信息
        /// </summary>
        /// <returns></returns>
        public JsonResult BackUpSettingDB()
        {
            try
            {
                List<string> nameList = getAllTables();
                bool exist = false;
                bll_bak_data.GetAllQuery();
                using (var _tran = bll_bak_data.BeginTransaction())
                {
                    DbParameter[] sqlParameters1 = {
                    //     //new MySql.Data.MySqlClient.MySqlParameter{ParameterName="log_date",Value=_d}
                     };
                    var cmdInt = bll_bak_data.ExecuteSQLCommand("delete from bak_data"
                            , sqlParameters1);
                    foreach (var tableFullName in nameList)
                    {
                        Type type = ltdbcontext.GetType().Assembly.GetType(tableFullName);
                        var tableSet = ltdbcontext.Set(type);
                        var lst = tableSet.AsNoTracking().ToListAsync();
                        while (true)
                        {
                            //备份WCS配置信息 
                            if (lst != null && lst.IsCompleted)
                            {
                                List<bak_data> lstBckData = new List<bak_data>();
                                foreach (var aa in lst.Result)
                                {//保存wcs配置信息                                    
                                    var Md = new bak_data();
                                    Md.createdate = DateTime.Now;
                                    Md.guid = Guid.NewGuid();
                                    Md.table_name = tableFullName.Substring(tableFullName.LastIndexOf(".") + 1);
                                    Md.json_data = Newtonsoft.Json.JsonConvert.SerializeObject(aa);
                                    Md.state = DbBackUpService.EntityStatus.Normal;
                                    lstBckData.Add(Md);
                                }
                                if (lstBckData.Count > 0)
                                {
                                    var rtvbak = bll_bak_data.AddRange(lstBckData);
                                    if (rtvbak == DbBackUpService.SimpleBackValue.False)
                                    {
                                        exist = true;
                                    }
                                }
                                break;
                            }
                        }
                        if (exist)
                        {
                            break;
                        }
                    }
                    if (!exist)
                    {
                        _tran.Commit();
                        return JsonSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                AddJsonError("备份配置信息异常！详细信息>>>" + ex.Message);
            }
            return JsonError();
            /*
            try
            {
                List<string> nameList = getAllTables();
                // Assembly assembly = Assembly.Load("LTWMSEFModel");
                bool exist = false;
                bll_bak_data.GetAllQuery();
                using (var _tran = bll_bak_data.BeginTransaction())
                {
                    //删除备份数据
                    var lstBakData = bll_bak_data.GetAllQuery();
                    if (lstBakData != null && lstBakData.Count > 0)
                    {
                        foreach (var item in lstBakData)
                        {
                            bll_bak_data.Delete(item);
                        }
                    }

                    foreach (var tableFullName in nameList)
                    {
                        Type type = ltdbcontext.GetType().Assembly.GetType(tableFullName);
                        var tableSet = ltdbcontext.Set(type);
                        var lst = tableSet.AsNoTracking().ToListAsync();
                        while (true)
                        {
                            // tableSet.Add();
                            //备份WCS配置信息 
                            if (lst != null && lst.IsCompleted)
                            {
                                foreach (var aa in lst.Result)
                                {//保存wcs配置信息
                                    var rtv1 = SaveBackToDb(tableFullName.Substring(tableFullName.LastIndexOf(".")+1), Newtonsoft.Json.JsonConvert.SerializeObject(aa));
                                    if (rtv1 == LTWMSEFModel.SimpleBackValue.False)
                                    {
                                        exist = true;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        if (exist)
                        {
                            break;
                        }
                    }
                    if (!exist)
                    {
                        _tran.Commit();
                        return JsonSuccess();
                    }
                }
                   
            }
            catch (Exception ex)
            {
                AddJsonError("备份配置信息异常！详细信息>>>" + ex.Message);
            }
            return JsonError();*/
        }
        /*
        /// <summary>
        /// 保存至备份数据库表中
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public LTWMSEFModel.SimpleBackValue SaveBackToDb(string tablename, string jsonData)
        {
            var Md = new DbBackUpService.bak.bak_data();
            Md.createdate = DateTime.Now;
            Md.guid = Guid.NewGuid();
            Md.table_name = tablename;
            Md.json_data = jsonData;
            Md.state = DbBackUpService.EntityStatus.Normal;
            var rtv = bll_bak_data.Add(Md);
            if (rtv == DbBackUpService.SimpleBackValue.True)
            {
                return LTWMSEFModel.SimpleBackValue.True;
            }
            return LTWMSEFModel.SimpleBackValue.False;
        }*/
        /// <summary>
        /// 还原配置信息
        /// </summary>
        /// <returns></returns>
        public JsonResult ReInitBackUpSettingDB()
        {
            try
            {
                List<string> nameList = getAllTables();
                // Assembly assembly = Assembly.Load("LTWMSEFModel");
                bool exist = false;
                using (var _tran = bll_wh_wcs_srv.BeginTransaction())
                {
                    foreach (var tableFullName in nameList)
                    {
                        /***********删除旧数据*************/
                        Type type = ltdbcontext.GetType().Assembly.GetType(tableFullName);
                        var tableSet = ltdbcontext.Set(type);
                        string tblName = tableFullName.Substring(tableFullName.LastIndexOf(".") + 1);
                        DbParameter[] sqlParameters1 = {
                        //     //new MySql.Data.MySqlClient.MySqlParameter{ParameterName="log_date",Value=_d}
                         };
                        var cmdInt = ltdbcontext.Database.ExecuteSqlCommand("delete from " + tblName, sqlParameters1);
                        /*******************/
                        var lstBackData = bll_bak_data.GetAllQuery(w => w.table_name == tblName);
                        if (lstBackData != null && lstBackData.Count > 0)
                        {
                            List<dynamic> lstobj = new List<dynamic>();
                            foreach (var backItem in lstBackData)
                            {
                                //保存记录。。。
                                dynamic backupMd = (dynamic)Newtonsoft.Json.JsonConvert.DeserializeObject(backItem.json_data, type);
                                lstobj.Add(backupMd);
                            }
                            tableSet.AddRange(lstobj);
                            if (ltdbcontext.SaveChanges() == 0)
                            {//保存失败
                                exist = true;
                                break;
                            }
                        }
                    }
                    if (!exist)
                    {
                        _tran.Commit();
                        return JsonSuccess();
                    }
                    else
                    {
                        AddJsonError("事务提交失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                AddJsonError("还原配置信息异常！详细信息>>>" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 完整备份数据库
        /// </summary>
        /// <returns></returns>
        public JsonResult BackUpDataBase()
        {
            try
            {
                /**********sqlserver数据库备份*************/
                //   int rtv= ltdbcontext.Database.ExecuteSqlCommand("backup database [LTDB-WMS-ShangShengSuo] to disk='"+ bakPath + "' ");
                if (BackUpDb())
                {
                    return JsonSuccess();
                }
                else
                {
                    AddJsonError("备份失败。。。");
                }
                /***********sqlserver数据库备份***********/

                /*mysql 备份
                string fileName =  "wms_bak_" + DateTime.Now.ToString("yyyyMMddhhmmss");
                string bakPath = Server.MapPath("~/")+ "db_backup/" + fileName + ".sql";               
                //string cmdStr = "/c mysqldump -h" + host + " -P" + port + " -u" + user + " -p" + password + " " + database + " > " + bakPath;
                string cmdStr = "/c mysqldump -h 127.0.0.1 -P 3306 -u sa -p sasa LTDB-WMS-ShangShengSuo > " + bakPath; 
                try
                {
                    System.Diagnostics.Process.Start("cmd", cmdStr);
                }
                catch (Exception ex)
                {
                    WMSFactory.Log.v("备份数据异常>>"+ex.Message);
                }
                */
            }
            catch (Exception ex)
            {
                AddJsonError("完整备份数据异常！详细信息>>>" + ex.Message);
            }
            return JsonError();
        }
        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <returns></returns>
        public bool BackUpDb()
        {
            string savedir = Server.MapPath("~/") + "db_backup/";
            LTLibrary.FileHelper.CreateDirectoryIfNotExists(savedir);
            string bakPath = savedir + "WMS_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak";


            using (SqlConnection conn = new SqlConnection(WMSFactory.Config.GetSqlConnectionStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = conn;
                    comm.CommandText = "use master;backup database @dbname to disk = @backupname;";
                    comm.Parameters.Add(new SqlParameter("dbname", SqlDbType.NVarChar));
                    comm.Parameters["dbname"].Value = "LTDB-WMS-ShangShengSuo";
                    comm.Parameters.Add(new SqlParameter("backupname", SqlDbType.NVarChar));
                    comm.Parameters["backupname"].Value = bakPath;
                    comm.CommandType = CommandType.Text;
                    comm.ExecuteNonQuery();
                }
                finally
                {
                    conn.Close();
                }
            }
            return true;
        }
    }
}