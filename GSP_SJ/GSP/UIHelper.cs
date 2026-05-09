using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP
{
    /// <summary>
    /// 最简单实用的UI刷新类
    /// </summary>
    public static class UIHelper
    {
        // 存储控件引用
        private static readonly ConcurrentDictionary<string, Control> _controls = new ConcurrentDictionary<string, Control>();

        /// <summary>
        /// 注册控件
        /// </summary>
        public static void Register(string key, Control control)
        {
            _controls[key] = control;
        }

        /// <summary>
        /// 移除控件
        /// </summary>
        public static void Remove(string key)
        {
            _controls.TryRemove(key, out _);
        }

        /// <summary>
        /// 更新UI - 最简单的方法
        /// </summary>
        public static void Run(string key, Action<Control> action)
        {
            if (!_controls.TryGetValue(key, out var control)) return;

            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() =>
                {
                    if (!control.IsDisposed) action(control);
                }));
            }
            else
            {
                if (!control.IsDisposed) action(control);
            }
        }

        /// <summary>
        /// 强类型更新
        /// </summary>
        public static void Run<T>(string key, Action<T> action) where T : Control
        {
            Run(key, c => action((T)c));
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        public static void RunAll(params (string key, Action<Control> action)[] updates)
        {
            foreach (var (key, action) in updates)
            {
                Run(key, action);
            }
        }
    }

    /// <summary>
    /// 最简控件扩展方法（修复了InvokeRequired错误）
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// 如果需要则Invoke
        /// </summary>
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
                control.Invoke(action);
            else
                action();
        }

        /// <summary>
        /// 安全设置文本
        /// </summary>
        public static void SafeText(this Control control, string text)
        {
            control.InvokeIfRequired(() => control.Text = text);
        }

        /// <summary>
        /// 安全设置启用状态
        /// </summary>
        public static void SafeEnable(this Control control, bool enabled)
        {
            control.InvokeIfRequired(() => control.Enabled = enabled);
        }

        /// <summary>
        /// 安全设置可见性
        /// </summary>
        public static void SafeVisible(this Control control, bool visible)
        {
            control.InvokeIfRequired(() => control.Visible = visible);
        }

        /// <summary>
        /// 安全添加项目到ListBox
        /// </summary>
        public static void SafeAdd(this ListBox listBox, object item)
        {
            listBox.InvokeIfRequired(() => listBox.Items.Add(item));
        }

        /// <summary>
        /// 安全设置进度条
        /// </summary>
        public static void SafeValue(this ProgressBar progressBar, int value)
        {
            progressBar.InvokeIfRequired(() => progressBar.Value = value);
        }

        /// <summary>
        /// 安全添加文本到TextBox
        /// </summary>
        public static void SafeAppend(this TextBox textBox, string text)
        {
            textBox.InvokeIfRequired(() => textBox.AppendText(text));
        }
    }

    /// <summary>
    /// 常用UI操作快捷方法
    /// </summary>
    public static class QuickUI
    {
        /// <summary>
        /// 显示加载状态
        /// </summary>
        public static void ShowLoading(string buttonKey, string labelKey, string message = "处理中...")
        {
            UIHelper.Run<Button>(buttonKey, btn =>
            {
                btn.InvokeIfRequired(() =>
                {
                    btn.Enabled = false;
                    btn.Text = "处理中...";
                });
            });

            UIHelper.Run<Label>(labelKey, lbl =>
            {
                lbl.InvokeIfRequired(() =>
                {
                    lbl.Text = message;
                    lbl.Visible = true;
                });
            });
        }

        /// <summary>
        /// 隐藏加载状态
        /// </summary>
        public static void HideLoading(string buttonKey, string labelKey, string originalText = "开始")
        {
            UIHelper.Run<Button>(buttonKey, btn =>
            {
                btn.InvokeIfRequired(() =>
                {
                    btn.Enabled = true;
                    btn.Text = originalText;
                });
            });

            UIHelper.Run<Label>(labelKey, lbl =>
            {
                lbl.InvokeIfRequired(() => lbl.Visible = false);
            });
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        public static void UpdateProgress(string progressKey, string labelKey, int value, string format = "进度: {0}%")
        {
            UIHelper.Run<ProgressBar>(progressKey, pb =>
            {
                pb.InvokeIfRequired(() => pb.Value = Math.Min(Math.Max(value, pb.Minimum), pb.Maximum));
            });

            UIHelper.Run<Label>(labelKey, lbl =>
            {
                lbl.InvokeIfRequired(() => lbl.Text = string.Format(format, value));
            });
        }
    }
}