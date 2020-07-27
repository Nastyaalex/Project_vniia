﻿using PickBoxTest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace project_vniia
{
    public partial class Form_System : Form
    {
        DataSet ds = new DataSet();
        public Dictionary<string, Form1.MyDB> myDBs;
        DataGridView dataGrid = new DataGridView();
        DataTable dt = new DataTable();

        List<Object> rows_Type_obj = new List<Object>();
        private PickBox pb = new PickBox();

        public static string System_ways;
        public static bool flag = true;
        public int t2;

        public Form_System()
        {
            InitializeComponent();
            dataGrid.Location = new Point(20, 180);
            dataGrid.Size = new Size(300, 200);
            dataGrid.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
            dataGrid.Visible = false;
            dataGrid.Name = "datagr";
            this.Controls.Add(dataGrid);
            dataGrid.DataError += DataGrid_DataError;
            
            dataGridViewLeft.DataError += DataGridViewLeft_DataError; ;
            dataGridViewRight.DataError += DataGridViewRight_DataError; ;

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].Name == "checkBox1" || Controls[i].Name == "dataGridViewLeft" || Controls[i].Name == "dataGridViewRight" || Controls[i].Name == "datagr")
                {
                    Control c = Controls[i];
                    pb.WireControl(c);
                }
            }
            
            //System_ways= Ways_to_txt(System_ways);

            string way = "\\build_systems.txt";
            bool pusto = Class_ways.Pusto_(way);
            bool flag_sysh = Class_ways.Log_pusto(way, pusto);
            if (!flag_sysh)
            {
                do
                {
                    Form1.F2[6] = Ways_to_txt(Form1.F2[6]);
                    if (Form_way_system.close_all1)
                    {
                        this.Close();
                        break;
                    }
                } while (Form1.F2[6] == null || Form1.F2[6] == "");
            }
            if (!Form_way_system.close_all1)
            {
                Zap(way, Form1.F2[6], flag_sysh);
            }

            // del old->!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }

        private void DataGridViewRight_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void DataGridViewLeft_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

       
        private void DataGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void Form_System_Load(object sender, EventArgs e)
        {
            Otrchet_BD(myDBs);
           
            dt.Clear();
            dt.Columns.Add("Номер системы");
            dt.Columns.Add("Тип системы");
            dt.Columns.Add("Тип БД");
        }
        public void Otrchet_BD(Dictionary<string, Form1.MyDB> myDBs)
        {
            var table1 = myDBs["[Блоки]"].table.Copy();

            var rows_to_delete = new List<DataRow>();


            var rows = table1.Rows;
            int t = 0, t1 = 0;
            int kolvo = 0;

            if (table1.Columns.Contains("Тип БД"))
            {
                t = table1.Columns["Тип БД"].Ordinal;
            }
            if (table1.Columns.Contains("Местоположение"))
            {
                t1 = table1.Columns["Местоположение"].Ordinal;
            }
            if (table1.Columns.Contains("Отметка выполнения"))
            {
                t2 = table1.Columns["Отметка выполнения"].Ordinal;
            }

            DataTable table = new DataTable();

            foreach (DataRow r in rows)
            {
                if (!rows_Type_obj.Contains(r[t]))
                {
                    kolvo++;
                    rows_Type_obj.Add(r[t]);

                    table = table1.Clone();
                    table.TableName = r[t].ToString();
                    table.BeginLoadData();
                    ds.Tables.AddRange(new DataTable[] { table.Copy() });
                }
            }

            foreach (DataRow r in rows)
            {
                bool f = false;

                object text = r[t];

                var ttt = r[t2].ToString();
                var tr = r[t1].ToString();

                int value;
                int.TryParse(string.Join("", tr.Where(c => char.IsDigit(c))), out value);

                if ((value != 56 && value != 561) || (!ttt.Contains("Г") && !ttt.Contains("г")) || ttt.Contains("в") || ttt.Contains("В") || ttt.Contains("_сохранено в файле") || tr.Contains("сдан") || tr.Contains("Сдан"))
                {
                    f = true;
                }

                if (f)
                {
                    rows_to_delete.Add(r);
                }
                Console.WriteLine();
            }

            foreach (var r in rows_to_delete)
            {
                rows.Remove(r);
            }

            rows_to_delete.Clear();

            Razsortirovka(table1, ds, t, rows_Type_obj);//проверить на правильность

            foreach (var obj in rows_Type_obj)
            {
                comboBox1.Items.Add(obj);
                comboBox2.Items.Add(obj);
            }

            comboBox1.SelectedItem = comboBox1.Items[0];
            //for (int i = 1; i < 9; i++)
            //{
            //    comboBox2.Items.Add(i);
            //    comboBox3.Items.Add(i);
            //}
            comboBox2.SelectedItem = comboBox2.Items[0];
            //comboBox3.SelectedItem = comboBox3.Items[0];
        }
        public void Razsortirovka(DataTable table, DataSet ds, int t, List<Object> rows_Type_obj)
        {
            var rows = table.Rows;
            foreach (DataRow r in rows)
            {
                var rt = r[t].ToString();
                int k = 0;
                foreach (var p in rows_Type_obj)
                {
                    if (p.ToString().Contains(rt))
                    {
                        ds.Tables[k].LoadDataRow(r.ItemArray, true);
                        break;
                    }
                    k++;
                }  
            }
        }

        public void Sobr_system()
        {
            string comb = comboBox1.Text;
            string comb1 = comboBox2.Text;
            List<string> myStr = new List<string>();
            List<string> myStr_1 = new List<string>();

            for (int i = dataGridViewLeft.RowCount - 1; i >= 0; i--)
            {
                DataGridViewRow row = dataGridViewLeft.Rows[i];
                if (Convert.ToBoolean(row.Cells["Выбрать"].Value))
                {
                    myStr.Add(row.Cells["Номер БД"].Value.ToString());
                    row.Cells["Выбрать"].Value = false;

                    bool arrl = false;
                    foreach (DataRow r in dt.Rows)
                    {
                        var arr = r.ItemArray;
                        arrl = arr.Contains(row.Cells["Номер БД"].Value.ToString());
                            if (arrl)
                            {
                                MessageBox.Show("Выбранный блок входит в другую систему! "+ row.Cells["Номер БД"].Value.ToString());
                            return;
                            }
                        
                    }
                }
            }
            
            for (int i = dataGridViewRight.RowCount - 1; i >= 0; i--)
            {
                DataGridViewRow row = dataGridViewRight.Rows[i];
                if (Convert.ToBoolean(row.Cells["Выбрать"].Value))
                {
                    myStr_1.Add(row.Cells["Номер БД"].Value.ToString());
                    row.Cells["Выбрать"].Value = false;

                    bool arrl = false;
                    foreach (DataRow r in dt.Rows)
                    {
                        var arr = r.ItemArray;
                        arrl = arr.Contains(row.Cells["Номер БД"].Value.ToString());
                        if (arrl)
                        {
                            MessageBox.Show("Выбранный блок входит в другую систему! " + row.Cells["Номер БД"].Value.ToString());
                            return;
                        }

                    }
                }
            }
            ///////////////


            //dt.Columns["Тип БД"].DefaultValue = comb + ","+comb1;
            if (myStr.Count + myStr_1.Count + 3 > dt.Columns.Count)
            {
                for (int i = dt.Columns.Count-2; i < myStr.Count + myStr_1.Count + 1; i++)
                {
                    dt.Columns.Add("Блок_" + i);
                }
            }
            int k = 0, jjj=0;
            try
            {
               DataRow _ravi = dt.NewRow();
                string[] l = new string[myStr.Count];
                l = myStr.ToArray();
                
                string[] l_1 = new string[myStr_1.Count];
                l_1 = myStr_1.ToArray();

                char nim1 = (l.Length == 0)&& (l_1.Length == 0) ? 'a' :
                                       (l.Length == 0) ? 'b' :
                                       (l_1.Length == 0) ? 'c' :
                                      'd';
                switch (nim1)
                {
                    case 'a':
                        MessageBox.Show("Выберите блоки!");
                        break;
                    case 'b':
                        _ravi["Тип БД"] = comb1;
                        break;
                    case 'c':
                        _ravi["Тип БД"] = comb;
                        break;
                    case 'd':
                        _ravi["Тип БД"] = comb + "/" + comb1;
                        break;
                }

               for (int j = 1; j < myStr.Count + 1; j++)
               {
                  _ravi["Блок_" + j] = l[k];
                  k++;
                    jjj = j;
               }
                k = 0;
                for (int j = jjj+1; j < myStr_1.Count + 1+jjj; j++)
                {
                    _ravi["Блок_" + j] = l_1[k];
                    k++;
                }
                dt.Rows.Add(_ravi);
            }
            catch (Exception p)
            {
                MessageBox.Show(p.Message);
            }

            dataGrid.DataSource = dt;
            dataGrid.Visible = true;

        }

        public static string Ways_to_txt(string path)
        {
            if (!Directory.Exists(path))
            {
                Form_way_system f = new Form_way_system();
                f.ShowDialog();
                if (Form_way_system.close_all1 == true)
                {
                    return null;
                }
                path = Form_way_system.textbox1_;
                if(path==null || path=="")
                {
                    flag = false;
                }
            }
            return path;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Sobr_system();
        }

        public void Prov_way(string _ways, DataRow row)
        {
            if (!Directory.Exists(System_ways))
                Directory.CreateDirectory(System_ways);
            if (!File.Exists(_ways))
            {
                File.Create(_ways).Close();
            }
            
            try
            {
                using (StreamWriter sr = new StreamWriter(_ways))
                {
                  sr.WriteLine(String.Join(",", row.ItemArray));   
                }
            }
            catch (Exception k)
            {
                // Let the user know what went wrong.
                MessageBox.Show("The file could not be read:");
                Console.WriteLine(k.Message);
            }
        }

        public void Zap(string way, string F, bool flag_sysh)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!flag_sysh)
            {
                using (StreamWriter sw = new StreamWriter(path + "\\TestWay" + way))
                {
                  sw.WriteLine("{0}", F);
                }
             
            }
            System_ways = F;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string way_ = "";
            DataTable data = (DataTable)dataGrid.DataSource;
            var rows = data.Rows;
            foreach (DataRow row in rows)
            {
                if (row[0].ToString() == "")
                {
                    way_ = System_ways + "\\" + row[3].ToString() + ".txt";
                }
                else
                {
                    way_ = System_ways + "\\" + row[0].ToString() + ".txt";
                }
                Prov_way(way_, row);
            }
            List<DataRow> kkk = new List<DataRow>();
            try
            {
                var rows_ = myDBs["[Блоки]"].table.Rows;
                
                foreach (DataRow row_ in rows)
                {
                    for (int i = 3; i < row_.ItemArray.Length; i++)
                    {
                        foreach (DataRow r in rows_)
                        {
                            if (r.ItemArray[0].ToString() == row_.ItemArray[i].ToString())
                            {
                                var ttt = r[t2].ToString();
                                if (ttt.Contains("_сохранено в файле"))
                                {
                                    MessageBox.Show("Блок уже сохранён в другой системе!");
                                    return;
                                }
                                else
                                    ttt = ttt + "_сохранено в файле";
                                r[t2] = ttt;
                            }
                        }
                        //nameds = row_[2].ToString();
                        string[] nameds = row_[2].ToString().Split('/');
                        foreach (var name_ in nameds)
                        {
                            foreach (DataRow r in ds.Tables[name_].Rows)
                            {
                                if (r.ItemArray[0].ToString() == row_.ItemArray[i].ToString())
                                {
                                    kkk.Add(r);
                                }
                            }
                            
                        }
                    }

                }
                foreach (var r in kkk)
                {
                    ds.Tables[r[1].ToString()].Rows.Remove(r);
                }
                kkk.Clear();
            }
            catch (Exception p)
            { MessageBox.Show("Возможно вы не нажали на кнопку 'Собрать' или не открыли файл перед добавлением в таблицу."); }
            dt.Clear();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox1.Text == null)
            {
                MessageBox.Show("Некорректный номер файла!");
            }
            else
            {
                string way_ = System_ways + "\\" + textBox1.Text + ".txt";
                try
                {
                    Stream fs = new FileStream(way_, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Номер системы");
                    dt.Columns.Add("Тип системы");
                    dt.Columns.Add("Тип БД");
                    string s = sr.ReadLine();
                    string[] parts = s.Split(',');
                    for (int i = 1; i < parts.Length - 2; i++)
                    {
                        dt.Columns.Add("Блок_" + i);
                    }
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < parts.Length; i++)
                        row[i] = parts[i];
                    dt.Rows.Add(row);

                    while (sr.Peek() != -1)
                    {
                        s = sr.ReadLine();
                        parts = s.Split(',');
                        DataRow row1 = dt.NewRow();
                        for (int i = 0; i < parts.Length; i++)
                            row1[i] = parts[i];
                        dt.Rows.Add(row1);
                    }
                    dataGrid.DataSource = dt;
                    dataGrid.Visible = true;
                }
                catch(Exception p)
                { MessageBox.Show("Файл не найден!"); }

                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Введите сначала количество столбцов (N) затем количество строк (N).\n\t" +
                "Номер для открытия сохранённого файла -> это номер Блока_1 в первой строке.\n\t После добавления в таблицу-> требуется сохранить таблицы иначе в Access не измениться.");
        }

        public void V_Table()
        {
            try
            {
                DataTable data = (DataTable)dataGrid.DataSource;
                var rows = data.Rows;
                var rows_ = myDBs["[Блоки]"].table.Rows;

                foreach (DataRow row_ in rows)
                {
                    var row_new = myDBs["[Системы в сборе]"].table.NewRow();
                    row_new["Номер системы"] = row_.ItemArray[0];
                    row_new["Тип системы"] = row_.ItemArray[1];
                    for (int i = 1; i < row_.ItemArray.Length - 1; i++)
                    {
                        row_new["Блок" + i] = row_.ItemArray[i + 1];
                        foreach (DataRow r in rows_)
                        {
                            if (r.ItemArray[0] == row_.ItemArray[i + 1])
                            {
                                var ttt = r[t2].ToString();
                                string rr;
                                char ch = ttt.Contains("Г") ? 'a' :
                                    ttt.Contains("г") ? 'b' :
                                    'c';
                                switch(ch)
                                {
                                    case 'a':
                                        rr = r[t2].ToString();
                                        rr = rr.Replace("Г", "в");
                                        r[t2] = rr;
                                        break;
                                    case 'b':
                                        rr = r[t2].ToString();
                                        rr = rr.Replace("г", "в");
                                        r[t2] = rr;
                                        break;
                                    case 'c':
                                        break;
                                }
                                    
                                
                            }
                        }
                    }
                    myDBs["[Системы в сборе]"].table.Rows.Add(row_new);
                }

               
                
            }
            catch (Exception p)
            { MessageBox.Show("Возможно вы не нажали на кнопку 'Собрать' или не открыли файл перед добавлением в таблицу."); }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            V_Table();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string comb = comboBox1.Text;
                if (!dataGridViewLeft.Columns.Contains("Выбрать"))
                {
                    DataGridViewCheckBoxColumn dataColumn = new DataGridViewCheckBoxColumn();
                    dataColumn.Name = "Выбрать";
                    dataColumn.DefaultCellStyle = null;
                    dataGridViewLeft.Columns.Add(dataColumn);
                }
                dataGridViewLeft.DataSource = ds.Tables[comb].DefaultView;

            }
            catch (Exception p)
            { MessageBox.Show(p.ToString()); }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string comb = comboBox2.Text;
                if (!dataGridViewRight.Columns.Contains("Выбрать"))
                {
                    DataGridViewCheckBoxColumn dataColumn = new DataGridViewCheckBoxColumn();
                    dataColumn.Name = "Выбрать";
                    dataColumn.DefaultCellStyle = null;
                    dataGridViewRight.Columns.Add(dataColumn);
                }
                dataGridViewRight.DataSource = ds.Tables[comb].DefaultView;

            }
            catch (Exception p)
            { MessageBox.Show(p.ToString()); }

        }

        private void dataGridViewLeft_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (Controls[i].Name == "dataGridViewLeft" || Controls[i].Name == "dataGridViewRight" || Controls[i].Name == "datagr")
                    {
                        Control c = Controls[i];
                        pb.WireControl1(c);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (Controls[i].Name == "dataGridViewLeft" || Controls[i].Name == "dataGridViewRight" || Controls[i].Name == "datagr")
                    {
                        Control c = Controls[i];
                        pb.WireControl(c);
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string filename;
            openFileDialog1.InitialDirectory = System_ways;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = openFileDialog1.SafeFileName;
                if(filename.Contains(".txt"))
                {
                    filename = filename.Substring(0,filename.IndexOf('.'));
                }
                textBox1.Text = filename;
                button3.PerformClick();
            }
        }
    }
}
// не обновляет таблицу  блоки после добавления сборки в табл 