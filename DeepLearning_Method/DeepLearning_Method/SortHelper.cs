/*     SortHelper
*
* Version: 1.0
* Author:  Siyu Yu
* Email:   siyuyu @yangtzeu.edu.cn(or 573315294@qq.com)
* Date:    2018
* 
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace DeepLearning_Method
{
    public class SortHelper
    {
        /// <summary>
        /// 产生不重复的随机数字序列
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public static int[] RandomNotRepeatNum(int min, int max, Random rnd)
        {
            int count = max - min + 1;
            int[] array = new int[count];

            for (int i = 0; i < count; i++)
            {
                array[i] = min;
                min++;
            }

            //洗牌算法
            for (int i = 0; i < count; i++)
            {
                int temp = rnd.Next(count);
                int temp2 = 0;
                temp2 = array[temp];
                array[temp] = array[i];
                array[i] = temp2;
            }

            return array;
        }

        /// <summary>
        /// 随机排序
        /// </summary>
        /// <param name="oldList"></param>
        /// <returns></returns>
        public static List<T> RandomSort<T>(List<T> oldList, Random rnd)
        {
            //排序后的对象
            List<T> newList = new List<T>();

            int count = oldList.Count;
            int max = oldList.Count - 1, min = 0;
            //生成sortArray的随机索引
            int[] randomIndex = RandomNotRepeatNum(min, max, rnd);

            for (int i = 0; i < count; i++)
            {
                int newIndex = randomIndex[i];
                T newItem = oldList[newIndex];
                newList.Add(newItem);
            }
            return newList;
        }

        /// <summary>
        /// 随机排序
        /// </summary>
        /// <param name="oldList"></param>
        /// <returns></returns>
        public static Dictionary<T, V> RandomSort<T, V>(Dictionary<T, V> oldList, Random rnd)
        {
            Dictionary<T, V> result = new Dictionary<T, V>();
            List<int> index = new List<int>();//随机序号
            for (int i = 0; i < oldList.Count; i++) index.Add(i);
            index = RandomSort<int>(index, rnd);//随机排序
            List<T> keys = new List<T>();
            List<V> values = new List<V>();
            foreach (var item in oldList)
            {
                keys.Add(item.Key);
                values.Add(item.Value);
            }
            for (int i = 0; i < index.Count; i++)
            {
                int rnd_index = index[i];//随机序号
                result.Add(keys[rnd_index], values[rnd_index]);
            }
            return result;
        }

        /// <summary>
        /// 随机抽样
        /// </summary>
        public static List<T> RandomSelect<T>(List<T> list, int N, Random rnd)
        {
            if (list.Count < N) N = list.Count;
            List<T> randoms = RandomSort<T>(list, rnd);
            List<T> result = new List<T>();
            for (int i = 0; i < N; i++)
            {
                result.Add(randoms[i]);
            }
            return result;
        }

        /// <summary>
        /// 随机抽样
        /// </summary>
        public static Dictionary<T, V> RandomSelect<T, V>(Dictionary<T, V> list, int N, Random rnd)
        {
            if (list.Count < N) N = list.Count;
            Dictionary<T, V> result = new Dictionary<T, V>();
            Dictionary<T, V> randoms = RandomSort<T, V>(list, rnd);

            List<int> index = new List<int>();
            for (int i = 0; i < list.Count; i++) index.Add(i);
            index = RandomSelect<int>(index, N, rnd);

            int counter = 1;
            foreach (var item in randoms)
            {
                result.Add(item.Key, item.Value);

                if (counter == N) break;
                counter++;
            }

            return result;
        }

    }
}
