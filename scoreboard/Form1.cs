using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace scoreboard
{
    public partial class Form1 : Form
    {
        public static string code = "LD F6 34 R2|LD F2 45 R3|MULT F0 F2 F4|SUBD F8 F6 F2|DIVD F10 F0 F6|ADDD F6 F8 F2";
        string[] codesplit = code.Split('|');//首先将code字符串转换为字符串数组，后面单独进行的时候还要进行split
         //初始化指令状态的表格是6x4大小的
         //string[,] IS = new string[,] { { "","", "", "" }, { "", "", "", "" }, { "", "", "", "" }, { "", "", "", "" }, { "", "", "", "" }, { "", "", "", "" } };
         string[,] IS = new string[6, 4];
            //初始化功能部件状态的表格是5x9大小的
            //string[,] FS = new string[,] { {"","","","","","","","",""}, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }, { "", "", "", "", "", "", "", "", "" }};
            string[,] FS = new string[5, 9];
            
            //初始化寄存器结果状态表1x10
            //string[,] RS = new string[,] { { "", "", "", "", "" } };
            string[,] RS = new string[1, 6];
           
        public Form1()
        {
            InitializeComponent();//初始化开始的组件，可以在这里将所有的表格也初始化
            for(int i=0;i<5;i++)
            {
                FS[i, 0] = "No";
            }
            
            DisplayTable(IS, dataGridView1);
            DisplayTable(FS, dataGridView2);
            DisplayTable(RS, dataGridView3);          
            
        }
        //还是设置单步执行的
        private void button1_Click(object sender, EventArgs e)
        {
            //string[,] TABLE = new string[,] { { "123", "456", "789" }, { "456", "789", "123" } };
            ////下面就是在前端将表格显示出来的过程。
            //DataTable dt = new DataTable();//新建一个表
            //for (int i = 0; i < TABLE.GetLength(1); i++)
            //    dt.Columns.Add(i.ToString(), typeof(int));
            //for (int i = 0; i < TABLE.GetLength(0); i++)
            //{
            //    DataRow dr = dt.NewRow();
            //    for (int j = 0; j < TABLE.GetLength(1); j++)
            //        dr[j] = TABLE[i, j];
            //    dt.Rows.Add(dr);//然后添加行
            //}
            //dataGridView1.DataSource = dt;//然后显示出来。

            //然后将代码分解成指令然后新建指令的类
            List<Instruction> Instructionlist = new List<Instruction>();
            foreach (string s in codesplit)
            {
                Instruction ins = new Instruction();
                string[] ss = s.Split(' ');
                ins.op = ss[0];
                ins.dest = ss[1];
                ins.source1 = ss[2];
                ins.source2 = ss[3];
                ins.state = 0;
                ins.stage = new int[4];
                Instructionlist.Add(ins);
                if (ss[0] == "MULT" || ss[0] == "DIVD")
                {
                    ins.clock = 3;
                }
                else if(ss[0]=="SUBD"||ss[0]=="ADDD")
                {
                    ins.clock = 2;
                }
                else
                    ins.clock = 1;
                
            }
            
            

            //Instructionlist[0].state = 1;//第一条指令开始执行
            //Instructionlist[0].stage[0] = cycle;//第一条指令发射
            int cycle = 1;
            int lastnow=0;//当前执行的指令最后一个
            while (true)//开始循环
            {//这里面进行clycle加1
                //第一条指令
                //首先找到当前正在执行的指令的后面一条指令
                int flag = 0;
                for(int i=0;i<6;i++)
                {
                    
                    if(Instructionlist[i].state==1&&Instructionlist[i+1].state==0)
                    {
                        lastnow = i;
                    }
                    
                    if (Instructionlist[0].state == 0)
                    {
                        lastnow = 0;
                    }
                    if(Instructionlist[5].state==1)
                    {
                        lastnow = 5;
                    }
                    if(Instructionlist[i].state==2)
                    {
                        flag++;
                    }
                    
                }
                Console.WriteLine(lastnow.ToString());
                if(flag==6)
                {
                    break;
                }

                //然后需要判断当前所有的正在执行的指令  和  当前指令的下一个指令，因为每个循环需要判断下一条指令是否需要执行。
                lastnow++;
                if (lastnow+1>5)
                {
                    lastnow = 5;
                }
                //记录每个州及执行中需要复原
                ArrayList recordreset = new ArrayList();
                int recordissue = 0;//用来记录每次只能发射一个。
                //判断所有的正在执行的指令在目前的状态下，能不能往下执行一下。
                for(int i=0;i<=lastnow;i++)//现在这个是需要考虑的指令的循环，最后一个是即将需要执行的指令，看能否执行
                {
                    //然后需要判断每条指令执行到的阶段，然后去执行下一个阶段。
                    if (Instructionlist[i].state == 2)//如果此指令执行完毕，直接跳过这个循环，到下一条指令
                        continue;
                    for(int j=0;j<4;j++)//看每条指令执行到了什么状态
                    {
                        if(Instructionlist[i].stage[j]==0)//首先看这条指令执行到了什么阶段
                        {
                            //进行判断如果是第一个状态的话需要判断所有的操作指令能否发射
                            if(j==0)//那么就是这条指令的发射看看能不能执行，主要考虑结构相关 issue
                            {
                                if(readFS(Instructionlist[i].op))//直接判断一下是否是busy,不是busy就发射
                                {
                                    //然后获取FS表中的位置然后写入对应的指令
                                    int x = FunNum(Instructionlist[i].op);
                                    FS[x, 0] = "Busy";
                                    FS[x, 1] = Instructionlist[i].op;
                                    FS[x, 2] = Instructionlist[i].dest;
                                    FS[x, 3] = Instructionlist[i].source1;
                                    FS[x, 4] = Instructionlist[i].source2;
                                    //然后读取对应的源操作数是否准备好，如果没有准备好写出对应的占用部件，可以读取register表,然后将源操作数的状态写入
                                    ReadRS(Instructionlist[i].source1, Instructionlist[i].source2, x);
                                    //指令进入之后，需要将目的寄存器的占用写在RS中。
                                    WriteRS(Instructionlist[i].dest, Instructionlist[i].op);
                                    //指令执行完毕之后需要将指令的状态写入
                                    Instructionlist[i].stage[j] = cycle;
                                    //然后需要在IS的表上显示出来
                                    IS[i, 0] = cycle.ToString();
                                    recordissue++;
                                    break;//说明这一条指令进来了，然后这条指令就算结束了。
                                }
                                else
                                {
                                    break;
                                }
                                
                            }
                            if(j==1)//read operand需要判断当前的两个是否是ready，如果是ready的话继续执行，如果不是ready就不执行
                            {
                                //读之前需要再重新更新一下这个状态，看看能否读。
                                ReadRS(Instructionlist[i].source1, Instructionlist[i].source2, FunNum(Instructionlist[i].op));
                                //首先获取指令对应的FS当中的位置
                                int index = FunNum(Instructionlist[i].op);
                                //然后判断FS中对应的源操作数是否都ready,
                                if(FS[index,7]=="Yes"&&FS[index,8]=="Yes")
                                {
                                    //都是ready的话就执行，执行的话就是读取操作数，然后在指令的状态上发生变化
                                    Instructionlist[i].stage[j] = cycle;
                                    //然后在IS上显示一下
                                    IS[i, 1] = cycle.ToString();
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if(j==2)//executic 对于不同的指令来说执行的时间会不同，所以在这里对不同指令的时间来进行区分。
                            {
                                //需要对每条指令进行匹配然后每条指令上需要记录执行的周期数。这个暂时不考虑
                                //就是直接执行。
                                Instructionlist[i].clock--;
                                if(Instructionlist[i].clock==0)//只有执行完毕之后才会到写入的过程
                                {
                                    Instructionlist[i].stage[j] = cycle;
                                    IS[i, 2] = cycle.ToString();
                                }
                                
                                break;

                            }
                            if(j==3)//write 写操作，需要改变指令的状态同时需要clear FS表上的内容。
                            {
                                Instructionlist[i].stage[j] = cycle;
                                Instructionlist[i].state = 2;
                                IS[i, 3] = cycle.ToString();

                                //此处的复原不能在这个周期还没有执行完的时候进行复原，要在这个循环结束之后复原。
                                //设置一个数组来记录状态为2的进行复原。而且只能复原一次。
                                //只要记录一下指令的编号即可进行复原。所以设置一个数组来记录。

                                //int index= FunNum(Instructionlist[i].op);
                                //ClearFun(index);//将function unit复原
                                //ClearRS(Instructionlist[i].dest);
                                recordreset.Add(i);//将本次循环中写入的指令的序号，便于后面的复原。
                                break;
                            }

                        }
                    }

                    if(recordissue!=0)
                    {
                        break;
                    }

                    
                }
                //进行复原
                foreach(int x in recordreset)
                {
                    int index = FunNum(Instructionlist[x].op);
                    ClearFun(index);//将FS复原
                    ClearRS(Instructionlist[x].dest);//将RS复原
                }
                //需要对所有的表重新display一下
                DisplayTable(IS, dataGridView1);
                DisplayTable(FS, dataGridView2);
                DisplayTable(RS, dataGridView3);
                
                //然后需要延迟一小会儿
                System.Threading.Thread.Sleep(1000);
                MessageBox.Show(cycle.ToString(),"当前循环数：");
                cycle++;

                //foreach(Instruction ins in Instructionlist)
                //{

                //    if(readFS(ins.op))//判断是否发生结构相关,没有结构相关的时候执行
                //    {
                //        //此时需要更新FS表
                //        int x = FunNum(ins.op);
                //        FS[x, 0] = "Busy";
                //        FS[x, 1] = ins.op;
                //        FS[x, 2] = ins.dest;
                //        FS[x, 3] = ins.source1;
                //        FS[x, 4] = ins.source2;
                //        //然后需要判断此时的寄存器是否已经准备好，准备好的写yes。没有准备好写no。
                //        break;
                //    }
                //    else
                //    {
                //    }
                //}
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();
            dataGridView3.ClearSelection();
        }
        public void DisplayTable1(string[,] s,DataGridView dgv)
        {
            DataTable dt = new DataTable();
            for(int i=0;i<s.GetLength(1);i++)//1的是列
            {
                dt.Columns.Add(i.ToString(), typeof(int));//添加列数
            }
            
            for(int i=0;i<s.GetLength(0);i++)//0的是行
            {
                DataRow dr = dt.NewRow();//新建一行
                for(int j=0;j<s.GetLength(1);j++)
                {
                    if(s[i,j]==null)
                    {
                        dr[j] = DBNull.Value;
                    }
                    else
                    {
                        dr[j] = s[i, j];
                    }
                   
                }
                dt.Rows.Add(dr);
            }
            dgv.DataSource = dt;
           


        }

        public void DisplayTable(string [,] s,DataGridView dgv)
        {
            DataTable dt = new DataTable();
            for(int i=0;i<s.GetLength(1);i++)
            {
                DataColumn dc = new DataColumn();
                dc.DataType = typeof(string);
                dt.Columns.Add(dc);
            }
            for(int i=0;i<s.GetLength(0);i++)
            {
                DataRow dr = dt.NewRow();
                for(int j=0;j<s.GetLength(1);j++)
                {
                    dr[j] = s[i, j];
                }
                dt.Rows.Add(dr);
            }
            dgv.Columns.Clear();
            dgv.DataSource = dt;
        }
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        public  class Instruction
        {
            public string op
            {
                set;get;
            }
            public string dest
            {
                get;set;
            }
            public string source1
            {
                get;set;
            }
            public string source2
            {
                get;set;
            }
            public int state//0:not run 1:running 2:finished
            {
                get;set;
            }
            public int[] stage
            {
                get;set;
            }
            public int clock
            {
                get;set;
            }


        }
        //获取操作指令对应的那个
        public int FunNum(string s)
        {
            if (s == "LD")
                return 0;
            else if (s == "MULT")
                return 1;
            else if (s == "ADDD"||s=="SUBD")
                return 3;
            else 
                return 4;
           
            
        }
        //清空function表的某一行
        public void ClearFun(int x)
        {
            FS[x, 0] = "No";
            for(int i=1;i<FS.GetLength(1);i++)
            {
                FS[x, i] =null;
            }
        }
        //判断是否发生结构相关。
        public bool readFS(string s)
        {
            if (s == "LD" && FS[0, 0] == "No")
            {
                return true;

            }
            else if (s == "MULT" && FS[1, 0] == "No")
            {
                return true;
            }
            else if (s == "SUBD" && FS[3, 0] == "No")//减法器
            {
                return true;
            }
            else if (s == "DIVD" && FS[4, 0] == "No")
            {
                return true;
            }
            else if (s == "ADDD" && FS[3, 0] == "No")
            {
                return true;
            }
            else
                return false;
        }
        //读取源操作数的状态是否有空闲然后写入
        //读取指令中的两个源操作数的状态看看是否ready，然后写入FS中,还有对应的占用是什么
        public void ReadRS(string x1,string x2,int x)
        {
            string[] F = new string[] { "F0", "F2", "F4", "F6","F8","F10"};
            if(F.Contains(x1))//如果包含的话需要判断是否被占用
            {
                //然后看此时的RS中的寄存器是否正在被占用
                int index = Array.IndexOf(F, x1);
                if(RS[0,index]!=null)
                {
                    FS[x, 7] = "No";
                    //在那个位置协商RS中的东西
                    FS[x, 5] = RS[0, index];
                }
                else
                {
                    FS[x, 7] = "Yes";
                }

            }
            else
            {
                FS[x, 7] = "Yes";
            }
            if(F.Contains(x2))
            {
                int index1 = Array.IndexOf(F, x2);
                if(RS[0,index1]!=null)
                {
                    FS[x, 8] = "No";
                    FS[x, 6] = RS[0, index1];
                }
                else
                {
                    FS[x, 8] = "Yes";
                }
            }
            else
            {
                FS[x, 8] = "Yes";
            }
        }
        /// <summary>
        /// 指令发射之后要将目的寄存器占用，然后在RS中写入指令的操作符
        /// </summary>
        /// <param name="x">输入是指令的类型 和 目的寄存器的地址</param>
        public void WriteRS(string x,string y)
        {
            string[] F = new string[] { "F0", "F2", "F4", "F6","F8","F10"};
            int index = Array.IndexOf(F,x);//获取到位置，然后写入对应的状态
            RS[0, index] = y;

        }
        /// <summary>
        /// 指令执行完毕之后对RS的表中的目的寄存器中的内容清空
        /// </summary>
        /// <param name="x">指令的操作符</param>
        public void ClearRS(string x)
        {
            string[] F = new string[] { "F0", "F2", "F4", "F6", "F8", "F10" };
            int index = Array.IndexOf(F, x);
            RS[0, index] = null;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
