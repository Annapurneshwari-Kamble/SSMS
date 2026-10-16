using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SSMS.Masters.DL
{
    public class DL
    {
        private readonly string connStr =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // SELECT → DataTable
        public DataTable ExecuteDataTable(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        // INSERT / UPDATE / DELETE → rows affected
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // SCALAR → object
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                con.Open();
                return cmd.ExecuteScalar();
            }
        }

        // Helper for parameters
        public SqlParameter Param(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }
    }
}