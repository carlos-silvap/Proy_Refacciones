using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Drawing;


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
        string fileName;
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
                fileName = openFileDialog.FileName;
                TextBox textBoxFoto = (TextBox)leftPanel.Controls["foto"];
                textBoxFoto.Text = fileName;
            }
        }
        private bool IsItemNameDuplicate()
        {
            List<TextBox> textBoxes = leftPanel.Controls.OfType<TextBox>().ToList();
            string itemName = textBoxes.First(tb => tb.Name == label).Text;

            DataGridView dataGridView = (DataGridView)rightPanel.Controls[0];
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[label].Value != null)
                {
                    string name = row.Cells[label].Value.ToString();
                    if (name.Equals(itemName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            byte[] imageData = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                imageData = File.ReadAllBytes(fileName);
            }
            string query = BuildInsertQuery();
            // Check if the name of the item being added is already in the filtered elements of the table
            if (IsItemNameDuplicate())
            {
                MessageBox.Show("Nombre de elemento duplicado");
                return;
            }
            ExecuteInsertQuery(query, imageData);
            MessageBox.Show("Data inserted successfully.", "Success", MessageBoxButtons.OK);
            UpdateDataGridView();
            UpdateMainForm();
        }
        private string BuildInsertQuery()
        {
            var columns = "";
            var values = "";
            var textBoxes = leftPanel.Controls.OfType<TextBox>()
                .Where(t => t.Name != "foto");
            if (label == "Procesos")
            {
                columns = string.Join(",", textBoxes.Select(t => t.Name));
                values = string.Join(",", textBoxes.Select(t => $"'{t.Text.ToUpper()}'"));
            }
            else if(label == "Equipos")
            {
                string getMaxIdEquipo = "SELECT MAX(idEquipo) FROM Equipos WHERE idProceso = " + idProceso;
                SqlCommand getMaxIdEquipoCmd = new SqlCommand(getMaxIdEquipo, cnx);
                int maxId = 0;
                object result = getMaxIdEquipoCmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    maxId = Convert.ToInt32(result);
                }
                int newIdEquipo = maxId + 1; 
                columns = string.Join(",", textBoxes.Select(t => t.Name)) + ",idProceso, idEquipo";
                values = string.Join(",", textBoxes.Select(t => $"'{t.Text.ToUpper()}'")) + $",'{idProceso}','{newIdEquipo}'";

            }
            else if (label == "Secciones")
            {
                string MaxIdSeccion = "SELECT MAX(idSecciones) FROM Secciones WHERE idProceso = '" + idProceso + "' AND idEquipo = '" + idEquipo + "'";
                SqlCommand getMaxIdSeccionCmd = new SqlCommand(MaxIdSeccion, cnx);
                int maxId = 0;
                object result = getMaxIdSeccionCmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    maxId = Convert.ToInt32(result);
                }
                int newIdSeccion = maxId + 1;
                columns = string.Join(",", textBoxes.Select(t => t.Name)) + ",idProceso, idEquipo,idSecciones";
                values = string.Join(",", textBoxes.Select(t => $"'{t.Text.ToUpper()}'")) + $",'{idProceso}','{idEquipo}','{newIdSeccion}'";
            }
            else if (label == "Subsistemas")
            {
                string MaxIdSubsistema = "SELECT MAX(idSubsistemas) FROM Subsistemas WHERE idProceso = '" + idProceso + "' AND idEquipo = '" + idEquipo + "'";
                SqlCommand getMaxIdSubsistemaCmd = new SqlCommand(MaxIdSubsistema, cnx);
                int maxId = 0;
                object result = getMaxIdSubsistemaCmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    maxId = Convert.ToInt32(result);
                }
                int newIdSubsistema = maxId + 1;
                columns = string.Join(",", textBoxes.Select(t => t.Name)) + ",idProceso, idEquipo,idSubsistemas";
                values = string.Join(",", textBoxes.Select(t => $"'{t.Text.ToUpper()}'")) + $",'{idProceso}','{idEquipo}','{newIdSubsistema}'";
            }
            else if (label == "Herramientas_Refacciones_Instructivos_Especificaciones")
            {
                columns = string.Join(",", textBoxes.Select(t => t.Name));
                values = string.Join(",", textBoxes.Select(t => $"'{t.Text.ToUpper()}'"));
            }
            return $"INSERT INTO {label} ({columns},foto) VALUES ({values},@foto)";
        }
        private void ExecuteInsertQuery(string query, byte[] imageData)
        {
            using (SqlCommand cmd = new SqlCommand(query, cnx))
            {

                cmd.Parameters.Add("@foto", SqlDbType.VarBinary, imageData?.Length ?? -1).Value = imageData ?? (object)DBNull.Value;


                cmd.ExecuteNonQuery();
            }
        }
        private void UpdateDataGridView()
        {
            var dataGridView = (DataGridView)rightPanel.Controls[0];
            var dataTable = (DataTable)dataGridView.DataSource;
            dataTable.Clear();

            string query = "SELECT * FROM " + label;

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
            adapter.Fill(dataTable);
        }
        private void UpdateMainForm()
        {
            if (label == "Procesos")
                mainForm.Procesos(cnx);
            else if (label == "Equipos")
                mainForm.Equipos(cnx, idProceso, "");
            else if (label == "Secciones" || label == "Subsistemas")
                mainForm.SeccionesSubsistemas(cnx, "", "", idProceso, idEquipo);
        }
    }
}
