using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Refacciones
{
    public partial class UserEntryForm : Form
    {
        private SqlConnection cnx;
        string label;
        private int idProceso;
        private int idEquipo;

        Panel entryFields = new Panel();
        Panel currentItems = new Panel();
        public UserEntryForm(SqlConnection cnx, string label, int idProceso, int idEquipo)
        {
            InitializeComponent();
            this.cnx = cnx;
            this.label = label;
            this.idProceso = idProceso;
            this.idEquipo = idEquipo;
        }
        //private void UserEntryForm_Load(object sender, EventArgs e)
        //{

        //    // create a new SplitContainer control
        //    var splitContainer = new SplitContainer();

        //    // set the SplitterIncrement to 1
        //    splitContainer.SplitterIncrement = 1;

        //    // calculate the SplitterDistance to divide the control evenly
        //    splitContainer.SplitterDistance = splitContainer.Width / 2 - splitContainer.SplitterWidth / 2;

        //    // set the Dock property to Fill
        //    splitContainer.Dock = DockStyle.Fill;

        //    // set the Dock property to Left
        //    entryFields.Dock = DockStyle.Left;

        //    // set the Dock property to Right
        //    currentItems.Dock = DockStyle.Right;

        //    // add the left and right panels to the SplitContainer control
        //    splitContainer.Panel1.Controls.Add(entryFields);
        //    splitContainer.Panel2.Controls.Add(currentItems);

        //    // add the SplitContainer control to the form
        //    this.Controls.Add(splitContainer);
        //    entryFieldsPanelLoad();
        //    //currentItemsPanelLoad();
        //}
        //private void entryFieldsPanelLoad()
        //{
        //    SqlCommand cmd = new SqlCommand("SELECT * FROM " + label, cnx);
        //    SqlDataReader reader = cmd.ExecuteReader();

        //    int top = 50;
        //    int left = 50;
        //    int textBoxLeft = 200;
        //    var temp = label;

        //    for (int i = 0; i < reader.FieldCount; i++)
        //    {
        //        string columnName = reader.GetName(i);
        //        if (columnName.Contains("id"))
        //        {
        //            continue;
        //        }
        //        Label label = new Label();
        //        label.Text = columnName;
        //        label.Top = top;
        //        label.Left = left;
        //        entryFields.Controls.Add(label);

        //        TextBox textBox = new TextBox();
        //        textBox.Name = columnName;
        //        textBox.Location = new Point(10, 10);
        //        textBox.Size = new Size(100, 20);
        //        textBox.Top = top + 30;
        //        textBox.Left = textBoxLeft;
        //        entryFields.Controls.Add(textBox);



        //        top += 60; // Increment the top position for the next label and text box
        //    }
        //    reader.Close();
        //    Create a "Browse" button
        //   Button buttonBrowse = new Button();
        //    buttonBrowse.Text = "Buscar";
        //    buttonBrowse.Top = top;
        //    buttonBrowse.Left = left;
        //    buttonBrowse.Click += new EventHandler(buttonBrowse_Click);
        //    entryFields.Controls.Add(buttonBrowse);
        //    Store data in database
        //   Button buttonInsert = new Button();
        //    buttonInsert.Text = "Insert";
        //    buttonInsert.Top = top + 40;
        //    buttonInsert.Left = left;
        //    buttonInsert.Click += new EventHandler(buttonInsert_Click);
        //    entryFields.Controls.Add(buttonInsert);
        //}
        //private void currentItemsPanelLoad()
        //{

        //}
        private void UserEntryForm_Load(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM " + label, cnx);
            SqlDataReader reader = cmd.ExecuteReader();

            int top = 50;
            int left = 50;
            int textBoxLeft = 200;
            //var temp = label;

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
                this.Controls.Add(label);

                TextBox textBox = new TextBox();
                textBox.Name = columnName;
                textBox.Top = top;
                textBox.Left = textBoxLeft;
                this.Controls.Add(textBox);

                top += 30; // Increment the top position for the next label and text box
            }
            reader.Close();

            // Store data in database
            Button buttonInsert = new Button();
            buttonInsert.Text = "Insert";
            buttonInsert.Top = top;
            buttonInsert.Left = left;
            buttonInsert.Click += new EventHandler(buttonInsert_Click);
            this.Controls.Add(buttonInsert);
            // Create a "Browse" button
            Button buttonBrowse = new Button();
            buttonBrowse.Text = "Buscar foto";
            buttonBrowse.Top = top;
            buttonBrowse.Left = textBoxLeft;
            buttonBrowse.Click += new EventHandler(buttonBrowse_Click);
            this.Controls.Add(buttonBrowse);
        }
        // Handle the "Click" event of the "Browse" button
        void buttonBrowse_Click(object sendesr, EventArgs es)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the path of the selected file and set it to the textbox corresponding to the "foto" column
                string fileName = openFileDialog.FileName;
                TextBox textBoxFoto = (TextBox)this.Controls["foto"];
                textBoxFoto.Text = fileName;
            }
        }
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            string sql = "INSERT INTO " + label + "(";
            if (label == "Procesos")
            {
                // Procesos
                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += control.Name + ",";
                    }
                }

                sql = sql.TrimEnd(',') + ") VALUES (";

                foreach (Control control in this.Controls)
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

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += control.Name + ",";
                    }
                }

                // Add the idProceso and new idEquipo columns to the INSERT statement
                sql += "idProceso, idEquipo,";

                sql = sql.TrimEnd(',') + ") VALUES (";

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += "'" + control.Text + "',";
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

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += control.Name + ",";
                    }
                }

                // Add the idProceso, idEquipo, and idSecciones columns to the INSERT statement
                sql += "idProceso,idEquipo,idSecciones,";

                sql = sql.TrimEnd(',') + ") VALUES (";

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += "'" + control.Text + "',";
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

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += control.Name + ",";
                    }
                }

                // Add the idProceso, idEquipo, and idSubsistemas columns to the INSERT statement
                sql += "idProceso,idEquipo,idSubsistemas,";

                sql = sql.TrimEnd(',') + ") VALUES (";

                foreach (Control control in this.Controls)
                {
                    if (control is TextBox)
                    {
                        sql += "'" + control.Text + "',";
                    }
                }

                // Add the idProceso, idEquipo, and new idSubsistemas values to the VALUES clause
                sql += "'" + idProceso + "','" + idEquipo + "','" + newId + "',";

                sql = sql.TrimEnd(',') + "); ";
            }

            // Execute the INSERT statement
            SqlCommand cmd = new SqlCommand(sql, cnx);
            cmd.ExecuteNonQuery();

            MessageBox.Show("Data inserted successfully.");
        }

    }
}
