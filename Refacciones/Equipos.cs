using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Refacciones
{
    public partial class Equipos : UserControl
    {
        private SqlConnection cnx;
        public MainForm mainForm;
        private int idProceso;
        public Equipos(MainForm mainForm, SqlConnection cnx, string proceso, int idProceso)
        {
            InitializeComponent();
            this.cnx = cnx;
            this.mainForm = mainForm;
            this.idProceso = idProceso;
        }

        private void Equipos_Load(object sender, EventArgs e)
        {
            var buttonsPanel = new Panel();
            Controls.Add(buttonsPanel);

            // Create a PANEL at the bottom
            var bottomPanel = new Panel();
            bottomPanel.BackColor = Color.LightGray;
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 50;
            buttonsPanel.Controls.Add(bottomPanel);

            // Create button to add items to sql database
            var smallButton = new Button();
            smallButton.Text = "Agregar Equipo";
            smallButton.Size = new Size(120, 30);
            smallButton.Anchor = AnchorStyles.None;
            smallButton.Location = new Point((bottomPanel.Width - smallButton.Width) / 2,
                                     (bottomPanel.Height - smallButton.Height) / 2);
            bottomPanel.Controls.Add(smallButton);
            // Create button to add items to sql database
            var backButton = new Button();
            backButton.Text = "Regresar";
            backButton.Size = new Size(120, 30);
            backButton.Anchor = AnchorStyles.None;
            backButton.Location = new Point(((bottomPanel.Width - smallButton.Width) / 2)+150, (bottomPanel.Height - smallButton.Height) / 2);
            // Add the button to the Controls collection of the panel
            bottomPanel.Controls.Add(backButton);

            var query = "SELECT * FROM dbo.Equipos WHERE idProceso = @idProceso";
            var command = new SqlCommand(query, cnx);
            command.Parameters.AddWithValue("@idProceso", idProceso);

            // Read data from the database
            var reader = command.ExecuteReader();

            var label = ButtonGenerator.GenerateButtons(cnx, buttonsPanel, reader, (equipo, idEquipo) =>
            {
                //var seccionesForm = new SeccionesySubsistemas(cnx, equipo, idProceso, idEquipo);
                //seccionesForm.Show();
                mainForm.SeccionesSubsistemas(cnx, equipo, idProceso, idEquipo);
            });
            // Set the initial size and position of the buttonsPanel
            ButtonGenerator.UpdatePanelSizeAndPosition(buttonsPanel, ClientSize);
            // Handle the Resize event of the form
            Resize += delegate
            {
                // Recalculate the size and position of the buttonsPanel
                ButtonGenerator.UpdatePanelSizeAndPosition(buttonsPanel, ClientSize);
            };
            //path = label.Text + " - " + proceso;
            smallButton.Click += (senders, ex) => agregarEquipo_Click(cnx, sender, e, label.Text, idProceso);
            backButton.Click += (senders, ex) => back_Click(cnx, sender, e, mainForm);
            reader.Close();
        }
        static void agregarEquipo_Click(SqlConnection cnx, object sender, EventArgs e, string label, int idProceso)
        {
            UserEntryForm agregarForm = new UserEntryForm(cnx, label, idProceso, 0);
            agregarForm.Show();
        }
        static void back_Click(SqlConnection cnx, object sender, EventArgs e, MainForm mainForm)
        {
            mainForm.Procesos(cnx);
        }
    }
}
