using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using StudentCard.Logics;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;

namespace StudentCard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WhyPark : MetroWindow
    {
        public WhyPark()
        {
            InitializeComponent();
        }

        private async void BtnRequest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime? startDate = DPstart.SelectedDate;
                DateTime? endDate = DPend.SelectedDate;

                if (startDate.HasValue && endDate.HasValue)
                {
                    if (startDate.Value > endDate.Value)
                    {
                        var mySettings = new MetroDialogSettings
                        {
                            AffirmativeButtonText = "확인",
                            AnimateShow = true,
                            AnimateHide = true
                        };

                        var result = await this.ShowMessageAsync("날짜선택", "날짜를 올바르게 설정해 주세요.");
                    }

                    else
                    {
                        using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                        {
                            if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                            var query = @"INSERT INTO managertbl
                                                    (studentID,
                                                    studentName,
                                                    reason,
                                                    startDate,
                                                    endDate)
                                                    VALUES
                                                    (@studentID,
                                                    @studentName,
                                                    @reason,
                                                    @startDate,
                                                    @endDate);";

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@studentID", TxtStudentId.Text);
                            cmd.Parameters.AddWithValue("@studentName", TxtStudentName.Text);
                            cmd.Parameters.AddWithValue("@reason", TxtReason.Text);
                            cmd.Parameters.AddWithValue("@startDate", DPstart.Text);
                            cmd.Parameters.AddWithValue("@endDate", DPend.Text);
                            cmd.ExecuteNonQuery();
                            Debug.WriteLine(query);
                            var set = new MetroDialogSettings
                            {
                                AffirmativeButtonText = "닫기",
                                AnimateShow = true,
                                AnimateHide = true
                            };

                            var result = await this.ShowMessageAsync("신청", "신청완료",
                                                 MessageDialogStyle.Affirmative, set);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"에러 {ex.Message}");
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtStudentId.Text = Convert.ToString(Commons.STUDENTID);
            TxtStudentName.Text = Convert.ToString(Commons.NAME);

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            Owner.Show();
        }
    }
}
