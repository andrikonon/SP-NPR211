using System.IO;
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
using Helpers;
using Microsoft.Win32;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CopyFiles;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string? _inputPath;
    private string? _outputPath;
    
    private delegate void CopiedFileDelegate();

    private event CopiedFileDelegate CopiedFileEvent;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BtnChooseInputFolder_OnClick(object sender, RoutedEventArgs e)
    {
        _inputPath = SelectFolder();
        if (_inputPath is null)
        {
            MessageBox.Show("Ви не вибрали папки");
            return;
        }
        
        lblInputFolder.Text = _inputPath;
        UpdateCopyButtonStatus();

        Console.WriteLine(_inputPath);
        MessageBox.Show($"Ваша папка: {_inputPath}");
    }

    private void UpdateCopyButtonStatus()
    {
        bool isReady = _inputPath is not null && _outputPath is not null;
        btnCopy.IsEnabled = isReady;
    }

    private void BtnChooseOutputFolder_OnClick(object sender, RoutedEventArgs e)
    {
        _outputPath = SelectFolder();
        if (_outputPath is null)
        {
            MessageBox.Show("Ви не вибрали папки");
            return;
        }

        lblOutputFolder.Text = _outputPath;
        UpdateCopyButtonStatus();
        Console.WriteLine(_outputPath);
        MessageBox.Show($"Ваша папка: {_outputPath}");
    }

    private string? SelectFolder()
    {
        using var dialog = new FolderBrowserDialog();
        var result = dialog.ShowDialog();
        if (result == System.Windows.Forms.DialogResult.OK)
            return dialog.SelectedPath;

        return null;
    }

    private async void BtnCopy_OnClick(object sender, RoutedEventArgs e)
    {
        var filesToCopy = Directory.GetFiles(_inputPath, "*.*", SearchOption.AllDirectories);
        int count = filesToCopy.Length;
        Console.WriteLine(count);

        pbStatus.Value = 0;
        pbStatus.Maximum = count;
        
        CopiedFileEvent += () =>
        {
            Dispatcher.Invoke(() =>
            {
                pbStatus.Value += 1;
            });
        };
        List<Task> tasks = new();

        foreach (var file in filesToCopy)
        {
            Console.WriteLine(file);
            var newPath = System.IO.Path.GetDirectoryName(file.Replace(_inputPath, _outputPath));
            Console.WriteLine(newPath);
            tasks.Add(Task.Run(() =>
            {
                Copier.Copy(file, newPath);
                CopiedFileEvent();
            }));
        }

        await Task.WhenAll(tasks);
    }
}