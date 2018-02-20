using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace LMT_Facebook_Messenger
{
    /// <summary>
    /// A helper class to login FB account
    /// </summary>
    public class AuthFaceBook
    {
        private bool islogin;
        public bool IsLogin
        {
            get => islogin;
            set => islogin = value;
        }
        private string username;
        public string UserName
        {
            get => username;
            set => username = value;
        }
        private string pass;
        public string Password
        {
            get => pass;
            set => pass = value;
        }
        private CookieContainer cookies;
        public CookieContainer Cookies
        {
            get => cookies;
            set => cookies = value;
        }
        private string useragent = "Mozilla/5.0 (BlackBerry; U; BlackBerry 9900; en) AppleWebKit/534.11+ (KHTML, like Gecko) Version/7.1.0.346 Mobile Safari/534.11+";//"Mozilla/5.0 (Windows NT x.y; Win64; x64; rv:10.0) Gecko/20100101 Firefox/10.0";
        
        /// <summary>
        /// Login FB account with email and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void AuthFaceBookNow(string username, string password)
        {
            UserName = username;
            Password = password;
            cookies = new CookieContainer();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.facebook.com/");
                request.CookieContainer = new CookieContainer();
                request.CookieContainer = cookies;
                request.UserAgent = useragent;
                request.KeepAlive = false;
                request.Timeout = 45000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                cookies.Add(response.Cookies);
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string html = sr.ReadToEnd();
                html = html.Substring(html.IndexOf("login_form"));
                html = html.Remove(html.IndexOf("/form"));
                Regex reg = new Regex(@"name=""[^""]+"" value=""[^""]+""");
                MatchCollection mc = reg.Matches(html);
                List<string> values = new List<string>();
                for (int k = 0; k < mc.Count; k++)
                {
                    if (k != 0)
                        postData += "&";
                    if (k == 1)
                    {
                        postData += "email=" + this.username + "&pass=" + this.pass + "&";
                    }
                    string m = mc[k].Value.Replace("\"", "");
                    m = m.Replace("name=", "");
                    m = m.Replace(" value=", "=");
                    postData += m;
                }


                string getUrl = "https://www.facebook.com/login.php?login_attempt=1";
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(getUrl);
                getRequest.CookieContainer = new CookieContainer();
                getRequest.CookieContainer = cookies; //recover cookies first request
                getRequest.Method = WebRequestMethods.Http.Post;
                getRequest.UserAgent = useragent;
                getRequest.AllowWriteStreamBuffering = true;
                getRequest.ProtocolVersion = HttpVersion.Version11;
                getRequest.AllowAutoRedirect = false;
                getRequest.ContentType = "application/x-www-form-urlencoded";
                getRequest.Referer = "https://www.facebook.com";
                getRequest.KeepAlive = false;
                getRequest.Timeout = 45000;

                byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                getRequest.ContentLength = byteArray.Length;
                Stream newStream = getRequest.GetRequestStream(); //open connection
                newStream.Write(byteArray, 0, byteArray.Length); // Send the data.
                newStream.Close();

                HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse();
                cookies.Add(getResponse.Cookies);
                if (getResponse.Cookies.Count > 6)
                    IsLogin = true;
                else
                    IsLogin = false;

            }
            catch { }

        }
        public string postData { get; set; }
        
        public string UserAgent
        {
            get => useragent;
            set => useragent = value;
        }

        /// <summary>
        /// Read cookies from a file on disk
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public CookieContainer ReadCookiesFromDisk(string file)
        {

            try
            {
                using (Stream stream = File.Open(file, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Write cookie to a file .dat
        /// </summary>
        /// <param name="file">Path to file</param>
        /// <param name="cookieJar">Cookie</param>
        /// <returns></returns>
        public bool WriteCookiesToDisk(string file, CookieContainer cookieJar)
        {
            using (Stream stream = File.Create(file))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookieJar);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

    }

    /// <summary>
    /// A helper class to send message
    /// </summary>
    public class FacebookHttpRequest
    {
        private string output_message;
        public string OutputMessage
        {
            get => output_message;
            set => output_message = value;
        }
        public string SendMessage(AuthFaceBook client, string TextMessage, OptionMessage option)
        {
            if (client.IsLogin)
            {
                string Data = string.Format("fb_dtsg={0}&body={1}&send=Send&wwwupp=V3&ids%5B{2}%5D={3}&referrer=&ctype=&cver=legacy", option.DTSG, Uri.EscapeDataString(TextMessage), option.UserId, option.UserId);
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create("https://m.facebook.com/messages/send/?icm=1");
                getRequest.CookieContainer = new CookieContainer();
                getRequest.CookieContainer = client.Cookies; //recover cookies First request
                getRequest.Method = WebRequestMethods.Http.Post;
                getRequest.UserAgent = client.UserAgent;
                getRequest.AllowWriteStreamBuffering = true;
                getRequest.ProtocolVersion = HttpVersion.Version11;
                getRequest.AllowAutoRedirect = false;
                getRequest.ContentType = "application/x-www-form-urlencoded";
                getRequest.Referer = "https://m.facebook.com";
                getRequest.KeepAlive = false;
                getRequest.Timeout = 45000;
                byte[] byteArray1 = Encoding.ASCII.GetBytes(Data);
                getRequest.ContentLength = byteArray1.Length;
                Stream newStream2 = getRequest.GetRequestStream(); //open connection
                newStream2.Write(byteArray1, 0, byteArray1.Length); // send data
                newStream2.Close();
                output_message = "Message Sent";
                return output_message;
            }
            else
            {
                output_message = "Facebook Auth Error";
                return output_message;
            }
        }
    }

    /// <summary>
    /// Option for messaage
    /// </summary>
    public class OptionMessage
    {
        private string tids;
        public string error = "";
        public string TIDS
        {
            get => tids;
            set => tids = value;
        }
        private string dtsg;
        public string DTSG
        {
            get => dtsg;
            set => dtsg = value;
        }
        private string userId;
        public string UserId
        {
            get => userId;
            set => userId = value;
        }
        private string linkReq = "";
        public OptionMessage(string uid, AuthFaceBook client)
        {
            try
            {
                userId = uid;
                linkReq = "https://m.facebook.com/messages/thread/" + userId;

                
                HttpWebRequest Arequest = (HttpWebRequest)WebRequest.Create(linkReq);
                Arequest.CookieContainer = new CookieContainer();
                Arequest.CookieContainer = client.Cookies;
                Arequest.UserAgent = client.UserAgent;
                Arequest.KeepAlive = false;
                Arequest.Timeout = 45000;
                Arequest.Proxy = null;
                
                using (HttpWebResponse Aresponse = (HttpWebResponse) Arequest.GetResponse())
                {
                    StreamReader Asr = new StreamReader(Aresponse.GetResponseStream());
                    error += "Step 1 ";
                    string Ahtml = Asr.ReadToEnd();
                    Asr.Close();

                    var splitted_fb_dtsg = Ahtml.Split(new string[] { "type=\"hidden\" name=\"fb_dtsg" }, StringSplitOptions.RemoveEmptyEntries);
                    string fb_dtsg_container = splitted_fb_dtsg[1];
                    var splitted_tids = Ahtml.Split(new string[] { "tids" }, StringSplitOptions.RemoveEmptyEntries);
                    string tids_container = "";
                    if (splitted_tids.Length > 2)
                    {
                        tids_container = splitted_tids[1];
                    }
                    else
                    {
                        tids_container = splitted_tids[0];
                    }
                    error += "Step 2 ";

                    var splitted1_fb_dtsg = fb_dtsg_container.Split('"');
                    string fb_dtsg_rubsh = splitted1_fb_dtsg[2];
                    for (int u = 0; u < fb_dtsg_rubsh.Length; u++)
                    {
                        if (fb_dtsg_rubsh[u] != '"')
                            dtsg += fb_dtsg_rubsh[u];
                    }
                    error += "Step 3 ";
                    var splitted1_tids = tids_container.Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

                    string tids_rubsh = splitted1_tids[1];
                    for (int u = 0; u < tids_rubsh.Length; u++)
                    {
                        if (tids_rubsh[u] != '"')
                            tids += tids_rubsh[u];
                    }
                }
                
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

        }
    }
}