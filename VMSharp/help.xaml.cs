using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VMSharp
{
    /// <summary>
    /// help.xaml 的交互逻辑
    /// </summary>
    public partial class help : Window
    {
        public help()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            help1.FontSize = 24;
            help1.Text = "Supported Instruction\n\n";
            help1.Text += "READ:10\n";
            help1.Text += "LOAD:20 STORE:21\n";
            help1.Text += "ADD:30 SUB:31 MUL:32 DIV:33\n";
            help1.Text += "JMP:40 JMPN:41 JMPZ:42 HALT:43\n";
        }
    }
}
