using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using StudentCard.Logics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace StudentCard
{
    /// <summary>
    /// FindPW.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FindPW : MetroWindow
    {
        public FindPW()
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
            if (string.IsNullOrEmpty(TxtName.Text) || string.IsNullOrEmpty(TxtStudentId.Text) || (CboMajor.SelectedValue == null))
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

                    var query = @"SELECT password
                                    FROM login_student
                                   WHERE studentId = @studentId
                                     AND studentName = @studentName
                                     AND major = @major";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@studentId", TxtStudentId.Text);
                    cmd.Parameters.AddWithValue("@studentName", TxtName.Text);
                    cmd.Parameters.AddWithValue("@major", CboMajor.SelectedValue.ToString());

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
                        TxbFindPW.Text = result.ToString();
                    }
                }
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtStudentId.Focus();
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                var query = @"SELECT name
                                FROM major
                               WHERE isAdmin <> '1'";
                var cmd = new MySqlCommand(query, conn);

                var result = cmd.ExecuteScalar();
                Debug.WriteLine(result.ToString());

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "major");
                List<string> majorName = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    majorName.Add(Convert.ToString(row["name"]));
                }
                CboMajor.ItemsSource = majorName;
            }

        }

        private void TxtStudentId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnFindPw_Click(sender, e);
            }
        }

        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnFindPw_Click(sender, e);
            }
        }

        private void CboMajor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnFindPw_Click(sender, e);
            }
        }
    }
}
