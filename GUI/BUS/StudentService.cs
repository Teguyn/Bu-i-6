using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class StudentService
    {   
        public List<Student> GetAll()
        {
            QuanLySinhVienDB context = new QuanLySinhVienDB();
            return context.Student.ToList();


        }

        public List<Student> GetAllHasNoMajor()
        {
            QuanLySinhVienDB context = new QuanLySinhVienDB();
            return context.Student.Where(p => p.MajorID == null).ToList();
        }

        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            QuanLySinhVienDB context = new QuanLySinhVienDB();
            return context.Student.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }

        public Student FindById(string studentId)
        {
            QuanLySinhVienDB context = new QuanLySinhVienDB();
            return context.Student.FirstOrDefault(p => p.StudentID == studentId);
        }

        public void InsertUpdate(Student s)
        {
            QuanLySinhVienDB context = new QuanLySinhVienDB();
            context.Student.AddOrUpdate(s);
            context.SaveChanges();
        }

        public void Add(Student student)
        {
            using (QuanLySinhVienDB context = new QuanLySinhVienDB())
            {
                // Thêm sinh viên mới vào bảng Student
                context.Student.Add(student);
                // Lưu thay đổi vào cơ sở dữ liệu
                context.SaveChanges();
            }
        }

        public void Delete(string studentID)
        {
            using (QuanLySinhVienDB context = new QuanLySinhVienDB())
            {
                // Tìm sinh viên theo mã sinh viên
                var student = context.Student.FirstOrDefault(s => s.StudentID == studentID);

                // Nếu sinh viên tồn tại thì xóa
                if (student != null)
                {
                    context.Student.Remove(student);
                    context.SaveChanges();
                }
            }
        }

        public void UpdateStudentAvatar(string studentID, string imageName)
        {
            using (QuanLySinhVienDB context = new QuanLySinhVienDB())
            {
                var student = context.Student.Find(studentID);
                if (student != null)
                {
                    student.Avatar = imageName;
                    context.SaveChanges();
                }
            }
        }

        public void UpdateStudentMajor(string studentID, int majorID)
        {
            using (QuanLySinhVienDB context = new QuanLySinhVienDB())
            {
                // Tìm sinh viên theo StudentID
                var student = context.Student.Find(studentID);

                if (student != null)
                {
                    // Cập nhật MajorID cho sinh viên
                    student.MajorID = majorID;

                    // Lưu thay đổi vào database
                    context.SaveChanges();
                }
            }
        }
    }
}
