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
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnRun_Click(object sender, RoutedEventArgs e)
    {
        // MessageBox.Show("Add items " + txtCount.Text);
        btnCancel.IsEnabled = true;
        btnSuspend.IsEnabled = true;
        btnSuspend.Content = "Призупинити";
        
        double count = double.Parse(txtCount.Text);
        var thread = new Thread(() => InsertItems(count));
        thread.Start();
        
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
        UserService userService = new();
        int count = (int)pbStatus.Value;
        userService.DeleteLastUsers(count);

        pbStatus.Value = 0;
        
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
        btnSuspend.Content = _isSuspended ? "Продовжити" : "Призупинити";

        _isSuspended = !_isSuspended;
    }

    private void UserService_InsertUserEvent(int count)
    {
        Dispatcher.Invoke(() => { pbStatus.Value = count; });
        _manualResetEvent.WaitOne(Timeout.Infinite);
    }
}