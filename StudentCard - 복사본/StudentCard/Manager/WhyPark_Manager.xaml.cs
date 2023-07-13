using MahApps.Metro.Controls;
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
using StudentCard.Logics;
using StudentCard.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System.Diagnostics;
using MahApps.Metro.Controls.Dialogs;

namespace StudentCard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WhyPark_Manager : MetroWindow
    {
        // bool isFavorite = false;
        public WhyPark_Manager()
        {
            InitializeComponent();
        }

        private async void BtnReqRealtime_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            // TxtMovieName.DataContext = string.Empty;

            List<WhyParkStudent> list = new List<WhyParkStudent>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"SELECT studentID,
                                        studentName,
                                        reason,
                                        startDate,
                                        endDate
                                    FROM managertbl";

                    var cmd = new MySqlCommand(query, conn);
                    var adapter = new MySqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "GrdResult");

                    foreach (DataRow dr in dSet.Tables["GrdResult"].Rows)
                    {
                        list.Add(new WhyParkStudent
                        {
                            StudentID = Convert.ToInt32(dr["studentID"]),
                            StudentName = Convert.ToString(dr["studentName"]),
                            Reason = Convert.ToString(dr["reason"]),
                            StartDate = Convert.ToDateTime(dr["startDate"]).ToString("yyyy-MM-dd"),
                            EndDate = Convert.ToDateTime(dr["endDate"]).ToString("yyyy-MM-dd")
                        });
                    }

                    this.DataContext = list;
                    // isFavorite = true;
                    StsResult.Content = $"{list.Count}건 조회완료";

                }

            }
            catch (Exception ex)
            {
                var mySettings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "확인",
                    AnimateShow = true,
                    AnimateHide = true
                };

                var result = await this.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}",
                                                         MessageDialogStyle.Affirmative, mySettings);
            }
            
        }


        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtSearch.Text))
            {
                var mySettings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "확인",
                    AnimateShow = true,
                    AnimateHide = true
                };

                var result = await this.ShowMessageAsync("검색", "검색할 학생명을 입력하세요.",
                                                         MessageDialogStyle.Affirmative, mySettings);
                return;
            }
            try
            {
                SearchName(TxtSearch.Text);
            }
            catch (Exception ex)
            {

                var mySettings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "확인",
                    AnimateShow = true,
                    AnimateHide = true
                };

                var result = await this.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}",
                                                         MessageDialogStyle.Affirmative, mySettings);
            }
        }

        private async void SearchName(string text)
        {
            this.DataContext = null;
            // TxtMovieName.DataContext = string.Empty;

            List<WhyParkStudent> list = new List<WhyParkStudent>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = $@"SELECT studentID,
                                        studentName,
                                        reason,
                                        startDate,
                                        endDate
                                    FROM managertbl
                                    WHERE studentName LIKE '%{TxtSearch.Text}%'";


                    var cmd = new MySqlCommand(query, conn);
                    var adapter = new MySqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "GrdResult");

                    foreach (DataRow dr in dSet.Tables["GrdResult"].Rows)
                    {
                        list.Add(new WhyParkStudent
                        {
                            StudentID = Convert.ToInt32(dr["studentID"]),
                            StudentName = Convert.ToString(dr["studentName"]),
                            Reason = Convert.ToString(dr["reason"]),
                            StartDate = Convert.ToDateTime(dr["startDate"]).ToString("yyyy-MM-dd"),
                            EndDate = Convert.ToDateTime(dr["endDate"]).ToString("yyyy-MM-dd")
                        });
                    }

                    this.DataContext = list;
                    // isFavorite = true;
                    StsResult.Content = $"{list.Count}건 조회완료";

                }

            }
            catch (Exception ex)
            {
                var mySettings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "확인",
                    AnimateShow = true,
                    AnimateHide = true
                };

                var result = await this.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}",
                                                         MessageDialogStyle.Affirmative, mySettings);
            }
        }

        private void TxtSearchStudent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearch_Click(sender, e);
            }
        }

        private async void BtnSearchDate_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;


            List<WhyParkStudent> list = new List<WhyParkStudent>();
            try
            {
                DateTime? startDate = DTPstartdate.SelectedDate;
                DateTime? endDate = DTPenddate.SelectedDate;

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
                            if (conn.State == ConnectionState.Closed) conn.Open();

                            var query = $@"SELECT studentID,
                                        studentName,
                                        reason,
                                        startDate,
                                        endDate
                                    FROM managertbl
                                    WHERE DATE(startDate) <= '{DTPstartdate}' AND DATE(endDate) >= '{DTPenddate}';";

                            var cmd = new MySqlCommand(query, conn);
                            var adapter = new MySqlDataAdapter(cmd);
                            var dSet = new DataSet();
                            adapter.Fill(dSet, "GrdResult");

                            foreach (DataRow dr in dSet.Tables["GrdResult"].Rows)
                            {
                                list.Add(new WhyParkStudent
                                {
                                    StudentID = Convert.ToInt32(dr["studentID"]),
                                    StudentName = Convert.ToString(dr["studentName"]),
                                    Reason = Convert.ToString(dr["reason"]),
                                    StartDate = Convert.ToDateTime(dr["startDate"]).ToString("yyyy-MM-dd"),
                                    EndDate = Convert.ToDateTime(dr["endDate"]).ToString("yyyy-MM-dd")
                                });
                            }

                            // string StartDate = StartDate.ToString("yyyy-MM-DD");
                            this.DataContext = list;
                            // isFavorite = true;
                            StsResult.Content = $"{list.Count}건 조회완료";

                        }
                    }
                }


            }
            catch (Exception )
            {
                var mySettings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "확인",
                    AnimateShow = true,
                    AnimateHide = true
                };

                var result = await this.ShowMessageAsync("오류", $"날짜를 선택해주세요",
                                                         MessageDialogStyle.Affirmative, mySettings);
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            Owner.Show();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BtnReqRealtime_Click(sender, e);
        }
    }
}
