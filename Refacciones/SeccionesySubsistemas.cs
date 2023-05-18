using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace Refacciones
{
    public partial class SeccionesySubsistemas : UserControl
    {
        private SqlConnection cnx;
        public MainForm mainForm;
        private int idProceso, idEquipo;
        private string proceso, equipo;
        Panel seccionesPanel = new Panel();
        Panel subsistemasPanel = new Panel();

        public SeccionesySubsistemas(MainForm mainForm, SqlConnection cnx, string proceso, string equipo, int idProceso, int idEquipo)
        {
            InitializeComponent();
            this.cnx = cnx;
            this.mainForm = mainForm;
            this.idProceso = idProceso;
            this.idEquipo = idEquipo;
            this.proceso = proceso;
            this.equipo = equipo;
        }
        private void SeccionesySubsistemas_Load(object sender, EventArgs e)
        {
            var splitContainer = new SplitContainer();
            splitContainer.SplitterIncrement = 1;
            splitContainer.SplitterDistance = splitContainer.Width / 2 - splitContainer.SplitterWidth / 2;
            splitContainer.Dock = DockStyle.Fill;
            seccionesPanel.Dock = DockStyle.Left;
            subsistemasPanel.Dock = DockStyle.Right;
            splitContainer.Panel1.Controls.Add(seccionesPanel);
            splitContainer.Panel2.Controls.Add(subsistemasPanel);
            this.Controls.Add(splitContainer);
            SeccionesPanelLoad();
            SubsistemasPanelLoad();
        }
        private void SeccionesPanelLoad()
        {
            //Panels
            var topPanel = new Panel();
            var buttonsPanel = new Panel();
            var bottomPanel = new Panel();
            //Add panels to form
            seccionesPanel.Controls.Add(topPanel);
            seccionesPanel.Controls.Add(buttonsPanel);
            seccionesPanel.Controls.Add(bottomPanel);
            var command = new SqlCommand
            {
                CommandText = "SELECT * FROM dbo.Secciones WHERE idProceso = @idProceso AND idEquipo = @idEquipo",
                Connection = cnx
            };
            command.Parameters.AddWithValue("@idProceso", idProceso);
            command.Parameters.AddWithValue("@idEquipo", idEquipo);
            // Read data from the database
            var reader = command.ExecuteReader();
            //Top panel
            var label = DynamicPanelBuilder.GenerateTopPanel(topPanel, reader, mainForm, cnx, (seccion, idSeccion) =>
            {
                mainForm.Equipos(cnx, idProceso, proceso);
            });
            //Buttons panel
            DynamicPanelBuilder.GenerateButtons(cnx, seccionesPanel, reader, (seccion, idSecciones) =>
            {
                mainForm.Opciones(cnx, proceso, equipo, seccion, idProceso, idEquipo, idSecciones);

            });
            //Bottom panel
            DynamicPanelBuilder.GenerateBottomPanel(bottomPanel, label.Text, (senders, ex) =>
            {
                UserEntryForm agregarForm = new UserEntryForm(cnx, label.Text, idProceso, idEquipo, mainForm);
                agregarForm.Show();
            });

            var fullPath = new Label
            {
                Text = " - " + proceso + " - " + equipo + " - ",
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = label.TextAlign,
                Dock = DockStyle.Top
            };
            topPanel.Controls.AddRange(new Control[] { fullPath, label });
            reader.Close();
        }
        private void SubsistemasPanelLoad()
        {
            //Panels
            var topPanel = new Panel();
            var buttonsPanel = new Panel();
            var bottomPanel = new Panel();
            //Add panels to form
            subsistemasPanel.Controls.Add(topPanel);
            subsistemasPanel.Controls.Add(buttonsPanel);
            subsistemasPanel.Controls.Add(bottomPanel);
            var command = new SqlCommand
            {
                CommandText = "SELECT * FROM dbo.Subsistemas WHERE idProceso = @idProceso AND idEquipo = @idEquipo",
                Connection = cnx
            };
            command.Parameters.AddWithValue("@idProceso", idProceso);
            command.Parameters.AddWithValue("@idEquipo", idEquipo);
            // Read data from the database
            var reader = command.ExecuteReader();
            //Top panel
            var label = DynamicPanelBuilder.GenerateTopPanel(topPanel, reader, mainForm, cnx, (subsistema, idSubsistema) =>
            {
                mainForm.Equipos(cnx, idProceso, proceso);
            });
            //Buttons panel
            DynamicPanelBuilder.GenerateButtons(cnx, subsistemasPanel, reader, (subsistema, idSubsistema) =>
            {
                mainForm.Opciones(cnx, proceso, equipo, subsistema, idProceso, idEquipo, idSubsistema);
            });
            //Bottom panel
            DynamicPanelBuilder.GenerateBottomPanel(bottomPanel, label.Text, (senders, ex) =>
            {
                UserEntryForm agregarForm = new UserEntryForm(cnx, label.Text, idProceso, idEquipo, mainForm);
                agregarForm.Show();
            });
            var fullPath = new Label
            {
                Text = " - " + proceso + " - " + equipo + " - ",
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = label.TextAlign,
                Dock = DockStyle.Top
            };
            topPanel.Controls.AddRange(new Control[] { fullPath, label });
            reader.Close();
        }
    }
}
