using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintBase
{
    /// <summary>
    /// 打印机状态
    /// </summary>
    public enum PrintError
    { /// <summary>
        /// 正常
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 发送内容为null
        /// </summary>
        SendNull=2,
        /// <summary>
        /// 发送失败
        /// </summary>
        SendFailure=3,
        /// <summary>
        /// 打开打印机失败
        /// </summary>
        OpenFailure=4,
        /// <summary>
        /// 连接打印机失败
        /// </summary>
        ConnectedFailure=5,
        /// <summary>
        /// 没有权限
        /// </summary>
        NoPermission=6,
        /// <summary>
        /// 没有找到打印设备
        /// </summary>
        NotFindDevice=7,
        /// <summary>
        /// 图片命令转换异常
        /// </summary>
        PosCMDErr=8,


        /// <summary>
        /// 打印机异常
        /// </summary>
        Error = 100,
        /// <summary>
        /// 缺纸
        /// </summary>
        Nopaper = 101,

        /// <summary>
        /// 不支持蓝牙
        /// </summary>
        NotSupportBluetooth=100,
        /// <summary>
        /// 蓝牙没有开启
        /// </summary>
        NotOpenBluetooth = 201,
        /// <summary>
        /// 蓝牙未连接
        /// </summary>
        NotConnectedBluetooth = 202

    }
}
