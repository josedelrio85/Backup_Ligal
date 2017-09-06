using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace WindowsFormsApplication1.Model
{
    class DAL
    {
        public static DataSet GetData(string sqlcmdString, string connString)
        {
            SqlConnection con = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sqlcmdString, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            con.Open();
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }



        public static DataSet GetDataMySQL(string sqlcmdString, string connString)
        {
            MySqlConnection con = new MySqlConnection(connString);
            MySqlCommand cmd = new MySqlCommand(sqlcmdString, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);

            con.Open();
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
    }
}
