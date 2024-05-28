using CourseProdject.ViewModel;
using System.Threading.Tasks;
using System.Windows;

namespace CourseProdject
{
    public partial class MainWindow : Window
    {
        MainViewModel ViewModel;
        public MainWindow()
        {
            //Ініціалізуємо початкові дані
            ViewModel = new MainViewModel();
            InitializeComponent();
            this.DataContext = ViewModel;
            InitializeAsync();
        }
        /// <summary>
        /// Асинхронний метод ініціалізації вікна.
        /// </summary>
        private async void InitializeAsync()
        {
            ViewModel.CPUPlot = CPUPlot;
            ViewModel.MemoryPlot = MemoryPlot;

            CPUPlot.Plot.XLabel("Time");
            MemoryPlot.Plot.XLabel("Time");
            CPUPlot.Plot.YLabel("Clock cycles(S)");
            MemoryPlot.Plot.YLabel("Mb");
            CPUPlot.Plot.Title("CPU");
            MemoryPlot.Plot.Title("Memory");
            //Запускаємо асинхронно дві функції
            await Task.WhenAll(ViewModel.LoadData(), ViewModel.LoggingData());
        }
        /// <summary>
        /// Обробник події для кнопки завершення процесу.
        /// </summary>
        private async void KillProcessTree_ButtonClick(object sender, RoutedEventArgs e)
        {
            await ViewModel.TerminateProcess();
        }

        private async void KillProcess_ButtonClick(object sender, RoutedEventArgs e)
        {
            await ViewModel.TerminateSingleProcess();
        }
    }
}
