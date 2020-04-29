using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperNova
{
    public class dbOracle
    {


        //Oracle Multiple Procedure Methods
        public static DataTable oracle_read_data(OracleCommand cmd, string connection_string, string procedure_name, bool multiple = false)
        {
            if (cmd == null)
                throw new NullReferenceException("Oracle command should not be null");
            if (!multiple)
            {
                if (string.IsNullOrEmpty(connection_string))
                    throw new NullReferenceException("Connection string should not be null");
            }
            if (string.IsNullOrEmpty(procedure_name))
                throw new NullReferenceException("Procedure name should not be null");

            return read_data_procedure(cmd, connection_string, procedure_name, multiple);
        }

        public static string update_query(OracleCommand cmd, string connection_string, string procedure_name, bool multiple = false)
        {
            if (cmd == null)
                throw new NullReferenceException("Oracle command should not be null");
            if (!multiple)
            {
                if (string.IsNullOrEmpty(connection_string))
                    throw new NullReferenceException("Connection string should not be null");
            }

            if (string.IsNullOrEmpty(procedure_name))
                throw new NullReferenceException("Procedure name should not be null");

            return update_query_procedure(cmd, connection_string, procedure_name, multiple);
        }


        //Core Libraries
        private static DataTable read_data_procedure(OracleCommand cmd, string connection_string, string procedure_name, bool multiple = false)
        {
            try
            {
                if (multiple)
                {
                    cmd.InitialLONGFetchSize = 1000;
                    cmd.CommandText = procedure_name;
                    cmd.CommandType = CommandType.StoredProcedure;
                    OracleDataAdapter adp = new OracleDataAdapter(cmd);
                    cmd.CommandTimeout = 0;
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    cmd.Dispose();
                    return dt;
                }
                else
                {
                    string db_connection_string = ConfigurationManager.ConnectionStrings[connection_string].ConnectionString;
                    OracleConnection con = new OracleConnection(db_connection_string);
                    con.Open();
                    // create command and execute the stored procedure
                    cmd.Connection = con;
                    cmd.InitialLONGFetchSize = 1000;
                    cmd.CommandText = procedure_name;
                    cmd.CommandType = CommandType.StoredProcedure;
                    OracleDataAdapter adp = new OracleDataAdapter(cmd);
                    cmd.CommandTimeout = 0;
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    cmd.Dispose();
                    con.Close();
                    return dt;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        private static string update_query_procedure(OracleCommand cmd, string connection_string, string procedure_name, bool multiple = false)
        {

            try
            {
                if (multiple)
                {
                    cmd.CommandText = procedure_name;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    // create parameters for the anonymous pl/sql block

                    OracleParameter p_line = new OracleParameter("", OracleDbType.Varchar2, 32000, "", ParameterDirection.Output);
                    OracleParameter p_status = new OracleParameter("", OracleDbType.Decimal, ParameterDirection.Output);

                    // anonymous pl/sql block to get the line of text
                    string anonymous_block = "begin dbms_output.get_line(:1, :2); end;";

                    // set command text and parameters to get the text output
                    // and execute the anonymous pl/sql block

                    cmd.CommandText = anonymous_block;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(p_line);
                    cmd.Parameters.Add(p_status);
                    cmd.ExecuteNonQuery();

                    // write location, return status value, and the text to the console window

                    //Console.WriteLine("In method GetSingleLine...");
                    //Console.WriteLine("Return status: {0}", p_status.Value.ToString());
                    //Console.WriteLine("Return text: {0}", p_line.Value.ToString());
                    //Console.WriteLine();
                    //Console.WriteLine();

                    // clean up
                    p_line.Dispose();
                    p_status.Dispose();
                    cmd.Dispose();


                    if (!string.IsNullOrEmpty(p_line.Value.ToString()))
                        return p_line.Value.ToString();
                    else
                        return null;
                }
                else
                {
                    string db_connection_string = ConfigurationManager.ConnectionStrings[connection_string].ConnectionString;
                    OracleConnection con = new OracleConnection(db_connection_string);


                    con.Open();
                    // create command and execute the stored procedure
                    cmd.Connection = con;

                    cmd.CommandText = procedure_name;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                    // create parameters for the anonymous pl/sql block

                    OracleParameter p_line = new OracleParameter("", OracleDbType.Varchar2, 32000, "", ParameterDirection.Output);
                    OracleParameter p_status = new OracleParameter("", OracleDbType.Decimal, ParameterDirection.Output);

                    // anonymous pl/sql block to get the line of text
                    string anonymous_block = "begin dbms_output.get_line(:1, :2); end;";

                    // set command text and parameters to get the text output
                    // and execute the anonymous pl/sql block

                    cmd.CommandText = anonymous_block;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(p_line);
                    cmd.Parameters.Add(p_status);
                    cmd.ExecuteNonQuery();

                    // write location, return status value, and the text to the console window

                    //Console.WriteLine("In method GetSingleLine...");
                    //Console.WriteLine("Return status: {0}", p_status.Value.ToString());
                    //Console.WriteLine("Return text: {0}", p_line.Value.ToString());
                    //Console.WriteLine();
                    //Console.WriteLine();

                    // clean up
                    p_line.Dispose();
                    p_status.Dispose();
                    cmd.Dispose();
                    con.Close();

                    if (!string.IsNullOrEmpty(p_line.Value.ToString()))
                        return p_line.Value.ToString();
                    else
                        return null;
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }


        }


    }
}
