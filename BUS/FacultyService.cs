using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class FacultyService
    {
        public List<Faculty>GetAll()
        {
            QuanLySinhVienDB context = new QuanLySinhVienDB();
            return context.Faculty.ToList();
        }
    }
}
