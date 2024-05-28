using CourseProdject.Model;
using ScottPlot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CourseProdject.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        string lastNameSelectProcess = "";
        EventWaitHandle loggerEvent;
        private ProcessInfo? selectedProcess;
        private ObservableCollection<ProcessInfo> listProcess;
        // Графік для відображення використання CPU.
        public WpfPlot CPUPlot { get; set; }
        // Графік для відображення використання пам'яті.
        public WpfPlot MemoryPlot { get; set; }

        // Виведення загальної кількості процесів
        public string TotalProcesses
        {
            get
            {
                string num_procc = "Started Processes: " + Process.GetProcesses().Length;
                return num_procc;
            }
        }

        public ObservableCollection<ProcessInfo> ListProcess
        {
            get { return listProcess; }
            set
            {
                if (listProcess != value)
                {
                    listProcess = value;
                    OnPropertyChanged(nameof(listProcess));
                }
            }
        }
        // Обраний процес.
        public ProcessInfo? SelectedProcess
        {
            get { return selectedProcess; }
            set
            {
                if (selectedProcess != value)
                {
                    selectedProcess = value;
                    OnPropertyChanged(nameof(SelectedProcess));
                }
            }
        }
        /// <summary>
        /// Конструктор класу <see cref="MainViewModel"/>.
        /// </summary>
        public MainViewModel()
        {
            listProcess = new ObservableCollection<ProcessInfo>();
            loggerEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
            SelectedProcess = null;
        }
        /// <summary>
        /// Асинхронний метод завантаження даних про процеси.
        /// </summary>
        public async Task LoadData()
        {
            Process[] processes;
            List<ProcessInfo> noGroupProcessesList = new List<ProcessInfo>();

            while (true)
            {
                //Подія, яка стає в false, якщо логування не почалося
                loggerEvent.Reset();

                processes = Process.GetProcesses();
                noGroupProcessesList.Clear();

                foreach (Process process in processes)
                {
                    try
                    {
                        //Отримання інформації про кожен процес
                        double cpuUsage = process.TotalProcessorTime.TotalSeconds;
                        double memoryUsage = process.WorkingSet64 / (1024 * 1024);
                        // description procces
                        string programName = "";
                        try
                        {
                            programName = process.MainModule.ModuleName;
                        }
                        catch (Exception ex)
                        {
                            // Обработка возможных ошибок при получении названия программы
                            programName = "Error. The file could not be found";
                        }
                        ProcessInfo infoProcess = new ProcessInfo(process.ProcessName, process.Id, cpuUsage, memoryUsage, programName);
                        noGroupProcessesList.Add(infoProcess);
                    }
                    catch
                    {

                    }
                }

                //Групування процесів
                var groupedData = noGroupProcessesList
                    .GroupBy(obj => new { obj.Name, obj.Description }) // Групування по Name та Description
                    .Select(group => new ProcessInfo(
                        group.Key.Name,
                        group.Sum(obj => obj.PidProcess),
                        group.Sum(obj => obj.CpuUsage),
                        group.Sum(obj => obj.MemoryUsage),
                        group.Key.Description)) // Виккористання Key.Description для опису
                    .ToList();

                //Виведення результав обробки на інтерфейс
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string name = "";
                    if (SelectedProcess != null)
                    {
                        name = SelectedProcess.Name;
                    }

                    ListProcess.Clear();
                    foreach (var processInfo in groupedData)
                    {
                        processInfo.CpuUsage = Math.Round(processInfo.CpuUsage, 1);
                        processInfo.MemoryUsage = Math.Round(processInfo.MemoryUsage, 1);
                        ListProcess.Add(processInfo);
                    }

                    if (name != "")
                    {
                        SelectedProcess = ListProcess.FirstOrDefault(a => a.Name == name);
                    }
                });

                //Подія в true для логування
                loggerEvent.Set();

                //Збереження останнього вибраного процесу та оновлення графіків
                if(SelectedProcess != null)
                {
                    if (lastNameSelectProcess != SelectedProcess.Name)
                    {
                        await UpdateGraphicsData();
                        lastNameSelectProcess = SelectedProcess.Name;
                    }
                }

                await Task.Delay(1000);
            }
        }
        /// <summary>
        /// Асинхронний метод логування даних про процеси.
        /// </summary>
        public async Task LoggingData()
        {
            bool newDate = true;
            while (true)
            {
                loggerEvent.WaitOne();

                try
                {
                    DateTime currentDate = DateTime.Now.Date;
                    if (File.Exists("logProcess.txt"))
                    {
                        newDate = false;
                        var logLines = File.ReadLines("logProcess.txt").ToList();

                        if (logLines.Count > 0)
                        {
                            //Отримання данних з логуючого файлу
                            DateTime logEntryDate = DateTime.ParseExact(logLines.Last().Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (currentDate != logEntryDate)
                            {
                                newDate = true;
                            }
                        }
                    }
                    //Запис данних в логуючий файл
                    using (StreamWriter writer = new StreamWriter("logProcess.txt", !newDate))
                    {
                        newDate = false;
                        foreach (var processInfo in listProcess)
                        {
                            writer.WriteLine(processInfo.CreateLog());
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Task.Run(() =>
                    {
                        MessageBox.Show(ex.Message);
                    });
                }
                //Оновлення графіків
                await UpdateGraphicsData();
                await Task.Delay(10000);
            }
        }
        /// <summary>
        /// Асинхронний метод завершення процесу.
        /// </summary>
        public async Task TerminateProcess()
        {
            if(SelectedProcess == null)
            {
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    Process[] processes = Process.GetProcessesByName(SelectedProcess.Name);

                    //Завершення всіх процесів в групі
                    foreach (Process process in processes)
                    {
                        process.Kill();
                    }
                    //Очищення графіків
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CPUPlot.Plot.Clear();
                        MemoryPlot.Plot.Clear();

                        CPUPlot.Render();
                        MemoryPlot.Render();
                    });
                });
            }
            catch(Exception ex)
            {
                await Task.Run(() =>
                {
                    MessageBox.Show(ex.Message);
                });
            }
        }
        /// <summary>
        /// Асинхронний метод завершення конкретного процесу.
        /// </summary>
        public async Task TerminateSingleProcess()
        {
            if (SelectedProcess == null)
            {
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    //находим процесс по pid
                    int pid = (int)SelectedProcess.PidProcess;
                    Process processes = Process.GetProcessById(pid);
                    processes.Kill();

                    // Очищение графиков (предполагается, что CPUPlot и MemoryPlot доступны)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CPUPlot.Plot.Clear();
                        MemoryPlot.Plot.Clear();
                        CPUPlot.Render();
                        MemoryPlot.Render();
                    });
                });
            }
            catch (Exception ex)
            {
                await Task.Run(() =>
                {
                    MessageBox.Show(ex.Message);
                });
            }
        }
        /// <summary>
        /// Асинхронний метод оновлення графіків.
        /// </summary>
        public async Task UpdateGraphicsData()
        {
            List<DateTime> listTime = new List<DateTime>();
            List<Double> listCPU = new List<Double>();
            List<Double> listMemory = new List<Double>();

            if (SelectedProcess == null)
            {
                return;
            }

            try
            {
                string[] logs = File.ReadAllLines("logProcess.txt");

                if (logs.Length == 0)
                {
                    return;
                }

                foreach (var log in logs)
                {
                    //Отримуємо дані для графіків з логуючого файлу
                    if (log.Contains("[" + SelectedProcess.Name + ","))
                    {
                        ArrayList array = GetDataProcessWithLine(log);
                        listTime.Add(DateTime.ParseExact(array[0].ToString(), "HH:mm:ss", null));
                        listCPU.Add(double.Parse(array[1].ToString().Replace(',', '.'), CultureInfo.InvariantCulture));
                        listMemory.Add(double.Parse(array[2].ToString().Replace(',', '.'), CultureInfo.InvariantCulture));
                    }
                }
                //Створюємо масив даних
                double[] x = listTime.Select(x => x.ToOADate()).ToArray();
                double[] yCPU = listCPU.ToArray();
                double[] yMemory = listMemory.ToArray();

                //Оновлюємо графіки
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CPUPlot.Plot.Clear();
                    MemoryPlot.Plot.Clear();

                    CPUPlot.Plot.AddScatter(x, yCPU);
                    MemoryPlot.Plot.AddScatter(x, yMemory);

                    CPUPlot.Plot.XAxis.DateTimeFormat(true);
                    MemoryPlot.Plot.XAxis.DateTimeFormat(true);

                    CPUPlot.Render();
                    MemoryPlot.Render();
                });
            }
            catch( Exception ex )
            {
                await Task.Run(() =>
                {
                    MessageBox.Show(ex.Message);
                });
            }
        }
        /// <summary>
        /// Отримання даних про процес з логу.
        /// </summary>
        private ArrayList GetDataProcessWithLine(string log)
        {
            ArrayList outputData = new ArrayList();
            string pattern = @"\d{4}-\d{2}-\d{2} (\d{2}:\d{2}:\d{2}) \[([\w\s.]+), CPU Usage: (\d+(?:[.,]\d+)?) s\., Memory Usage: (\d+(?:[.,]\d+)?) MB\]";
            Match match = Regex.Match(log, pattern);

            if (match.Success)
            {
                string timeString = match.Groups[1].Value;
                DateTime time = DateTime.ParseExact(timeString, "HH:mm:ss", null);
                double cpuUsage = double.Parse(match.Groups[3].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
                double memoryUsage = double.Parse(match.Groups[4].Value.Replace(',', '.'), CultureInfo.InvariantCulture);

                outputData.Add(time.TimeOfDay);
                outputData.Add(cpuUsage);
                outputData.Add(memoryUsage);
            }

            return outputData;
        }
    }
}
