using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace FindFileByName
{
    public class FileFind 
    {
        private static double TIMER_INTERVAL = 1000;

        private readonly MainWindow _window;
        private readonly BackgroundWorker _worker;
        private readonly Timer _timer;

        private DateTime _startTime;
        private int _totalFiles;
        private int _foundFiles;
        private bool _isCancel;
        private bool _isPaused;
        private ObservableCollection<Node> _nodes;

        private string _folder;
        private string _mask;

        /// <summary>
        /// Объект для поиска файлов.
        /// </summary>
        /// <param name="window">Объект для обратной связи (отображения результатов)</param>
        public FileFind(MainWindow window)
        {
            _window = window;

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += Worker_DoWork;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            _timer = new Timer(TIMER_INTERVAL);
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e) => showResalt();

        /// <summary>
        /// Запускает поиск
        /// </summary>
        /// <param name="folder">Стартовая папка</param>
        /// <param name="mask">Маска поиска</param>
        /// <param name="nodes">Корень собираемой коллекции(возможно есть смысл передавать при запуске поиска)</param>
        public void Start(string folder, string mask, ObservableCollection<Node> nodes)
        {
            if (_worker.IsBusy)
                return;

            _folder = folder;
            _mask = mask;
            _nodes = nodes;
            _foundFiles = 0;
            _totalFiles = 0;
            _isCancel = false;
            _startTime = DateTime.Now;

            _worker.RunWorkerAsync();
            showResalt();
            _timer.Start();
        }

        /// <summary>
        /// Приостанавливает поиск с возможномтью продолжения.
        /// </summary>
        public void Pause()
        {
            _isPaused = !_isPaused;
        }

        /// <summary>
        /// Останавливает поиск
        /// </summary>
        public void Stop()
        {
            _isCancel = true;
            _worker.CancelAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (!Directory.Exists(_folder))
                    throw new ArgumentException($"Папка не существует: {_folder}");

                Regex reg = new Regex(_mask);

                FindFile(_folder, reg);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorWindow(ex.Message);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _timer.Stop();
            showResalt();
        }

        private void FindFile(string folder, Regex reg)
        {
            if (_isCancel) return;

            _window.ShowCurrentDir(folder);
            foreach (string file in Directory.EnumerateFiles(folder))
            {
                if (reg.IsMatch(file))
                {
                    _nodes[0].Add(file, _window.Dispatcher);
                    _foundFiles++;
                }
                _totalFiles++;

                while (_isPaused)
                    Thread.Sleep(100);
            }

            foreach (string dir in Directory.EnumerateDirectories(folder))
                FindFile(dir, reg);
        }

        private void showResalt()
        {
            TimeSpan duraction = DateTime.Now - _startTime;
            var result = new Result()
            {
                totalFiles = _totalFiles,
                foundFiles = _foundFiles,
                timeLeft = duraction,
            };
            _window.SetResult(result);
        }
    }
}
