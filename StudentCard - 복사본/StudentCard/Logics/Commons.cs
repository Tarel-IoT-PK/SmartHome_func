using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudentCard.Logics
{
    public class Commons
    {
        // MySQL용
        public static readonly string myConnString = "Server=210.119.12.57;" +
                                                     "Port=3306;" +
                                                     "Database=campusdb;" +
                                                     "Uid=root;" +
                                                     "Pwd=12345;";

        // 메트로 다이얼로그창을 위한 메서드
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }

        public static int STUDENTID { get; set; }
        public static string NAME { get; set; }
        public string Brithday { get; set; }
        public static string Major { get; set; }
        public string PhoneNum { get; set; }
        public string Address { get; set; }
        public string gender { get; set; }

        public string Password { get; set; }

    }
}
