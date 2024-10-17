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
        }


    private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                var listFacultys = facultyService.GetAll();
                FillFalcultyCombobox(listFacultys);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                if (item.MajorID != null)
                    dgvStudent.Rows[index].Cells[4].Value = item.Major.Name + "";

            }
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
            //if (selectedFaculty != null)
            //{
            //    var listMajor = majorService.GetAllByFaculty(selectedFaculty.FacultyName);
            //    FillMajorCombobox(listMajor);
            //    var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
            //    BindGrid(listStudents);
            //}
        }

        private void FillMajorCombobox(object listMajor)
        {
            this.cmbFaculty.DataSource = listMajor;
            this.cmbFaculty.DisplayMember = "Name";
            this.cmbFaculty.ValueMember = "MajorID";
        }
    }
}
