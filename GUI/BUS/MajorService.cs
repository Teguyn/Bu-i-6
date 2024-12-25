using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{ 
    public class MajorService
    {
        public List<Major> GetAll()
        {
            QuanLySinhVienDB context = new QuanLySinhVienDB();
            return context.Major.ToList();


        }
        public List<Major> GetAllByFaculty(int? facultyID)
        {
            QuanLySinhVienDB context = new QuanLySinhVienDB();

            // Lọc danh sách chuyên ngành theo FacultyID
            return context.Major
                        .Where(m => m.FacultyID == facultyID) // Lọc theo FacultyID
                        .ToList();
        }

    }
    
}
