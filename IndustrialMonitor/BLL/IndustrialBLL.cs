using Communication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialMonitor.BLL
{
    public class IndustrialBLL
    {
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
                DataResult dr = new DataResult();
                dr.State = false;
                dr.Message = ex.Message.ToString();
                dr.Data = null;
            }

            return result;
        }
    }
}
