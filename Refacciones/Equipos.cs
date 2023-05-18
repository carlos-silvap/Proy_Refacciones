using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Refacciones
{
    public partial class Equipos : UserControl
    {
        private SqlConnection cnx;
        public MainForm mainForm;
        private int idProceso;
        private string proceso;
        public Equipos(MainForm mainForm, SqlConnection cnx, int idProceso, string proceso)
        {
            InitializeComponent();
            this.cnx = cnx;
            this.mainForm = mainForm;
            this.idProceso = idProceso;
            this.proceso = proceso;
        }
        
        private void Equipos_Load(object sender, EventArgs e)
        {
            //Panels
            var topPanel = new Panel();
            var buttonsPanel = new Panel();
            var bottomPanel = new Panel();

            //Add panels to form
            Controls.Add(topPanel);
            Controls.Add(buttonsPanel);
            Controls.Add(bottomPanel);

            //Filter the elements in the sql table
            var command = new SqlCommand
            {
                CommandText = "SELECT * FROM dbo.Equipos WHERE idProceso = @idProceso",
                Connection = cnx
            };
            command.Parameters.AddWithValue("@idProceso", idProceso);
            var reader = command.ExecuteReader();
            
            //Top panel
            var label = DynamicPanelBuilder.GenerateTopPanel(topPanel, reader, mainForm, cnx, (equipo, idEquipo) =>
            {
                mainForm.Procesos(cnx);
            });
            //Buttons panel
            DynamicPanelBuilder.GenerateButtons(cnx, buttonsPanel, reader, (equipo, idEquipo) =>
            {
                mainForm.SeccionesSubsistemas(cnx, proceso, equipo, idProceso, idEquipo);
            });
            //Bottom panel
            DynamicPanelBuilder.GenerateBottomPanel(bottomPanel, label.Text, (senders, ex) =>
            {
                UserEntryForm agregarForm = new UserEntryForm(cnx, label.Text, idProceso, 0, mainForm);
                agregarForm.Show();
            });

            var fullPath = new Label
            {
                Text = " - " + proceso + " - ",
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = label.TextAlign,
                Dock = DockStyle.Top
            };
            topPanel.Controls.AddRange(new Control[] { fullPath, label });
            reader.Close();
        }

    }
}
