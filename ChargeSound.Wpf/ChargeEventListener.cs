using System.Windows;
using System.Windows.Interop;
using Windows.Win32;
using Windows.Win32.System.Power;
using Windows.Win32.Foundation;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace ChargeSound.Wpf;

public sealed class ChargeEventListener : IDisposable
{
    private readonly HiddenWindow hiddenWindow = new();
    private bool disposedValue;

    public event Action<PowerLineStatus>? PowerLineStatusChanged;

    public ChargeEventListener()
    {
        hiddenWindow.Show();
        hiddenWindow.PowerStateChanged += OnPowerLineStatusChanged;
    }

    private void OnPowerLineStatusChanged()
    {
        BOOL result = PInvoke.GetSystemPowerStatus(out SYSTEM_POWER_STATUS status);

        if (result == 0)
        {
            int error = Marshal.GetLastPInvokeError();
            throw new Win32Exception(error);
        }

        PowerLineStatusChanged?.Invoke((PowerLineStatus)status.ACLineStatus);
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
                hiddenWindow.Close();
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
    }

    ~ChargeEventListener()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

internal class HiddenWindow : Window
{
    public event Action? PowerStateChanged;

    public HiddenWindow() : base()
    {
        Title = "电源事件接收器窗口";
        Width = 0;
        Height = 0;
        ShowInTaskbar = false;
        Visibility = Visibility.Hidden;
        ResizeMode = ResizeMode.NoResize;
        WindowStyle = WindowStyle.None; 
        Loaded += HiddenWindow_Loaded;
    }

    private void HiddenWindow_Loaded(object sender, RoutedEventArgs e)
    {
        HwndSource? hwndSource = PresentationSource.FromVisual(this) as HwndSource;
        hwndSource?.AddHook(new HwndSourceHook(WndProc));
    }

    protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case 0x218:
                {
                    if (wParam.ToInt32() == 0xA)
                    {
                        PowerStateChanged?.Invoke();
                        handled = true;
                    }

                    break;
                }
        }
        return IntPtr.Zero;
    }
}
