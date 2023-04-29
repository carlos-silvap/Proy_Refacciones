using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Refacciones
{
    public partial class Procesos : UserControl
    {
        private SqlConnection cnx;
        public MainForm mainForm;
        public Procesos(MainForm mainForm, SqlConnection cnx)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.cnx = cnx; 
        }

        private void Procesos_Load(object sender, EventArgs e)
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
            smallButton.Text = "Agregar Proceso";
            smallButton.Size = new Size(120, 30);
            smallButton.Anchor = AnchorStyles.None;
            smallButton.Location = new Point((bottomPanel.Width - smallButton.Width) / 2,
                                     (bottomPanel.Height - smallButton.Height) / 2);

            // Add the button to the Controls collection of the panel
            bottomPanel.Controls.Add(smallButton);

            var query = "SELECT * FROM dbo.Procesos";
            var command = new SqlCommand(query, cnx);

            // Read data from the database
            var reader = command.ExecuteReader();

            var label = ButtonGenerator.GenerateButtons(cnx, buttonsPanel, reader, (proceso, idProceso) =>
            {
                mainForm.Equipos(cnx, proceso, idProceso);
            });
            //path = label.Text + " - ";

            // Add a scrollbar to the buttonsPanel
            var scrollBar = new VScrollBar();
            scrollBar.Dock = DockStyle.Right;
            buttonsPanel.Controls.Add(scrollBar);

            // Set the initial size and position of the buttonsPanel
            ButtonGenerator.UpdatePanelSizeAndPosition(buttonsPanel, ClientSize);

            Resize += delegate
            {
                // Recalculate the size and position of the buttonsPanel
                ButtonGenerator.UpdatePanelSizeAndPosition(buttonsPanel, ClientSize);
            };

            smallButton.Click += (senders, ex) => agregarProceso_Click(cnx, sender, e, label.Text);

            reader.Close();
        }

        static void agregarProceso_Click(SqlConnection cnx, object sender, EventArgs e, string label)
        {
            UserEntryForm agregarForm = new UserEntryForm(cnx, label, 0, 0);
            agregarForm.Show();
        }
    }
}
