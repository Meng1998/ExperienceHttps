using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ExperienceMiddleware.C.PGsql
{
    class PGoperation
    {
        static string ConStr = "";
        NpgsqlConnection SqlConn = null;
        public PGoperation()
        {
            var builder = new ConfigurationBuilder()
                                         .SetBasePath(Directory.GetCurrentDirectory()) // <== compile failing here
                                         .AddJsonFile("appsettings.json");
            //AppContext.BaseDirectory + "appsettings.json";
            IConfiguration configuration = builder.Build();
            ConStr = configuration["pgSql"];
            //SqlConn = new NpgsqlConnection(ConStr);

        }

        public DataSet ExecuteQuery(string sqrstr)
        {
            DataSet ds = new DataSet();
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqrstr, SqlConn))
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

        public DataTable ExecuteQueryData(string sqrstr)
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(ConStr);
            DataTable ds = new DataTable();
            try
            {
                using (NpgsqlDataAdapter sqldap = new NpgsqlDataAdapter(sqrstr, sqlConn))
                {
                    sqldap.Fill(ds);
                }
                return ds;
            }
            catch (System.Exception ex)
            {
                // throw ex;
                return ds;
            }

        }

        public int ExecuteNonQuery(string sqrstr)
        {
            try
            {
                SqlConn.Open();
                using (NpgsqlCommand SqlCommand = new NpgsqlCommand(sqrstr, SqlConn))
                {
                    int r = SqlCommand.ExecuteNonQuery();  //执行查询并返回受影响的行数
                    SqlConn.Close();
                    return r; //r如果是>0操作成功！ 
                }
            }
            catch (System.Exception ex)
            {
                SqlConn.Close();
                return 0;
            }

        }
    }
}
