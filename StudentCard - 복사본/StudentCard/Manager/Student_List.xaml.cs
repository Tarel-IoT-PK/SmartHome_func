using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using StudentCard.Logics;
using StudentCard.Module;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StudentCard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Student_List : MetroWindow
    {

        public string selItem;
        public string target;


        public Student_List()
        {
            InitializeComponent();
        }

        public async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string text = TbxSearch.Text;
                switch (selItem)
                {
                    case "0":   // 학번
                        target = $"WHERE studentID LIKE '%{text}%'";
                        break;
                    case "1":   // 이름
                        target = $"WHERE studentName LIKE '%{text}%'"; ;
                        break;
                    default:
                        target = $"WHERE studentID LIKE '%{text}%'";
                        break;
                }

                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT studentID,
                            studentName,
                            birthday,
                            major,
                            PhoneNum,
                            address,
                            gender
                            FROM student_list " + target;
                    Debug.WriteLine(query);
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "List");
                    List<StudentList> strings = new List<StudentList>();
                    foreach (DataRow row in ds.Tables["List"].Rows)
                    {
                        strings.Add(new StudentList
                        {
                            studentID = Convert.ToInt32(row["studentID"]),
                            studentName = Convert.ToString(row["studentName"]),
                            birthday = Convert.ToString(row["birthday"]),
                            major = Convert.ToString(row["major"]),
                            PhoneNum = Convert.ToString(row["PhoneNum"]),
                            address = Convert.ToString(row["address"]),
                            gender = Convert.ToString(row["gender"])
                        });
                    }

                    this.DataContext = strings;
                    StsResult.Content = $"{CboDivision.Text} : {TbxSearch.Text}의 검색 결과 : {strings.Count}명 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장오류 {ex.Message}");

            }
        }


        private void CboDivision_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selItem = Convert.ToString(CboDivision.SelectedIndex);


        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CboDivision.SelectedIndex = 0;
            BtnSearch_Click(sender, e);

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            Owner.Show();
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            var mqttPopWin = new List_Edit();
            mqttPopWin.Owner = this;
            mqttPopWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            var isResult = mqttPopWin.ShowDialog();
            Debug.WriteLine(isResult);
            if (isResult == false)
            {
                BtnSearch_Click(sender, e); // 삭제후 닫히면 조회 다시 -> 그리드 갱신
            }
        }

        private void TbxSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearch_Click(sender, e);
            }
        }

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mqttPopWin = new List_Edit();
            var studentlist = new StudentList();            
            var studentInfo = GrdResult.SelectedItem as StudentList;
            var studentName = studentInfo.studentName;
            var birthday = studentInfo.birthday;
            var major = studentInfo.major;
            var PhoneNum = studentInfo.PhoneNum;
            var address = studentInfo.address;
            var gender = studentInfo.gender;

            mqttPopWin.Owner = this;
            mqttPopWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mqttPopWin.BtnNew.Content = "수정";
            mqttPopWin.BtnClose.Content = "삭제";
            mqttPopWin.TxtStudentId.Text = studentInfo.studentID.ToString();
            mqttPopWin.TxtStudentId.IsReadOnly = true;
            mqttPopWin.TxtStudentName.Text = studentName;
            mqttPopWin.TxtBirthday.Text = birthday;
            mqttPopWin.CboMajor.SelectedItem = major;
            mqttPopWin.TxtBirthday.Text = birthday;
            mqttPopWin.TxtPhoneNum.Text = PhoneNum;
            mqttPopWin.TxtAddress.Text = address;
            mqttPopWin.TxtPhoneNum.Text = PhoneNum;

            if (gender == "남성")
            {
                mqttPopWin.RdoMale.IsChecked = true;
            }
            else if (gender == "여성")
            {
                mqttPopWin.RdoFemale.IsChecked = true;
            }

            mqttPopWin.BtnNew.Click -= mqttPopWin.BtnNew_Click;
            mqttPopWin.BtnNew.Click += mqttPopWin.editStudent;

            mqttPopWin.BtnClose.Click -= mqttPopWin.BtnClose_Click;
            mqttPopWin.BtnClose.Click += mqttPopWin.StudentDel;
            mqttPopWin.TxtAddress.KeyDown -= mqttPopWin.TxtAddress_KeyDown;
            mqttPopWin.TxtAddress.KeyDown += mqttPopWin.editStudentKeyDown;

            var isResult = mqttPopWin.ShowDialog();
            if (isResult == false)
            {
                BtnSearch_Click(sender, e); // 삭제후 닫히면 조회 다시 -> 그리드 갱신
              
            }
        }
    }
}