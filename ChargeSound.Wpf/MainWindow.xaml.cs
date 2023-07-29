using System.Windows;

namespace ChargeSound.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ChargeEventListener chargeEventListener;

    public MainWindow()
    {
        InitializeComponent();
        chargeEventListener = new();
        chargeEventListener.PowerLineStatusChanged += ChargeEventListener_PowerLineStatusChanged;
    }

    private void ChargeEventListener_PowerLineStatusChanged(PowerLineStatus status)
    {
        switch (status)
        {
            case PowerLineStatus.Offline:
                ListView.Items.Add($"{DateTimeOffset.UtcNow}：已拔掉电源");
                break;
            case PowerLineStatus.Online:
                ListView.Items.Add($"{DateTimeOffset.UtcNow}：已插入电源");
                break;
            case PowerLineStatus.Unknown:
            default:
                ListView.Items.Add($"{DateTimeOffset.UtcNow}：(?) 电源状态未知");
                break;
        }
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        chargeEventListener.Dispose();
    }
}
