using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using StudentCard.Logics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudentCard
{
    /// <summary>
    /// FindID.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FindID : MetroWindow
    {
        public FindID()
        {
            InitializeComponent();
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Owner.Show();
        }



        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            Owner.Show();
        }

        private async void BtnFindPw_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtName.Text) || string.IsNullOrEmpty(TxtPhoneNum.Text) || string.IsNullOrEmpty(TxtBirthday.Text))
            {
                var mySettings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "확인",
                    AnimateShow = true,
                    AnimateHide = true
                };

                var result = await this.ShowMessageAsync("주의", "정보를 입력하세요",
                                                         MessageDialogStyle.Affirmative, mySettings);
            }
            else
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"SELECT studentID
                                    FROM studenttbl
                                   WHERE studentName = @studentName
                                     AND birthday = @birthday
                                     AND PhoneNum = @PhoneNum";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@studentName", TxtName.Text);
                    cmd.Parameters.AddWithValue("@birthday", TxtBirthday.Text);
                    cmd.Parameters.AddWithValue("@PhoneNum", TxtPhoneNum.Text);

                    var result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        var mySettings = new MetroDialogSettings
                        {
                            AffirmativeButtonText = "확인",
                            AnimateShow = true,
                            AnimateHide = true
                        };

                        var msgresult = await this.ShowMessageAsync("주의", "잘못된 정보 입력",
                                                                    MessageDialogStyle.Affirmative, mySettings);
                    }
                    else
                    {
                        Debug.WriteLine(result.ToString());
                        TxbFindId.Text = result.ToString();
                    }

                }
            }
        }

        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnFindPw_Click(sender, e);
            }
        }

        private void TxtBirthday_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnFindPw_Click(sender, e);
            }
        }

        private void TxtPhoneNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnFindPw_Click(sender, e);
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtName.Focus();
        }

        private void TxtName_KeyDown_1(object sender, KeyEventArgs e)
        {

        }
    }
}

