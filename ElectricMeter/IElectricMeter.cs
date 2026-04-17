using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricMeter
{
    /// <summary>
    /// 电表接口
    /// </summary>
    public interface IElectricMeter
    {

        /// <summary>
        /// 连接电表
        /// </summary>
        void Connect();

        /// <summary>
        /// 断开连接
        /// </summary>
        void Close();

        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        bool IsConnected();

        /// <summary>
        /// 发送读请求
        /// </summary>
        void ReadRequest();

        ReadStatus GetReadStatus();

        /// <summary>
        /// 复位读请求状态
        /// </summary>
        void ResetReadStatus();

        /// <summary>
        /// 当前值
        /// </summary>
        /// <returns></returns>
        double Value();
        
        /// <summary>
        /// 设置测量电阻
        /// </summary>
        void SetType(TestType type);

        /// <summary>
        /// 设置评率
        /// </summary>
        /// <param name="frequency"></param>
        void SetFrequency(string frequency);
    }

    public enum ReadStatus
    {
        /// <summary>
        /// 未发送指令
        /// </summary>
        None,
        /// <summary>
        /// 等待返回
        /// </summary>
        Waiting,
        /// <summary>
        /// 已接收数据
        /// </summary>
        Finished
    }

    /// <summary>
    /// 测量元件类型
    /// </summary>
    public enum TestType
    {
        /// <summary>
        /// 无类型
        /// </summary>
        None,
        /// <summary>
        /// 电阻
        /// </summary>
        R,
        /// <summary>
        /// 电容
        /// </summary>
        C,
        /// <summary>
        /// 电感
        /// </summary>
        H
    }

    /// <summary>
    /// 电表类型
    /// </summary>
    public enum ElectricMeterType
    {

    }
}
