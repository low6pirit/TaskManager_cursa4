   M S B A M L   `   `   `  ����F  BTaskManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:CourseProdjectCourseProdject  5   	       CourseProdject.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QScottPlot.WPF, Version=4.1.69.0, Culture=neutral, PublicKeyToken=e53b06131e34a3aae aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:CourseProdject     5   	   _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$  Task Manager��5	   	   $	��700��6   $	��900��6+    � Icon$ /Content/logo.png��67   .��5
      � -   5         �Name$ Main_Window��5
      $��5,5,5,5q�6     � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ dataGrid��5         
IsReadOnly$
 True��6%   $��0��6h      AutoGenerateColumns$ False��6x   $��2��6�   $��
0,31,0,255q�6�   ��67   �� *ListProcess+ �� Mode$ TwoWay=� �� SelectedItem 6�   �� *SelectedProcess+$ TwoWay=�	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Name process��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 1* 6T   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6;   �� *Name+6T     5      $	
 PID��6%   $
 0.5* 6Q    62   �� *
PidProcess+6Q     5      $	
 CPU��6%   $ 1* 6O    62   �� *
CpuUsage+6O     5      $
 Memory��6%   $ 1* 6U    65   �� *MemoryUsage+6U     5      $
 Description��6%   $ 2* 6Z    6:   �� *Description+6Z   5      �� 5   
   -   5         ��Name$ Graph��5      $��Bottom=�6$   $
��Left=�6?   $	��700��6Z   $��2��6f    �� Orientation$ 
Horizontal=�6z   ���#031019.+�5        ScottPlot.WpfPlot  -   5          Name$ CPUPlot��5      $	��250��6'   $	��350��64   $
��Left=�6@     5      -   5        $ 
MemoryPlot��5      $	��250��6*   $	��366��67   $��Right=�6C   ���#0310196f   5      �� 5   
   -   5        $ ButtomPlace��5      $��3��6+   $��Bottom=�6;   $��Right=�6V   $	��186��6r   $	��218��6~   .+�5       �� -   5        $��Finish task��5       $	��130��6+   $��26��67   $��0 0 0 30q�6c   ���#0310196u   �� 5!      -   5        $��Finish task tree��5!      $	��130��60   $��26��6<   ���#0310196l   5"      �� 5#   
   $
��Left=�6   $��10,10,0,647q�6/   $
��Wrap=�6D   $��
Noto Serif)�6x   $��14(�6�   ��6X   �� *TotalProcesses+6�   5$      5%      