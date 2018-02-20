using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LMT_Get_ID_Friend_FB
{
    class Program
    {
        private static List<string> ID = new List<string>();
        private static string link;
        private static string id;
        private static WebClient wc;
        private static string token;

        static void Main(string[] args)
        {
            Console.Write("Nhap token: ");
            token = Console.ReadLine();
            link = $"https://graph.facebook.com/v2.12/me/friends?access_token={token}";
            GetID();

        }

        static void GetID()
        {
            string doc;

            wc=new WebClient();
            while (true)
            {
                doc = wc.DownloadString(link);
                MatchCollection listID = Regex.Matches(doc, "(?i)\\s*\"id\":\\s*\\s*(\"([^\"]*\")|'[^']*'|([^'\">]+))");
                foreach (Match item in listID)
                {
                    ID.Add(new String(item.Value.Where(Char.IsDigit).ToArray()));
                }
               
                //var _link= Regex.Match(doc.Replace(@"\/", "/"), "(?i)\\s*https://\\s*\\s*(\"([^\"]*\")|'[^']*'|([^'\">]+))").Groups[0].Value;
                var after = Regex.Match(doc.Replace(@"\/", "/"), "(?i)\\s*\"after\":\\s*\\s*(\"([^\"]*\")|'[^']*'|([^'\">]+))").Groups[0].Value.Replace(@"""","").Replace("after:","");
                var _link = $"https://graph.facebook.com/v2.12/me/friends?access_token={token}&pretty=1&limit=25&after={after}";
                if (_link == null || _link == "")
                    break;
                link = _link;
                Thread.Sleep(200);
            }
        }
    }
}
