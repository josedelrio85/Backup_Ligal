using System;
using System.Configuration;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private String cn1 = ConfigurationManager.ConnectionStrings["WindowsFormsApplication1.Properties.Settings.LigalConnectionString"].ConnectionString;
        //private String cn2 = ConfigurationManager.ConnectionStrings["WindowsFormsApplication1.Properties.Settings.Ligal_backupConnectionString"].ConnectionString;
        private string connString;
        private string whereTabla = "";
        private string schema = "";

        private DataSet ds;
        private DataSet ds2;

        public Form1()
        {
            InitializeComponent();

            textBox1.Multiline = true;
            textBox1.ScrollBars = ScrollBars.Vertical;

            button2.Text = String.Format("Volcado (Backup)");
            button4.Text = String.Format("Creación índices");

            button2.Enabled = false;
            button4.Enabled = false;

            button1.Text = String.Format("Copiar");
            button3.Text = String.Format("Ejecutar en BD");

            button5.Text = String.Format("Aceptar");

            comboBox1.Items.Insert(0, "System.Data.SqlClient");
            comboBox1.Items.Insert(1, "MySql.Data.MySqlClient");

            checkBox4.Checked = true;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME  like 'gen_%' order by TABLE_NAME asc";
            string sql = "";

            string resultado = "";
            string tabla = "";
            string sql2 = "";

            textBox1.Text = "";
          
            if (comboBox1.SelectedIndex == 1)
            {
                //mysql
                sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = '" + schema + "' AND " + whereTabla + " order by TABLE_NAME asc";
                ds = Model.DAL.GetDataMySQL(sql, cn1);
            }
            else if (comboBox1.SelectedIndex == 0)
            {
                sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where " + whereTabla + " order by TABLE_NAME asc";
                ds = Model.DAL.GetData(sql, cn1);
            }

            

            foreach (DataTable table in ds.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    tabla = row["TABLE_NAME"].ToString();
                    string resultado2 = "";

                    if (comboBox1.SelectedIndex == 1)
                    {
                        //mysql
                        sql2 = "SELECT COLUMN_NAME as Name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '" + schema + "' AND TABLE_NAME = '" + tabla + "'";
                        ds2 = Model.DAL.GetDataMySQL(sql2, cn1);

                        foreach (DataTable table2 in ds2.Tables)
                        {
                            foreach (DataRow row2 in table2.Rows)
                            {
                                //if (row2["NAME"].ToString() != "ID")
                                if (row2[0].ToString() != "ID")
                                {
                                    resultado2 += row2["NAME"].ToString() + ", ";
                                }

                            }
                        }

                    }
                    else if (comboBox1.SelectedIndex == 0)
                    {
                        sql2 = "SELECT c.Name FROM sys.columns c JOIN sys.objects o ON o.object_id = c.object_id WHERE o.object_id = OBJECT_ID('" + tabla + "')";
                        ds2 = Model.DAL.GetData(sql2, cn1);

                        foreach (DataTable table2 in ds2.Tables)
                        {
                            foreach (DataRow row2 in table2.Rows)
                            {
                                if (row2["NAME"].ToString() != "ID")
                                {
                                    resultado2 += row2["NAME"].ToString() + ", ";
                                }

                            }
                        }
                    }

                    

                    resultado2 = resultado2.Remove(resultado2.Length - 2);

                    resultado = "INSERT INTO Ligal_backup.dbo."+tabla+" SELECT "+resultado2+ " FROM Ligal.dbo." + tabla +";";
                    textBox1.Text += "" + resultado.ToString() + Environment.NewLine;
                    textBox1.Text += Environment.NewLine;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sql = "";
            string resultado = "";
            string tabla = "";
            string sql2 = "";

            List<String> listaIndex = new List<String>();
            int cont = 0;

            textBox1.Text = "";

            if (comboBox1.SelectedIndex == 1)
            {
                sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = '' AND " + whereTabla + " order by TABLE_NAME asc";
                ds = Model.DAL.GetDataMySQL(sql, cn1);
            }
            else if (comboBox1.SelectedIndex == 0)
            {
                sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where " + whereTabla + " order by TABLE_NAME asc";
                ds = Model.DAL.GetData(sql, cn1);
            }

            foreach (DataTable table in ds.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    tabla = row["TABLE_NAME"].ToString();

                    if (comboBox1.SelectedIndex == 1)
                    {
                        //mysql
                        sql2 = "SELECT COLUMN_NAME as Name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '" + schema + "' AND TABLE_NAME = '" + tabla + "'";
                        ds2 = Model.DAL.GetDataMySQL(sql2, cn1);
                    }
                    else if (comboBox1.SelectedIndex == 0)
                    {
                        sql2 = "SELECT c.Name FROM sys.columns c JOIN sys.objects o ON o.object_id = c.object_id WHERE o.object_id = OBJECT_ID('" + tabla + "')";
                        ds2 = Model.DAL.GetData(sql2, cn1);
                    }


                    foreach (DataTable table2 in ds2.Tables)
                    {
                        foreach (DataRow row2 in table2.Rows)
                        {
                            if (row2["NAME"].ToString().StartsWith("ID"))
                            {
                                resultado = "CREATE INDEX IDX_INDEX_" + cont + " ON " + tabla + "(" + row2["NAME"] + ");";
                                listaIndex.Add(resultado);
                                cont++;

                            }else if (row2["NAME"].ToString().Equals("ROWID"))
                            {
                                resultado = "CREATE INDEX IDX_INDEX_" + cont + " ON " + tabla + "(" + row2["NAME"] + ");";
                                listaIndex.Add(resultado);
                                cont++;
                            }
                        }
                    }
                }
            }

            foreach(string sentencia in listaIndex)
            {
                textBox1.Text += "" + sentencia.ToString() + Environment.NewLine;
                textBox1.Text += Environment.NewLine;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string datasource = txtDataSource.Text.ToString();
            string database = txtDatabase.Text.ToString();
            string userid = txtUserId.Text.ToString();
            string password = txtPassword.Text.ToString();
            var provider = comboBox1.SelectedItem;

            schema = database;
            
            if (datasource == "" || database == "" || userid == "" || password == "" || provider.ToString() == "")
            {
                MessageBox.Show("Debe cubrir todos los parámetros.","Aviso");
            }
            else
            {
                button2.Enabled = true;
                button4.Enabled = true;

                if (comboBox1.SelectedIndex == 0)
                {
                    connString = "Data Source=" + datasource + ";Initial Catalog=" + database + ";Persist Security Info=True;User ID=" + userid + ";Password=" + password;
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    connString = "Data Source=" + datasource + ";Database=" + database + ";User ID=" + userid + ";Password=" + password;
                }

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                connectionStringsSection.ConnectionStrings["WindowsFormsApplication1.Properties.Settings.LigalConnectionString"].ConnectionString = connString;
                connectionStringsSection.ConnectionStrings["WindowsFormsApplication1.Properties.Settings.LigalConnectionString"].ProviderName = provider.ToString();

                config.Save();
                ConfigurationManager.RefreshSection("connectionStrings");

                Console.Out.WriteLine("ConnectionStrings " + connectionStringsSection.ConnectionStrings["WindowsFormsApplication1.Properties.Settings.LigalConnectionString"].ConnectionString);
                Console.Out.WriteLine("ProviderName " + connectionStringsSection.ConnectionStrings["WindowsFormsApplication1.Properties.Settings.LigalConnectionString"].ProviderName);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            iterateChecks();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            iterateChecks();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;

            if (checkBox4.Checked)
            {
                whereTabla = "1=1";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            iterateChecks();
        }

        private void iterateChecks()
        {
            string condicion = "";
            int cont = 0;
                            
            if (checkBox4.Checked) checkBox4.Checked = false;


            foreach (Control c in this.Controls)
            {
                if (c is CheckBox)
                {
                    CheckBox check = (CheckBox) c;
                    if (check.Checked)
                    {
                        switch (check.Name)
                        {
                            case "checkBox1":
                                if(cont > 0) condicion += " OR ";
                                condicion += "TABLE_NAME  like 'gen_%'";
                                cont++;
                                break;
                            case "checkBox2":
                                if (cont > 0) condicion += " OR ";
                                condicion += "TABLE_NAME  like 'adm_%'";
                                cont++;
                                break;
                            case "checkBox3":
                                if (cont > 0) condicion += " OR ";
                                condicion += "TABLE_NAME  like 'master_%'";
                                cont++;
                                break;
                        }
                    }
                }
            }

            whereTabla = condicion;

            if (cont == 0) whereTabla = "1=2";

            //Console.Out.WriteLine("whereTabla: " + whereTabla.ToString() + Environment.NewLine);
        }
    }
}
