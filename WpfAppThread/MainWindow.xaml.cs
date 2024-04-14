using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
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
using Helpers;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Webp;

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

        var time = Helpers.Timer.Time(() =>
        {
            var userService = new UserService();
            var users = userService.GetUsers();
        });
        MessageBox.Show($"Час отримання {time}", "Результат");
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

    private void BtnAddDragons_OnClick(object sender, RoutedEventArgs e)
    {
        const int count = 1000;
        Dispatcher.Invoke(() =>
        {
            pbStatus.Value = 0;
            pbStatus.Maximum = 2 * count;
        });
        
        UserService userService = new();
        userService.InsertUserEvent += UserService_InsertUserEvent;
        List<Thread> threads = new();

        var lengthThreads = Helpers.Timer.Time(() =>
        {
            for (int _ = 0; _ < count; _++)
            {
                Thread thread = new Thread(() =>
                {
                    ImageLoader.SaveImage(new Uri(@"https://loremflickr.com/320/240"), "dragons");
                });
                thread.Start();
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        });


        var lengthSync = Helpers.Timer.Time(() =>
        {
            for (int _ = 0; _ < count; _++)
            {
                ImageLoader.SaveImage(new Uri(@"https://loremflickr.com/320/240"), "dragons");
            }
        });
        
        var ratio = lengthSync / lengthThreads;
        
        MessageBox.Show(
            $"Час із потоками: {lengthThreads} \nЧас без потоків: {lengthSync}\nБез потоків повільніше у {ratio} разів",
            "Результат");
    }
}