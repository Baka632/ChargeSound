using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Controls;

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
        chargeEventListener.PowerLineStatusChanged += OnPowerLineStatusChanged;

        Application.Current.Exit += (sender, e) => chargeEventListener.Dispose();
    }

    private void OnPowerLineStatusChanged(PowerLineStatus status)
    {
        switch (status)
        {
            case PowerLineStatus.Offline:
                {
                    ListView.Items.Add($"{DateTimeOffset.UtcNow}：已拔掉电源");
                    SoundPlayer player = new(Properties.Resources.Windows_Hardware_Fail);
                    player.Play();
                    break;
                }
            case PowerLineStatus.Online:
                {
                    ListView.Items.Add($"{DateTimeOffset.UtcNow}：已插入电源");
                    SoundPlayer player = new(Properties.Resources.Windows_Notify_Messaging);
                    player.Play();
                    break;
                }
            case PowerLineStatus.Unknown:
            default:
                ListView.Items.Add($"{DateTimeOffset.UtcNow}：(?) 电源状态未知");
                break;
        }
    }

    private void OnTrayDoubleClick(object sender, RoutedEventArgs e)
    {
        Application.Current.MainWindow.Show();
    }

    private void ExitApplication(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        this.Hide();
        NotifyIcon.Visibility = Visibility.Visible;
        e.Cancel = true;
    }
}
