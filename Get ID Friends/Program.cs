using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Get_ID_Friends
{
    class Program
    {
        #region private member

        private static List<string>listID=new List<string>();
        #endregion


        static void Main(string[] args)
        {
            string[] doc = File.ReadAllLines("C:\\Users\\lemin\\Desktop\\linkFB.txt");

            foreach (var item in doc)
            {
                var match = Regex.Match(item, @"(?<=cid.c.)(.*)(?=&refid)").Groups[1].Value.Replace("%3A","-");
                string[] a = match.Split('-');
                listID.AddRange(a.ToList());
            }

            listID = xoaTrungLap(listID);

            File.WriteAllLines("C:\\Users\\lemin\\Desktop\\ListFriendID.txt",listID);
        }

        /// <summary>
        /// Xuất ra danh sách đã loại bỏ hết các link trùng lập
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static List<string> xoaTrungLap(List<string> ds)
        {
            List<string> list = new List<string>();
            foreach (string item in ds)
            {
                if (!Contains(list, item))
                {
                    list.Add(item);
                }
            }
            return list;
        }
        private static bool Contains(List<string> list, string comparedValue)
        {
            bool result;
            foreach (string current in list)
            {
                if (current == comparedValue)
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }
    }
}
