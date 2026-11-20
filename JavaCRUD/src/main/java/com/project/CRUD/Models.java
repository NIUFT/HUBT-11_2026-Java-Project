package com.project.CRUD;

import com.fasterxml.jackson.annotation.JsonAlias;
import java.sql.Timestamp;

public class Models {

    public static class Khoa {
        @JsonAlias({"MaKhoa", "MAKHOA", "makhoa"})
        private String maKhoa;
        @JsonAlias({"TenKhoa", "TENKHOA", "tenkhoa"})
        private String tenKhoa;

        public Khoa() {}
        public Khoa(String maKhoa, String tenKhoa) {
            this.maKhoa = maKhoa;
            this.tenKhoa = tenKhoa;
        }
        public String getMaKhoa() { return maKhoa; }
        public void setMaKhoa(String maKhoa) { this.maKhoa = maKhoa; }
        public String getTenKhoa() { return tenKhoa; }
        public void setTenKhoa(String tenKhoa) { this.tenKhoa = tenKhoa; }
    }

    public static class Nganh {
        @JsonAlias({"MaNganh", "MANGANH", "manganh"})
        private String maNganh;
        @JsonAlias({"TenNganh", "TENGANH", "tenganh"})
        private String tenNganh;

        public Nganh() {}
        public Nganh(String maNganh, String tenNganh) {
            this.maNganh = maNganh;
            this.tenNganh = tenNganh;
        }
        public String getMaNganh() { return maNganh; }
        public void setMaNganh(String maNganh) { this.maNganh = maNganh; }
        public String getTenNganh() { return tenNganh; }
        public void setTenNganh(String tenNganh) { this.tenNganh = tenNganh; }
    }

    public static class Lop {
        @JsonAlias({"MaLop", "MALOP", "malop"})
        private String maLop;
        @JsonAlias({"TenLop", "TENLOP", "tenlop"})
        private String tenLop;
        @JsonAlias({"MaKhoa", "MAKHOA", "makhoa"})
        private String maKhoa;

        public Lop() {}
        public Lop(String maLop, String tenLop, String maKhoa) {
            this.maLop = maLop;
            this.tenLop = tenLop;
            this.maKhoa = maKhoa;
        }
        public String getMaLop() { return maLop; }
        public void setMaLop(String maLop) { this.maLop = maLop; }
        public String getTenLop() { return tenLop; }
        public void setTenLop(String tenLop) { this.tenLop = tenLop; }
        public String getMaKhoa() { return maKhoa; }
        public void setMaKhoa(String maKhoa) { this.maKhoa = maKhoa; }
    }

    public static class Mon {
        @JsonAlias({"MaMon", "MAMON", "mamon"})
        private String maMon;
        @JsonAlias({"TenMon", "TENMON", "tenmon"})
        private String tenMon;
        @JsonAlias({"SoTinChi", "SOTINCHI", "sotinchi"})
        private Double soTinChi;

        public Mon() {}
        public Mon(String maMon, String tenMon, Double soTinChi) {
            this.maMon = maMon;
            this.tenMon = tenMon;
            this.soTinChi = soTinChi;
        }
        public String getMaMon() { return maMon; }
        public void setMaMon(String maMon) { this.maMon = maMon; }
        public String getTenMon() { return tenMon; }
        public void setTenMon(String tenMon) { this.tenMon = tenMon; }
        public Double getSoTinChi() { return soTinChi; }
        public void setSoTinChi(Double soTinChi) { this.soTinChi = soTinChi; }
    }

    public static class Hssv {
        @JsonAlias({"Msv", "MSV", "msv"})
        private String msv;
        @JsonAlias({"HoDem", "HODEM", "hodem"})
        private String hoDem;
        @JsonAlias({"Ten", "TEN", "ten"})
        private String ten;
        @JsonAlias({"GioiTinh", "GIOITINH", "gioitinh"})
        private Boolean gioiTinh;
        @JsonAlias({"NgaySinh", "NGAYSINH", "ngaysinh"})
        private Timestamp ngaySinh;
        @JsonAlias({"MaNganh", "MANGANH", "manganh"})
        private String maNganh;
        @JsonAlias({"MaLop", "MALOP", "malop"})
        private String maLop;

        public Hssv() {}
        public Hssv(String msv, String hoDem, String ten, Boolean gioiTinh,
                    Timestamp ngaySinh, String maNganh, String maLop) {
            this.msv = msv;
            this.hoDem = hoDem;
            this.ten = ten;
            this.gioiTinh = gioiTinh;
            this.ngaySinh = ngaySinh;
            this.maNganh = maNganh;
            this.maLop = maLop;
        }
        public String getMsv() { return msv; }
        public void setMsv(String msv) { this.msv = msv; }
        public String getHoDem() { return hoDem; }
        public void setHoDem(String hoDem) { this.hoDem = hoDem; }
        public String getTen() { return ten; }
        public void setTen(String ten) { this.ten = ten; }
        public Boolean getGioiTinh() { return gioiTinh; }
        public void setGioiTinh(Boolean gioiTinh) { this.gioiTinh = gioiTinh; }
        public Timestamp getNgaySinh() { return ngaySinh; }
        public void setNgaySinh(Timestamp ngaySinh) { this.ngaySinh = ngaySinh; }
        public String getMaNganh() { return maNganh; }
        public void setMaNganh(String maNganh) { this.maNganh = maNganh; }
        public String getMaLop() { return maLop; }
        public void setMaLop(String maLop) { this.maLop = maLop; }
    }

    public static class Diem {
        @JsonAlias({"Msv", "MSV", "msv"})
        private String msv;
        @JsonAlias({"MaMon", "MAMON", "mamon"})
        private String maMon;
        @JsonAlias({"DiemL1", "DIEML1", "dieml1"})
        private Double diemL1;
        @JsonAlias({"DiemL2", "DIEML2", "dieml2"})
        private Double diemL2;
        @JsonAlias({"GhiChu", "GHICHU", "ghichu"})
        private String ghiChu;

        public Diem() {}
        public Diem(String msv, String maMon, Double diemL1, Double diemL2, String ghiChu) {
            this.msv = msv;
            this.maMon = maMon;
            this.diemL1 = diemL1;
            this.diemL2 = diemL2;
            this.ghiChu = ghiChu;
        }
        public String getMsv() { return msv; }
        public void setMsv(String msv) { this.msv = msv; }
        public String getMaMon() { return maMon; }
        public void setMaMon(String maMon) { this.maMon = maMon; }
        public Double getDiemL1() { return diemL1; }
        public void setDiemL1(Double diemL1) { this.diemL1 = diemL1; }
        public Double getDiemL2() { return diemL2; }
        public void setDiemL2(Double diemL2) { this.diemL2 = diemL2; }
        public String getGhiChu() { return ghiChu; }
        public void setGhiChu(String ghiChu) { this.ghiChu = ghiChu; }
    }
}