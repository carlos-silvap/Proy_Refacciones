using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Refacciones
{
    public partial class SeccionesySubsistemas : UserControl
    {
        private SqlConnection cnx;
        public MainForm mainForm;
        private int idProceso, idEquipo;
        private string proceso, equipo;

        // create a new Panel control for the left panel
        Panel seccionesPanel = new Panel();

        // create a new Panel control for the right panel
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
            // calculate the SplitterDistance to divide the control evenly
            splitContainer.SplitterDistance = splitContainer.Width / 2 - splitContainer.SplitterWidth / 2;
            splitContainer.Dock = DockStyle.Fill;
            // set the Dock property to Left
            seccionesPanel.Dock = DockStyle.Left;
            // set the Dock property to Right
            subsistemasPanel.Dock = DockStyle.Right;
            // add the left and right panels to the SplitContainer control
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
            //Buttons
            var addButton = new Button();
            //Panel properties
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 30;

            buttonsPanel.Dock = DockStyle.Bottom;

            bottomPanel.BackColor = Color.LightGray;
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 50;

            //Add panels to form
            seccionesPanel.Controls.Add(topPanel);
            seccionesPanel.Controls.Add(buttonsPanel);
            seccionesPanel.Controls.Add(bottomPanel);


            //Buttons properties
            addButton.Text = "Agregar Seccion";
            addButton.Size = new Size(120, 30);
            addButton.Anchor = AnchorStyles.None;
            addButton.Location = new Point((bottomPanel.Width - addButton.Width) / 2,
                                     (bottomPanel.Height - addButton.Height) / 2);
            bottomPanel.Controls.Add(addButton);
            //Filter the elements in the sql table
            var command = new SqlCommand
            {
                CommandText = "SELECT * FROM dbo.Secciones WHERE idProceso = @idProceso AND idEquipo = @idEquipo",
                Connection = cnx
            };
            command.Parameters.AddWithValue("@idProceso", idProceso);
            command.Parameters.AddWithValue("@idEquipo", idEquipo);

            // Read data from the database
            var reader = command.ExecuteReader();
            var label = DynamicPanelBuilder.GenerateTopPanel(topPanel, reader, mainForm, cnx, (seccion, idSeccion) =>
            {
                mainForm.Equipos(cnx, idProceso, proceso);
            });
            var fpath = new Label();
            string text = label.Text + " - " + proceso + " - " + equipo;
            fpath.Text = text;
            fpath.Font = label.Font;
            fpath.TextAlign = label.TextAlign;
            fpath.Dock = DockStyle.Top;
            topPanel.Controls.Add(fpath);
            DynamicPanelBuilder.GenerateButtons(cnx, seccionesPanel, reader, (seccion, idSecciones) =>
            {
                int hi = 0;
                //var form4 = new Herramientas_Refacciones_Instructivos_Especificaciones(cnx, equipo);
                //form4.Show();
            });
            addButton.Click += (sender, e) => agregarSeccion_Click(cnx, sender, e, label.Text, idProceso, idEquipo);
            reader.Close();
            // Set the initial size and position of the buttonsPanel
            DynamicPanelBuilder.UpdatePanelSizeAndPosition(seccionesPanel, ClientSize);
            // Handle the Resize event of the form
            Resize += delegate
            {
                // Recalculate the size and position of the buttonsPanel
                DynamicPanelBuilder.UpdatePanelSizeAndPosition(seccionesPanel, ClientSize);
            };
            //label.Text += " - " + equipo;
        }
        private void SubsistemasPanelLoad()
        {
            //Panels
            var topPanel = new Panel();
            var buttonsPanel = new Panel();
            var bottomPanel = new Panel();
            //Buttons
            var addButton = new Button();
            //Panel properties
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 30;

            buttonsPanel.Dock = DockStyle.Bottom;

            bottomPanel.BackColor = Color.LightGray;
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 50;

            //Add panels to form
            subsistemasPanel.Controls.Add(topPanel);
            subsistemasPanel.Controls.Add(buttonsPanel);
            subsistemasPanel.Controls.Add(bottomPanel);


            //Buttons properties
            addButton.Text = "Agregar Subsistema";
            addButton.Size = new Size(120, 30);
            addButton.Anchor = AnchorStyles.None;
            addButton.Location = new Point((bottomPanel.Width - addButton.Width) / 2,
                                     (bottomPanel.Height - addButton.Height) / 2);
            bottomPanel.Controls.Add(addButton);
            //Filter the elements in the sql table
            var command = new SqlCommand
            {
                CommandText = "SELECT * FROM dbo.Subsistemas WHERE idProceso = @idProceso AND idEquipo = @idEquipo",
                Connection = cnx
            };
            command.Parameters.AddWithValue("@idProceso", idProceso);
            command.Parameters.AddWithValue("@idEquipo", idEquipo);

            // Read data from the database
            var reader = command.ExecuteReader();
            var label = DynamicPanelBuilder.GenerateTopPanel(topPanel, reader, mainForm, cnx, (subsistema, idSubsistema) =>
            {
                mainForm.Equipos(cnx, idProceso, proceso);
            });
            topPanel.Controls.Add(label);
            DynamicPanelBuilder.GenerateButtons(cnx, subsistemasPanel, reader, (subsistema, idSubsistema) =>
            {
                //var form4 = new Herramientas_Refacciones_Instructivos_Especificaciones(cnx, equipo);
                //form4.Show();
            });
            addButton.Click += (sender, e) => agregarSeccion_Click(cnx, sender, e, label.Text, idProceso, idEquipo);
            reader.Close();
            // Set the initial size and position of the buttonsPanel
            DynamicPanelBuilder.UpdatePanelSizeAndPosition(subsistemasPanel, ClientSize);
            // Handle the Resize event of the form
            Resize += delegate
            {
                // Recalculate the size and position of the buttonsPanel
                DynamicPanelBuilder.UpdatePanelSizeAndPosition(subsistemasPanel, ClientSize);
            };
            //label.Text += " - " + equipo;
        }
        private void agregarSeccion_Click(SqlConnection cnx, object sender, EventArgs e, string label, int idProceso, int idEquipo)
        {
            UserEntryForm agregarForm = new UserEntryForm(cnx, label, idProceso, idEquipo, mainForm);
            agregarForm.Show();
        }

    }
}
