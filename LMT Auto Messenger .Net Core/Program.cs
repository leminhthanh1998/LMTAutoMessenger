using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace LMT_Auto_Messenger_.Net_Core
{
    class Program
    {
        #region private member

        private static string email;
        private static string passWord;
        private static AuthFaceBook login = new AuthFaceBook();
        private static OptionMessage option;
        private static FacebookHttpRequest _facebookHttpRequest = new FacebookHttpRequest();
        private static List<string> listID;
        private static string Message;
        private static Vietnamese vn = new Vietnamese();
        private static Thread send = new Thread(Send);

        private static string fileCookie =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "usersCookie.dat");
        #endregion


        private static void Main(string[] args)
        {
            Console.Title = "LMT Auto Messenger";
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.Clear();
            Console.WindowHeight = 30;
            Console.WindowWidth = 90;
            //Hien thi thong tin
            InFo();
            LoginFB();
            Console.WriteLine("1. Gửi tin nhắn");
            Console.WriteLine("2. Gửi tin nhắn theo thời gian định trước");
            string command = Console.ReadLine();
            if (command == "1") SendInstantMessages();
            else if (command == "2")
            {
                Console.Write("Nhập thời gian muốn gửi tin nhắn (vd: 12:00 PM, 10:45 AM): ");
                string time = Console.ReadLine();
                SetTimeToSendMessage(time);
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Hien thi thong tin khi khoi chay chuong trinh
        /// </summary>1
        private static void InFo()
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("-------------LMT Auto Messenger v1.0--------------");
            Console.WriteLine("----------Copyright © 2018 Le Minh Thanh----------");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("/*");
            Console.WriteLine(" *Đây là công cụ hỗ trợ bạn dễ dàng gửi tin nhắn");
            Console.WriteLine(" *đến các id Facebook trong danh sách được nhập sẵn.");
            Console.WriteLine(" *Công cụ hoàn toàn không thu thập hay đánh cắp bất cứ");
            Console.WriteLine(" *thông tin gì về tài khoản Facebook của bạn cả!");
            Console.WriteLine(" *Trang chủ: http://lêminhthành.vn");
            Console.WriteLine(" */\n");
            Console.Write("");
        }

        /// <summary>
        /// Dang nhap vao tai khoan Facebook
        /// </summary>
        private static void LoginFB()
        {
            if (File.Exists(fileCookie))
            {
                Console.Write("Bạn đã lưu Cookie trước đây, bạn có muốn dùng Cookie này để đăng nhập hay không (y/n): ");
                string command = Console.ReadLine();
                if (command.ToLower() == "y")
                {
                    login.Cookies = login.ReadCookiesFromDisk(fileCookie);
                    login.IsLogin = true;
                    return;
                }
            }

            Console.WriteLine("Đăng nhập vài tài khoản Facebook của bạn!!!");
            stone:
            Console.Write("Nhập email: ");
            email = Console.ReadLine();
            Console.Write("Nhập mật khẩu: ");
            passWord = Console.ReadLine();

            login.AuthFaceBookNow(email, passWord);
            if (!login.IsLogin)
            {
                Console.WriteLine("Đăng nhập thất bại, hãy kiểm tra lại email và mật khẩu của bạn!");
                goto stone;
            }
            else
            {
                //Hoi xem co muon luu lai cookie khong
                Console.WriteLine("Đã đăng nhập thành công!");
                Console.Write("Bạn có muốn lưu lại Cookie để đăng nhập không (y/n): ");
                string command = Console.ReadLine();
                if (command.ToLower() == "y")
                {
                    if (login.WriteCookiesToDisk(fileCookie, login.Cookies))
                        Console.WriteLine("Đã lưu Cookie thành công!");
                    else Console.WriteLine("Đã có lỗi!");
                }
            }



        }

        /// <summary>
        /// Gui tin nhan den cac id trong danh sach cho truoc
        /// </summary>
        private static void SendInstantMessages()
        {
            stone2:
            Console.Write("Nhập đường dẫn đến file chứ danh sách ID (.txt): ");
            string path = Console.ReadLine();
            if (File.Exists(path))
            {
                listID = File.ReadAllLines(path).ToList();
            }
            else
            {
                Console.WriteLine("Không tìm thấy file!");
                goto stone2;
            }

            Console.Write("Nhập tin nhắn bạn muốn gửi: ");
            Message = vn.ReadLine();

            Console.Write("Bạn có muốn gửi tin nhắn đến các id trong danh sách (y/n): ");
            string command = Console.ReadLine();
            if (command.ToLower() == "y")
                send.Start();
        }

        /// <summary>
        /// Gui tin nhan theo thoi gian dinh truoc
        /// </summary>
        private static void SetTimeToSendMessage(string time)
        {
            stone2:
            Console.Write("Nhập đường dẫn đến file chứ danh sách ID (.txt): ");
            string path = Console.ReadLine();
            if (File.Exists(path))
            {
                listID = File.ReadAllLines(path).ToList();
            }
            else
            {
                Console.WriteLine("Không tìm thấy file!");
                goto stone2;
            }

            Console.Write("Nhập tin nhắn bạn muốn gửi: ");
            Message = vn.ReadLine();

            Thread timer = new Thread(() =>
            {
                while (true)
                {
                    if (DateTime.Now.ToString("t") == time)
                    {
                        send.Start();
                        break;
                    }
                    Thread.Sleep(1000);
                }
            });
            timer.Start();
            Console.WriteLine("Đang chờ, vui lòng đừng tắt phần mềm!!!");
        }

        /// <summary>
        /// Gui tin nhan den cac id trong danh sach
        /// </summary>
        private static void Send()
        {
            foreach (var s in listID)
            {
                option = new OptionMessage(s, login);
                _facebookHttpRequest.SendMessage(login, Message, option);
                if (_facebookHttpRequest.OutputMessage == "Message Sent")
                    Console.WriteLine("Đã gửi thành công đến " + s);
                else Console.WriteLine("Đã có lỗi xảy ra!");
                Thread.Sleep(300);
            }
            Console.WriteLine("Đã hoàn tất!");
        }
    }
}
