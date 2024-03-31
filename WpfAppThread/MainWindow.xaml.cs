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
using Microsoft.EntityFrameworkCore;

namespace WpfAppThread;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool _isCancelled = false;
    private bool _isSuspended = false;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnRun_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Add items " + txtCount.Text);
        btnCancel.IsEnabled = true;
        btnSuspend.IsEnabled = true;
        btnSuspend.Content = "Призупинити";
        
        double count = double.Parse(txtCount.Text);
        var thread = new Thread(() => InsertItems(count));
        thread.Start();
        btnRun.IsEnabled = false;
    }

    private void InsertItems(double count)
    {
        var faker = new Faker<UserDbEntity>()
            .RuleFor(user => user.Id, _ => Guid.NewGuid())
            .RuleFor(user => user.FirstName, f => f.Name.FirstName())
            .RuleFor(user => user.LastName, f => f.Name.LastName())
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.FirstName, user.LastName))
            .RuleFor(user => user.PasswordHash, f => f.Internet.Password().GetHashCode());
        
        Dispatcher.Invoke(() =>
        {
            pbStatus.Value = 0;
            pbStatus.Maximum = count;
        });
        using ApplicationDbContext context = new();
        int i = 0;
        while (i < count)
        {
            if (_isCancelled)
            {
                _isCancelled = false;
                Dispatcher.Invoke(() =>
                {
                    pbStatus.Value = 0;
                    btnSuspend.IsEnabled = false;
                });
                break;
            }

            if (_isSuspended)
            {
                Thread.Sleep(1000);
                continue;
            }
            Dispatcher.Invoke(() => { pbStatus.Value++; });
            context.Users.Add(faker.Generate());
            Thread.Sleep(1000);
            i++;
        }

        context.SaveChanges();

        Dispatcher.Invoke(() => { btnRun.IsEnabled = true; });
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        _isCancelled = true;
        btnCancel.IsEnabled = false;
    }

    private void BtnSuspend_Click(object sender, RoutedEventArgs e)
    {
        _isSuspended = !_isSuspended;
        btnSuspend.Content = _isSuspended ? "Продовжити" : "Призупинити";
    }
}