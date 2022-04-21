using LTWMSService.Basic;
using LTWMSWebMVC.Areas.System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.System.Controllers
{
    public class GeneralConfigController : BaseController
    {
        sys_control_dicBLL bll_sys_control_dic;
        public GeneralConfigController(sys_control_dicBLL bll_sys_control_dic)
        {
            this.bll_sys_control_dic = bll_sys_control_dic;
        }

        public ActionResult Update()
        {
            return View(GetGeneralConfigModel());
        }
        private GeneralConfigModel GetGeneralConfigModel(string mess = "")
        {
            GeneralConfigModel Model = new GeneralConfigModel();
            Model.LoginTimeOut = bll_sys_control_dic.GetValueByType(CommDictType.LoginTimeOut, Guid.Empty);
            Model.PassWordExpiration = bll_sys_control_dic.GetValueByType(CommDictType.PassWordExpiration, Guid.Empty);
            Model.NearTerm = bll_sys_control_dic.GetValueByType(CommDictType.NearTerm, Guid.Empty);
            Model.Message = mess;
            return Model;
        }
        [HttpPost]
        public ActionResult Update(GeneralConfigModel model)
        {
            string _text = "";
            try
            {
                int _PassWordExpiration = LTLibrary.ConvertUtility.ToInt(model.PassWordExpiration);
                if (_PassWordExpiration < 300)
                {
                    _text = "密码过期时间必须是大于300(5分钟)的整数";
                    return View(GetGeneralConfigModel(_text));
                }
                int _LoginTimeOut = LTLibrary.ConvertUtility.ToInt(model.LoginTimeOut);
                if (_LoginTimeOut < 300)
                {
                    _text = "登录超时时间必须是大于300(5分钟)的整数";
                    return View(GetGeneralConfigModel(_text));
                }
                int _nearTerm = LTLibrary.ConvertUtility.ToInt(model.NearTerm);
                if (_nearTerm <= 0)
                {
                    _text = "临近有效期必须是大于0的整数";
                    return View(GetGeneralConfigModel(_text));
                }
                //if (_LoginTimeOut < 300)
                //{
                //    _LoginTimeOut = 300;
                //}
                //if (_PassWordExpiration < 300)
                //{
                //    _PassWordExpiration = 300;
                //}
               string _oldLoginTimeOutVal  = bll_sys_control_dic.GetValueByType(CommDictType.LoginTimeOut, Guid.Empty);
                string _oldtext = "登录超时时间：" + _oldLoginTimeOutVal
                    + "，密码过期时间：" + bll_sys_control_dic.GetValueByType(CommDictType.PassWordExpiration, Guid.Empty)
                   +",临近有效期：" + bll_sys_control_dic.GetValueByType(CommDictType.NearTerm, Guid.Empty);
                //判断原LoginTimeOut登录超时时间 是否修改，如果修改，删除cookie
                if (LTLibrary.ConvertUtility.ToInt(_oldLoginTimeOutVal)!= _LoginTimeOut)
                {
                    //删除cookie
                    WMSFactory.AuthorCookieHelper.RemoveToken();
                }
                var rtv1 = bll_sys_control_dic.SetValueByType(CommDictType.LoginTimeOut, _LoginTimeOut.ToString(), Guid.Empty);
                var rtv2 = bll_sys_control_dic.SetValueByType(CommDictType.PassWordExpiration, _PassWordExpiration.ToString(), Guid.Empty);
                var rtv3 = bll_sys_control_dic.SetValueByType(CommDictType.NearTerm, _nearTerm.ToString(), Guid.Empty);
                if (rtv1 == LTWMSEFModel.SimpleBackValue.True && rtv2 == LTWMSEFModel.SimpleBackValue.True&&
                    rtv3== LTWMSEFModel.SimpleBackValue.True)
                {
                    string _newtext = "登录超时时间：" + _LoginTimeOut + "，密码过期时间：" + _PassWordExpiration+"，临近有效期："+_nearTerm;
                    AddUserOperationLog("修改配置参数成功！>>原值：【" + _oldtext + "】>>【 新值：" + _newtext + "】"
                        , LTWMSWebMVC.App_Start.AppCode.CurrentUser.UserName);
                    _text = "数据保存成功！";
                }
                else
                {
                    _text = "修改参数配置失败";
                }

            }
            catch (Exception ex)
            {
                _text = "保存数据出错！异常：" + ex.Message;
            }

            return View(GetGeneralConfigModel(_text));
        }
    }
}