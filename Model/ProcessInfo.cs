using ScottPlot.Statistics;
using System;

namespace CourseProdject.Model
{
    /// <summary>
    /// Клас, який представляє інформацію про процес.
    /// </summary>
    public class ProcessInfo
    {
        public string Name { get; set; }
        public double CpuUsage { get; set; }
        public double MemoryUsage { get; set; }
        public double PidProcess { get; set; }
        public string Description { get; set; }


        /// <summary>
        /// Конструктор класу <see cref="ProcessInfo"/>.
        /// </summary>
        /// <param name="name">Ім'я процесу.</param>
        /// <param name="cpuUsage">Використання CPU у процентнах.</param>
        /// <param name="memoryUsage">Використання пам'яті у мегабайтах.</param>
        public ProcessInfo(string name, double pid, double cpuUsage, double memoryUsage, string descr)
        {
            Name = name;
            PidProcess = pid;
            CpuUsage = cpuUsage;
            MemoryUsage = memoryUsage;
            Description = descr;
        }

        /// <summary>
        /// Створює рядок із записом для логування.
        /// </summary>
        /// <returns>Рядок для логування.</returns>
        public string CreateLog()
        {
            return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{Name}, CPU Usage: {CpuUsage.ToString("F1")} s., Memory Usage: {MemoryUsage.ToString("F1")} MB]";
        }
    }
}
