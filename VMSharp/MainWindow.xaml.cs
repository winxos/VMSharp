using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Deployment.Application;

namespace VMSharp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void update_linenum()
        {
            var x = string.Empty;
            for (var i = 0; i < input.LineCount; i++)
            {
                x += i.ToString("00\n");
            }
            linnum.Text = x;
        }
        Dictionary<string, int> tags = new Dictionary<string, int>();
        public static Dictionary<string, int> keywords = new Dictionary<string, int>
        {
            {"READ",10 },{"WRITE",11},{"LOAD",20},{"STORE",21},
            {"ADD",30 },{"SUB",31 },{"MUL",32 },{"DIV",33 },{"MOD",34},
            {"JMP",40 },{"JMPN",41},{"JMPZ",42},
            {"HALT",43 }
        };
        int delay = 0;
        string pretranslate(string s)
        {
            string str = "";
            string[] ls = s.Split('\n');
            for (int i = 0; i < ls.Length; i++) //scan the :label
            {
                if (ls[i] == "") str += "00000000\n"; //blank
                else str += ls[i] + "\n"; //normal
            }
            foreach (KeyValuePair<string, int> key in tags) //replace label
            {
                str = Regex.Replace(str, " " + key.Key + "\\W", string.Format(" {0:00}", key.Value));
            }
            return str;
        }
        string translate(string s) //to machine language
        {
            string sml = string.Empty;
            string[] ss = s.Split('\n');
            foreach(var it in ss)
            {
                var c = it.Split(' ');
                if (c.Length == 2)
                {
                    if (keywords.ContainsKey(c[0]))
                    {
                        int t = 0;
                        int.TryParse(c[1], out t);
                        sml += keywords[c[0]] + t.ToString("000000\n");
                    }
                }
                else if(c.Length == 1)
                {
                    if (c[0] == "HALT")
                    {
                        sml += "43000000\n";
                    }
                    else
                    {
                        int t;
                        int.TryParse(c[0], out t);
                        sml += t.ToString("00000000\n");
                    }
                } 
            }
            return sml;
        }
        void auto_translate()
        {
            update_linenum();
            int pos = input.SelectionStart;
            input.Text = input.Text.ToUpper();
            input.Select(pos, 0);
            if (mcode != null)
            {
                mcode.Text = translate(input.Text);
            }
        }
        private void input_TextChanged(object sender, TextChangedEventArgs e)
        {
            auto_translate();
        }
        int read()
        {
            string str = Interaction.InputBox("请输入", "输入框", "0", -1, -1);
            return int.Parse(str);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = Assembly.GetExecutingAssembly().GetName().Name;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                this.Title += " v"+ApplicationDeployment.CurrentDeployment.
                    CurrentVersion.ToString();
            }
            vm.BindView(ref vmview);
            update_linenum();
            vm.read = read;
            input.Text = "READ 20\nLOAD 21\nADD 12\nSTORE 21\n" +
                "LOAD 22\nADD 21\nSTORE 22\nLOAD 21\nSUB 20\n" +
                "JMPZ 11\nJMP 01\nHALT\n0001\n";
            auto_translate();
            Thread t = new Thread(auto_update_view);
            t.IsBackground = true;
            t.Start();
        }
        VM vm = new VM();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] cs = mcode.Text.Trim().Split('\n');
            var ccs = Array.ConvertAll(cs, int.Parse);
            vm.loadCode(ccs);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            vm.Step();
        }
        void runvm()
        {
            while (true)
            {
                vm.Step();
                if (vm.state != VM.RUN_STATE.OK)
                {
                    break;
                }
                Thread.Sleep(delay);
            }
        }
        void auto_update_view()
        {
            while (true)
            {
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        vm.UpdateView();
                    }));
                    Thread.Sleep(500);
                }
                catch (Exception)
                {

                }
                
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(runvm);
            t.IsBackground = true;
            t.Start();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            spd.Content = (int)e.NewValue;
            delay = (int)e.NewValue;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            help h = new help();
            h.ShowDialog();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
