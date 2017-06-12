using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Iclass
{
    public partial class Form3 : Form
    {
        //public SqlCommand cmd = new SqlCommand();
        public SqlConnection cnn1 = new SqlConnection();
        public DataTable dt = new DataTable();
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();

            da.SelectCommand = new SqlCommand();
            da.SelectCommand.Connection = cnn1;
            da.SelectCommand.CommandText = "Select * from " + textBox1.Text;
            da.Fill(ds, textBox1.Text);
            dt = ds.Tables[0];
            cnn1.Close();
            this.Close();
        }
    }
}
