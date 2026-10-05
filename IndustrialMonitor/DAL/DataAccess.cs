using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.DAL
{
    public class DataAccess
    {
        string dbConfig = ConfigurationManager.ConnectionStrings["db_config"].ToString();
        MySqlConnection conn;
        MySqlCommand comm;
        MySqlDataAdapter adapter;
        MySqlTransaction trans;

        // 先销毁所有的连接实例
        private void Dispose()
        {
            if(conn != null)
            {
                conn.Close();
                conn.Dispose();
                conn = null;
            }
            if (comm != null)
            {
                comm.Dispose();
                comm = null;
            }
            if (adapter != null)
            {
                adapter.Dispose();
                adapter = null;
            }
            if (trans != null)
            {
                trans.Dispose();
                trans = null;
            }
        }

        /// <summary>
        /// 获取数据的通用方法，不对外暴露
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private DataTable GetDatas(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                conn = new MySqlConnection(dbConfig);
                conn.Open();

                adapter = new MySqlDataAdapter(sql, conn);
                adapter.Fill(dt);  // 把数据填充到 DataTable 中

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                this.Dispose();
            }

            return dt;
        }

        /// <summary>
        /// 获取存储区域信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetStorageArea()
        {
            string sql = "select * from storage_area";
            return this.GetDatas(sql);
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetDevices()
        {
            string sql = "select * from devices";
            return this.GetDatas(sql);
        }

        /// <summary>
        /// 获取监控值
        /// </summary>
        /// <returns></returns>
        public DataTable GetMonitorValues()
        {
            string sql = "select * from monitor_values order by d_id,value_id";
            return this.GetDatas(sql);
        }
    }
}
