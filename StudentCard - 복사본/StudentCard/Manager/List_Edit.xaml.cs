using MahApps.Metro.Controls;
using StudentCard.Logics;
using StudentCard.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using StudentCard.Module;
using System.Diagnostics;
using Org.BouncyCastle.Asn1.X509;
using static System.Net.Mime.MediaTypeNames;
using MahApps.Metro.Controls.Dialogs;
using Bogus.DataSets;
using Org.BouncyCastle.Utilities.Collections;
using static Bogus.DataSets.Name;

namespace StudentCard
{
    /// <summary>
    /// List_Edit.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class List_Edit : MetroWindow
    {
        public List_Edit()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtStudentId.Focus();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT * FROM major;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var command = new MySqlCommand(query, conn);
                    var adapter = new MySqlDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    string name = dataTable.Rows[0]["name"].ToString();

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        Debug.WriteLine(dataTable.Rows[i]["name"].ToString());
                        CboMajor.Items.Add(dataTable.Rows[i]["name"].ToString());
                    }



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

                var result = await this.ShowMessageAsync("오류", $"{ex.Message}",
                                                         MessageDialogStyle.Affirmative, mySettings);
            }


        }


        // 신규 추가
        public void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            newStudent();
            TxtStudentId.Focus();
        }


        // 학생편집 삭제 기능
        public async void StudentDel(object sender, RoutedEventArgs e)
        {

            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "삭제",
                NegativeButtonText = "취소",
                AnimateShow = true,
                AnimateHide = true
            };

            var result = await this.ShowMessageAsync("경고", "정말로 삭제하시겠습니까?",
                                                             MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Negative)
            {

            }
            else if (result == MessageDialogResult.Affirmative)
            {
                Deletefunction();
            }


        }
        public async void Deletefunction()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"DELETE FROM studenttbl WHERE(studentID = @studentId);";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@studentID", int.Parse(TxtStudentId.Text));

                    cmd.ExecuteNonQuery();

                    var mySettings = new MetroDialogSettings
                    {
                        AffirmativeButtonText = "확인",
                        AnimateShow = true,
                        AnimateHide = true
                    };
                    var result = await this.ShowMessageAsync("성공", "삭제 성공",
                                                              MessageDialogStyle.Affirmative, mySettings);
                    this.Close();
                }

            }
            catch (Exception)
            {
                var mySettings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "확인",
                    AnimateShow = true,
                    AnimateHide = true
                };
                var result = await this.ShowMessageAsync("실패", "삭제 실패",
                                                          MessageDialogStyle.Affirmative, mySettings);
            }
        }



        // 학생편집 수정기능
        public async void editStudent(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"UPDATE studenttbl
                                SET studentName = @studentName, 
                                    birthday = @birthday,
                                    major = @major, 
                                 PhoneNum = @PhoneNum, 
                                    address = @address,
                                    gender = @gender
                                WHERE(studentID = @studentId);";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@studentID", int.Parse(TxtStudentId.Text));
                    cmd.Parameters.AddWithValue("@studentName", TxtStudentName.Text);
                    cmd.Parameters.AddWithValue("@birthday", TxtBirthday.Text);
                    cmd.Parameters.AddWithValue("@major", CboMajor.SelectedIndex);
                    cmd.Parameters.AddWithValue("@PhoneNum", TxtPhoneNum.Text);
                    cmd.Parameters.AddWithValue("@address", TxtAddress.Text);
                    if (RdoMale.IsChecked == true)
                    {
                        cmd.Parameters.AddWithValue("@gender", "남성");
                    }
                    else if (RdoFemale.IsChecked == true)
                    {
                        cmd.Parameters.AddWithValue("@gender", "여성");
                    }
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    var mySettings = new MetroDialogSettings
                    {
                        AffirmativeButtonText = "확인",
                        AnimateShow = true,
                        AnimateHide = true
                    };

                    var result = await this.ShowMessageAsync("성공", "수정 성공",
                                                             MessageDialogStyle.Affirmative, mySettings);

                }
            }
            catch (Exception)
            {
                var mySettings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "확인",
                    AnimateShow = true,
                    AnimateHide = true
                };
                var result = await this.ShowMessageAsync("실패", "삭제 실패",
                                                          MessageDialogStyle.Affirmative, mySettings);
            }
           
        }
        public async void newStudent()
        {
            string studentID = TxtStudentId.Text;
            string studentName = TxtStudentName.Text;
            string birthday = TxtBirthday.Text;
            string major = Convert.ToString(CboMajor.SelectedIndex);
            string PhoneNum = TxtPhoneNum.Text;
            string address = TxtAddress.Text;
            string gender = "";

            try
            {
                
                if (RdoMale.IsChecked == true)
                {
                    gender = "남성";
                }
                else if (RdoFemale.IsChecked == true)
                {
                    gender = "여성";
                }
                if ((string.IsNullOrEmpty(TxtStudentId.Text))|| string.IsNullOrEmpty(TxtStudentName.Text)
                    || string.IsNullOrEmpty(TxtBirthday.Text) || (major == "-1") || (gender == "")
                    || string.IsNullOrEmpty(TxtPhoneNum.Text) || string.IsNullOrEmpty(TxtAddress.Text) )
                {
                    var set = new MetroDialogSettings
                    {
                        AffirmativeButtonText = "확인",
                        AnimateShow = true,
                        AnimateHide = true
                    };

                    var result = await this.ShowMessageAsync("오류", "정보를 모두 입력해주세요",
                                            MessageDialogStyle.Affirmative, set);
                    return;
                }
                //else if (birthday == "")
                //{
                //    MessageBox.Show("생년월일을 입력하세요!!", "생년월일 입력 오류");
                //    return;
                //}
                //else if (major == "-1")
                //{
                //    MessageBox.Show("전공를 선택하세요!!", "전공 선택 오류");
                //    return;
                //}
                //else if (PhoneNum == "")
                //{
                //    MessageBox.Show("전화번호를 입력하세요!!", "전화번호 입력 오류");
                //    return;
                //}
                //else if (address == "")
                //{
                //    MessageBox.Show("주소를 입력하세요!!", "주소 입력 오류");
                //    return;
                //}
                //else if (gender == "")
                //{
                //    MessageBox.Show("성별을 선택하세요!!", "성별 선택 오류");
                //    return;
                //}

            }
            catch
            {
                //MessageBox.Show("학번을 입력하세요!!", "학번 입력 오류");
                //return;
            }




            try
            {

                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();


                    var query = @"INSERT INTO studenttbl
                                (studentID,
                                studentName,
                                birthday,
                                major,
                                PhoneNum,
                                address,
                                gender)
                                VALUES
                                (@studentID,
                                @studentName,
                                @birthday,
                                @major,
                                @PhoneNum,
                                @address,
                                @gender)";
                    MySqlCommand command = new MySqlCommand(query, conn);

                    command.Parameters.AddWithValue("@studentID", Convert.ToInt32(studentID));
                    command.Parameters.AddWithValue("@studentName", studentName);
                    command.Parameters.AddWithValue("@birthday", birthday);
                    command.Parameters.AddWithValue("@major", major);
                    command.Parameters.AddWithValue("@PhoneNum", PhoneNum);
                    command.Parameters.AddWithValue("@address", address);
                    command.Parameters.AddWithValue("@gender", gender);

                    command.ExecuteNonQuery();

                    conn.Close();


                }
                var set = new MetroDialogSettings
                {
                    AffirmativeButtonText = "닫기",
                    AnimateShow = true,
                    AnimateHide = true
                };

                var result = await this.ShowMessageAsync("저장", "저장성공",
                                        MessageDialogStyle.Affirmative, set);
                TxtStudentId.Clear();
                TxtStudentName.Clear();
                TxtBirthday.Clear();
                TxtPhoneNum.Clear();
                TxtAddress.Clear();
                CboMajor.SelectedIndex = -1;
                RdoFemale.IsChecked = false;
                RdoMale.IsChecked = false;



            }
            catch (Exception ex)
            {
                var mySettings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "확인",
                    AnimateShow = true,
                    AnimateHide = true
                };

                var result = await this.ShowMessageAsync("오류", $"{ex.Message}",
                                                         MessageDialogStyle.Affirmative, mySettings);
            }
        }


        // 닫기 아무것도 안해도 기능
        public void BtnClose_Click(object sender, RoutedEventArgs e)
        {
  
                this.Close();
            

        }

        private void TxtStudentId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TxtStudentName.Focus();
            }
        }

        private void TxtStudentName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TxtBirthday.Focus();
            }
        }

        private void TxtBirthday_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CboMajor.Focus();
            }

        }

        private void CboMajor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TxtPhoneNum.Focus();
            }
        }

        private void TxtPhoneNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TxtAddress.Focus();
            }
        }

       public void TxtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                newStudent();
            }
        }

        public void editStudentKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                editStudent(sender,e);
            }
        }
    }
}
