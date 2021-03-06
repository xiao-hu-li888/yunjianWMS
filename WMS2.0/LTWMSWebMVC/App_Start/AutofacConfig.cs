using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using LTWMSWebMVC.App_Start.AutoMap;
using LTWMSWebMVC.App_Start.WebMvCEx;
using LTWMSService.Basic;
using LTWMSService.Bills;
using LTWMSService.Hardware;
using LTWMSService.Logs;
using LTWMSService.Stock;
using LTWMSService.Warehouse;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using LTWMSService.ApplicationService.StockInOut;
using LTWMSService.ApplicationService.WmsServer;
using LTWMSService.ApplicationService.Basic;
using LTWMSService.BillsAihua;
using LTWMSService.ApplicationService;
using DbBackUpService;
using DbBackUpService.bakBLL;

namespace LTWMSWebMVC.App_Start
{
    public class AutofacConfig
    {
        /// <summary>
        /// Autofac容器对象
        /// </summary>
        private static IContainer _container;

        /// <summary>
        /// 初始化autofac
        /// </summary>
        public static IContainer Init()
        {
            ContainerBuilder builder = new ContainerBuilder();
            //配置接口依赖
            builder.RegisterType<LTWMSEFModel.LTModel>().As<LTWMSEFModel.LTModel>().InstancePerRequest();
            /*每次访问只生成一个对象*/

            builder.RegisterType<log_sys_executeBLL>().As<log_sys_executeBLL>().InstancePerRequest();

            builder.RegisterType<bill_stockcheckBLL>().As<bill_stockcheckBLL>().InstancePerRequest();
            builder.RegisterType<bill_stockcheck_detailBLL>().As<bill_stockcheck_detailBLL>().InstancePerRequest();
            builder.RegisterType<bill_stockinBLL>().As<bill_stockinBLL>().InstancePerRequest();
            builder.RegisterType<bill_stockin_detailBLL>().As<bill_stockin_detailBLL>().InstancePerRequest();
            builder.RegisterType<bill_stockoutBLL>().As<bill_stockoutBLL>().InstancePerRequest();
            builder.RegisterType<bill_stockout_detailBLL>().As<bill_stockout_detailBLL>().InstancePerRequest();
            //builder.RegisterType<hdw_agvBLL>().As<hdw_agvBLL>().InstancePerRequest();
            //builder.RegisterType<hdw_agv_logBLL>().As<hdw_agv_logBLL>().InstancePerRequest();
            //builder.RegisterType<hdw_agv_taskqueueBLL>().As<hdw_agv_taskqueueBLL>().InstancePerRequest();
            //builder.RegisterType<hdw_agv_taskqueue_hisBLL>().As<hdw_agv_taskqueue_hisBLL>().InstancePerRequest();
            builder.RegisterType<hdw_plcBLL>().As<hdw_plcBLL>().InstancePerRequest();
            builder.RegisterType<log_hdw_plcBLL>().As<log_hdw_plcBLL>().InstancePerRequest();
            builder.RegisterType<hdw_stacker_taskqueueBLL>().As<hdw_stacker_taskqueueBLL>().InstancePerRequest();
            builder.RegisterType<hdw_stacker_taskqueue_hisBLL>().As<hdw_stacker_taskqueue_hisBLL>().InstancePerRequest();
            builder.RegisterType<stk_matterBLL>().As<stk_matterBLL>().InstancePerRequest();
            builder.RegisterType<log_stk_matter_shelfunitsBLL>().As<log_stk_matter_shelfunitsBLL>().InstancePerRequest();
            builder.RegisterType<stk_mattertypeBLL>().As<stk_mattertypeBLL>().InstancePerRequest();
            builder.RegisterType<stk_stockBLL>().As<stk_stockBLL>().InstancePerRequest();
            builder.RegisterType<log_stk_stock_accountBLL>().As<log_stk_stock_accountBLL>().InstancePerRequest();
            builder.RegisterType<log_sys_alarmBLL>().As<log_sys_alarmBLL>().InstancePerRequest();
            builder.RegisterType<sys_annexBLL>().As<sys_annexBLL>().InstancePerRequest();
            builder.RegisterType<sys_control_dicBLL>().As<sys_control_dicBLL>().InstancePerRequest();
            builder.RegisterType<sys_dictionaryBLL>().As<sys_dictionaryBLL>().InstancePerRequest();
            builder.RegisterType<sys_loginBLL>().As<sys_loginBLL>().InstancePerRequest();
            builder.RegisterType<sys_loginroleBLL>().As<sys_loginroleBLL>().InstancePerRequest();
            builder.RegisterType<sys_roleBLL>().As<sys_roleBLL>().InstancePerRequest();
            builder.RegisterType<log_sys_useroperationBLL>().As<log_sys_useroperationBLL>().InstancePerRequest();
            builder.RegisterType<wh_shelfunitsBLL>().As<wh_shelfunitsBLL>().InstancePerRequest();
            builder.RegisterType<wh_shelvesBLL>().As<wh_shelvesBLL>().InstancePerRequest();
            builder.RegisterType<wh_trayBLL>().As<wh_trayBLL>().InstancePerRequest();
            builder.RegisterType<wh_traymatterBLL>().As<wh_traymatterBLL>().InstancePerRequest();
            builder.RegisterType<log_wh_traymatterBLL>().As<log_wh_traymatterBLL>().InstancePerRequest();
            builder.RegisterType<wh_warehouseBLL>().As<wh_warehouseBLL>().InstancePerRequest();
            builder.RegisterType<wh_service_statusBLL>().As<wh_service_statusBLL>().InstancePerRequest();
            builder.RegisterType<log_wh_wcsBLL>().As<log_wh_wcsBLL>().InstancePerRequest();
            builder.RegisterType<bill_stockin_printBLL>().As<bill_stockin_printBLL>().InstancePerRequest();
            builder.RegisterType<bill_stockout_taskBLL>().As<bill_stockout_taskBLL>().InstancePerRequest();
            // builder.RegisterType<hdw_agv_task_mainBLL>().As<hdw_agv_task_mainBLL>().InstancePerRequest();
            builder.RegisterType<sys_number_ruleBLL>().As<sys_number_ruleBLL>().InstancePerRequest();
            builder.RegisterType<sys_table_idBLL>().As<sys_table_idBLL>().InstancePerRequest();
            builder.RegisterType<wh_warehouse_typeBLL>().As<wh_warehouse_typeBLL>().InstancePerRequest();
            builder.RegisterType<wh_wcs_srvBLL>().As<wh_wcs_srvBLL>().InstancePerRequest();
            builder.RegisterType<wh_wcs_deviceBLL>().As<wh_wcs_deviceBLL>().InstancePerRequest();

            builder.RegisterType<wh_shelves_devBLL>().As<wh_shelves_devBLL>().InstancePerRequest();
            builder.RegisterType<StockOutService>().As<StockOutService>().InstancePerRequest();
            

            builder.RegisterType<bill_stockin_detail_traymatterBLL>().As<bill_stockin_detail_traymatterBLL>().InstancePerRequest();
            builder.RegisterType<bill_stockout_detail_traymatterBLL>().As<bill_stockout_detail_traymatterBLL>().InstancePerRequest();

            builder.RegisterType<bill_task_tray_relationBLL>().As<bill_task_tray_relationBLL>().InstancePerRequest();
            //预留单
            builder.RegisterType<billah_reserved_orderBLL>().As<billah_reserved_orderBLL>().InstancePerRequest();
            builder.RegisterType<billah_reserved_order_detailBLL>().As<billah_reserved_order_detailBLL>().InstancePerRequest();
            builder.RegisterType<ReservedOrderService>().As<ReservedOrderService>().InstancePerRequest();

            builder.RegisterType<stk_inout_recodBLL>().As<stk_inout_recodBLL>().InstancePerRequest();

            builder.RegisterType<stk_inout_recod_hisBLL>().As<stk_inout_recod_hisBLL>().InstancePerRequest();

            /**************注册公共服务类（复用）******************/
            builder.RegisterType<StockInService>().As<StockInService>().InstancePerRequest();
            builder.RegisterType<WCSService>().As<WCSService>().InstancePerRequest();
            builder.RegisterType<TaskQueueService>().As<TaskQueueService>().InstancePerRequest();
            builder.RegisterType<CodeHelperService>().As<CodeHelperService>().InstancePerRequest();

            ////注册将当前程序集的类
            builder.RegisterType<InitializationList>().As<InitializationList>().SingleInstance();
            builder.RegisterType<ListDataManager>().As<ListDataManager>().InstancePerRequest();
            builder.RegisterType<InitData>().As<InitData>().InstancePerRequest();
            builder.RegisterType<Filters.MyAuthorizeAttribute>().As<Filters.MyAuthorizeAttribute>().InstancePerRequest();
            /*********配置备份********/
            builder.RegisterType<LTBackUpModel>().As<LTBackUpModel>().InstancePerRequest();

            builder.RegisterType<bak_dataBLL>().As<bak_dataBLL>().InstancePerRequest();

            //builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            //     .AsImplementedInterfaces().AsSelf();
            //***************注册controller**********************
            //获取IAutoInject的Type
            var baseType = typeof(System.Web.Mvc.IController);
            //获取所有程序集
            var assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            //自动注册接口
            builder.RegisterAssemblyTypes(assemblies).Where(b => b.GetInterfaces().
            Any(c => c == baseType && b != baseType)).AsImplementedInterfaces().AsSelf().InstancePerRequest();
            //***************注册api***************
            var apibaseType = typeof(System.Web.Http.Controllers.IHttpController);
            //获取所有程序集
            var api_assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            //自动注册接口
            builder.RegisterAssemblyTypes(api_assemblies).Where(b => b.GetInterfaces().
            Any(c => c == apibaseType && b != apibaseType)).AsImplementedInterfaces().AsSelf().InstancePerRequest();

            _container = builder.Build();
            // Set the WebApi dependency resolver.
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(_container);
            return _container;

            //    return new AutofacDependencyResolver(_container);//返回针对MVC的解析器
        }

        /// <summary>
        /// 从Autofac容器获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetFromFac<T>()
        {
            //return _container.Resolve<T>();
            return AutofacDependencyResolver.Current.RequestLifetimeScope.Resolve<T>();
        }
    }
}