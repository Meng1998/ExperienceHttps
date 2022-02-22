using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExperienceHttps.C.PG
{
    public class Pgsql
    {
        private String ConnStr;
        private NpgsqlConnection SqlConn;
        public Pgsql() {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory()) // <== compile failing here
               .AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();
            ConnStr = configuration["pgSql"];
            SqlConn = new NpgsqlConnection(ConnStr);
        }
        public DataSet PgExecute(String sqlstr) {
            DataSet ds = new DataSet();
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqlstr, SqlConn))
                {
                    sqldap.Fill(ds);
                }
                return ds;
            }
            catch (System.Exception ex)
            {
                SqlConn.Close();
                return ds;
            }
        }
    }
}
