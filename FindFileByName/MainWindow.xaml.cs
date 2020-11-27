using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FindFileByName
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileFind _fileFind;
        private ObservableCollection<Node> _nodes;

        public MainWindow()
        {
            InitializeComponent();

            _nodes = GetEmptyCollection();
            FileTree.ItemsSource = _nodes;

            _fileFind = new FileFind(this);

            FileMask.Text = Properties.Settings.Default.Mask;
            FolderName.Text = Properties.Settings.Default.StartFolder;
        }

        /// <summary>
        /// Отображает текущую дирректорию
        /// </summary>
        /// <param name="folder">Текущая директория</param>
        public void ShowCurrentDir(string folder)
        {
            DoInvoke(() => { CurrentDir.Content = folder; });
        }
        
        /// <summary>
        /// Отобразить промежуточные итоги
        /// </summary>
        public void SetResult(Result result) 
        {
            DoInvoke(() =>
            {
                TotalTime.Content = $"Время: " + result.timeLeft.ToString("hh':'mm':'ss");
                TotalFiles.Content = $"Файлов обработано: {result.totalFiles}";
                FindFiles.Content = $"Файлов найдено: {result.foundFiles}";
            });
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            _nodes = GetEmptyCollection();
            FileTree.ItemsSource = _nodes;
            _fileFind.Start(FolderName.Text, FileMask.Text, _nodes);
        }

        private void Button_Pause(object sender, RoutedEventArgs e)
        {
            _fileFind.Pause();
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            _fileFind.Stop();
        }

        private void DoInvoke(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            Dispatcher.Invoke(action, priority);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Mask = FileMask.Text;
            Properties.Settings.Default.StartFolder = FolderName.Text;
            Properties.Settings.Default.Save();
        }

        private ObservableCollection<Node> GetEmptyCollection()
        {
            return new ObservableCollection<Node>() { new Node() { Name = "Root", Nodes = new ObservableCollection<Node>() } };
        }
    }
}
