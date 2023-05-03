using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Refacciones
{
    public partial class UserEntryForm : Form
    {
        private SqlConnection cnx;
        string label;
        MainForm mainForm;
        private int idProceso, idEquipo, idSeccion, idSubsistema;
        Panel leftPanel = new Panel();
        Panel rightPanel = new Panel();
        public UserEntryForm(SqlConnection cnx, string label, int idProceso, int idEquipo, MainForm mainForm)
        {
            InitializeComponent();
            this.cnx = cnx;
            this.label = label;
            this.idProceso = idProceso;
            this.idEquipo = idEquipo;
            this.mainForm = mainForm;
        }
        private void UserEntryForm_Load(object sender, EventArgs e)
        {
            var splitContainer = new SplitContainer();
            splitContainer.SplitterIncrement = 1;
            splitContainer.SplitterDistance = splitContainer.Width / 2 - splitContainer.SplitterWidth / 2;
            splitContainer.Dock = DockStyle.Fill;
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Width = this.Width / 2;
            rightPanel.Dock = DockStyle.Right;
            rightPanel.Width = this.Width / 2;

            splitContainer.Panel1.Controls.Add(leftPanel);
            splitContainer.Panel2.Controls.Add(rightPanel);

            this.Controls.Add(splitContainer);
            LeftPanelLoad();
            RightPanelLoad();
            // Subscribe to the Resize event of the form
            this.Resize += new EventHandler(UserEntryForm_Resize);
        }
        private void UserEntryForm_Resize(object sender, EventArgs e)
        {
            rightPanel.Width = this.Width / 2;
            leftPanel.Width = this.Width / 2;
        }
        private void RightPanelLoad()
        {
            var dataGridView = new DataGridView();
            dataGridView.Dock = DockStyle.Fill;
            rightPanel.Controls.Add(dataGridView);

            string query = "SELECT * FROM " + label;

            // Modify the query to filter results based on idProceso and/or idElemento
            if (idProceso != 0 && idSeccion == 0 && idSubsistema == 0)
            {
                query += " WHERE idProceso = " + idProceso;
            }
            else if (idProceso != 0 && idSeccion != 0 && idSubsistema == 0)
            {
                query += " WHERE idProceso = " + idProceso + " AND idSeccion = " + idSeccion;
            }
            else if (idProceso != 0 && idSeccion != 0 && idSubsistema != 0)
            {
                query += " WHERE idProceso = " + idProceso + " AND idSeccion = " + idSeccion + " AND idSubsistema = " + idSubsistema;
            }

            SqlDataAdapter adapter = new SqlDataAdapter(query, cnx);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView.DataSource = dataTable;
        }

        private void LeftPanelLoad()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + label, cnx);
            SqlDataReader reader = cmd.ExecuteReader();

            int top = 50;
            int left = 50;
            int textBoxLeft = 200;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                if (columnName.Contains("id"))
                {
                    continue;
                }
                Label label = new Label();
                label.Text = columnName;
                label.Top = top;
                label.Left = left;
                leftPanel.Controls.Add(label);

                TextBox textBox = new TextBox();
                textBox.Name = columnName;
                textBox.Top = top;
                textBox.Left = textBoxLeft;
                leftPanel.Controls.Add(textBox);

                top += 30; // Increment the top position for the next label and text box
            }
            reader.Close();

            // Store data in database
            Button buttonInsert = new Button();
            buttonInsert.Text = "Insert";
            buttonInsert.Top = top;
            buttonInsert.Left = left;
            buttonInsert.Click += new EventHandler(buttonInsert_Click);
            leftPanel.Controls.Add(buttonInsert);
            // Create a "Browse" button
            Button buttonBrowse = new Button();
            buttonBrowse.Text = "Buscar foto";
            buttonBrowse.Top = top;
            buttonBrowse.Left = textBoxLeft;
            buttonBrowse.Click += new EventHandler(buttonBrowse_Click);
            leftPanel.Controls.Add(buttonBrowse);
        }
        private void buttonBrowse_Click(object sendesr, EventArgs es)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the path of the selected file and set it to the textbox corresponding to the "foto" column
                string fileName = openFileDialog.FileName;
                TextBox textBoxFoto = (TextBox)leftPanel.Controls["foto"];
                textBoxFoto.Text = fileName;
            }
        }
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            string sql = "INSERT INTO " + label + "(";
            if (label == "Procesos")
            {
                // Procesos
                foreach (Control control in leftPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += control.Name + ",";
                    }
                }

                sql = sql.TrimEnd(',') + ") VALUES (";

                foreach (Control control in leftPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += "'" + control.Text.ToUpper() + "',";
                    }
                }

                sql = sql.TrimEnd(',') + "); ";
            }
            else if (label == "Equipos")
            {
                // Generate a new idEquipo that increments within the same idProceso
                string getMaxIdEquipoSql = "SELECT MAX(idEquipo) FROM Equipos WHERE idProceso = " + idProceso;
                SqlCommand getMaxIdEquipoCmd = new SqlCommand(getMaxIdEquipoSql, cnx);
                int maxIdEquipo = 0;
                object result = getMaxIdEquipoCmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    maxIdEquipo = Convert.ToInt32(result);
                }
                int newIdEquipo = maxIdEquipo + 1;

                foreach (Control control in leftPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += control.Name + ",";
                    }
                }

                // Add the idProceso and new idEquipo columns to the INSERT statement
                sql += "idProceso, idEquipo,";

                sql = sql.TrimEnd(',') + ") VALUES (";

                foreach (Control control in leftPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += "'" + control.Text.ToUpper() + "',";
                    }
                }

                // Add the idProceso and new idEquipo values to the VALUES clause
                sql += "'" + idProceso + "','" + newIdEquipo + "',";

                sql = sql.TrimEnd(',') + "); ";
            }
            else if (label == "Secciones")
            {
                // Get the latest idSecciones for the given idProceso and idEquipo
                string sqlMaxId = "SELECT MAX(idSecciones) FROM Secciones WHERE idProceso = '" + idProceso + "' AND idEquipo = '" + idEquipo + "'";
                int latestId = 0;
                using (SqlCommand command = new SqlCommand(sqlMaxId, cnx))
                {
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        latestId = Convert.ToInt32(result);
                    }
                }

                // Increment the latest idSecciones for the given idProceso and idEquipo
                int newId = latestId + 1;

                foreach (Control control in leftPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += control.Name + ",";
                    }
                }

                // Add the idProceso, idEquipo, and idSecciones columns to the INSERT statement
                sql += "idProceso,idEquipo,idSecciones,";

                sql = sql.TrimEnd(',') + ") VALUES (";

                foreach (Control control in leftPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += "'" + control.Text.ToUpper() + "',";
                    }
                }

                // Add the idProceso, idEquipo, and new idSecciones values to the VALUES clause
                sql += "'" + idProceso + "','" + idEquipo + "','" + newId + "',";

                sql = sql.TrimEnd(',') + "); ";
            }
            else if (label == "Subsistemas")
            {
                // Get the latest idSubsistemas for the given idProceso and idEquipo
                string sqlMaxId = "SELECT MAX(idSubsistemas) FROM Subsistemas WHERE idProceso = '" + idProceso + "' AND idEquipo = '" + idEquipo + "'";
                int latestId = 0;
                using (SqlCommand command = new SqlCommand(sqlMaxId, cnx))
                {
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        latestId = Convert.ToInt32(result);
                    }
                }

                // Increment the latest idSubsistemas for the given idProceso and idEquipo
                int newId = latestId + 1;

                foreach (Control control in leftPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += control.Name + ",";
                    }
                }

                // Add the idProceso, idEquipo, and idSubsistemas columns to the INSERT statement
                sql += "idProceso,idEquipo,idSubsistemas,";

                sql = sql.TrimEnd(',') + ") VALUES (";

                foreach (Control control in leftPanel.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += "'" + control.Text.ToUpper() + "',";
                    }
                }

                // Add the idProceso, idEquipo, and new idSubsistemas values to the VALUES clause
                sql += "'" + idProceso + "','" + idEquipo + "','" + newId + "',";

                sql = sql.TrimEnd(',') + "); ";
            }

            // Execute the INSERT statement
            SqlCommand cmd = new SqlCommand(sql, cnx);
            cmd.ExecuteNonQuery();
            DialogResult resultt = MessageBox.Show("Data inserted successfully.", "Success", MessageBoxButtons.OK);
            if (resultt == DialogResult.OK)
            {
                if (label == "Procesos")
                    mainForm.Procesos(cnx);
                else if(label == "Equipos")
                    mainForm.Equipos(cnx, idProceso, "");
                else if(label == "Secciones" || label=="Subsistemas")
                    mainForm.SeccionesSubsistemas(cnx,"","",idProceso, idEquipo);

                this.Close();
            }
            
        }

    }
}
