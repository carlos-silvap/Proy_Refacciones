using System;
using System.Collections.Generic;
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

            Controls.Add(topPanel);
            Controls.Add(buttonsPanel);
            Controls.Add(bottomPanel);

            //Filter the elements in the sql table
            var command = new SqlCommand
            {
                CommandText = "SELECT * FROM dbo.Procesos",
                Connection = cnx
            };
            var reader = command.ExecuteReader();
            //Top panel
            var label = DynamicPanelBuilder.GenerateTopPanel(topPanel, reader, mainForm, cnx,(equipo, idEquipo) =>
            {
                mainForm.Procesos(cnx);
            });
            topPanel.Controls.Add(label);
            //Buttons panel
            DynamicPanelBuilder.GenerateButtons(cnx, buttonsPanel, reader, (proceso, idProceso) =>
            {
                mainForm.Equipos(cnx, idProceso, proceso);
            });
            //Bottom panel
            DynamicPanelBuilder.GenerateBottomPanel(bottomPanel, label.Text, (senders, ex) =>
            {
                UserEntryForm agregarForm = new UserEntryForm(cnx, label.Text, 0, 0, mainForm);
                agregarForm.Show();
            });
            reader.Close();
        }
    }
}
