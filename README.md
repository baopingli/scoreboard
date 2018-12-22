1.  实验目的：

>   模拟记分牌算法，实现指令的动态调度，加深对指令相关性的理解。

1.  记分牌指令动态调度算法原理：

>   记分牌是一个集中控制部件，其功能是控制数据寄存器与处理部件之间的数据传送。在记分牌中保存有与各个处理部件相联系的寄存器中的数据装载情况。当一个处理部件所要求的数据都已就绪，记分牌允许处理部件开始执行。当执行完成后，处理部件通知记分牌释放相关资源。所以在记分牌中记录了数据寄存器和多个处理部件状态的变化情况，通过它来检测和消除或减少数据相关性，加快程序的执行速度。

>   目的是再无资源竞争的前提下保持每一个时钟周期执行一条指令的速率。

>   思想方法：尽可能提早指令的执行。当一条指令暂停执行时，如果其他后继指令与暂停指令及已发射的指令无任何相关，则仍然可以发射，执行。

>   记分牌动态调度技术需要将ID译码段分为两个阶段：1是发射，2是读取操作数。发射阶段对指令进行译码，检查结构冒险（Function
>   unit中有没有器件正在使用）；读取操作数阶段检查数据冒险（读之前检查寄存器的值是否已经写回，或者是否会覆盖之前的值）。读写冒险（RAW）的解决方法：将指令和操作数保存起来，然后只能在读操作数阶段进行读取。写写冒险（WAW）：检测是否有其它指令会写回到相同的寄存器，有则等待，直到其它的完成。

>   1.发射阶段：

>   假如检测到结构冒险和数据冒险，那么记分板将会将指令发射到相关的运算器，假如结构冒险或者写写冒险发生了，那么该指令将会等待，直到冒险消失为止。

>   2.读取操作数：

>   没有数据相关之后，读取操作数。读取操作数后将交给运算器，之后开始运算。发送到运算器的顺序可能是乱序的。

>   3.执行阶段：

>   不同的指令的执行周期不同。使用运算器部件对指令进行执行。

>   4.写回阶段：

>   将执行结果写回到目标寄存器当中，然后通知记分板将目标寄存器、运算部件解除占用。

1.  实验内容：

>   本报告在Visual Studio
>   2017上使用C\#语言编写Winform程序实现模拟记分牌算法的应用程序并且具有可视化界面。选择C\#语言是因为C\#同java、C++等都是面向对象语言对于编写应用程序来说更加简便，有成熟的数组库可以使用，C\#可以编写在Windows上运行的桌面应用程序，可以对动态调度算法进行较为友好的可视化的展示。

1.  程序功能及执行界面说明：![1](图片/1.png)

>   图1 程序初始界面

>   本程序是使用C\#开发的Winform程序，可以直接在Windows系统上直接运行，程序主要对三个状态表进行显示：Instruction
>   status指令状态表、Functional Unit status功能部件状态表和Register result
>   status寄存器结果状态表。

>   1.点击程序左上角的start按钮，开始执行动态调度程序。![2](图片/2.png)

>   图2 程序点击start之后界面

>   从图中可以看到程序执行第一个周期，将指令LD F6 34
>   R2发射，在功能部件状态表中将Interger功能部件置为Busy，然后将对应的操作指令、目的寄存器、源寄存器1、源寄存器2填到相应的位置，同时在Register
>   result
>   status表中对应指令的目的寄存器的位置F6处写入本条指令的操作符LD。第一个周期执行完毕之后，程序会弹出一个对话窗口显示执行完毕的cycle数。然后我们点击对话框中的确定按钮，程序开始执行下一个周期。

>   2.点击对话窗口中的确定按钮，程序继续执行。![3](图片/3.png)

>   图3 程序cycle=2的界面

>   首先第二个周期包括两个部分：第一条指令的读操作数阶段和第二条指令是否能发射的问题。首先是第一条指令的读操作数阶段，在此过程需要对Register
>   result status寄存器状态表进行检测，检测LD F6 34
>   R2中的源寄存器是否正在被占用，此条指令中原寄存器没有占用，所以可以进行第一条指令的读操作数阶段。然后是第二条指令是否可以发射，由于第二条指令的操作符是LD需要Functional
>   Unit
>   status的Interger部件，但是目前读取功能部件表发现Interger部件状态为Busy，所以不能发射。到此第二个循环周期结束。

>   3.在程序界面的上直接显示了乘和除占用3个周期，加和减占用2个周期，从下图中可以看出程序执行到第11个周期的时候SUBD
>   F8 F6
>   F2指令完成了指令的执行过程，从指令状态表中可以看出该指令的上一阶段Read对应的周期是9，所以计算可以得出减法指令的执行周期是2，观察该指令上一条指令MULT
>   F0 F2
>   F4在11次周期时还没有执行完毕是因为乘法运算需要3个周期进行，所以第三条指令的Exec完成周期是12。所以说该程序实现了不同指令执行不同的周期。![4](图片/4.png)

>   图4 程序cycle=11的界面

>   4.程序执行完毕之后功能部件表和寄存器表都会清空，点击程序的左上角处的Close按钮可以将程序关闭。

![5](图片/5.png)

>   图5 程序执行完毕时的界面

1.  数据结构及其说明：

>   程序可视化界面只是将状态表中状态信息和指令的执行步骤和顺序显示了出来但是我认为更为重要的是程序内部的实现，这一部分主要介绍程序的详细实现过程。图6展示了整个程序的执行流程。

![6](图片/6.png)

图6 程序的技术路线图

>   1.可视化界面设计

>   在编写记分牌调度算法之前我先对程序的可视化界面进行了设计，实现将程序中字符串数字显示在Winform程序界面中的DataGridView中，其中主要是根据字符串长度大小创建DataTable的行列，同时必须将DataTable中值的类型设置为String否则出现第一列无法写入字符串的错误。最后形成了DisplayTable（string[,]
>   s,DataGridView dgv）方法。

>   2.创建三个表

>   创建三个字符串二维数组用来存放Function unit status、Instructions
>   status、Register result
>   status依次是FS：6x4、IS：5x9、RS：1x6大小的。本程序中对输入的指令集进行了固定，程序重点在于实现动态调度的算法。

3.创建需要执行指令的类Instruction

>   经过考虑Instruction类需要包括op指令的操作符（string类型）、dest指令的目的寄存器地址（string类型）、source1指令中源寄存器1（string类型）、source2指令中源寄存器2（string类型）、state指令的当前运行状态（0：未运行、1：正在运行、2：运行完毕）、stage指令当前的执行阶段（int数组类型，记录当前Cycle数）、clock该条指令的执行周期（int类型）。

4.程序的循环（核心部分）

>   首先通过观察可以发现，记分牌调度算法每一个周期的执行都会判断此时正在执行的指令的下一条指令能否发射，并且记分牌调度算法还有一个原则就是每次只能发射一条指令，然后是所有正在执行的指令能否进行此条指令的下一个阶段，然后我们再针对每一条指令在执行到Issue、Read、Exec、Write阶段所需要面对的冲突检测，这样我们就可以把整个动态调度算法总结在一起。

>   所以创建一个循环，然后在每个循环中我们需要做到：

1.  首先找到当前指令执行到了第几条，那么我们需要将这条指令的下一条指令包含进来进行考虑，当然也有特殊情况的判断如果此时执行到最后一条指令那么此时就考虑所有的指令。

2.  将（1）中考虑到的所有指令构建循环，首先我们需要判断此条指令是否执行完毕，因为程序执行完毕的顺序不一定和指令的顺序是一致的。排除掉执行完毕的指令之后我们需要对每一条指令的四个状态进行判断。

3.  如果指令执行到了Issue阶段，我们需要根据指令的op判断此时功能部件是否Busy，如果不Busy就是可以发射的，然后将指令的信息对应填写到Functional
    unit
    status的表中，然后读取指令中源寄存器的状态是否被占用，并将占用状态填写到Functional
    unit status表中。然后需要将指令的目的寄存器在Resiger result
    status对应位置写入指令的op，表示此寄存器被占用。然后将当前的cycle写入指令的stage中对应的位置，将指令的state置为1，然后IS表中写入当前cycle显示出来，同时将此次循环中的issue标志位置一，从而此次循环结束，因为算法每次只能发射一条指令。然后从指令的状态循环中跳出。然后去执行一条指令。

4.  如果指令执行到了Read阶段，首先需要更新一下Functional unit
    status的状态，获取一下当前最新的功能部件表的状态，然后去判断FS表中对应的两个源寄存器的状态是不是Yes，如果都是Yes那么执行Read阶段，然后将当前cycle写入指令中对应的stage的位置上，然后将cycle显示在指令状态表中，然后跳出。

5.  如果指令执行到了Exec阶段，主要是由于指令的执行周期可能不同，所以每次执行到了这个状态，将指令的clock的数值减1，然后每一次对clock的值进行判断，如果为0那么就是执行完毕然后将cycle写入指令中的stage中然后在IS表中显示出来，然后跳出。

6.  如果指令执行到了Write阶段，直接进行执行，将cycle写入对应指令中的stage中然后在IS表中显示出来，然后将指令的state置为2表示词条指令执行完毕，然后需要记录当前指令的序号到一个数组当中，在所有的指令都循环完毕之后对数组中对应的指令在Funtional
    unit status和Register reault status表中的记录进行清除。

7.  然后每次大循环执行完毕之后延时1s，将结果Display一下，然后cycle++。

>   以上就是整个记分牌算法中的核心部分，就是一个循环，去判断可能出现的所有条件。
