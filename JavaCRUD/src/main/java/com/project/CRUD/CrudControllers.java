package com.project.CRUD;

import com.project.CRUD.Models.*;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;

class InputSanitizer {
    static String trimOrNull(String value) {
        if (value == null) return null;
        String trimmed = value.trim();
        return trimmed.isEmpty() ? null : trimmed;
    }

    static String trimKeepEmpty(String value) {
        return value == null ? null : value.trim();
    }
}

@RestController
@RequestMapping("/api/khoa")
class KhoaController {
    private final DatabaseHelper db;

    KhoaController(DatabaseHelper db) {
        this.db = db;
    }

    private Khoa mapRow(ResultSet rs) throws SQLException {
        return new Khoa(rs.getString("MAKHOA"), rs.getString("TENKHOA"));
    }

    @GetMapping
    public List<Khoa> getAll() {
        return db.queryList("SELECT MAKHOA, TENKHOA FROM KHOA", this::mapRow);
    }

    @GetMapping("/{id}")
    public ResponseEntity<Khoa> getById(@PathVariable String id) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null) return ResponseEntity.badRequest().build();
        Khoa item = db.queryOne("SELECT MAKHOA, TENKHOA FROM KHOA WHERE MAKHOA = ?", this::mapRow, cleanId);
        return item != null ? ResponseEntity.ok(item) : ResponseEntity.notFound().build();
    }

    @PostMapping
    public ResponseEntity<Khoa> create(@RequestBody Khoa body) {
        if (body == null) return ResponseEntity.badRequest().build();
        String maKhoa = InputSanitizer.trimOrNull(body.getMaKhoa());
        String tenKhoa = InputSanitizer.trimOrNull(body.getTenKhoa());
        if (maKhoa == null) return ResponseEntity.badRequest().build();

        body.setMaKhoa(maKhoa);
        body.setTenKhoa(tenKhoa);
        db.executeUpdate("INSERT INTO KHOA (MAKHOA, TENKHOA) VALUES (?, ?)", maKhoa, tenKhoa);
        return ResponseEntity.status(HttpStatus.CREATED).body(body);
    }

    @PutMapping("/{id}")
    public ResponseEntity<Khoa> update(@PathVariable String id, @RequestBody Khoa body) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null || body == null) return ResponseEntity.badRequest().build();
        String tenKhoa = InputSanitizer.trimOrNull(body.getTenKhoa());

        int updated = db.executeUpdate("UPDATE KHOA SET TENKHOA = ? WHERE MAKHOA = ?", tenKhoa, cleanId);
        if (updated == 0) return ResponseEntity.notFound().build();
        body.setMaKhoa(cleanId);
        body.setTenKhoa(tenKhoa);
        return ResponseEntity.ok(body);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable String id) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null) return ResponseEntity.badRequest().build();
        int deleted = db.executeUpdate("DELETE FROM KHOA WHERE MAKHOA = ?", cleanId);
        return deleted > 0 ? ResponseEntity.noContent().build() : ResponseEntity.notFound().build();
    }
}

@RestController
@RequestMapping("/api/nganh")
class NganhController {
    private final DatabaseHelper db;

    NganhController(DatabaseHelper db) {
        this.db = db;
    }

    private Nganh mapRow(ResultSet rs) throws SQLException {
        return new Nganh(rs.getString("MANGANH"), rs.getString("TENGANH"));
    }

    @GetMapping
    public List<Nganh> getAll() {
        return db.queryList("SELECT MANGANH, TENGANH FROM NGANH", this::mapRow);
    }

    @GetMapping("/{id}")
    public ResponseEntity<Nganh> getById(@PathVariable String id) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null) return ResponseEntity.badRequest().build();
        Nganh item = db.queryOne("SELECT MANGANH, TENGANH FROM NGANH WHERE MANGANH = ?", this::mapRow, cleanId);
        return item != null ? ResponseEntity.ok(item) : ResponseEntity.notFound().build();
    }

    @PostMapping
    public ResponseEntity<Nganh> create(@RequestBody Nganh body) {
        if (body == null) return ResponseEntity.badRequest().build();
        String maNganh = InputSanitizer.trimOrNull(body.getMaNganh());
        String tenNganh = InputSanitizer.trimOrNull(body.getTenNganh());
        if (maNganh == null) return ResponseEntity.badRequest().build();

        body.setMaNganh(maNganh);
        body.setTenNganh(tenNganh);
        db.executeUpdate("INSERT INTO NGANH (MANGANH, TENGANH) VALUES (?, ?)", maNganh, tenNganh);
        return ResponseEntity.status(HttpStatus.CREATED).body(body);
    }

    @PutMapping("/{id}")
    public ResponseEntity<Nganh> update(@PathVariable String id, @RequestBody Nganh body) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null || body == null) return ResponseEntity.badRequest().build();
        String tenNganh = InputSanitizer.trimOrNull(body.getTenNganh());

        int updated = db.executeUpdate("UPDATE NGANH SET TENGANH = ? WHERE MANGANH = ?", tenNganh, cleanId);
        if (updated == 0) return ResponseEntity.notFound().build();
        body.setMaNganh(cleanId);
        body.setTenNganh(tenNganh);
        return ResponseEntity.ok(body);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable String id) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null) return ResponseEntity.badRequest().build();
        int deleted = db.executeUpdate("DELETE FROM NGANH WHERE MANGANH = ?", cleanId);
        return deleted > 0 ? ResponseEntity.noContent().build() : ResponseEntity.notFound().build();
    }
}

@RestController
@RequestMapping("/api/lop")
class LopController {
    private final DatabaseHelper db;

    LopController(DatabaseHelper db) {
        this.db = db;
    }

    private Lop mapRow(ResultSet rs) throws SQLException {
        return new Lop(rs.getString("MALOP"), rs.getString("TENLOP"), rs.getString("MAKHOA"));
    }

    @GetMapping
    public List<Lop> getAll() {
        return db.queryList("SELECT MALOP, TENLOP, MAKHOA FROM LOP", this::mapRow);
    }

    @GetMapping("/{id}")
    public ResponseEntity<Lop> getById(@PathVariable String id) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null) return ResponseEntity.badRequest().build();
        Lop item = db.queryOne("SELECT MALOP, TENLOP, MAKHOA FROM LOP WHERE MALOP = ?", this::mapRow, cleanId);
        return item != null ? ResponseEntity.ok(item) : ResponseEntity.notFound().build();
    }

    @PostMapping
    public ResponseEntity<Lop> create(@RequestBody Lop body) {
        if (body == null) return ResponseEntity.badRequest().build();
        String maLop = InputSanitizer.trimOrNull(body.getMaLop());
        String tenLop = InputSanitizer.trimOrNull(body.getTenLop());
        String maKhoa = InputSanitizer.trimOrNull(body.getMaKhoa());
        if (maLop == null) return ResponseEntity.badRequest().build();

        body.setMaLop(maLop);
        body.setTenLop(tenLop);
        body.setMaKhoa(maKhoa);
        db.executeUpdate("INSERT INTO LOP (MALOP, TENLOP, MAKHOA) VALUES (?, ?, ?)", maLop, tenLop, maKhoa);
        return ResponseEntity.status(HttpStatus.CREATED).body(body);
    }

    @PutMapping("/{id}")
    public ResponseEntity<Lop> update(@PathVariable String id, @RequestBody Lop body) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null || body == null) return ResponseEntity.badRequest().build();
        String tenLop = InputSanitizer.trimOrNull(body.getTenLop());
        String maKhoa = InputSanitizer.trimOrNull(body.getMaKhoa());

        int updated = db.executeUpdate("UPDATE LOP SET TENLOP = ?, MAKHOA = ? WHERE MALOP = ?", tenLop, maKhoa, cleanId);
        if (updated == 0) return ResponseEntity.notFound().build();
        body.setMaLop(cleanId);
        body.setTenLop(tenLop);
        body.setMaKhoa(maKhoa);
        return ResponseEntity.ok(body);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable String id) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null) return ResponseEntity.badRequest().build();
        int deleted = db.executeUpdate("DELETE FROM LOP WHERE MALOP = ?", cleanId);
        return deleted > 0 ? ResponseEntity.noContent().build() : ResponseEntity.notFound().build();
    }
}

@RestController
@RequestMapping("/api/mon")
class MonController {
    private final DatabaseHelper db;

    MonController(DatabaseHelper db) {
        this.db = db;
    }

    private Mon mapRow(ResultSet rs) throws SQLException {
        double credits = rs.getDouble("SOTINCHI");
        Double soTinChi = rs.wasNull() ? null : credits;
        return new Mon(rs.getString("MAMON"), rs.getString("TENMON"), soTinChi);
    }

    @GetMapping
    public List<Mon> getAll() {
        return db.queryList("SELECT MAMON, TENMON, SOTINCHI FROM MON", this::mapRow);
    }

    @GetMapping("/{id}")
    public ResponseEntity<Mon> getById(@PathVariable String id) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null) return ResponseEntity.badRequest().build();
        Mon item = db.queryOne("SELECT MAMON, TENMON, SOTINCHI FROM MON WHERE MAMON = ?", this::mapRow, cleanId);
        return item != null ? ResponseEntity.ok(item) : ResponseEntity.notFound().build();
    }

    @PostMapping
    public ResponseEntity<Mon> create(@RequestBody Mon body) {
        if (body == null) return ResponseEntity.badRequest().build();
        String maMon = InputSanitizer.trimOrNull(body.getMaMon());
        String tenMon = InputSanitizer.trimOrNull(body.getTenMon());
        if (maMon == null) return ResponseEntity.badRequest().build();

        body.setMaMon(maMon);
        body.setTenMon(tenMon);
        db.executeUpdate("INSERT INTO MON (MAMON, TENMON, SOTINCHI) VALUES (?, ?, ?)", maMon, tenMon, body.getSoTinChi());
        return ResponseEntity.status(HttpStatus.CREATED).body(body);
    }

    @PutMapping("/{id}")
    public ResponseEntity<Mon> update(@PathVariable String id, @RequestBody Mon body) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null || body == null) return ResponseEntity.badRequest().build();
        String tenMon = InputSanitizer.trimOrNull(body.getTenMon());

        int updated = db.executeUpdate("UPDATE MON SET TENMON = ?, SOTINCHI = ? WHERE MAMON = ?", tenMon, body.getSoTinChi(), cleanId);
        if (updated == 0) return ResponseEntity.notFound().build();
        body.setMaMon(cleanId);
        body.setTenMon(tenMon);
        return ResponseEntity.ok(body);
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable String id) {
        String cleanId = InputSanitizer.trimOrNull(id);
        if (cleanId == null) return ResponseEntity.badRequest().build();
        int deleted = db.executeUpdate("DELETE FROM MON WHERE MAMON = ?", cleanId);
        return deleted > 0 ? ResponseEntity.noContent().build() : ResponseEntity.notFound().build();
    }
}

@RestController
@RequestMapping("/api/hssv")
class HssvController {
    private final DatabaseHelper db;

    HssvController(DatabaseHelper db) {
        this.db = db;
    }

    private Hssv mapRow(ResultSet rs) throws SQLException {
        boolean gender = rs.getBoolean("GIOITINH");
        Boolean gioiTinh = rs.wasNull() ? null : gender;
        return new Hssv(
                rs.getString("MSV"),
                rs.getString("HODEM"),
                rs.getString("TEN"),
                gioiTinh,
                rs.getTimestamp("NGAYSINH"),
                rs.getString("MANGANH"),
                rs.getString("MALOP")
        );
    }

    @GetMapping
    public List<Hssv> getAll() {
        return db.queryList(
                "SELECT MSV, HODEM, TEN, GIOITINH, NGAYSINH, MANGANH, MALOP FROM HSSV",
                this::mapRow
        );
    }

    @GetMapping("/{msv}")
    public ResponseEntity<Hssv> getById(@PathVariable String msv) {
        String cleanMsv = InputSanitizer.trimOrNull(msv);
        if (cleanMsv == null) return ResponseEntity.badRequest().build();
        Hssv item = db.queryOne(
                "SELECT MSV, HODEM, TEN, GIOITINH, NGAYSINH, MANGANH, MALOP FROM HSSV WHERE MSV = ?",
                this::mapRow,
                cleanMsv
        );
        return item != null ? ResponseEntity.ok(item) : ResponseEntity.notFound().build();
    }

    @PostMapping
    public ResponseEntity<Hssv> create(@RequestBody Hssv body) {
        if (body == null) return ResponseEntity.badRequest().build();
        String msv = InputSanitizer.trimOrNull(body.getMsv());
        String hoDem = InputSanitizer.trimOrNull(body.getHoDem());
        String ten = InputSanitizer.trimOrNull(body.getTen());
        String maNganh = InputSanitizer.trimOrNull(body.getMaNganh());
        String maLop = InputSanitizer.trimOrNull(body.getMaLop());
        if (msv == null) return ResponseEntity.badRequest().build();

        body.setMsv(msv);
        body.setHoDem(hoDem);
        body.setTen(ten);
        body.setMaNganh(maNganh);
        body.setMaLop(maLop);

        boolean genderVal = body.getGioiTinh() != null && body.getGioiTinh();
        db.executeUpdate(
                "INSERT INTO HSSV (MSV, HODEM, TEN, GIOITINH, NGAYSINH, MANGANH, MALOP) VALUES (?, ?, ?, ?, ?, ?, ?)",
                msv,
                hoDem,
                ten,
                genderVal,
                body.getNgaySinh(),
                maNganh,
                maLop
        );
        return ResponseEntity.status(HttpStatus.CREATED).body(body);
    }

    @PutMapping("/{msv}")
    public ResponseEntity<Hssv> update(@PathVariable String msv, @RequestBody Hssv body) {
        String cleanMsv = InputSanitizer.trimOrNull(msv);
        if (cleanMsv == null || body == null) return ResponseEntity.badRequest().build();

        String hoDem = InputSanitizer.trimOrNull(body.getHoDem());
        String ten = InputSanitizer.trimOrNull(body.getTen());
        String maNganh = InputSanitizer.trimOrNull(body.getMaNganh());
        String maLop = InputSanitizer.trimOrNull(body.getMaLop());
        boolean genderVal = body.getGioiTinh() != null && body.getGioiTinh();

        int updated = db.executeUpdate(
                "UPDATE HSSV SET HODEM = ?, TEN = ?, GIOITINH = ?, NGAYSINH = ?, MANGANH = ?, MALOP = ? WHERE MSV = ?",
                hoDem,
                ten,
                genderVal,
                body.getNgaySinh(),
                maNganh,
                maLop,
                cleanMsv
        );
        if (updated == 0) return ResponseEntity.notFound().build();

        body.setMsv(cleanMsv);
        body.setHoDem(hoDem);
        body.setTen(ten);
        body.setMaNganh(maNganh);
        body.setMaLop(maLop);
        return ResponseEntity.ok(body);
    }

    @DeleteMapping("/{msv}")
    public ResponseEntity<Void> delete(@PathVariable String msv) {
        String cleanMsv = InputSanitizer.trimOrNull(msv);
        if (cleanMsv == null) return ResponseEntity.badRequest().build();
        int deleted = db.executeUpdate("DELETE FROM HSSV WHERE MSV = ?", cleanMsv);
        return deleted > 0 ? ResponseEntity.noContent().build() : ResponseEntity.notFound().build();
    }
}

@RestController
@RequestMapping("/api/diem")
class DiemController {
    private final DatabaseHelper db;

    DiemController(DatabaseHelper db) {
        this.db = db;
    }

    private Diem mapRow(ResultSet rs) throws SQLException {
        double d1 = rs.getDouble("DIEML1");
        Double diemL1 = rs.wasNull() ? null : d1;
        double d2 = rs.getDouble("DIEML2");
        Double diemL2 = rs.wasNull() ? null : d2;
        return new Diem(
                rs.getString("MSV"),
                rs.getString("MAMON"),
                diemL1,
                diemL2,
                rs.getString("GHICHU")
        );
    }

    @GetMapping
    public List<Diem> getAll() {
        return db.queryList("SELECT MSV, MAMON, DIEML1, DIEML2, GHICHU FROM DIEM", this::mapRow);
    }

    @GetMapping("/student/{msv}")
    public List<Diem> getByStudent(@PathVariable String msv) {
        String cleanMsv = InputSanitizer.trimOrNull(msv);
        return db.queryList(
                "SELECT MSV, MAMON, DIEML1, DIEML2, GHICHU FROM DIEM WHERE MSV = ?",
                this::mapRow,
                cleanMsv
        );
    }

    @GetMapping("/item")
    public ResponseEntity<Diem> getByKey(@RequestParam String msv, @RequestParam String mamon) {
        String cleanMsv = InputSanitizer.trimOrNull(msv);
        String cleanMaMon = InputSanitizer.trimOrNull(mamon);
        if (cleanMsv == null || cleanMaMon == null) return ResponseEntity.badRequest().build();

        Diem item = db.queryOne(
                "SELECT MSV, MAMON, DIEML1, DIEML2, GHICHU FROM DIEM WHERE MSV = ? AND MAMON = ?",
                this::mapRow,
                cleanMsv,
                cleanMaMon
        );
        return item != null ? ResponseEntity.ok(item) : ResponseEntity.notFound().build();
    }

    @PostMapping
    public ResponseEntity<Diem> create(@RequestBody Diem body) {
        if (body == null) return ResponseEntity.badRequest().build();
        String msv = InputSanitizer.trimOrNull(body.getMsv());
        String maMon = InputSanitizer.trimOrNull(body.getMaMon());
        String ghiChu = InputSanitizer.trimOrNull(body.getGhiChu());
        if (msv == null || maMon == null) return ResponseEntity.badRequest().build();

        body.setMsv(msv);
        body.setMaMon(maMon);
        body.setGhiChu(ghiChu);

        db.executeUpdate(
                "INSERT INTO DIEM (MSV, MAMON, DIEML1, DIEML2, GHICHU) VALUES (?, ?, ?, ?, ?)",
                msv,
                maMon,
                body.getDiemL1(),
                body.getDiemL2(),
                ghiChu
        );
        return ResponseEntity.status(HttpStatus.CREATED).body(body);
    }

    @PutMapping("/item")
    public ResponseEntity<Diem> update(@RequestParam String msv, @RequestParam String mamon, @RequestBody Diem body) {
        String cleanMsv = InputSanitizer.trimOrNull(msv);
        String cleanMaMon = InputSanitizer.trimOrNull(mamon);
        if (cleanMsv == null || cleanMaMon == null || body == null) return ResponseEntity.badRequest().build();

        String ghiChu = InputSanitizer.trimOrNull(body.getGhiChu());
        int updated = db.executeUpdate(
                "UPDATE DIEM SET DIEML1 = ?, DIEML2 = ?, GHICHU = ? WHERE MSV = ? AND MAMON = ?",
                body.getDiemL1(),
                body.getDiemL2(),
                ghiChu,
                cleanMsv,
                cleanMaMon
        );
        if (updated == 0) return ResponseEntity.notFound().build();

        body.setMsv(cleanMsv);
        body.setMaMon(cleanMaMon);
        body.setGhiChu(ghiChu);
        return ResponseEntity.ok(body);
    }

    @DeleteMapping("/item")
    public ResponseEntity<Void> delete(@RequestParam String msv, @RequestParam String mamon) {
        String cleanMsv = InputSanitizer.trimOrNull(msv);
        String cleanMaMon = InputSanitizer.trimOrNull(mamon);
        if (cleanMsv == null || cleanMaMon == null) return ResponseEntity.badRequest().build();

        int deleted = db.executeUpdate("DELETE FROM DIEM WHERE MSV = ? AND MAMON = ?", cleanMsv, cleanMaMon);
        return deleted > 0 ? ResponseEntity.noContent().build() : ResponseEntity.notFound().build();
    }
}
