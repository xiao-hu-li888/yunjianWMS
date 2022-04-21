using AutoMapper;
using LTERPWebMVC.Areas.Admin.Models; 
using LTERPWebMVC.Areas.History.Data; 
using LTERPWebMVC.Areas.System.Data;
using LTERPEFModel.Basic; 
using LTERPEFModel.Logs;
using LTERPEFModel.Stock; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LTERPWebMVC.Areas.Stock.Data;

namespace LTERPWebMVC
{
    public class MapperConfig
    {
        private static IMapper mapper;
        public static void Config()
        {
            var config = new MapperConfiguration(cfg =>
            {
                //  cfg.AddProfile<AppProfile>();
                cfg.CreateMap<sys_login, sysloginModel>().ReverseMap(); 
                cfg.CreateMap<sys_role, sysRoleModel>().ReverseMap(); 
                cfg.CreateMap<log_sys_useroperation, UserOperationLogModel>().ReverseMap();
                cfg.CreateMap<log_sys_execute, ExecuteLogModel>().ReverseMap(); 
                cfg.CreateMap<stk_matter, MatterModel>().ReverseMap(); 
                cfg.CreateMap<stk_mattertype, MatterTypeModel>().ReverseMap(); 

            });
            mapper = config.CreateMapper();
        }
        public static IMapper Mapper
        {
            get
            {
                return mapper;
            }
        }
    }
}