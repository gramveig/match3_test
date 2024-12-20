using System;
using System.Collections.Generic;

namespace Match3Test.Utility
{
    public class ArrayHelper
    {
        public static void ShuffleArray<T>(T[] arr)
        {
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int r = UnityEngine.Random.Range(0, i + 1);
                T tmp = arr[i];
                arr[i] = arr[r];
                arr[r] = tmp;
            }
        }

        public static void ShuffleList<T>(List<T> lst)
        {
            for (int i = lst.Count - 1; i > 0; i--)
            {
                int r = UnityEngine.Random.Range(0, i + 1);
                T tmp = lst[i];
                lst[i] = lst[r];
                lst[r] = tmp;
            }
        }

        public static void EnlargeList<T>(List<T> lst, int count)
        {
            for (int i = 0; i < count - lst.Count; i++) lst.Add(default(T));
        }

        public static string IntArrToStr(int[] array)
        {
            return "[" + string.Join(", ", Array.ConvertAll<int, String>(array, Convert.ToString)) + "]";
        }

        public static bool Contains(int[] arr, int value)
        {
            if (arr == null) return false;

            foreach (int v in arr)
                if (v == value)
                    return true;

            return false;
        }

        public static bool Contains(int[] arr, int value, int maxIndex)
        {
            if (arr == null || maxIndex < 0) return false;

            int mi;
            if (maxIndex < arr.Length - 1) mi = maxIndex;
            else mi = arr.Length - 1;

            for (int i = 0; i <= mi; i++)
                if (arr[i] == value)
                    return true;

            return false;
        }

        public static T[] Add<T>(T[] array, T item)
        {
            if (array == null) return new T[] {item};

            T[] returnarray = new T[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
                returnarray[i] = array[i];

            returnarray[array.Length] = item;

            return returnarray;
        }

        public static T[] Add<T>(T[] array, T item, int at)
        {
            if (array == null || array.Length == 0) return new T[] {item};

            if (at < 0 || at > array.Length) at = array.Length;

            T[] returnarray = new T[array.Length + 1];
            int c = 0;
            for (int i = 0; i < array.Length + 1; i++)
            {
                if (at == i)
                {
                    returnarray[i] = item;
                    c = 1;
                }
                else
                    returnarray[i] = array[i - c];
            }

            return returnarray;
        }

        public static T[] Delete<T>(T[] array, int at)
        {
            if (array == null || array.Length == 0) return array;

            if (at < 0 || at > array.Length) at = array.Length - 1;

            T[] returnarray = new T[array.Length - 1];
            int c = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (i == at)
                {
                    c = 1;
                    continue;
                }

                returnarray[i - c] = array[i];
            }

            return returnarray;
        }

        public static void MoveElementDown<T>(T[] array, int at)
        {
            if (array == null || array.Length == 0 || at == 0) return;

            if (at < 0 || at > array.Length) at = array.Length - 1;

            T e1 = array[at];
            T e2 = array[at - 1];

            array[at - 1] = e1;
            array[at] = e2;
        }

        public static void MoveElementUp<T>(T[] array, int at)
        {
            if (array == null || array.Length == 0 || at >= array.Length - 1) return;

            if (at < 0) at = 0;

            T e1 = array[at];
            T e2 = array[at + 1];

            array[at + 1] = e1;
            array[at] = e2;
        }

        public static void ShiftLeft<T>(T[] array)
        {
            if (array == null || array.Length == 0) return;

            for (int i = 0; i < array.Length; i++)
                if (i < array.Length - 1)
                    array[i] = array[i + 1];
                else
                    array[i] = default;
        }

        public static int[] GetRandomizedIndexes(int to)
        {
            int[] idxs = new int[to];
            for (int i = 0; i < to; i++) idxs[i] = i;
            ShuffleArray(idxs);

            return idxs;
        }

        public static int[] GetRandomizedIndexes(int to, int oldLastValue)
        {
            int[] idxs = new int[to];
            for (int i = 0; i < to; i++) idxs[i] = i;

            if (to <= 1)
            {
                ShuffleArray(idxs);
                return idxs;
            }

            while (true)
            {
                ShuffleArray(idxs);
                if (idxs[0] != oldLastValue) break;
            }

            return idxs;
        }
    }
}