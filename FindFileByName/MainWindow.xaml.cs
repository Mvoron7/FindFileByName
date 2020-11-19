﻿using System;
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

        public MainWindow()
        {
            InitializeComponent();

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
        /// Добавляет файл в дерево
        /// </summary>
        /// <param name="file">Имя файла</param>
        public void addFile(string file)
        {
            DoInvoke(() => { FileStack.Children.Add(new Label() { Content = file }); });
        }
        
        /// <summary>
        /// Отобразить промежуточные итоги
        /// </summary>
        public void SetResult(Result result) 
        {
            DoInvoke(() =>
            {
                TotalTime.Content = result.timeLeft.ToString();
                TotalFiles.Content = $"Всего файлов: {result.totalFiles}";
                FindFiles.Content = $"Найдено файлов: {result.foundFiles}";
                FileTree.ItemsSource = result.nodes;
            });
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            FileStack.Children.Clear();
            _fileFind.Start(FolderName.Text, FileMask.Text);
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
    }
}
