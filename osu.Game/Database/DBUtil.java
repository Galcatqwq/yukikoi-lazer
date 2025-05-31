package com.cqeec.util;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

/**
 * 数据库工具类
 */
public class DBUtil {
    /*定义驱动类的路径*/
    static String driverClass = "com.mysql.cj.jdbc.Driver";
    /*数据库账号*/
    static String user = "root";
    /*数据库账号密码*/
    static String password = "";
    /**
     * 数据库连接配置字符串
     * 协议://数据库服务器地址:数据库服务器端口/数据库名字
     * 数据库服务器地址:数据库服务器端口  (如果使用的本地服务器并且端口使用的默认的3306，则可省略)
     * jdbc:mysql://127.0.0.1:3306/test
     * jdbc:mysql:///test
     */
    static String url = "jdbc:mysql://127.0.0.1:3306/test";
    static Connection connection = null;

    /**
     * 数据库连接对象 Connection
     * 尽量使用唯一的连接对象(使用单例模式控制)
     */
    public static Connection getCon() {
        if (connection == null) {
            try {
                /*加载驱动类*/
                Class.forName(driverClass);
                /*获取数据库连接对象*/
                connection = DriverManager.getConnection(
                        url,
                        user,
                        password
                );
            } catch (ClassNotFoundException e) {
                throw new RuntimeException(e);
            } catch (SQLException e) {
                throw new RuntimeException(e);
            }
        }
        return connection;
    }
}
