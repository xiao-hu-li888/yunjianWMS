using LTERPEFModel;
using LTERPEFModel.Basic;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LTERPService.Basic
{
    public class sys_loginBLL : LTERPEFModel.ComDao<LTERPEFModel.Basic.sys_login>
    {

        sys_loginroleBLL bll_loginrole;
        sys_roleBLL bll_sysrole;
        public sys_loginBLL(LTERPEFModel.LTModel dbContext) : base(dbContext)
        {
            bll_loginrole = new sys_loginroleBLL(dbContext);
            bll_sysrole = new sys_roleBLL(dbContext);
        }
        /// <summary>
        /// 添加角色关联信息
        /// </summary>
        /// <param name="ex_role"></param>
        /// <param name="login_guid"></param>
        /// <returns></returns>
        public LTERPEFModel.SimpleBackValue AddRoles(string ex_role,Guid login_guid)
        {
            //删除关联的角色
           // bll_loginrole.Delete(w => w.loginguid == login_guid);
            var DelList= bll_loginrole.GetAllQuery(w =>w.loginguid == login_guid);
            if (DelList != null&& DelList.Count>0)
            {
                foreach (var item in DelList)
                {
                    bll_loginrole.Delete(item);
                }
            }
            List<sys_role> lstRole = new List<sys_role>();
            //添加角色
            string rols = "," + ex_role + ",";
            var rollist = bll_sysrole.GetAllQuery(w => w.state == LTERPEFModel.EntityStatus.Normal);
            foreach (var res in rollist)
            {
                if (rols.IndexOf("," + res.guid + ",") >= 0)
                {
                    lstRole.Add(res);
                }
            }
            if (lstRole.Count > 0)
            {//保存用户对应的角色信息 
                List<sys_loginrole> lst_lgnrole = new List<sys_loginrole>();
                foreach (sys_role sysRole in lstRole)
                {
                    sys_loginrole lgnrole = new sys_loginrole();
                    lgnrole.guid = Guid.NewGuid();
                    lgnrole.loginguid = login_guid;
                    lgnrole.roleguid = sysRole.guid;
                    lst_lgnrole.Add(lgnrole);
                }
               return bll_loginrole.AddRange(lst_lgnrole);
            }
            return SimpleBackValue.True;
        }
       /// <summary>
       /// 获取登录用户的所有角色信息
       /// </summary>
       /// <param name="login_guid"></param>
       /// <returns></returns>
        public List<sys_role> GetLoginRoles(Guid login_guid)
        {
            IQueryable<sys_role> query = from a in dbcontext.sys_role
                                        join b in dbcontext.sys_loginrole
                                        on a.guid equals b.roleguid
                                        where b.loginguid==login_guid
                                        select a;
            return query.AsNoTracking().ToList();
        }
        ///// <summary>
        ///// 获取当前用户的所有权限
        ///// </summary>
        ///// <returns></returns>
        //public List<sys_function> getFunctions(sys_login login)
        //{ 
        //    if (login == null)
        //        return null;
        //    List<sys_function> retfuc = new List<sys_function>();
        //    var _loginM = _dbcontext.sys_login.Where(w => w.guid == login.guid).FirstOrDefault();
        //    if (_loginM != null)
        //    {
        //        IQueryable<sys_role> query2 = from a in _dbcontext.sys_role
        //                                      join b in _dbcontext.sys_loginrole
        //                                    on a.guid equals b.roleguid
        //                                      where b.loginguid == _loginM.guid
        //                                      select a;
        //        _loginM.sysroles = query2.ToList();
        //        if (_loginM.sysroles != null && _loginM.sysroles.Count > 0)
        //        {
        //            foreach (sys_role item in _loginM.sysroles)
        //            {
        //                IQueryable<sys_function> query3 = from a in _dbcontext.sys_function
        //                                                  join b in _dbcontext.sys_functionrole
        //                                            on a.guid equals b.functionguid
        //                                                  where b.roleguid == item.guid
        //                                                  select a;
        //                var flist = query3.ToList();
        //                if (flist != null && flist.Count > 0)
        //                {
        //                    foreach (var _it in flist)
        //                    {
        //                        if (!retfuc.Exists(w => w.guid == _it.guid))
        //                        {
        //                            retfuc.Add(_it);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    } 
        //    return retfuc;
        //}
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="newpass"></param>
        /// <returns></returns>
        public SimpleBackValue UpdatePassword(Guid guid, string newpass)
        {
            if (string.IsNullOrWhiteSpace(newpass))
            {
                return  SimpleBackValue.False;
            }
            string npd = SHA256(newpass);
            var _us = GetFirstDefault(w => w.guid == guid);
            if (_us != null)
            {
                _us.password = npd;
                return Update(_us);
            }
            return SimpleBackValue.False;
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public SimpleBackValue AddLogin(sys_login login, string password)
        {
            if (string.IsNullOrWhiteSpace(login.loginname)
                || string.IsNullOrWhiteSpace(password))
            {
                return  LTERPEFModel.SimpleBackValue.False;
            }
            login.password = SHA256(password);
            return this.AddIfNotExists<string>(login,a=>a.loginname);
            //  return this.Add(login); 
        }
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="loginname"></param>
        /// <param name="password"></param>
        public sys_login Login(string loginname, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginname) || string.IsNullOrWhiteSpace(password))
                {
                    return null;
                }
                string passbyte = SHA256(password);
                sys_login userMode = base.GetFirstDefault(w => w.loginname == (loginname ?? "").Trim() && w.state== EntityStatus.Normal);
                if (userMode != null)
                {
                    string newp = passbyte;
                    string oldp = userMode.password;
                    if (newp == oldp)
                    {
                        return userMode;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        /// <summary>
        ///SHA256加密
        /// </summary>
        /// <param name="byPlainText"></param>
        /// <returns></returns>
        public string SHA256(string data)
        {
            //SHA1CryptoServiceProvider mCSP = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            //return mCSP.ComputeHash(byPlainText);
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }
            return builder.ToString();
        }

    }
}
