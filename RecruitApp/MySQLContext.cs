using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Forms;

namespace PersonalApp
{
    class MySQLContext
    {
        public DataTable GetQuery(String sql)
        {
            DataTable dt = new DataTable();

            MySqlConnectionStringBuilder mysqlCSB;
            mysqlCSB = new MySqlConnectionStringBuilder();
            mysqlCSB.Server = "localhost";
            mysqlCSB.Database = "personal_db";
            mysqlCSB.UserID = "root";
            mysqlCSB.Password = "";
            mysqlCSB.SslMode = MySqlSslMode.None;
            mysqlCSB.CharacterSet = "utf8";
            mysqlCSB.ConvertZeroDateTime = true;

            using (MySqlConnection con = new MySqlConnection())
            {
                con.ConnectionString = mysqlCSB.ConnectionString;

                MySqlCommand com = new MySqlCommand(sql, con);

                try
                {
                    con.Open();

                    using (MySqlDataReader dr = com.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                    }
                    con.Close();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "Перевірте підключення до бази даних.");
                }
            }
            return dt;
        }
    }
}
