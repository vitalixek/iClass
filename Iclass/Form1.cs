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
    public partial class Form1 : Form
    {
        public Form2 f = new Form2();
        public Form3 f1 = new Form3();
        public COM com = new COM();
        DataTable dtClasses = new DataTable();
        DataTable dtAtrributes = new DataTable();
        Table table;
        TabPage tabpage1;
        DataGridView DGV;
        List<string> classifier = new List<string>();
        List<DataTable> datatable = new List<DataTable>();
        Visualizer artist = new Visualizer();

        public string page_name;
        string filePath, filePath1;
        int value = 0;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        public DSHierarchyWrapper hierarchy_ = new DSHierarchyWrapper();
        //List<DSClassWrapper> classes_ = new List<DSClassWrapper>();
        //List<DSAttributeWrapper> attributes_ = new List<DSAttributeWrapper>();
        //Image drawing;
        public Form1()
        {
            InitializeComponent();
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {

             ///
            /*attributes_ = hierarchy_.getAttributes();
            classes_ = hierarchy_.getClasses();*/
            ///
            
            table = new Table(com, dataGridView1,tabControl1);
            tabControl1.TabPages.Remove(tabControl1.TabPages["tabPage1"]);

            button1.Enabled = true;
            table.ReadAttributeTable(dtAtrributes);
            dtClasses = table.ReadClassTable();
            ReadClassifierTable();
            
        }


        public void ReadClassifierTable()
        {

            
            for (int i = com.DictionaryAttributes() + com.DictionaryClasses(); i < com.DictionaryElements(); i++)
            {
                classifier.Add(com.GetNumber(i));
            }

            List<DSClassifierWrapper> classifiers = hierarchy_.getClassifiers();

            foreach (string cl in classifier)
            {

                DataTable dt22 = new DataTable();
                DataColumn column;
                DataRow row;

                DSClassifierWrapper classifier_ = classifiers[classifier.IndexOf(cl)];

                dt22.TableName = "dat"+classifier.IndexOf(cl);

                column = new DataColumn();
                column.ColumnName = "Класс";
                dt22.Columns.Add(column);


                column = new DataColumn();
                column.ColumnName = "Степень принадлежности";
                dt22.Columns.Add(column);

                tabpage1 = new TabPage();
                tabpage1.SuspendLayout();
                tabpage1.Name = cl;
                tabControl1.TabPages.Add(tabpage1);
                tabControl1.SelectedTab = tabpage1;
                tabpage1.Text = cl.ToString();

                DGV = new DataGridView();
                DGV.Name = "DataGridV" + classifier.IndexOf(cl);
                DGV.BackgroundColor = Color.White;
                DGV.Parent = tabpage1;
                DGV.Location = new Point(7, 7);
                DGV.Size = new Size(299, 162);
                DGV.BorderStyle = BorderStyle.FixedSingle;
                DGV.Visible = true;
                DGV.AllowUserToAddRows = false;
                DGV.ReadOnly = true;

                

                foreach (DSClassWrapper cls in classifier_.getClasses())
                {
                    foreach(DataRow dr in dtClasses.Rows)
                    {
                        if((cls.getID() + " " + cls.getName()) == dr["Класс"].ToString())
                        {
                            row = dt22.NewRow();
                            row["Класс"] = dr["Класс"];
                            dt22.Rows.Add(row);
                        }

                    }
                }

                DGV.DataSource = dt22;
                datatable.Add(dt22);
                tabControl1.TabPages[cl].Controls.Add(DGV);

                tabpage1.ResumeLayout();
                tabpage1.Refresh();
            } 
        }


        public void Classify()
        {

            
            com.Send(dtAtrributes);
            com.Execute();

            for (int m = 0; m < tabControl1.TabCount; m++)
            {
                datatable[m] = com.Receive(datatable[m]);
                tabControl1.SelectedTab = tabControl1.TabPages[m];
                tabpage1 = tabControl1.TabPages[m];
                if (tabControl1.SelectedTab != null) // Добавление значений в каждый DataGridView!
                {
                    Control[] matches = tabControl1.SelectedTab.Controls.Find("DataGridV"+m.ToString(), true);
                    if (matches.Length > 0 && matches[0] is DataGridView)
                    {
                        DataGridView dgv = (DataGridView)matches[0];
                        dgv.DataSource = datatable[m];
                    }
                }
            }
            
        }

        public void ClassifyForManyElements()
        {
            com.Send(dtAtrributes);
            com.Execute();

            for (int m = 0; m < tabControl1.TabCount; m++)
            {
                datatable[m] = com.Receive(datatable[m]);
                tabControl1.SelectedTab = tabControl1.TabPages[m];
                page_name = tabControl1.TabPages[m].ToString();
            }
            //...(Создать таблицу для хранения степеней прнадлжности)
            //...(Загрузить в нее данные классификации)
        }


        private bool Dragging;
        private int xPos;
        private int yPos;
        private void pictureBox1_MouseUp_1(object sender, MouseEventArgs e) { Dragging = false; }

        private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Dragging = true;
                xPos = e.X;
                yPos = e.Y;

            }
        }
        private void pictureBox1_MouseMove_1(object sender, MouseEventArgs e)
        {
            Control c = sender as Control;
            if (Dragging && c != null)
            {
                c.Top = e.Y + c.Top - yPos;
                c.Left = e.X + c.Left - xPos;
            }
        }
        private void picBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                if (trackBar1.Value - 10 >= trackBar1.Minimum)
                    trackBar1.Value -= 10;
            }
            else
            {
                if (trackBar1.Value + 10 <= trackBar1.Maximum)
                    trackBar1.Value += 10;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (trackBar1.Value >= 0)
            {
                if (trackBar1.Value > value)
                {
                    zoom(5 * (trackBar1.Value - value));
                }
                else zoom(-5 * (value - trackBar1.Value));
                value = trackBar1.Value;
            }
            else if (trackBar1.Value < 0)
            {
                if (trackBar1.Value > value)
                {
                    zoom(5 * Math.Abs(trackBar1.Value - value));
                }
                else zoom(-5 * Math.Abs(value - trackBar1.Value));
                value = trackBar1.Value;
            }
        }
        private void zoom(int score)
        {
            pictureBox1.Height += score;
            pictureBox1.Width += score * 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            com.NumberOfObjects(1);
            Classify();
        }

        private void загрузитьДанныетипFscToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                filePath = openFileDialog1.FileName;
                filePath1 = filePath.Remove(filePath.Length - 3) + "fsc";
                com.Open(filePath1);
                hierarchy_.clear();
                File.OpenRead(filePath1);
                if (hierarchy_.load(filePath))
                {
                    //visualize();
                    artist.visualize(hierarchy_, panel1, pictureBox1, trackBar1);
                }
                Form1_Load(sender, e);
                данныеToolStripMenuItem.Enabled = true;
            }
            else MessageBox.Show("Ошибка открытия файла.");
        }

        public void вToolStripMenuItem_Click(object sender, EventArgs e)
        {
            f.ShowDialog();
        }

        private void загрузитьДанныеИзБДToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    f.cnn.Open();
            //    f1.cnn1 = f.cnn;
            //    f1.ShowDialog(); 
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return;
            //}
            //Считывания кол-ва объектов для классификации
            //com.NumberOfObjects(f1.dt.Rows.Count);
            table.ClassifyFromDataBase(dtAtrributes, datatable, f.cnn);
            //com.NumberOfObjects(1);
            //foreach(DataRow row in f1.dt.Rows)
            //{
            //    for (int i =1; i<f1.dt.Columns.Count; i++)
            //    {
            //        dtAtrributes.Rows[i - 1][1] = row[i].ToString();
            //    }
            //    ClassifyForManyElements();
            //}
        }

        private void создатьСкриптДляКлассификацииБДToolStripMenuItem_Click(object sender, EventArgs e)
        {
            table.CreateDataBaseScript(dtAtrributes, datatable);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            com.Store();
        }
    }
}
