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

            //Buttons
            var addButton = new Button();
            
            //Panel properties
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 30;

            buttonsPanel.Dock = DockStyle.Fill;

            bottomPanel.BackColor = Color.LightGray;
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 50;
            
            //Add panels to form
            Controls.Add(topPanel);
            Controls.Add(buttonsPanel);
            Controls.Add(bottomPanel);
            
            //Buttons properties
            addButton.Text = "Agregar Equipo";
            addButton.Size = new Size(120, 30);
            addButton.Anchor = AnchorStyles.None;
            addButton.Location = new Point((bottomPanel.Width - addButton.Width) / 2,
                                     (bottomPanel.Height - addButton.Height) / 2);
            bottomPanel.Controls.Add(addButton);

            //Filter the elements in the sql table
            var command = new SqlCommand
            {
                CommandText = "SELECT * FROM dbo.Equipos WHERE idProceso = @idProceso",
                Connection = cnx
            };
            command.Parameters.AddWithValue("@idProceso", idProceso);
            var reader = command.ExecuteReader();
            var label = DynamicPanelBuilder.GenerateTopPanel(topPanel, reader, mainForm, cnx, (equipo, idEquipo) =>
            {
                mainForm.Procesos(cnx);
            });
            var fpath = new Label();
            string text = label.Text + " - " + proceso;
            fpath.Text= text;
            fpath.Font = label.Font;
            fpath.TextAlign = label.TextAlign;
            fpath.Dock = DockStyle.Top;

            topPanel.Controls.Add(fpath);
            DynamicPanelBuilder.GenerateButtons(cnx, buttonsPanel, reader, (equipo, idEquipo) =>
            {
                mainForm.SeccionesSubsistemas(cnx, proceso, equipo, idProceso, idEquipo);
            });
            addButton.Click += (senders, ex) => agregarEquipo_Click(cnx, sender, e, label.Text, idProceso);
            reader.Close();
            //DynamicPanelBuilder.UpdatePanelSizeAndPosition(buttonsPanel, ClientSize);
            //Resize += delegate
            //{
            //    DynamicPanelBuilder.UpdatePanelSizeAndPosition(buttonsPanel, ClientSize);
            //};
        }
        private void agregarEquipo_Click(SqlConnection cnx, object sender, EventArgs e, string label, int idProceso)
        {
            UserEntryForm agregarForm = new UserEntryForm(cnx, label, idProceso, 0, mainForm);
            agregarForm.Show();
        }
    }
}
