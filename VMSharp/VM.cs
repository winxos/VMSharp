using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace VMSharp
{
    internal class VM
    {
        Dictionary<int, string> keywords = new Dictionary<int, string>{
            {10,"READ"},{11,"WRITE"},
            {20,"LOAD"},{21,"STORE"},
            {30,"ADD"},{31,"SUB"},{32,"MUL"},{33,"DIV"},
            {40,"JMP"},{41,"JMPN"},{42,"JMPZ"},{43,"HALT"}};
        const int MAX_MEM = 100;
        const int Columns = 10;
        int[] mem = new int[MAX_MEM];
        int acc = 0;
        int pc = 0;
        int count = 0;
        public Func<int> read = null;
        public Action<int> write = null;
        private TextBlock view;
        public enum RUN_STATE
        {
            OK,
            FINISHED,
            OVERFLOW
        }
        public RUN_STATE state;
        public void loadCode(int[] cs)
        {
            Array.Clear(mem, 0, mem.Length);
            cs.CopyTo(mem, 0);
            acc = 0;
            pc = 0;
            count = 0;
            state = RUN_STATE.OK;
        }
        public void BindView(ref TextBlock tb)
        {
            view = tb;
        }
        public void UpdateView()
        {
            view.Text  = $"{' ',10}Virtal Machine Sharp\n";
            view.Text += $"{' ',30}winxos 2010-2023\n";
            view.Text += $"     ACC:{acc,5}  PC:{pc,5} CNT:{count,5} STA:{state,5}\n";
            view.Text += $"MEM:\n{' ',4}";
            for (int i = 0; i < Columns; i++)
            {
                view.Inlines.Add(new Run($"{i,5}"));
            }
            view.Inlines.Add(new Run("\n"));
            for (int i = 0; i < MAX_MEM / Columns; i++)
            {
                view.Inlines.Add(new Run($"{i * Columns,3}: "));
                for (int j = 0; j < Columns; j++)
                {
                    if (i * 10 + j == pc)
                    {
                        view.Inlines.Add(new Run($"{mem[i * Columns + j]:0000} ")
                        {
                            Foreground = Brushes.OrangeRed
                        });
                    }
                    else
                    {
                        view.Inlines.Add(new Run($"{mem[i * Columns + j]:0000} "));
                    }
                }
                view.Inlines.Add(new Run("\n"));
            }
        }
        public void Step()
        {
            state = _step();
        }
        private RUN_STATE _step()
        {
            if (state != RUN_STATE.OK)
            {
                return state;
            }
            if (pc >= mem.Length)
            {
                return RUN_STATE.OVERFLOW;
            }
            int code = mem[pc];
            int opcode = code / mem.Length;
            int operand = code % mem.Length;
            count++;
            if (keywords.ContainsKey(opcode))
            {
                switch (keywords[opcode])
                {
                    case "READ":
                        if (read != null)
                        {
                            mem[operand] = read(); 
                        }
                        break;
                    case "WRITE":
                        if (write != null)
                        {
                            write(mem[operand]);
                        }
                        break;
                    case "LOAD": acc = mem[operand]; break;
                    case "STORE": mem[operand] = acc; break;
                    case "ADD": acc += mem[operand]; break;
                    case "SUB": acc -= mem[operand]; break;
                    case "MUL": acc *= mem[operand]; break;
                    case "DIV": acc /= mem[operand]; break;
                    case "JMP": pc = operand - 1; break;
                    case "JMPN": pc = acc < 0 ? operand - 1 : pc; break;
                    case "JMPZ": pc = acc == 0 ? operand - 1 : pc; break;
                    case "HALT":  return RUN_STATE.FINISHED;
                }
            }
            pc++;
            return RUN_STATE.OK;
        }
    }
}
