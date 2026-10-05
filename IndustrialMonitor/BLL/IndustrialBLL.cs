using Communication;
using IndustrialMonitor.common;
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
using System.Windows.Media.Media3D;

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

        /// <summary>
        /// 初始化存储区-从数据库中读取存储区信息
        /// </summary>
        /// <returns></returns>
        public DataResult<List<StorageModel>> InitStorageArea()
        {
            DataResult<List<StorageModel>> result = new DataResult<List<StorageModel>>();
            try
            {
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
                result.Data = values;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message.ToString();
            }


            return result;
        }

        public DataResult<List<DevicesModel>> InitDevices()
        {
            DataResult<List<DevicesModel>> result = new DataResult<List<DevicesModel>>();
            try
            {
                DataTable deviceTable = dataAccess.GetDevices();
                DataTable monitorValueTable = dataAccess.GetMonitorValues();
                List<DevicesModel> deviceList = new List<DevicesModel>();

                foreach(var q in deviceTable.AsEnumerable())
                {
                    DevicesModel dModel = new DevicesModel();
                    deviceList.Add(dModel);
                    dModel.Id = q.Field<string>("id");
                    dModel.DeviceName = q.Field<string>("d_name");

                    foreach(var mv in monitorValueTable.AsEnumerable().Where(m => m.Field<string>("d_id") == dModel.Id))
                    {
                        MonitorValuesModel mvm = new MonitorValuesModel();
                        dModel.MonitorValuesList.Add(mvm);

                        mvm.ValueId = mv.Field<string>("value_id");
                        mvm.ValueName = mv.Field<string>("value_name");
                        mvm.StorageAreaId = mv.Field<string>("s_area_id");
                        mvm.StartAddress = mv.Field<int>("address");
                        mvm.IsAlarm = mv.Field<bool>("is_alarm");
                        mvm.Description = mv.Field<string>("description");
                        mvm.Unit = mv.Field<string>("unit");
                        // 警戒值
                        var cloumn = mv.Field<string>("alarm_lolo");
                        mvm.LoLoAlarm = cloumn == null ? 0.0 : double.Parse(cloumn);
                        cloumn = mv.Field<string>("alarm_low");
                        mvm.LowAlarm = cloumn == null ? 0.0 : double.Parse(cloumn);
                        cloumn = mv.Field<string>("alarm_high");
                        mvm.HighAlarm = cloumn == null ? 0.0 : double.Parse(cloumn);
                        cloumn = mv.Field<string>("alarm_hihi");
                        mvm.HiHiAlarm = cloumn == null ? 0.0 : double.Parse(cloumn);

                        mvm.ValueStateChanged = (state, msg, valueId) =>
                        {
                            // 先把当前值对应的警告信息删除在添加 
                            var index = dModel.WarngingMessageList.ToList().FindIndex(w => w.ValueId == valueId);
                            if(index > -1) // 说明存在
                            {
                                dModel.WarngingMessageList.RemoveAt(index);
                            }
                            if (state != MonitorValueState.OK)
                            {
                                dModel.isWarnning = true;
                                // 往设备中添加报警信息
                                WarningMessageModel wmm = new WarningMessageModel() { ValueId = valueId, Message = msg };
                                dModel.WarngingMessageList.Add(wmm);
                                
                            }
                            if(dModel.WarngingMessageList.Count > 0)
                            {
                                dModel.isWarnning = true;
                            }
                            else
                            {
                                dModel.isWarnning = false;
                            }
                        };
                    }
                }
                result.State = true;
                result.Data = deviceList;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message.ToString();
            }

            return result;
        }
    }
}
