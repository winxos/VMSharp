﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            for (var i = 0; i < input.LineCount && i < 2000; i++)
            {
                x += i.ToString("00\n");
            }
            linnum.Text = x;
        }
        Dictionary<string, int> tags = new Dictionary<string, int>();
        Dictionary<string, int> keywords = new Dictionary<string, int>
        {
            {"READ",10 },{"WRITE",11},{"LOAD",20},{"STORE",21},
            {"ADD",30 },{"SUB",31 },{"MUL",32 },{"DIV",33 },
            {"JMP",40 },{"JMPN",41},{"JMPZ",42},
            {"HALT",43 }
        };
        string pretranslate(string s)
        {
            string str = "";
            string[] ls = s.Split('\n');
            for (int i = 0; i < ls.Length; i++) //scan the :label
            {
                if (ls[i].StartsWith(":")) //label
                {
                    string ts = ls[i].Trim();
                    string[] token = ts.Substring(1, ts.Length - 1).Split();
                    if (token[0] != "")
                    {
                        tags[token[0]] = i;
                        if (token.Length == 2) //:x 1, label with value
                        {
                            str += token[1] + "\n";
                        }
                        else
                        {
                            str += "0000\n";
                        }
                    }
                    continue;
                }
                else if (ls[i] == "") str += "0000\n"; //blank
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
            string sml = pretranslate(s);
            foreach (var key in keywords)
            {
                sml = Regex.Replace(sml, key.Key + "\\W", string.Format("{0:00}", key.Value)); //op
            }
            sml = Regex.Replace(sml, "( )+", ""); //blank
            sml = Regex.Replace(sml, @"\D43\D", "\n4300\n"); //halt
            return sml;
        }
        private void input_TextChanged(object sender, TextChangedEventArgs e)
        {
            update_linenum();
            int pos = input.SelectionStart;
            input.Text = input.Text.ToUpper();
            input.Select(pos, 0);
            if(mcode != null)
            {
                string s = translate(input.Text);
                mcode.Text = s;
            }
        }
        int read()
        {
            string str = Interaction.InputBox("请输入", "输入框", "0", -1, -1);
            return int.Parse(str);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            v.BindView(ref vmview);
            update_linenum();
            v.UpdateView();
            v.read = read;
        }
        VM v = new VM();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] cs = mcode.Text.Trim().Split('\n');
            var ccs = Array.ConvertAll(cs, int.Parse);
            v.loadCode(ccs);
            v.UpdateView();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            v.Step();
            v.UpdateView();
        }
    }
}