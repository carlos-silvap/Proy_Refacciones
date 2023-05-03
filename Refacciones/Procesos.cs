using System;
using System.Data.SqlClient;
using System.Diagnostics;
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

            Controls.Add(topPanel);
            Controls.Add(buttonsPanel);
            Controls.Add(bottomPanel);

            //Buttons properties
            addButton.Text = "Agregar Proceso";
            addButton.Size = new Size(120, 30);
            addButton.Anchor = AnchorStyles.None;
            addButton.Location = new Point((bottomPanel.Width - addButton.Width) / 2,
                                     (bottomPanel.Height - addButton.Height) / 2);
            bottomPanel.Controls.Add(addButton);

            //Filter the elements in the sql table
            var command = new SqlCommand
            {
                CommandText = "SELECT * FROM dbo.Procesos",
                Connection = cnx
            };
            var reader = command.ExecuteReader();
            var label = DynamicPanelBuilder.GenerateTopPanel(topPanel, reader, mainForm, cnx,(equipo, idEquipo) =>
            {
                mainForm.Procesos(cnx);
            });
            topPanel.Controls.Add(label);
            DynamicPanelBuilder.GenerateButtons(cnx, buttonsPanel, reader, (proceso, idProceso) =>
            {
                mainForm.Equipos(cnx, idProceso, proceso);
            });
            addButton.Click += (senders, ex) => agregarProceso_Click(cnx, sender, e, label.Text);
            reader.Close();
            //DynamicPanelBuilder.UpdatePanelSizeAndPosition(buttonsPanel, ClientSize);
            //Resize += delegate
            //{
            //    DynamicPanelBuilder.UpdatePanelSizeAndPosition(buttonsPanel, ClientSize);
            //};
        }
        private void agregarProceso_Click(SqlConnection cnx, object sender, EventArgs e, string label)
        {
            UserEntryForm agregarForm = new UserEntryForm(cnx, label, 0, 0, mainForm);
            agregarForm.Show();
        }
    }
}
