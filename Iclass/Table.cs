using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using DSCoreWrapper;
using System.IO;
using System.Data.SqlClient;


namespace Iclass
{

    class Table : Form1
    {
        COM xyz;
        DataGridView datagridview1;
        TabControl tabcontrol;
        DataTable dt2 = new DataTable();

        public Table(COM xxx, DataGridView dgv1, TabControl tb)
        {
            xyz = xxx;
            datagridview1 = dgv1;
            tabcontrol = tb;
        }
        public void ReadAttributeTable(DataTable dt1)
        {
            int f = xyz.DictionaryAttributes();
            int r = xyz.DictionaryClasses();
            dt1.Columns.Add("Признак");
            dt1.Columns.Add("Значение");

            for (int i = 0; i < f; i++)
            {
                dt1.Rows.Add(xyz.GetNumber(i));
            }
            datagridview1.DataSource = dt1;
            datagridview1.AllowUserToAddRows = false;
            datagridview1.Columns[0].ReadOnly = true;
        }


        public DataTable ReadClassTable()
        {
            DataTable dt1 = new DataTable();
            int f = xyz.DictionaryAttributes();
            int r = xyz.DictionaryClasses();
            dt1.Columns.Add("Класс");
            dt1.Columns.Add("Степень принадлежности");

            for (int j = 0; j < r; j++)
            {
                dt1.Rows.Add(xyz.GetNumber(f + j));
            }
            return dt1;
        }
        
        /*public DataTable ReadDataBsae()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add();
            return dt;
        }*/

        public void CreateDataBase()
        {
            f.cnn.Open();
            
            SqlCommand cmd = new SqlCommand();
            string sql = "CREATE TABLE " + page_name;
            cmd = new SqlCommand(sql, f.cnn);

            cmd.ExecuteNonQuery();
            f.cnn.Close();
        }

        public void CreateDataBaseScript(DataTable dt, List<DataTable> datatables)
        {
            string sql = "create table CObject ( \n ";
            sql += "CObject_id int identity(1,1) primary key ";
            foreach (DataRow dr in dt.Rows)
            {
                sql += ", \n [" + dr[0] + "]  varchar(50)   null";
            }
            sql += "\n)\ngo";
            
            for (int i = 0; i < tabcontrol.TabPages.Count; i++)
            {
                sql += "\n \ncreate table [" + tabcontrol.TabPages[i].Name + "]  ( \n ";
                sql += "c" + i + "_id int identity(1,1) primary key, \n ";
                sql += "CObject_id int not null, \n";
                foreach (DataRow dr in datatables[i].Rows)
                {
                    sql += "[" + dr[0] + "] varchar(50) null, \n";
                }
                sql += " foreign key (CObject_id) references CObject(CObject_id)\n";
                sql += "on update cascade on delete cascade\n";
                sql += "\n)\ngo";
            }
            StreamWriter file = new StreamWriter(@"C:\Users\Виталий\Desktop\4 курс\Диплом\script.sql");
            file.WriteLine(sql);
            file.Close();

        }

        public void ClassifyFromDataBase(DataTable dt, List<DataTable> datatables, SqlConnection cnn)
        {
            SqlDataAdapter da_attributes = new SqlDataAdapter();
            List<SqlDataAdapter> da_classifiers = new List<SqlDataAdapter>();
            List<string> names = new List<string>();
            da_attributes.SelectCommand = new SqlCommand("Select * from CObject", cnn);
            for (int i = 0; i < datatables.Count; i++)
            {
                names.Add(tabcontrol.TabPages[i].Name);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand("Select * from [" + names[i] + "]", cnn);
                string fields = "";
                foreach (DataRow dr in datatables[i].Rows)
                {
                    fields += "[" + dr[0] + "], ";
                }
                fields = fields.Remove(fields.Length - 2);
                string insertCommand = "Insert into [" + names[i] + "] (CObject_id, " + fields + ") values (@CObject_id, ";
                foreach (DataRow dr in datatables[i].Rows)
                {
                    insertCommand += "@Cla" + datatables[i].Rows.IndexOf(dr).ToString() + ", ";
                }
                insertCommand = insertCommand.Remove(insertCommand.Length - 2);
                insertCommand += ")";
                da.InsertCommand = new SqlCommand(insertCommand, cnn);
                da.InsertCommand.Parameters.Add("@CObject_id", SqlDbType.Int, 16, "CObject_id");
                foreach (DataRow dr in datatables[i].Rows)
                {
                    da.InsertCommand.Parameters.Add("@Cla" + datatables[i].Rows.IndexOf(dr).ToString(), SqlDbType.VarChar, 50, "[" + dr[0] + "]");
                }





                string updateCommand = "Update [" + names[i] + "] set ";
                foreach (DataRow dr in datatables[i].Rows)
                {
                    fields = "[" + dr[0] + "]=@Cla" + datatables[i].Rows.IndexOf(dr).ToString() + ", ";
                    updateCommand += fields;
                }
                updateCommand += "CObject_id=@CObject_id ";
                updateCommand += "where c" + i+"_id=@c"+i+"_id";
                da.UpdateCommand = new SqlCommand(updateCommand, cnn);
                da.UpdateCommand.Parameters.Add("@c"+i+"_id", SqlDbType.Int, 16, "c" + i + "_id");
                da.UpdateCommand.Parameters.Add("@CObject_id", SqlDbType.Int, 16, "CObject_id");
                foreach (DataRow dr in datatables[i].Rows)
                {
                    da.UpdateCommand.Parameters.Add("@Cla" + datatables[i].Rows.IndexOf(dr).ToString(), SqlDbType.VarChar, 50, "[" + dr[0] + "]");
                }







                da_classifiers.Add(da);
            }
            DataSet ds = new DataSet();
            da_attributes.Fill(ds, "CObject");
            foreach (SqlDataAdapter da in da_classifiers)
            {
                da.Fill(ds, names[da_classifiers.IndexOf(da)]);
            }

            xyz.NumberOfObjects(1);

            DataTable dta = new DataTable();
            dta.Columns.Add("Признак");
            dta.Columns.Add("Значение");

            foreach (DataRow row in ds.Tables["CObject"].Rows)
            {   
                for (int i = 1; i < ds.Tables["CObject"].Columns.Count; i++)
                {
                    DataRow dr = dta.NewRow();
                    dr[0] = ds.Tables["CObject"].Columns[i].ColumnName;
                    dr[1] = row[i];
                    dta.Rows.Add(dr);
                }
                xyz.Send(dta);
                xyz.Execute();

                for (int m = 0; m < tabControl1.TabCount; m++)
                {
                    datatables[m] = xyz.Receive(datatables[m]);
                    DataRow dr = ds.Tables[names[m]].NewRow();
                    dr[1] = row[0];
                    for (int k = 0; k < datatables[m].Rows.Count; k++)
                    {
                        dr[k + 2] = datatables[m].Rows[k][1].ToString();
                    }
                    ds.Tables[names[m]].Rows.Add(dr);
                   da_classifiers[m].Update(ds.Tables[names[m]]);
                }
            }
        }
    }
}
