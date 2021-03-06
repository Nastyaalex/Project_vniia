﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Data.OleDb;
using PickBoxTest;
using System.Drawing;
using System.IO;
using project_vniia.Properties;

namespace project_vniia
{
    public partial class Form1 : Form
    {

        DataSet ds = new DataSet();

        public int i;
        bool flag_filtr = false;
        public static string[] cmdText = new string[13] { "SELECT * FROM [CANNote] ORDER BY Номер_КАН ASC",
        "SELECT * FROM [БлокиМетро]","SELECT * FROM [Замечания по БД]","SELECT * FROM [КАН]",
        "SELECT * FROM [КАНы]","SELECT * FROM [ОперацииМетро]","SELECT * FROM [Проверка]",
            "SELECT * FROM [Проверка ФЭУ]","SELECT * FROM [ПроверкаТСРМ61]","SELECT * FROM [Работы по БД]",
        "SELECT * FROM [Системы в сборе]","SELECT * FROM [Термокалибровка] ORDER BY Номер_БД ASC",
        "SELECT * FROM Блоки ORDER BY [Номер БД] ASC"};// если понадобиться порядок по определённому столбцу

        public static string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "change_2_rows.txt");
        public static string filePath_calibr = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "calibration_check.txt");
        Button button_filtr = new Button();

        //
        // Create an instance of the PickBox class
        //
        private PickBox pb = new PickBox();

        public static string conString;// = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\nasty\\Desktop\\_TCPM82_New.mdb";

        public static string[] F2 = new string[4];

        public static string Log_ways;
        public static string Log_ways_peremesti;
        public static string Zamech_ways;
        public static string Zamech_ways_peremesti;

        public static string[] _ways_=new string[4] {"\\log_ways.txt", "\\log_peremesti.txt", "\\zamech_ways.txt", "\\zamech_peremesti.txt" };
        
        public Form1()
        {
            InitializeComponent();
            //this.KeyPreview = true;

            for (int t = 4; this.Controls[t] != this.Controls[6]; t++)
            {
                Control c = this.Controls[t];
                pb.WireControl(c);
            }

            dataGridView1.DataError += new DataGridViewDataErrorEventHandler(DataGridView1_DataError);
            dataGridView2.DataError += new DataGridViewDataErrorEventHandler(DataGridView2_DataError);
            dataGridView2.RowPrePaint += DataGridView2_RowPrePaint;
            dataGridView1.RowPrePaint += DataGridView1_RowPrePaint;
            textBox1.KeyUp += TextBox1_KeyUp;
            
            //резервное копирование
            //File.Copy(openFileDialog1.FileName, "C:\\Users\\APM\\Desktop\\2.mdb", true);

        }
        
        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            button_filtr_Click();
        }


        private void DataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int index = e.RowIndex;
            string indexStr = (index + 1).ToString();
            object header = this.dataGridView1.Rows[index].HeaderCell.Value;
            if (header == null || !header.Equals(indexStr))
                this.dataGridView1.Rows[index].HeaderCell.Value = indexStr;
        }

        private void DataGridView2_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int index = e.RowIndex;
            string indexStr = (index + 1).ToString();
            object header = this.dataGridView2.Rows[index].HeaderCell.Value;
            if (header == null || !header.Equals(indexStr))
                this.dataGridView2.Rows[index].HeaderCell.Value = indexStr;
        }


        private void DataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //MessageBox.Show("Ошибка");
            e.ThrowException = false;
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            do
            {
                conString = Class_zagruz.Try_(conString, openFileDialog1);
            }
            while (conString == null);

            bool[] flag_sysh = new bool[4];
            
            for (int i = 0; i < 4; i++)
            {
                bool pusto = Class_ways.Pusto_(_ways_[i]);
                flag_sysh[i] = Class_ways.Log_pusto(_ways_[i], pusto);
            }
            int k_tr=0;
            
            foreach (bool f in flag_sysh)
            {
                if (f) { k_tr++; }
            }
            try
            {
                if (k_tr != 4 && k_tr < 4)
                {
                    do
                    {
                        F2 = Class_ways.Forma2_();
                        if (Form2.close_all)
                        {
                            Environment.Exit(0);
                        }
                    } while (Array.Exists(F2, element => element == "") || Array.Exists(F2, element => element == null));
                }
            }
            catch (Exception p)
            {
                Console.WriteLine(p.Message);
            }

            Class_ways.Zap_(_ways_, F2, k_tr);
            
            //
            MyDB myDB = new MyDB();

            Class_zagruz.Combobox_(conString, comboBox1, ds, myDB, myDBs);

            dataGridView1.DataSource = myDBs["[Блоки]"].table.DefaultView;
            dataGridView1.Columns["Номер БД"].ReadOnly = true;

            Datagrid_columns_delete_blocks();
            Datagrid_columns_delete();


            //ready and work
            Calibr calibr = new Calibr();
            calibr.Main_calibr(this);

            Zamech_BD zamech_BD = new Zamech_BD();
            zamech_BD.Main_Zamech_BD(this);

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        string[] stolbez = new string[5] { "Номер блока", "Номер КАН", "Номер БД", "Номер изделия", "Номер системы"};

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView2.DataSource = myDBs["[" + comboBox1.Text + "]"].table.DefaultView;
            
            for (i = 0; i < 5; i++)
            {
                if (dataGridView2.Columns.Contains(stolbez[i]))
                {
                    dataGridView2.Columns[stolbez[i]].ReadOnly = true;
                    break;
                }
            }
            Datagrid_columns_delete();
            if (flag_filtr)
                button_filtr_Click();

        }
        public void Datagrid_columns_delete()
        {
            if (dataGridView2.Columns.Contains("s_ColLineage") == true)
                dataGridView2.Columns.Remove("s_ColLineage");
            if (dataGridView2.Columns.Contains("s_Generation") == true)
                dataGridView2.Columns.Remove("s_Generation");
            if (dataGridView2.Columns.Contains("s_GUID") == true)
                dataGridView2.Columns.Remove("s_GUID");
            if (dataGridView2.Columns.Contains("s_Lineage") == true)
                dataGridView2.Columns.Remove("s_Lineage");

        }
        public void Datagrid_columns_delete_blocks()
        {
            if (dataGridView1.Columns.Contains("s_ColLineage") == true)
                dataGridView1.Columns.Remove("s_ColLineage");
            if (dataGridView1.Columns.Contains("s_Generation") == true)
                dataGridView1.Columns.Remove("s_Generation");
            if (dataGridView1.Columns.Contains("s_GUID") == true)
                dataGridView1.Columns.Remove("s_GUID");
            if (dataGridView1.Columns.Contains("s_Lineage") == true)
                dataGridView1.Columns.Remove("s_Lineage");
        }

        
        public Add_Blocks CreateForm()
        {
            // Проверяем существование формы
            foreach (Form frm in Application.OpenForms)
                if (frm is Add_Blocks)
                {
                    frm.Activate();
                    return frm as Add_Blocks;
                }
            // Создаем новую форму
           
            Add_Blocks add_ = new Add_Blocks();
            add_.blocks_T = myDBs["[Блоки]"].table;
            add_.zamech_T = myDBs["[Замечания по БД]"].table;
            add_.peregr = this.but_peregruzka;
            add_.Show();
            return add_;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
       
        public void filtr()
        {
            if (textBox1.Text == null)
                dataGridView1.DataSource = myDBs["[Блоки]"].table;
            else
            {
                var table1 = myDBs["[Блоки]"].table;
                int k = 0; bool s_ = false;
                if (table1.Columns.Contains("s_ColLineage") == true)
                    k++; //table1.Columns.Remove("s_ColLineage");
                if (table1.Columns.Contains("s_Generation") == true)
                    k++; // table1.Columns.Remove("s_Generation");
                if (table1.Columns.Contains("s_GUID") == true)
                    k++; // table1.Columns.Remove("s_GUID");
                if (table1.Columns.Contains("s_Lineage") == true)
                    k++;// table1.Columns.Remove("s_Lineage");
                var table2 = table1.Copy();
                if (k != 0)
                    s_ = true;

                //переписать t1 -> t2 С учетом фильтра

                var rows_to_delete = new List<DataRow>();

                var rows = table2.Rows;
                foreach (DataRow r in rows)
                {
                    bool f = true;
                    int kolvo = r.ItemArray.Length;
                    k = 1;
                    foreach (var c in r.ItemArray)
                    {
                        if (s_)
                        {
                            if ((k < kolvo) && (k < kolvo - 1) && (k < kolvo - 2) && (k < kolvo - 3))
                            {
                                if (c.ToString().Contains(textBox1.Text))
                                {
                                    f = false;
                                }
                            }
                            else { break; }
                        }
                        else
                        {
                            if (c.ToString().Contains(textBox1.Text))
                            {
                                f = false;
                            }
                        }
                        k++;
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
                
                dataGridView1.DataSource = table2;
                Datagrid_columns_delete_blocks();
              
            }
        }
        private void button_filtr_Click()
        {
            filtr(); // for 1 table
            if (textBox1.Text == null)
                dataGridView2.DataSource = myDBs["[" + comboBox1.Text + "]"].table;
            else
            {
                var table1 = myDBs["[" + comboBox1.Text + "]"].table;
                int k = 0;bool s_ = false;
                if (table1.Columns.Contains("s_ColLineage") == true)
                    k++; //table1.Columns.Remove("s_ColLineage");
                if (table1.Columns.Contains("s_Generation") == true)
                    k++; // table1.Columns.Remove("s_Generation");
                if (table1.Columns.Contains("s_GUID") == true)
                    k++; // table1.Columns.Remove("s_GUID");
                if (table1.Columns.Contains("s_Lineage") == true)
                    k++;// table1.Columns.Remove("s_Lineage");
                var table2 = table1.Copy();
                if (k != 0)
                    s_ = true; 
                //переписать t1 -> t2 С учетом фильтра

                var rows_to_delete = new List<DataRow>();

                var rows = table2.Rows;
                foreach (DataRow r in rows)
                {
                    bool f = true;
                    int kolvo= r.ItemArray.Length;
                    k = 1;
                    foreach (var c in r.ItemArray)
                    {
                        if (s_)
                        {
                            if ((k < kolvo) && (k < kolvo - 1) && (k < kolvo - 2) && (k < kolvo - 3))
                            {
                                if (c.ToString().Contains(textBox1.Text))
                                {
                                    f = false;
                                }
                            }
                            else { break; }
                        }
                        else
                        {
                            //Console.Write (c.ToString() + " "); // для проверки
                            if (c.ToString().Contains(textBox1.Text))
                            {
                                f = false;
                            }
                        }
                        k++;
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

                dataGridView2.DataSource = table2;
                Datagrid_columns_delete();
                flag_filtr = true;
                
            }
        }
        
        private void добавитьБлокиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateForm();
           
        }

        public class MyDB
        {
            public DataSet ds;
            public OleDbDataAdapter adapter;
            public DataTable table;
        }

        private Dictionary<string, MyDB> myDBs = new Dictionary<string, MyDB>();

        private void but_peregruzka_Click(object sender, EventArgs e)
        {//работает- для замены строк
            dataGridView1.DataSource = myDBs["[Блоки]"].table.DefaultView;
            Datagrid_columns_delete_blocks();
        }
        
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        public Form_cod CreateForm_zamena()
        {
            // Проверяем существование формы
            foreach (Form frm in Application.OpenForms)
                if (frm is Form_cod)
                {
                    frm.Activate();
                    return frm as Form_cod;
                }
            // Создаем новую форму
            Form_cod cod = new Form_cod();
            cod.dataTables[0] = myDBs["[Блоки]"].table;
            i = 1;
            foreach (string str in comboBox1.Items)
            {
                cod.dataTables[i] = myDBs["[" + str + "]"].table;
                i++;
            }
            cod.Show();

            return cod;
        }
        
        private void поменятьСтрокиМестамиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateForm_zamena();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //сохранение для таблицы(авто)

            ds = new DataSet();
            myDBs["[Блоки]"].adapter.Fill(ds);
            myDBs["[Блоки]"].ds = ds;

            Class_Save_blocks.AnalizTable(myDBs["[Блоки]"].ds.Tables[0], myDBs["[Блоки]"].table, myDBs["[Блоки]"].adapter);

            foreach (string str in comboBox1.Items)
            {
                ds = new DataSet();
                myDBs["[" + str + "]"].adapter.Fill(ds);
                myDBs["[" + str + "]"].ds = ds;
            }

            #region SAVE
            Class_Save_cannote.AnalizTable(myDBs["[CANNote]"].ds.Tables[0], myDBs["[CANNote]"].table, myDBs["[CANNote]"].adapter);

            Class_Save_blockMetro.AnalizTable(myDBs["[БлокиМетро]"].ds.Tables[0], myDBs["[БлокиМетро]"].table, myDBs["[БлокиМетро]"].adapter);

            Class_Save_kan.AnalizTable(myDBs["[КАН]"].ds.Tables[0], myDBs["[КАН]"].table, myDBs["[КАН]"].adapter);

            Class_Save_kanS.AnalizTable(myDBs["[КАНы]"].ds.Tables[0], myDBs["[КАНы]"].table, myDBs["[КАНы]"].adapter);

            Class_Save_operMetro.AnalizTable(myDBs["[ОперацииМетро]"].ds.Tables[0], myDBs["[ОперацииМетро]"].table, myDBs["[ОперацииМетро]"].adapter);

            Class_Save_prov.AnalizTable(myDBs["[Проверка]"].ds.Tables[0], myDBs["[Проверка]"].table, myDBs["[Проверка]"].adapter);

            Class_Save_provFey.AnalizTable(myDBs["[Проверка ФЭУ]"].ds.Tables[0], myDBs["[Проверка ФЭУ]"].table, myDBs["[Проверка ФЭУ]"].adapter);

            Class_Save_provTCPM.AnalizTable(myDBs["[ПроверкаТСРМ61]"].ds.Tables[0], myDBs["[ПроверкаТСРМ61]"].table, myDBs["[ПроверкаТСРМ61]"].adapter);

            Class_Save_rabotBD.AnalizTable(myDBs["[Работы по БД]"].ds.Tables[0], myDBs["[Работы по БД]"].table, myDBs["[Работы по БД]"].adapter);

            Class_Save_systemVsbore.AnalizTable(myDBs["[Системы в сборе]"].ds.Tables[0], myDBs["[Системы в сборе]"].table, myDBs["[Системы в сборе]"].adapter);

            Class_Save_termocalibr.AnalizTable(myDBs["[Термокалибровка]"].ds.Tables[0], myDBs["[Термокалибровка]"].table, myDBs["[Термокалибровка]"].adapter);

            Class_Save_zamechPoBD.AnalizTable(myDBs["[Замечания по БД]"].ds.Tables[0], myDBs["[Замечания по БД]"].table, myDBs["[Замечания по БД]"].adapter);

            #endregion
        }
        public static bool zam = false;
        private void заменитьНомерБДToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zam = true;
            CreateForm_zamena();
            
        }
    }
    

    //замену изменить+
    /// калибровку закончила

    //сделать проверку на наличие при добавление новых блоков->сделала

    // создать блоки и проверить класс замечания по бд->+

}
