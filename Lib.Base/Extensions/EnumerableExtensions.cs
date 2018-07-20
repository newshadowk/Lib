using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Lib.Base
{
    public static class EnumerableExtensions
    {
        public static void Add<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = new List<TValue>();
            }

            dictionary[key].Add(value);
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<TValue>());
            }
        }

        public static List<TValue> ToValueList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary)
        {
            List<TValue> ret = new List<TValue>();
            foreach (List<TValue> value in dictionary.Values)
            {
                ret.AddRange(value);
            }
            return ret;
        }

        public static TValue GetValueIfNotExistRetNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue outValue;
            if (!dictionary.TryGetValue(key, out outValue))
                return default(TValue);
            return outValue;
        }

        public static int RemoveAll<T>(this ObservableCollection<T> collctions, Predicate<T> pre)
        {
            //Please check it because it is by Guifan~haha
            //find all the items
            ObservableCollection<T> removedItems = new ObservableCollection<T>();
            foreach (var c in collctions.Where(c => pre(c)))
            {
                removedItems.Add(c);
            }
            //delete
            foreach (var item in removedItems)
            {
                collctions.Remove(item);
            }
            return removedItems.Count;
        }

        public static void UpdateSourceWithoutRemove<T>(this IList<T> src, IEnumerable<T> newList, Func<T, T, bool> equalsAction, Action<T, T> modifyAction,
            Action<T> beforeAddNewAction = null, bool order = false) where T : class
        {
            foreach (T newOne in newList)
            {
                T find = src.FirstOrDefault(i => equalsAction(i, newOne));
                if (find != null)
                {
                    modifyAction(find, newOne);
                    continue;
                }
                //,
                if (beforeAddNewAction != null)
                {
                    beforeAddNewAction(newOne);
                }
                if (order)
                {
                    src.Add(newOne);
                }
                else
                {
                    src.Insert(0, newOne);
                }
            }
        }

        public static void GetDifferent<T>(this IEnumerable<T> src, IEnumerable<T> newList, Func<T, T, bool> equalsAction, out List<T> removedItems, out List<T> addedItems)
        {
            var tSrc = src as List<T> ?? src.ToList();
            var tNewList = newList as List<T> ?? newList.ToList();

            //get remove list
            removedItems = new List<T>();
            foreach (var srcItem in tSrc)
            {
                if (!tNewList.Exists(i => equalsAction(i, srcItem)))
                    removedItems.Add(srcItem);
            }

            //get add list
            addedItems = new List<T>();
            foreach (var newItem in tNewList)
            {
                if (!tSrc.Exists(i => equalsAction(i, newItem)))
                    addedItems.Add(newItem);
            }
        }

        public static void UpdateSource<T>(this IList<T> src, IEnumerable<T> newList, Func<T, T, bool> equalsAction, Action<T, T> modifyAction = null,
            Action<T> beforeAddNewAction = null,
            bool order = false) where T : class
        {
            List<T> removedItems;
            List<T> addedItems;
            List<T> updatedOldItems;
            List<T> updatedNewItems;
            UpdateSource(src, newList, equalsAction, out removedItems, out addedItems, out updatedOldItems, out updatedNewItems, modifyAction, beforeAddNewAction, order);
        }

        public static void UpdateSource<T>(this IList<T> src, IEnumerable<T> newList, Func<T, T, bool> equalsAction, out List<T> removedItems, out List<T> addedItems,
            out List<T> updatedOldItems, out List<T> updatedNewItems, Action<T, T> updateAction = null, Action<T> beforeAddNewAction = null, bool order = false) 
            where T : class
        {
            var tNewList = newList as IList<T> ?? newList.ToList();

            addedItems = new List<T>();
            updatedOldItems = new List<T>();
            updatedNewItems = new List<T>();
            foreach (T newOne in tNewList)
            {
                T found = src.FirstOrDefault(i => equalsAction(i, newOne));
                if (found != null)
                {
                    updatedOldItems.Add(found);
                    updatedNewItems.Add(newOne);
                    updateAction?.Invoke(found, newOne);
                    continue;
                }

                beforeAddNewAction?.Invoke(newOne);

                addedItems.Add(newOne);
                if (order)
                    src.Add(newOne);
                else
                    src.Insert(0, newOne);
            }

            removedItems = new List<T>();
            foreach (T old in src)
            {
                bool found = tNewList.Any(i => equalsAction(i, old));
                if (!found)
                    removedItems.Add(old);
            }

            foreach (T item in removedItems)
                src.Remove(item);
        }

        /// <summary>
        /// parse the ip from SRVI to xxx.xxx.xxx.xxx format.
        /// </summary>
        /// <param name="ipnum"></param>
        /// <returns></returns>
        public static string ExtUintToStringAsIp(this uint ipnum)
        {
            var ipstr = Convert.ToString(ipnum, 16);
            if (ipstr.Length < 8)
            {
                string zero = "";
                for (int i = 0; i < 8 - ipstr.Length; i++)
                    zero += "0";
                ipstr = zero + ipstr;
            }

            char[] a = {ipstr[0], ipstr[1]};
            char[] b = {ipstr[2], ipstr[3]};
            char[] c = {ipstr[4], ipstr[5]};
            char[] d = {ipstr[6], ipstr[7]};

            var astr = Convert.ToInt32(string.Concat(a), 16);
            var bstr = Convert.ToInt32(string.Concat(b), 16);
            var cstr = Convert.ToInt32(string.Concat(c), 16);
            var dstr = Convert.ToInt32(string.Concat(d), 16);

            return string.Format("{0}.{1}.{2}.{3}", astr, bstr, cstr, dstr);
        }

        public static byte[] ExtIpToByte(this string ip)
        {
            var ipStrs = ip.Split('.');
            byte[] ret = new byte[4];
            ret[0] = byte.Parse(ipStrs[0]);
            ret[1] = byte.Parse(ipStrs[1]);
            ret[2] = byte.Parse(ipStrs[2]);
            ret[3] = byte.Parse(ipStrs[3]);
            return ret;
        }

        public static string ExtIpToString(this byte[] ip)
        {
            return string.Format("{0}.{1}.{2}.{3}", ip[0], ip[1], ip[2], ip[3]);
        }

        /// <summary>
        /// select the keyword
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string ExtSplitKeywordsOfClip(this string keywords, ushort index)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return string.Empty;
            }
            var keywordarray = keywords.Split(' ');

            return keywordarray.Length - 1 < index
                ? string.Empty
                : keywordarray[index].Length > 12
                    ? keywordarray[index].Substring(0, 12)
                    : keywordarray[index];
        }

        public static int ToInt32(this string inpoint, int index, int length)
        {
            int temp;
            var intstr = inpoint.Substring(index, length);
            int.TryParse(intstr, out temp);
            return temp;
        }

        public static bool SequenceEqualAfterSort<T>(this List<T> src, List<T> list)
        {
            src.Sort();
            list.Sort();
            return src.SequenceEqual(list);
        }

        /// <summary>
        /// Page, pageIndex start with 1.
        /// </summary>
        public static List<T> Page<T>(this IList<T> list, int pageIndex, int pageSize)
        {
            var newlists = new List<T>();
            if (pageIndex < 1)
                return newlists;
            var startIndex = (pageIndex - 1)*pageSize;
            var endIndex = startIndex + pageSize;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (list.Count > i)
                    newlists.Add(list[i]);
            }
            return newlists;
        }

        public static V GetValueOrDefaut<K, V>(this ConcurrentDictionary<K, V> dic, K key)
        {
            V value;
            if (!dic.TryGetValue(key, out value))
                return default(V);
            return value;
        }

        /// <summary>
        /// int, 。
        /// </summary>
        /// <returns>，</returns>
        public static List<List<int>> ToContinuousGroup(this List<int> numbers)
        {
            List<List<int>> list = new List<List<int>>(); //
            int previousValue = -1; //，
            List<int> group = new List<int>(); //
            numbers = numbers.OrderBy(i => i).ToList();
            for (int i = 0; i < numbers.Count; i++)
            {
                if (previousValue != numbers[i] - 1) //，
                {
                    if (group.Count != 0)
                    {
                        list.Add(group); //
                    }
                    group = new List<int>();
                }

                //
                previousValue = numbers[i];
                group.Add(numbers[i]);

                //
                if (i == numbers.Count - 1)
                {
                    list.Add(group);
                }
            }
            return list;
        }

        public static List<T> ToList<T>(this IList list)
        {
            List<T> ret = new List<T>();
            foreach (var i in list)
                ret.Add((T) i);
            return ret;
        }

        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            List<T> sorted = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }

        public static void Sort<T>(this ObservableCollection<T> collection, IComparer<T> comparer)
        {
            List<T> sorted = collection.ToList();
            sorted.Sort(comparer);
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }

        public static List<List<T>> BatchList<T>(this IEnumerable<T> source, int size)
        {
            List<List<T>> ret = new List<List<T>>();

            var batch = Batch(source, size);
            foreach (var inList in batch)
            {
                List<T> outList = new List<T>();
                foreach (var item in inList)
                    outList.Add(item);
                ret.Add(outList);
            }
            return ret;
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            T[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new T[size];

                bucket[count++] = item;

                if (count != size)
                    continue;

                yield return bucket.Select(x => x);

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }

        public static List<T> CopyTo<T>(this IList<T> list, int startIndex, int length)
        {
            if (startIndex + length > list.Count)
                throw new ArgumentOutOfRangeException();

            List<T> ret = new List<T>();
            for (int i = startIndex; i < length; i++)
                ret.Add(list[i]);

            return ret;
        }

        public static List<T> Top<T>(this IList<T> list, int length)
        {
            if (list == null)
                return null;

            List<T> ret = new List<T>();
            int count = 0;
            foreach (var i in list)
            {
                count ++;
                ret.Add(i);
                if (count == length)
                    break;
            }

            return ret;
        }

        public static StringDictionary ToStringDictionary(this IEnumerable<string> list)
        {
            StringDictionary dic = new StringDictionary();
            foreach (var item in list)
                dic.Add(item, item);
            return dic;
        }

        public static void Compare<T>(this IEnumerable<T> srcList, IEnumerable<T> newList, out List<T> addedList, out List<T> removedList, out List<T> updatedList)
        {
            addedList = new List<T>();
            removedList = new List<T>();
            updatedList = new List<T>();

            Dictionary<T, T> srcDic = new Dictionary<T, T>();
            foreach (var item in srcList)
                srcDic.Add(item, item);

            Dictionary<T, T> newDic = new Dictionary<T, T>();
            foreach (var item in newList)
                newDic.Add(item, item);

            foreach (T key in newDic.Keys)
            {
                if (srcDic.ContainsKey(key))
                    updatedList.Add(key);
                else
                    addedList.Add(key);
            }

            foreach (T key in srcDic.Keys)
            {
                if (!newDic.ContainsKey(key))
                    removedList.Add(key);
            }
        }

        public static List<string> ToLower(this IEnumerable<string> srcList)
        {
            List<string> ret = new List<string>();
            foreach (var s in srcList)
                ret.Add(s.ToLower());
            return ret;
        }

        public static string ListToString<T>(this IEnumerable<T> list, string split)
        {
            StringBuilder sb = new StringBuilder();
            var toList = list as IList<T> ?? list.ToList();
            foreach (var s in toList)
            {
                sb.Append(s);
                sb.Append(split);
            }

            return sb.ToString().TrimEndString(split);
        }

        public static string ListToStringForDisplay<T>(this IEnumerable<T> list, string split)
        {
            StringBuilder sb = new StringBuilder();

            var toList = list as IList<T> ?? list.ToList();
            sb.Append("[Count:" + toList.Count() + "]");
            sb.Append(split);

            foreach (var s in toList)
            {
                sb.Append(s);
                sb.Append(split);
            }

            return sb.ToString().TrimEndString(split);
        }

        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
                yield return (T) enumerator.Current;
        }

        public static IEnumerable ToIEnumerable(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        public static List<string> GetList(int count)
        {
            List<string> l = new List<string>();
            for (int i = 0; i < count; i++)
                l.Add("");
            return l;
        }
    }
}