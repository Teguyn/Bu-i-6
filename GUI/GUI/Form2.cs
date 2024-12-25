using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class Form2 : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();


        public Form2()
        {
            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form2_FormClosed);
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 form1 = (Form1)Application.OpenForms["Form1"];
            if (form1 != null)
            {
                form1.RefreshData();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                var listStudents = studentService.GetAllHasNoMajor();
                BindGrid(listStudents);

                // Gọi phương thức này để đổ dữ liệu khoa từ DataGridView vào ComboBox
                BindFacultyFromGridToComboBox();

                var listFacultys = facultyService.GetAll();
                FillFalcultyCombobox(listFacultys);

                // Load danh sách chuyên ngành ban đầu
                var listMajors = majorService.GetAll();
                FillMajorCombobox(listMajors);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void BindFacultyFromGridToComboBox()
        {
            HashSet<string> faculties = new HashSet<string>();

            // Duyệt qua các hàng của DataGridView
            foreach (DataGridViewRow row in dgvStudent.Rows)
            {
                if (row.Cells[2].Value != null)
                {
                    string facultyName = row.Cells[2].Value.ToString();
                    faculties.Add(facultyName); // Sử dụng HashSet để tránh trùng lặp tự động
                }
            }

            // Nếu có dữ liệu thì mới bind vào ComboBox
            if (faculties.Count > 0)
            {
                cmbFaculty.DataSource = faculties.ToList(); // Chuyển HashSet thành List
            }
            else
            {
                cmbFaculty.DataSource = null; // Nếu không có dữ liệu, xóa các mục trước đó
            }

        }

        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            this.cmbFaculty.DataSource = listFacultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có nhấn vào một dòng hợp lệ không (không nhấn vào tiêu đề)
            if (e.RowIndex >= 0)
            {
                // Lấy thông tin của dòng đã chọn
                DataGridViewRow row = dgvStudent.Rows[e.RowIndex];

                // Lấy tên khoa từ dòng được chọn
                string facultyName = row.Cells[2].Value.ToString();

                // Lấy danh sách khoa
                var listFacultys = facultyService.GetAll();


                // Lấy mã khoa dựa trên tên khoa
                Faculty selectedFaculty = listFacultys.FirstOrDefault(f => f.FacultyName == facultyName);

                if (selectedFaculty != null)
                {
                    // Lấy danh sách chuyên ngành theo mã khoa
                    var listMajors = majorService.GetAllByFaculty(selectedFaculty.FacultyID);

                    // Hiển thị danh sách chuyên ngành lên combobox chuyên ngành
                    FillMajorCombobox(listMajors);
                }
                else
                {
                    MessageBox.Show("Không thể lấy mã khoa.");
                }
                if (!string.IsNullOrEmpty(facultyName))
                {
                    // Tìm mục tương ứng trong ComboBox
                    cmbFaculty.SelectedIndex = cmbFaculty.FindStringExact(facultyName);

                    if (cmbFaculty.SelectedIndex == -1) // Nếu không tìm thấy khoa trong ComboBox
                    {
                        MessageBox.Show("Khoa không có trong danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Không thể lấy tên khoa từ dòng này.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void FillMajorCombobox(List<Major> listMajor)
        {
            this.cmbMajor.DataSource = listMajor;
            this.cmbMajor.DisplayMember = "Name";
            this.cmbMajor.ValueMember = "MajorID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)
                    dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore + "";
            }
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void cmbMajor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem một dòng hợp lệ có được chọn trong DataGridView không
            if (dgvStudent.CurrentRow != null && dgvStudent.CurrentRow.Index >= 0)
            {
                // Kiểm tra xem một dòng hợp lệ có được chọn trong DataGridView không
                if (dgvStudent.CurrentRow != null && dgvStudent.CurrentRow.Index >= 0)
                {
                    // Lấy thông tin sinh viên từ DataGridView (StudentID)
                    string studentID = dgvStudent.CurrentRow.Cells[0].Value.ToString();

                    // Lấy MajorID từ ComboBox chuyên ngành
                    Major selectedMajor = cmbMajor.SelectedItem as Major;
                    if (selectedMajor != null)
                    {
                        int majorID = selectedMajor.MajorID;

                        // Gọi service để cập nhật chuyên ngành cho sinh viên
                        studentService.UpdateStudentMajor(studentID, majorID);

                        MessageBox.Show("Đăng ký chuyên ngành thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Cập nhật lại DataGridView sau khi lưu
                        var listStudents = studentService.GetAllHasNoMajor();
                        BindGrid(listStudents);
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn chuyên ngành.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên từ danh sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            Form1 form1 = (Form1)Application.OpenForms["Form1"];
            if (form1 != null)
            {
                form1.RefreshData();
            }
        }
    }
}
