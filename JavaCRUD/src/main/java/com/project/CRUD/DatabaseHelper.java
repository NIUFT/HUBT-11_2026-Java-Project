package com.project.CRUD;

import org.springframework.stereotype.Component;

import javax.sql.DataSource;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

@Component
public class DatabaseHelper {

    private final DataSource dataSource;

    public DatabaseHelper(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    @FunctionalInterface
    public interface RowMapper<T> {
        T map(ResultSet rs) throws SQLException;
    }

    public <T> List<T> queryList(String sql, RowMapper<T> mapper, Object... params) {
        List<T> list = new ArrayList<>();
        try (Connection conn = dataSource.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            bindParams(stmt, params);

            try (ResultSet rs = stmt.executeQuery()) {
                while (rs.next()) {
                    list.add(mapper.map(rs));
                }
            }
            return list;
        } catch (SQLException e) {
            throw new RuntimeException("SQL query error: " + e.getMessage(), e);
        }
    }

    public <T> T queryOne(String sql, RowMapper<T> mapper, Object... params) {
        List<T> list = queryList(sql, mapper, params);
        return list.isEmpty() ? null : list.get(0);
    }

    public int executeUpdate(String sql, Object... params) {
        try (Connection conn = dataSource.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            bindParams(stmt, params);
            return stmt.executeUpdate();
        } catch (SQLException e) {
            throw new RuntimeException("SQL write error: " + e.getMessage(), e);
        }
    }

    private void bindParams(PreparedStatement stmt, Object... params) throws SQLException {
        if (params == null) return;
        for (int i = 0; i < params.length; i++) {
            stmt.setObject(i + 1, params[i]);
        }
    }
}