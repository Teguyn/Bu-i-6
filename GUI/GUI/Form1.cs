using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace GUI
{
    public partial class Form1 : Form
    {

        QuanLySinhVienDB context = new QuanLySinhVienDB();
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();
        public Form1()
        {
            InitializeComponent();

        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        { 
            // Kiểm tra xem người dùng có nhấn vào một dòng hợp lệ không (không nhấn vào tiêu đề)
            if (e.RowIndex >= 0)
            {

                // Lấy thông tin sinh viên từ cơ sở dữ liệu dựa trên mã sinh viên
                var student = studentService.FindById(studentID);

                // Kiểm tra xem sinh viên có tồn tại và có ảnh không
                if (student != null && !string.IsNullOrEmpty(student.Avatar))
                {
                    // Hiển thị ảnh của sinh viên
                    ShowAvatar(student.Avatar);
                }
                else
                {
                    // Nếu không có ảnh, đặt PictureBox về null
                    picAvatar.Image = null;
                }

                // Kích hoạt TextBox để người dùng có thể sửa trực tiếp
                txtMaSV.ReadOnly = false; // Không cho phép sửa mã sinh viên

                // Gán giá trị cho cmbChuyenNghanh (nếu có)
                if (row.Cells[4].Value != null)
                {
                    cmbFaculty.SelectedValue = row.Cells[4].Value.ToString();
                }
            }
        }

        private void ShowAvatar(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                picAvatar.Image = null;
            }
            else
            {
                try
                {
                    // Lấy thư mục gốc của ứng dụng
                    string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;

                    // Tạo đường dẫn tới thư mục "Images"
                    string imagePath = Path.Combine(parentDirectory, "Images", imageName);

                    // Kiểm tra xem file ảnh có tồn tại không
                    if (File.Exists(imagePath))
                    {
                        // Hiển thị ảnh trong PictureBox
                        picAvatar.Image = System.Drawing.Image.FromFile(imagePath);
                        picAvatar.Refresh();
                    }
                    else
                    {
                        picAvatar.Image = null;
                        MessageBox.Show("Ảnh không tồn tại.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message);
                    picAvatar.Image = null;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try

            {
                setGridViewStyle(dgvStudent);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           

        }
        private void FillFalcultyCombobox(List< Faculty> listFacultys)
        {
            listFacultys.Insert(0, new Faculty());
            this.cmbFaculty.DataSource = listFacultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
            }
        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void BindGrid(List< Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore + "" ;
                if (item.MajorID != null)
                    dgvStudent.Rows[index].Cells[4].Value = item.Major.Name + "";
                //ShowAvatar(item.Avatar);
            }
        }

        private void cmbChuyenNghanh_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem người dùng đã chọn sinh viên để xóa hay chưa
                if (dgvStudent.SelectedRows.Count > 0)
                {
                    // Lấy mã sinh viên từ dòng đã chọn
                    string studentID = dgvStudent.SelectedRows[0].Cells[0].Value.ToString();

                    // Tìm sinh viên theo mã sinh viên
                    var student = studentService.FindById(studentID);

                    // Kiểm tra xem sinh viên có tồn tại trong cơ sở dữ liệu không
                    if (student != null)
                    {
                        // Xóa sinh viên khỏi cơ sở dữ liệu
                        studentService.Delete(studentID);

                        // Cập nhật lại danh sách sinh viên trong DataGridView
                        var listStudents = studentService.GetAll();
                        BindGrid(listStudents);

                        MessageBox.Show("Xóa sinh viên thành công!");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để xóa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy mã sinh viên từ TextBox
                string studentID = txtMaSV.Text;

                // Tìm sinh viên theo mã sinh viên
                var student = studentService.FindById(studentID);

                if (student != null)
                {
                    // Cập nhật thông tin sinh viên
                    student.FullName = txtTen.Text;  // Cập nhật họ tên
                    student.FacultyID = Convert.ToInt32(cmbFaculty.SelectedValue); // Cập nhật khoa

                    // Xóa giá trị của MajorID
                    student.MajorID = null;

                    student.AverageScore = Convert.ToDouble(txtDiemTB.Text);  // Cập nhật điểm trung bình

                    // Lưu thay đổi vào database
                    studentService.InsertUpdate(student);

                    // Cập nhật lại danh sách sinh viên
                    var listStudents = studentService.GetAll();
                    BindGrid(listStudents);

                    MessageBox.Show("Cập nhật sinh viên thành công!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.checkBox1.Checked)
                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void đăngKýChuyênToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
            Form2 form2 = new Form2();
            if (form2.ShowDialog() == DialogResult.OK)
            {
                // Cập nhật dữ liệu trên Form 1
                var listStudents = studentService.GetAll();
                BindGrid(listStudents);
            }

        }

        private void picAvatar_Click(object sender, EventArgs e)
        {

        }

        private void btnadd_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem thông tin sinh viên đã được nhập đầy đủ chưa
                if (string.IsNullOrEmpty(txtMaSV.Text) ||
                    string.IsNullOrEmpty(txtTen.Text) ||
                    string.IsNullOrEmpty(txtDiemTB.Text) ||
                    cmbFaculty.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên.");
                    return;
                }

                // Tạo mới sinh viên
                var student = new Student()
                {
                    StudentID = txtMaSV.Text,
                    FullName = txtTen.Text,
                    FacultyID = Convert.ToInt32(cmbFaculty.SelectedValue),
                    AverageScore = Convert.ToDouble(txtDiemTB.Text)
                };

                // Cập nhật chuyên ngành (nếu có)
                if (cmbFaculty.SelectedValue != null)
                {
                    student.MajorID = Convert.ToInt32(cmbFaculty.SelectedValue);
                }

                // Thêm sinh viên vào database
                studentService.Add(student);

                // Cập nhật lại danh sách sinh viên
                var listStudents = studentService.GetAll();
                BindGrid(listStudents);

                MessageBox.Show("Thêm sinh viên thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem mã sinh viên đã được nhập chưa
            if (string.IsNullOrEmpty(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên trước khi chọn ảnh.");
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Chỉ định các loại file mà OpenFileDialog có thể mở
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            // Hiển thị OpenFileDialog và kiểm tra xem người dùng có chọn file hay không
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Lấy đường dẫn file đã chọn
                string selectedFilePath = openFileDialog.FileName;

                try
                {
                    // Hiển thị file ảnh vào PictureBox
                    picAvatar.Image = System.Drawing.Image.FromFile(selectedFilePath);

                    // Lưu ảnh vào thư mục "Images" với tên mới dựa trên mã sinh viên
                    SaveImageToFolder(selectedFilePath, txtMaSV.Text);  // Truyền mã sinh viên
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        
            private void SaveImageToFolder(string sourceFilePath, string studentID)
            {
            // Lấy thư mục gốc của ứng dụng
            string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;

            // Tạo đường dẫn đến thư mục "Images"
            string imagesFolder = Path.Combine(parentDirectory, "Images");

            // Tạo thư mục "Images" nếu chưa tồn tại
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            // Lấy phần mở rộng file
            string fileExtension = Path.GetExtension(sourceFilePath);

            // Tạo đường dẫn đích để lưu ảnh vào thư mục "Images" với tên dựa trên mã sinh viên
            string destinationFilePath = Path.Combine(imagesFolder, studentID + fileExtension);

            // Sao chép file ảnh từ vị trí nguồn đến vị trí đích
            File.Copy(sourceFilePath, destinationFilePath, true);

            // Lưu tên ảnh vào cơ sở dữ liệu (trong trường hợp này chỉ lưu tên file, không lưu đường dẫn đầy đủ)
            string imageName = studentID + fileExtension;

            // Cập nhật tên ảnh vào cơ sở dữ liệu cho sinh viên
            studentService.UpdateStudentAvatar(studentID, imageName);

            MessageBox.Show("Ảnh đã được lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        internal void RefreshData()
        {
            // Cập nhật dữ liệu trên Form 1
            var listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }
    }
        
    
}
