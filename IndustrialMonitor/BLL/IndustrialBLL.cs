using Communication;
using IndustrialMonitor.DAL;
using IndustrialMonitor.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.BLL
{
    public class IndustrialBLL
    {
        // 获取数据信息的对象 
        DataAccess dataAccess = new DataAccess();

        // 获取串口信息
        public DataResult<SerialInfo> InitSerialInfo()
        {
            // 获取串口信息
            DataResult<SerialInfo> result = new DataResult<SerialInfo>();
            result.State = false;
            try
            {
                SerialInfo si = new SerialInfo();
                //通过配置文件获取串口信息
                si.PortName = ConfigurationManager.AppSettings["portName"].ToString();
                si.BaudRate = Convert.ToInt32(ConfigurationManager.AppSettings["baudRate"].ToString());
                si.DataBits = Convert.ToInt32(ConfigurationManager.AppSettings["dataBits"].ToString());
                si.StopBits = (StopBits)Enum.Parse(typeof(StopBits), ConfigurationManager.AppSettings["stopBits"].ToString(), true);
                si.Parity = (Parity)Enum.Parse(typeof(Parity), ConfigurationManager.AppSettings["parity"].ToString(), true);
                result.State = true;
                result.Data = si;
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = ex.Message.ToString();
            }

            return result;
        }

        public DataResult<StorageModel> InitStorageArea()
        {
            DataResult<StorageModel> result = new DataResult<StorageModel>();
            try
            {
                StorageModel model = new StorageModel();
                DataTable table = dataAccess.GetStorageArea();
                List<StorageModel> values = (from q in table.AsEnumerable()
                                       select new StorageModel
                                       {
                                           Id=q.Field<String>("id"),
                                           SlaveAddress=q.Field<Int32>("slave_add"),
                                           FuncCode=q.Field<string>("func_code"),
                                           StartAddress=int.Parse(q.Field<string>("start_reg")),
                                           Length= int.Parse(q.Field<string>("length"))
                                       }).ToList();

                result.State = true;
                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message.ToString();
            }


            return result;
        }
    }
}
