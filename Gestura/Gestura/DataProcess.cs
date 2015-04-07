#region Library Files
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data.SqlTypes;
#endregion

namespace Gestura
{
    class DataProcess
    {
        #region Variables and Initialisation
        private SqlConnection sqlConn = null;
        private DataSet dataSet = null;
        private SqlDataAdapter sqlAdapter;
        private SqlCommand sqlCommand = null;
        private SqlDataReader sqlReader1 = null;
        #endregion

        #region Connect To Database
        private void databaseConnect()
        {
            //defining the connection string
            try
            {

                sqlConn = new SqlConnection();
                sqlConn.ConnectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=I:\HMM\HMM\Database1.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
                sqlConn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured" + ex.ToString()+ " \n Check for database connection");

            }
        }
        #endregion 

        #region Fill Datagrid
        public void fillDataGrid(DataGridView dataGrid,string tableName)
        {
            dataSet = new DataSet();
            databaseConnect();
            
            //filling the data in grid views 
            sqlAdapter = new SqlDataAdapter("Select * from "+tableName, sqlConn);
            sqlAdapter.Fill(dataSet, tableName);
            dataGrid.DataSource = dataSet.Tables[tableName];
            
            sqlConn.Close();
        }
        #endregion

        #region HMM Data Functions
        //initialising the arrays for HMM data for processing
        public void readHMMData(ref double[,] matrix,string tableName,int num)
        {
            sqlCommand = new SqlCommand();
            databaseConnect();
            int K, I, Columns;
            if (tableName.Equals("A"))
            {
                if (num == 1)
                    Columns = 2;
                else
                    Columns = num;
            }
            else 
                Columns = 8;
            if(num!=-1)
                tableName = tableName + num;
            string comm="select ";
            string temp="";
            for (int i = 1; i < Columns; i++) 
                temp += "S" + i.ToString() + ",";
            temp += "S" + Columns.ToString();

            comm += temp + " from " + tableName;
            sqlCommand.Connection = sqlConn;
            sqlCommand.CommandText = comm;
            sqlReader1 = sqlCommand.ExecuteReader();
            K = 0;
            while (sqlReader1.Read())
            {
                for (I = 0; I < Columns; I++)
                    matrix[K, I] = (double)sqlReader1.GetValue(I);
                K++;
            }

            sqlConn.Close();
        }

        public void deleteHMMData(string tableName)
        {
            databaseConnect();
            
            sqlCommand = new SqlCommand("delete"+tableName, sqlConn);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.ExecuteNonQuery();

            sqlConn.Close();
        }

        //updating values of the A, B, PI into the database
        public void updateHMMData(double[,] matrix, string tableName,int num)
        {
            dataSet = new DataSet();
            sqlCommand = new SqlCommand();
            int K, I,Columns;
            if (tableName.Equals("A"))
            {
                if (num == 1 || num == 2)
                    Columns = 2;
                else
                    Columns = num ;
            }
            else
                Columns = 8;
            tableName = tableName + num;
            string param, col;
            string temp1, temp2;
            temp1 = "@val";

            if (num == 1)
                num = 2;

            databaseConnect();
            sqlCommand.Connection = sqlConn;
            sqlAdapter = new SqlDataAdapter("Select * from "+tableName, sqlConn);
            sqlAdapter.Fill(dataSet, tableName);

            sqlAdapter.InsertCommand = new SqlCommand("insert"+tableName, sqlConn);
            sqlCommand = sqlAdapter.InsertCommand;
            sqlCommand.CommandType = CommandType.StoredProcedure;

            temp2 = "S";
            for (I = 1; I <= Columns; I++)
            {
                param = temp1 + I.ToString();
                col = temp2 + I.ToString();
                sqlCommand.Parameters.Add(new SqlParameter(param, SqlDbType.Float));
                sqlCommand.Parameters[param].SourceColumn = col;
            }

            for (I = 0; I < num; I++)
            {
                DataRow newRow = dataSet.Tables[tableName].NewRow();
                for (K = 0; K < Columns; K++)
                {
                    float val = (float)matrix[I, K];// (0.125);
                    val = (float)Math.Round(val, 3);
                    newRow[K] = val;
                }
                dataSet.Tables[tableName].Rows.Add(newRow);
                sqlAdapter.Update(dataSet, tableName);
            }
            sqlConn.Close();
        }
        #endregion
    }
}