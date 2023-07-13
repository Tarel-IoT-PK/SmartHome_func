using MahApps.Metro.Controls;
using StudentCard.Logics;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace StudentCard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtStudentId.Text)
              || string.IsNullOrEmpty(PwbPassword.Password.ToString()))
            {
                await Commons.ShowMessageAsync("주의", "학번과 비밀번호를 입력하세요");
                return;
            }

            try
            {
                Check_IdPw(TxtStudentId.Text, PwbPassword.Password.ToString());

            }
            catch (Exception ex)
            {

                await Commons.ShowMessageAsync("오류", $"오류발생 : {ex.Message}"); ;
            }

        }

        private async void Check_IdPw(string Id, string Pw)
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                //var studentId = TxtStudentId.Text;
                var studentId = TxtStudentId.Text;
                var password = PwbPassword.Password.ToString();
                var query = @"SELECT studentID
                                   , studentname
                                   , major
                                   , password
                                FROM login_student
                               WHERE studentID = @STUDENTID";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@STUDENTID", TxtStudentId.Text);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Debug.WriteLine(Convert.ToInt32(reader["studentId"]));
                        Debug.WriteLine(reader["password"].ToString());
                        Debug.WriteLine(reader["studentname"]);
                        Debug.WriteLine(reader["major"]);
                        if (studentId == ((Convert.ToInt32(reader["studentId"])).ToString()) && password == ((string)reader["password"]))
                        {
                            if (Convert.ToString(reader["major"]) == "관리자")
                            {
                                var manager = new Monitoring();
                                manager.Owner = this;
                                manager.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                await Commons.ShowMessageAsync("로그인", "로그인 성공");
                                this.Hide();
                                this.TxtStudentId.Text = string.Empty;
                                this.PwbPassword.Password = string.Empty.ToString();
                                this.TxtStudentId.Focus();
                                manager.ShowDialog();
                                return;
                            }
                            else
                            {
                                Commons.STUDENTID = Convert.ToInt32(reader["studentId"]);
                                Commons.NAME = Convert.ToString(reader["studentname"]);
                                Commons.Major = Convert.ToString(reader["major"]);

                                var studentManagement = new StudentManagement(Commons.STUDENTID, Commons.NAME);
                                // 부모창 위치값을 자식창으로 전달
                                studentManagement.Owner = this;
                                studentManagement.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                await Commons.ShowMessageAsync("로그인", "로그인 성공");
                                this.Hide();
                                this.TxtStudentId.Text = string.Empty;
                                this.PwbPassword.Password = string.Empty.ToString();
                                this.TxtStudentId.Focus();
                                studentManagement.ShowDialog();
                                return;
                            }



                        }
                        else
                        {
                            await Commons.ShowMessageAsync("로그인", "로그인 실패");
                            return;
                        }
                    }
                }
                else
                {
                    await Commons.ShowMessageAsync("로그인", "로그인 아이디가 없습니다.");
                    return;
                }
                
            }
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnID_Click(object sender, RoutedEventArgs e)
        {
            var findId = new FindID();
            findId.Owner = this;
            findId.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Hide();
            findId.ShowDialog();
        }

        private void BtnPW_Click(object sender, RoutedEventArgs e)
        {
            var findPw = new FindPW();
            findPw.Owner = this;
            findPw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Hide();
            findPw.ShowDialog();
        }

        private void PwbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtStudentId.Focus();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
