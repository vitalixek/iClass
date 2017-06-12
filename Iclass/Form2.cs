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
    public partial class Form2 : Form
    {
        public SqlConnection cnn = new SqlConnection();
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //@"Data Source=" + textBox1.Text + ";Initial Catalog=" + textBox2.Text + ";Integrated Security=True"
            //@"Data Source=vitalik\vitalik;Initial Catalog=iClass;Integrated Security=True"
            try
            {
                cnn.ConnectionString = @"Data Source=" + textBox1.Text + ";Initial Catalog=" + textBox2.Text + ";Integrated Security=True";
                
                cnn.Open();
                MessageBox.Show("Соединение установленно");
                cnn.Close();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = "vitalik\vitalik";
            textBox2.Text = "iClass";
        }
    }
}
