using BrowApp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GSP
{
    public class TcpClass : IDisposable
    {
        private bool _disposed = false;
        private readonly object _eventLock = new object();
        private EventHandler<string> _tcpMessageReceived;
        private int _receiveCount = 0;

        public BrowLib.Communication.Client TCP { get; private set; }
        public bool TCP_AStaart { get; set; }
        public bool TCP_MStaart { get; set; }
        public string TCP_Result { get; private set; }

        // 简单的事件定义
        public event EventHandler<string> TcpMessageReceived
        {
            add
            {
                lock (_eventLock)
                {
                    _tcpMessageReceived += value;
                }
            }
            remove
            {
                lock (_eventLock)
                {
                    _tcpMessageReceived -= value;
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void Send(string message, string step = "Null")
        {
            TCP.SendData(message);
            APP.Log.I_Log("FAI", step, "S:" + message.Trim());
        }
        /// <summary>
        /// TCP连接初始化
        /// </summary>
        public bool TCPini(string ip, int port, object deviceType)
        {
            try
            {
                // 清理旧的连接
                CleanupTCP();
                TCP = new BrowLib.Communication.Client(ip, port, deviceType);
                TCP.DoInit();
                TCP.DoStart();
                TCP.OnSocketReceive += SocketReceiveHandler;
                return TCP.IsConnected;
            }
            catch (Exception ex)
            {
                APP.Log.E_Log("TCPConnect连接失败:", ex);
                return false;
            }
        }
        /// <summary>
        /// TCP接收处理器
        /// </summary>
        private void SocketReceiveHandler(object deviceType, System.Net.Sockets.Socket socket,
                                        byte[] dataB, string dataS)
        {
            try
            {
                Interlocked.Increment(ref _receiveCount);
                TCP_Result = dataS?.Trim() ?? "";

                APP.Log.I_Log("FAI Return", "Null", TCP_Result);

                if (string.IsNullOrEmpty(TCP_Result)) return;

                // 处理消息
                if (TCP_Result.StartsWith("A:"))
                {
                    TCP_AStaart = true;
                }
                else if (TCP_Result.StartsWith("M:"))
                {
                    TCP_MStaart = true;
                    OnMessageReceived(TCP_Result);
                }

                // 防止过快处理导致CPU过高（每处理100条消息休息1ms）
                if (_receiveCount % 100 == 0)
                {
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                APP.Log.E_Log("TCPReceive处理消息失败",ex);
            }
        }

        /// <summary>
        /// 安全触发事件
        /// </summary>
        private void OnMessageReceived(string message)
        {
            EventHandler<string> handlers;
            lock (_eventLock)
            {
                handlers = _tcpMessageReceived;
            }

            handlers?.Invoke(this, message);
        }

        /// <summary>
        /// 关闭TCP连接
        /// </summary>
        public void TCPClose()
        {
            try
            {
                if (TCP != null)
                {
                    TCP.OnSocketReceive -= SocketReceiveHandler;
                    TCP.DoStop();
                    TCP.DoRelease();
                    TCP = null;
                }

                // 清理事件订阅（防止内存泄漏）
                ClearEventSubscriptions();

                APP.Log.I_Log("TCP", "Close", "连接已关闭");
            }
            catch (Exception ex)
            {
                APP.Log.E_Log("TCPClose关闭失败", ex);
            }
        }

        /// <summary>
        /// 清理TCP资源
        /// </summary>
        private void CleanupTCP()
        {
            if (TCP != null)
            {
                TCP.OnSocketReceive -= SocketReceiveHandler;
                TCP.DoStop();
                TCP.DoRelease();
                TCP = null;
            }
        }

        /// <summary>
        /// 清除所有事件订阅
        /// </summary>
        public void ClearEventSubscriptions()
        {
            lock (_eventLock)
            {
                _tcpMessageReceived = null;
            }
        }

        /// <summary>
        /// 获取当前事件订阅数量
        /// </summary>
        public int GetEventSubscriberCount()
        {
            lock (_eventLock)
            {
                return _tcpMessageReceived?.GetInvocationList()?.Length ?? 0;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                TCPClose();
                _disposed = true;
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~TcpClass()
        {
            Dispose();
        }
    }
}