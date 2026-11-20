using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWinFormsUI
{
    public class KhoaDto
    {
        public string MaKhoa { get; set; }
        public string TenKhoa { get; set; }
    }

    public class NganhDto
    {
        public string MaNganh { get; set; }
        public string TenNganh { get; set; }
    }

    public class LopDto
    {
        public string MaLop { get; set; }
        public string TenLop { get; set; }
        public string MaKhoa { get; set; }
    }

    public class MonDto
    {
        public string MaMon { get; set; }
        public string TenMon { get; set; }
        public double? SoTinChi { get; set; }
    }

    public class HssvDto
    {
        public string Msv { get; set; }
        public string HoDem { get; set; }
        public string Ten { get; set; }
        public bool? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string MaNganh { get; set; }
        public string MaLop { get; set; }
    }

    public class DiemDto
    {
        public string Msv { get; set; }
        public string MaMon { get; set; }
        public double? DiemL1 { get; set; }
        public double? DiemL2 { get; set; }
        public string GhiChu { get; set; }
    }
}
