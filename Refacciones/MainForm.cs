using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Refacciones
{
    public partial class MainForm : Form
    {
        public UserControl currentControl;
        private SqlConnection cnx;
        public MainForm()
        {
            InitializeComponent();
        }

        public void MainForm_Load(object sender, EventArgs e)
        {
            var connectionString = "Data Source=LAPTOP-MU5H26MK\\SQLEXPRESS;Initial Catalog=Refacciones;Integrated Security=True";
            cnx = new SqlConnection(connectionString);
            
            //Test if the first connection works properly 
            try
            {
                cnx.Open();
                MessageBox.Show("Conectado correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message, "Error inesperado", MessageBoxButtons.OK);
            }
            // Launch the first form
            Procesos(cnx);
        }
        public void Procesos(SqlConnection cnx)
        {
            // Remove the current content from the panel
            if (currentControl != null)
            {
                Controls.Remove(currentControl);
                currentControl.Dispose();
            }
            var procesosControl = new Procesos(this, cnx);
            procesosControl.Dock = DockStyle.Fill;
            Controls.Add(procesosControl);
            currentControl = procesosControl;
        }
        public void Equipos(SqlConnection cnx, string proceso, int idProceso)
        {
            // Remove the current content from the panel
            if (currentControl != null)
            {
                Controls.Remove(currentControl);
                currentControl.Dispose();
            }
            // Add the new content to the panel
            var equiposControl = new Equipos(this, cnx, proceso, idProceso);
            equiposControl.Dock = DockStyle.Fill;
            Controls.Add(equiposControl);
            currentControl = equiposControl;
        }
        public void SeccionesSubsistemas(SqlConnection cnx, string proceso, int idProceso, int idEquipo)
        {
            // Remove the current content from the panel
            if (currentControl != null)
            {
                Controls.Remove(currentControl);
                currentControl.Dispose();
            }
            // Add the new content to the panel
            var equiposControl = new SeccionesySubsistemas(this, cnx, proceso, idProceso, idEquipo);
            equiposControl.Dock = DockStyle.Fill;
            Controls.Add(equiposControl);
            currentControl = equiposControl;
        }
    }
}
