using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Refacciones
{
    public partial class SeccionesySubsistemas : UserControl
    {
        private SqlConnection cnx;
        public MainForm mainForm;
        private int idProceso;
        private int idEquipo;

        // create a new Panel control for the left panel
        Panel seccionesPanel = new Panel();

        // create a new Panel control for the right panel
        Panel subsistemasPanel = new Panel();

        public SeccionesySubsistemas(MainForm mainForm, SqlConnection cnx, string equipo, int idProceso, int idEquipo)
        {
            InitializeComponent();
            this.cnx = cnx;
            this.idProceso = idProceso;
            this.idEquipo = idEquipo;
            this.mainForm = mainForm;
        }

        private void SeccionesySubsistemas_Load(object sender, EventArgs e)
        {
            // create a new SplitContainer control
            var splitContainer = new SplitContainer();

            // set the SplitterIncrement to 1
            splitContainer.SplitterIncrement = 1;

            // calculate the SplitterDistance to divide the control evenly
            splitContainer.SplitterDistance = splitContainer.Width / 2 - splitContainer.SplitterWidth / 2;

            // set the Dock property to Fill
            splitContainer.Dock = DockStyle.Fill;

            // set the Dock property to Left
            seccionesPanel.Dock = DockStyle.Left;

            // set the Dock property to Right
            subsistemasPanel.Dock = DockStyle.Right;

            // add the left and right panels to the SplitContainer control
            splitContainer.Panel1.Controls.Add(seccionesPanel);
            splitContainer.Panel2.Controls.Add(subsistemasPanel);

            // add the SplitContainer control to the form
            this.Controls.Add(splitContainer);
            SeccionesPanelLoad();
            SubsistemasPanelLoad();
        }
        private void SeccionesPanelLoad()
        {
            // Create a PANEL at the bottom
            var bottomPanel = new Panel();
            bottomPanel.BackColor = Color.LightGray;
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 50;
            seccionesPanel.Controls.Add(bottomPanel);


            // Create button to add items to sql database
            var smallButton = new Button();
            smallButton.Text = "Agregar Seccion";
            smallButton.Size = new Size(120, 30);
            smallButton.Anchor = AnchorStyles.None;
            smallButton.Location = new Point((bottomPanel.Width - smallButton.Width) / 2,
                                     (bottomPanel.Height - smallButton.Height) / 2);

            // Add the button to the Controls collection of the panel
            bottomPanel.Controls.Add(smallButton);

            var query = "SELECT * FROM dbo.Secciones WHERE idProceso = @idProceso AND idEquipo = @idEquipo";
            var command = new SqlCommand(query, cnx);
            command.Parameters.AddWithValue("@idProceso", idProceso);
            command.Parameters.AddWithValue("@idEquipo", idEquipo);

            // Read data from the database
            var reader = command.ExecuteReader();

            var label = ButtonGenerator.GenerateButtons(cnx, seccionesPanel, reader, (equipo, idSecciones) =>
            {
                //var form4 = new Herramientas_Refacciones_Instructivos_Especificaciones(cnx, equipo);
                //form4.Show();
            });
            // Set the initial size and position of the buttonsPanel
            ButtonGenerator.UpdatePanelSizeAndPosition(seccionesPanel, ClientSize);
            // Handle the Resize event of the form
            Resize += delegate
            {
                // Recalculate the size and position of the buttonsPanel
                ButtonGenerator.UpdatePanelSizeAndPosition(seccionesPanel, ClientSize);
            };
            reader.Close();
            smallButton.Click += (sender, e) => agregarSeccionOrSubsistema_Click(cnx, sender, e, label.Text, idProceso, idEquipo);
            //label.Text += " - " + equipo;
        }
        private void SubsistemasPanelLoad()
        {
            var bottomPanel = new Panel();
            bottomPanel.BackColor = Color.LightGray;
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 50;
            subsistemasPanel.Controls.Add(bottomPanel);


            // Create button to add items to sql database
            var smallButton = new Button();
            smallButton.Text = "Agregar Subsistema";
            smallButton.Size = new Size(120, 30);
            smallButton.Anchor = AnchorStyles.None;
            smallButton.Location = new Point((bottomPanel.Width - smallButton.Width) / 2,
                                     (bottomPanel.Height - smallButton.Height) / 2);

            // Add the button to the Controls collection of the panel
            bottomPanel.Controls.Add(smallButton);
            var query = "SELECT * FROM dbo.Subsistemas WHERE idProceso = @idProceso AND idEquipo = @idEquipo";
            var command = new SqlCommand(query, cnx);
            command.Parameters.AddWithValue("@idProceso", idProceso);
            command.Parameters.AddWithValue("@idEquipo", idEquipo);


            // Read data from the database
            var reader = command.ExecuteReader();

            var label = ButtonGenerator.GenerateButtons(cnx, subsistemasPanel, reader, (equipo, idSecciones) =>
            {
                //var form4 = new Herramientas_Refacciones_Instructivos_Especificaciones(cnx, equipo);
                //form4.Show();
            });
            // Set the initial size and position of the buttonsPanel
            ButtonGenerator.UpdatePanelSizeAndPosition(subsistemasPanel, ClientSize);
            // Handle the Resize event of the form
            Resize += delegate
            {
                // Recalculate the size and position of the buttonsPanel
                ButtonGenerator.UpdatePanelSizeAndPosition(subsistemasPanel, ClientSize);
            };
            reader.Close();
            smallButton.Click += (sender, e) => agregarSeccionOrSubsistema_Click(cnx, sender, e, label.Text, idProceso, idEquipo);
            //label.Text += " - " + equipo;
        }
        static void agregarSeccionOrSubsistema_Click(SqlConnection cnx, object sender, EventArgs e, string label, int idProceso, int idEquipo)
        {
            UserEntryForm agregarForm = new UserEntryForm(cnx, label, idProceso, idEquipo);
            agregarForm.Show();
        }

    }
}
