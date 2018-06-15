using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DapperORM
{
    /// <summary>
    /// 获取数据库链接串的工厂类
    /// </summary>
    public class DbConnectionStringFactory
    {
        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dataBaseTypeEnum">数据库类型枚举</param>
        /// <returns></returns>
        public static string GetDbConnectionString(string dbName = "default", DataBaseTypeEnum dataBaseTypeEnum = DataBaseTypeEnum.Write)
        {
            try
            {
                string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/connectionStrings.config";
                XElement root = XElement.Load(configFilePath);
                XElement dbNode = null;
                if (dbName == "default")
                {
                    dbNode = root.Elements().Where(e => e.Attribute("default").Value == "true").FirstOrDefault();
                }
                else
                {
                    dbNode = root.Elements().Where(e => e.Attribute("name").Value.ToLower() == dbName.ToLower()).FirstOrDefault();
                }
                if (dbNode == null)
                {
                    throw new Exception("未找到名称为：{0}的数据库配置节点");
                }
                if (dataBaseTypeEnum == DataBaseTypeEnum.Write)
                {
                    return dbNode.Element("Write").Attribute("connectionString").Value;
                }
                else
                {
                    return dbNode.Element("Read").Attribute("connectionString").Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据库配置文件配置错误", ex);
            }
        }


        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dataBaseTypeEnum">数据库类型枚举</param>
        /// <returns></returns>
        public static Dictionary<string,string> GetDbConnectionConfig(string dbName = "default", DataBaseTypeEnum dataBaseTypeEnum = DataBaseTypeEnum.Write)
        {
            var dic = new Dictionary<string, string>();

            try
            {
                string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/connectionStrings.config";
                XElement root = XElement.Load(configFilePath);
                XElement dbNode = null;
                if (dbName == "default")
                {
                    dbNode = root.Elements().Where(e => e.Attribute("default").Value == "true").FirstOrDefault();
                }
                else
                {
                    dbNode = root.Elements().Where(e => e.Attribute("name").Value.ToLower() == dbName.ToLower()).FirstOrDefault();
                }
                if (dbNode == null)
                {
                    throw new Exception("未找到名称为：{0}的数据库配置节点");
                }
                if (dataBaseTypeEnum == DataBaseTypeEnum.Write)
                {
                    var providerName = dbNode.Element("Write").Attribute("providerName").Value;
                    dic.Add("providerName", providerName);
                    var connectionString = dbNode.Element("Write").Attribute("connectionString").Value;
                    dic.Add("connectionString", connectionString);

                    return dic;
                }
                else
                {
                    var providerName = dbNode.Element("Read").Attribute("providerName").Value;
                    dic.Add("providerName", providerName);
                    var connectionString = dbNode.Element("Read").Attribute("connectionString").Value;
                    dic.Add("connectionString", connectionString);

                    return dic;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据库配置文件配置错误", ex);
            }
        }
    }
}
