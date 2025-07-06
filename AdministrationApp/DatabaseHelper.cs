using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace AdministrationApp
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Ошибка при выполнении SQL-запроса", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Общая ошибка при работе с базой данных", ex);
            }

            return dataTable;
        }

        public int ExecuteNonQuery(string commandText, SqlParameter[] parameters = null)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(commandText, connection);

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Ошибка при выполнении SQL-команды", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Общая ошибка при работе с базой данных", ex);
            }

            return rowsAffected;
        }

        public object ExecuteScalar(string commandText, SqlParameter[] parameters = null)
        {
            object result = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(commandText, connection);

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    result = command.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Ошибка при выполнении SQL-скалярного запроса", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Общая ошибка при работе с базой данных", ex);
            }

            return result;
        }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
