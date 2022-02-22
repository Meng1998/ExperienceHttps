using System;
using System.Collections.Generic;
using System.Text;

namespace ExperienceMiddleware.External
{
    class refoutUnderstand
    {

        public refoutUnderstand()
        {
            #region ref 可进可出，out 只出不进

           
            //正确 （out test）
            int a, b;
            //out使用前变量赋值
            outTest(out a, out b);
            Console.WriteLine("a={0};b={1}", a, b);
            //在使用out关键字时，不需要在此处初始化，初始化也不会影响到方法内部的值，所以你初始化没用
            int c = 11, d = 22;
            outTest(out c, out d);
            Console.WriteLine("c={0};d={1}", c, d);


            //正确（ref test）
            int o = 11, p = 22;
            refTest(ref o, ref p);
            Console.WriteLine("o={0};p={1}", o, p);
            #endregion

            #region ref局部变量和ref返回结果
            Simple mod = new Simple();//分配空mod间
            mod.Display();//执行display（）方法，看此时变量数

            ref int mod1 = ref mod.RefToVal();//建立局部变量，此时mod1和mod指向同一堆中空间

            mod1 = 10;//改变其中一个赋值
            mod.Display();//检查另一个赋值是否改变


            int[] data = new int[10];
           // Console.WriteLine($"Before change, element at 2 is: {data[2]}");
            ref int value = ref Simple.ElementAt(ref data, 2);
            // Change the ref value.
            value = 5;
            Console.WriteLine($"After change, element at 2 is: {data[2]}");



            #endregion


        }
        static void outTest(out int x, out int y)
        {
            //离函数前必须xy赋值否则报错
            //y = x;
            //上面行报错使用outxy都清空需要重新赋值即使调用函数前赋值行
            x = 1;
            y = 2;
        }

        static void refTest(ref int x, ref int y)
        {
            x = 1;
            y = x;
        }
    }
    class Simple
    {
        private int score = 5; //声明赋值
        public ref int RefToVal() //建立方法

        {
            return ref score; //返回局部变量
        }
        public void Display() //建立方法
        {
            Console.WriteLine($"Value inside class object:{score}");//查看变量赋值
        }

        public static ref T ElementAt<T>(ref T[] array, int position)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array)); //值不能为空
            }

            if (position < 0 || position >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position));//指定的参数超出了有效值的范围
            }

            return ref array[position];
        }
    }
}
