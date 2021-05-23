﻿using MaterialSurface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TutteeFrame2.Controller;
using TutteeFrame2.DataAccess;
using TutteeFrame2.Model;
using TutteeFrame2.Utils;
namespace TutteeFrame2.View
{
    public partial class OneStudentView : Form
    {
        public enum Mode { Add, Edit };
        #region Win32
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
        #endregion
        public Mode mode;
        public string studentID;
        public Student student;
        public bool doneSuccess = false;
        HomeView homeView;
        List<Class> classes = new List<Class>();
        public OneStudentView(object passValue,HomeView homeView)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            if (passValue is null)
            {
                this.Close();
            }
            if (passValue is string)
            {
                this.studentID = (string)passValue;
                this.mode = Mode.Add;
            }
            else
            {
                if (passValue is Student)
                {
                    this.student = (Student)passValue;
                    this.mode = Mode.Edit;
                }
            }
            this.homeView = homeView;
        }
        protected async override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, ptbAvatar.Width, ptbAvatar.Height);
            Region rg = new Region(gp);
            ptbAvatar.Region = rg;
            await Task.Run(() =>
            {
                ClassController classController = new ClassController();
                this.classes = classController.GetClasses();
            });
            if (classes.Count > 0)
            {
                cbbCurrentClass.Items.Clear();
                foreach (var _class in classes)
                {
                    cbbCurrentClass.Items.Add(_class.classID);
                }
            }
            OnFirstLoad();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            homeView.Activate();
        }
        void OnFirstLoad()
        {
            if (mode == Mode.Edit && student != null)
            {

                lbID.Text = student.ID;
                txtSurname.Text = student.SurName;
                txtFirstname.Text = student.FirstName;
                txtAddress.Text = student.Address;
                dateBornPicker.Value = student.DateBorn;
                cbbSex.SelectedIndex = (student.Sex) ? 1 : 0;
                txtPhone.Text = student.Phone;
                txtMail.Text = student.Mail;
                txtCurrentClass.Text = student.ClassID;
                cbbStatus.SelectedIndex = (student.Status) ? 1 : 0;
                ptbAvatar.Image = student.Avatar;
            }
            else
            {
                if (mode == Mode.Add && student == null)
                {
                    lbID.Text = this.studentID;
                }
            }
        }
        void SetLoad(bool isLoading, string loadInformation = "")
        {
            lbInformation.Text = loadInformation;
            mainProgressbar.Visible = lbInformation.Visible = isLoading;
        }

        private void cbbCurrentClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCurrentClass.Text = (string)cbbCurrentClass.SelectedItem;
        }

        private void FetchDataOfStudentFromUI()
        {
            student.ID = lbID.Text;
            student.SurName = txtSurname.Text;
            student.FirstName = txtFirstname.Text;
            student.Address = txtAddress.Text;
            student.DateBorn = dateBornPicker.Value;
            student.Sex = cbbSex.SelectedIndex == 0 ? false : true;
            student.Phone = txtPhone.Text;
            student.Mail = txtMail.Text;
            student.ClassID = txtCurrentClass.Text;
            student.Status = cbbStatus.SelectedIndex == 0 ? false : true;
            student.Avatar = ptbAvatar.Image;
        }
        private void OnClickConfirmButton(object sender, EventArgs e)
        {
            FetchDataOfStudentFromUI();
            OnUpdateData();

        }

        private async void OnUpdateData()
        {
            SetLoad(true, "Đang thực hiện cập nhật dữ liệu...");
          
            await Task.Delay(1000);
            await Task.Run(()=> {
                StudentController studentController = new StudentController(null);
                bool progressResult = studentController.UpdateStudent(this.student);
                if (progressResult)
                {
                  
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                    
                }
            });
            SetLoad(false);
            if(this.DialogResult == DialogResult.OK)
            {
                Dialog.Show(this, "Cập nhật thành công.", tittle: "Thông báo");
                this.Close();
            }
            else
            {
                Dialog.Show(this, "Cập nhật thất bại, vui lòng kiển tra dữ liệu và thử lại.", tittle: "Cảnh báo");
            }

        }
    }
}