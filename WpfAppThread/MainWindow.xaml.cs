using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bogus;
using DAL.Data;
using DAL.Data.Entities;
using DAL.Services;
using Microsoft.EntityFrameworkCore;

namespace WpfAppThread;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static ManualResetEvent _manualResetEvent = new(false);
    private bool _isSuspended = false;
    private Thread _insertThread;
    private CancellationTokenSource _ctSource;
    private CancellationToken _cancellationToken;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnRun_Click(object sender, RoutedEventArgs e)
    {
        _ctSource = new();
        _cancellationToken = _ctSource.Token;
        // MessageBox.Show("Add items " + txtCount.Text);
        btnCancel.IsEnabled = true;
        btnSuspend.IsEnabled = true;
        btnSuspend.Content = "Призупинити";
        
        double count = double.Parse(txtCount.Text);
        _insertThread = new Thread(() => InsertItems(count));
        _insertThread.Start();
        
        btnRun.IsEnabled = false;
        _manualResetEvent.Set();
    }

    private void InsertItems(double count)
    {
        UserService userService = new();
        userService.InsertUserEvent += UserService_InsertUserEvent;
        Dispatcher.Invoke(() =>
        {
            // pbStatus.Value = 0;
            pbStatus.Maximum = (int)count;
        });

        
        userService.InsertRandomUser((int)count);

        Dispatcher.Invoke(() =>
        {
            btnRun.IsEnabled = true;
            // btnSuspend.IsEnabled = false;
        });
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        _ctSource.Cancel();
        pbStatus.Value = 0;

        btnRun.IsEnabled = true;
        btnCancel.IsEnabled = false;
        btnSuspend.IsEnabled = false;
    }

    private void BtnSuspend_Click(object sender, RoutedEventArgs e)
    {
        if (_isSuspended)
        {
            _manualResetEvent.Set();
        }
        else
        {
            _manualResetEvent.Reset();
        }
        btnSuspend.Content = _isSuspended ? "Призупинити" : "Продовжити";

        _isSuspended = !_isSuspended;
    }

    private void UserService_InsertUserEvent(int count)
    {
        Dispatcher.Invoke(() => { pbStatus.Value = count; });
        _manualResetEvent.WaitOne(Timeout.Infinite);
    }
}