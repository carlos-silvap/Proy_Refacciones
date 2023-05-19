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
    public partial class UserDeleteForm : Form
    {
        private SqlConnection cnx;
        string label;
        MainForm mainForm;
        private int idProceso, idEquipo, idSeccion, idSubsistema;
        Panel leftPanel = new Panel();
        Panel rightPanel = new Panel();
        private TextBox idTextBox;
        string idName = "idProceso"; // Default column name
        public UserDeleteForm(SqlConnection cnx, string label, int idProceso, int idEquipo, MainForm mainForm)
        {
            InitializeComponent();
            this.cnx = cnx;
            this.label = label;
            this.idProceso = idProceso;
            this.idEquipo = idEquipo;
            this.mainForm = mainForm;
            idTextBox = new TextBox(); // Instantiate the textbox
            idTextBox.Name = "idTextBox";
            idTextBox.Top = 20;
            idTextBox.Left = 100;
            leftPanel.Controls.Add(idTextBox);
            var searchButton = new Button(); // Add a search button
            searchButton.Text = "Search";
            searchButton.Top = 20;
            searchButton.Left = 200;
            searchButton.Click += new EventHandler(searchButton_Click);
            leftPanel.Controls.Add(searchButton);

           
            switch (label)
            {
                case "Procesos":
                    idName = "idProceso";
                    break;
                case "Equipos":
                    idName = "idEquipo";
                    break;
                case "Secciones":
                    idName = "idSeccion";
                    break;
                case "Subsistemas":
                    idName = "idSubsistema";
                    break;
                    // Add additional cases for other labels if needed
            }
        }
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(idTextBox.Text, out int id))
            {
                string deleteQuery = $"DELETE FROM {label} WHERE {idName} = {id}";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, cnx))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item deleted successfully.", "Success", MessageBoxButtons.OK);
                        UpdateDataGridView();
                        UpdateMainForm();
                    }
                    else
                    {
                        MessageBox.Show("Item not found.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid ID");
            }
        }
        private void searchButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(idTextBox.Text, out int id))
            {
                string query = $"SELECT * FROM {label} WHERE {idName} = {id}";
                
                using (SqlCommand cmd = new SqlCommand(query, cnx))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            if (columnName.Contains("id"))
                            {
                                continue;
                            }
                            TextBox textBox = (TextBox)leftPanel.Controls[columnName];
                            if (textBox != null)
                            {
                                textBox.Text = reader[columnName].ToString();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Element not found");
                    }

                    reader.Close();
                }
            }
            else
            {
                MessageBox.Show("Invalid ID");
            }
        }
        private void UserDeleteForm_Load(object sender, EventArgs e)
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

            if (idProceso != 0 && idEquipo == 0 && (idSeccion == 0 && idSubsistema == 0))
            {
                query += " WHERE idProceso = " + idProceso;
            }
            else if (idProceso != 0 && idEquipo != 0 && (idSeccion == 0 && idSubsistema == 0))
            {
                query += " WHERE idProceso = " + idProceso + " AND idEquipo = " + idEquipo;
            }
            else if (idProceso != 0 && idEquipo != 0 && (idSeccion != 0 && idSubsistema == 0))
            {
                query += " WHERE idProceso = " + idProceso + " AND idEquipo = " + idEquipo + " AND idSeccion = " + idSeccion;
            }
            else if (idProceso != 0 && idEquipo != 0 && (idSeccion == 0 && idSubsistema != 0))
            {
                query += " WHERE idProceso = " + idProceso + " AND idEquipo = " + idEquipo + " AND idSubsistema = " + idSubsistema;
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
            Button deleteButton = new Button();
            deleteButton.Text = "Delete";
            deleteButton.Top = top;
            deleteButton.Left = left;
            deleteButton.Click += new EventHandler(deleteButton_Click);
            leftPanel.Controls.Add(deleteButton);
        }
        private void UpdateDataGridView()
        {
            var dataGridView = (DataGridView)rightPanel.Controls[0];
            var dataTable = (DataTable)dataGridView.DataSource;
            dataTable.Clear();

            string query = "SELECT * FROM " + label;


            if (idProceso != 0 && idEquipo == 0 && (idSeccion == 0 && idSubsistema == 0))
            {
                query += " WHERE idProceso = " + idProceso;
            }
            else if (idProceso != 0 && idEquipo != 0 && (idSeccion == 0 && idSubsistema == 0))
            {
                query += " WHERE idProceso = " + idProceso + " AND idEquipo = " + idEquipo;
            }
            else if (idProceso != 0 && idEquipo != 0 && (idSeccion != 0))
            {
                query += " WHERE idProceso = " + idProceso + " AND idEquipo = " + idEquipo + " AND idSeccion = " + idSeccion;
            }
            else if (idProceso != 0 && idEquipo != 0 && (idSubsistema != 0))
            {
                query += " WHERE idProceso = " + idProceso + " AND idEquipo = " + idEquipo + " AND idSubsistema = " + idSubsistema;
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
