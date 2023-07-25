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
        enum KeyWords
        {
            READ = 10, WRITE,
            LOAD = 20, STORE,
            ADD = 30, SUB, MUL, DIV,MOD,
            JMP = 40, JMPN, JMPZ, HALT
        }
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
            view.Text = $"{' ',30}Virtal Machine Sharp\n";
            view.Text += $"{' ',60}winxos 2010-2023\n";
            view.Text += $"ACC:{acc,9}  PC:{pc,9} CNT:{count,9} STA:{state,9}\n";
            view.Text += $"MEM:\n{' ',4}";
            for (int i = 0; i < Columns; i++)
            {
                view.Inlines.Add(new Run($"{i,9}"));
            }
            view.Inlines.Add(new Run("\n"));
            for (int i = 0; i < MAX_MEM / Columns; i++)
            {
                view.Inlines.Add(new Run($"{i * Columns,3}: "));
                for (int j = 0; j < Columns; j++)
                {
                    if (i * 10 + j == pc)
                    {
                        view.Inlines.Add(new Run($"{mem[i * Columns + j]:00000000} ")
                        {
                            Foreground = Brushes.OrangeRed
                        });
                    }
                    else
                    {
                        view.Inlines.Add(new Run($"{mem[i * Columns + j]:00000000} "));
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
            KeyWords opcode =(KeyWords)( code / 1000000);
            int operand = code % 1000000;
            count++;
            if (operand>= mem.Length)
            {
                return RUN_STATE.OVERFLOW;
            }
            switch (opcode)
            {
                case KeyWords.READ:
                    if (read != null)
                    {
                        mem[operand] = read();
                    }
                    break;
                case KeyWords.WRITE:
                    if (write != null)
                    {
                        write(mem[operand]);
                    }
                    break;
                case KeyWords.LOAD: acc = mem[operand]; break;
                case KeyWords.STORE: mem[operand] = acc; break;
                case KeyWords.ADD: acc += mem[operand]; break;
                case KeyWords.SUB: acc -= mem[operand]; break;
                case KeyWords.MUL: acc *= mem[operand]; break;
                case KeyWords.DIV: acc /= mem[operand]; break;
                case KeyWords.MOD: acc %= mem[operand]; break;
                case KeyWords.JMP: pc = operand - 1; break;
                case KeyWords.JMPN: pc = acc < 0 ? operand - 1 : pc; break;
                case KeyWords.JMPZ: pc = acc == 0 ? operand - 1 : pc; break;
                case KeyWords.HALT: return RUN_STATE.FINISHED;
                default:break;
            }
            pc++;
            return RUN_STATE.OK;
        }
    }
}
