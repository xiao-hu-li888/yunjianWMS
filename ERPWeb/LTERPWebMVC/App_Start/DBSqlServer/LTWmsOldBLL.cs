using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LTERPWebMVC.App_Start.DBSqlServer
{
    public enum DictType
    {
        /// <summary>
        /// 品牌
        /// </summary>
        brand=0,
        /// <summary>
        /// 单位
        /// </summary>
        Unit=1
    }
    public class LTWmsOldBLL
    {
        LTWmsOld_DBContext ltoldwmsDBContext;
        public LTWmsOldBLL(LTWmsOld_DBContext ltoldwmsDBContext)
        {
            this.ltoldwmsDBContext = ltoldwmsDBContext;
        }  
        /// <summary>
        /// 通过字典类型获取字典集
        /// </summary>
        /// <returns></returns>
        public List<PubCodeInfo> GetAllDictByType(DictType dictype)
        {
            string _sql = "";
            if(dictype== DictType.brand)
            {
                _sql = @" select [ID]
                      ,[GUID]
                      ,[PubName]
                      ,[State]
                      ,[createDate]
                      ,[Code]
                      ,[CodeGuid]
                      ,[Remark]
                      ,[OrderNumb]
                      ,[IsGroupbyType]
                      ,[Active] from PubCodeInfo where CodeGuid='E976162A-321E-47E7-A6DE-AEB440F9AE41' ";
            }else if(dictype== DictType.Unit)
            {
                _sql = @"  select * from dbo.PubCodeInfo where CodeGuid='FB0D3F75-D693-4096-99C2-9FCC2E24099E' ";
            }
            if (string.IsNullOrWhiteSpace(_sql))
            {
                return null;
            }
            return ltoldwmsDBContext.Database.SqlQuery<PubCodeInfo>(_sql).ToList(); 
        }
        public List<Matter> GetAllMatter()
        {  
            var lst = ltoldwmsDBContext.Database.SqlQuery<Matter>(
               @"select [ID]
                      ,[guid]
                      ,[CodeNumb]
                      ,[ModelNumber]
                      ,[MName]
                      ,[MType]
                      ,[Unit]
                      ,[Stockinventory]
                      ,[state]
                      ,[createdate]
                      ,[stock]
                      ,[PriceMoney]
                      ,[weight]
                      ,[lastUpdateDate]
                      ,[SamllImage]
                      ,[MattCCode]
                      ,[BarCode]
                      ,[BrandGuid]
                      ,[Remark] from Matter").ToList();
            return lst; 
        }
        public List<MatterClass> GetAllMatterClass()
        {
          // return ltoldwmsDBContext.MatterClass.ToList();
            var lst = ltoldwmsDBContext.Database.SqlQuery<MatterClass>(
               @"select [Id]
              ,[Guid]
              ,[MatCCode]
              ,[ParentGuid]
              ,[bMatCEnd]
              ,[CreateDate]
              ,[MatCName]
              ,[MatCGrade]
              ,[cBarCode]
              ,[Remark]
              ,[ParentP]
              ,[ShortCode]  from MatterClass").ToList();
            return lst;
        }
    }
}

