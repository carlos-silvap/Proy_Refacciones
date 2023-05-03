using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Refacciones
{
    internal class DynamicPanelBuilder
    {
        static int tagValue = 0;
        public static void GenerateButtons(SqlConnection cnx, Panel panel, SqlDataReader reader, Action<string, int> btnClickHandler)
        {
            // Create a new BUTTON for each row and add it to the panel
            var buttons = new List<Button>();
            while (reader.Read())
            {
                var btn = new Button { Text = reader.GetString(1), Tag = reader.GetInt32(0) };
                var orden = reader.GetInt32(2);
                if (orden > 0 && orden <= buttons.Count)
                {
                    buttons.Insert(orden - 1, btn);
                }
                else
                {
                    buttons.Add(btn);
                }
                btn.Click += (sender, e) => {
                    tagValue = Convert.ToInt32(btn.Tag);
                    btnClickHandler(btn.Text, tagValue);
                };
                btn.Click += (sender, e) => btnClickHandler(btn.Text, Convert.ToInt32(btn.Tag));
            }

            // Add the buttons to the panel in the correct order
            foreach (var button in buttons)
            {
                panel.Controls.Add(button);
            }

            // Recalculate the size and position of each button
            var buttonWidth = 100;
            var buttonHeight = 50;
            var horizontalSpacing = 10; // horizontal spacing between buttons
            var verticalSpacing = 20; // vertical spacing between buttons
            var labelButtonSpacing = 40; // spacing between label and buttons


            // Set up autoscroll
            panel.AutoScroll = true;
            panel.AutoScrollMargin = new Size(0, 10); // margin to prevent buttons from being too close to the edge
            
            panel.AutoScrollMinSize = new Size(0, buttons.Count * (buttonHeight + verticalSpacing) + labelButtonSpacing);


            panel.Resize += (sender, e) =>
            {
                panel.AutoScrollMinSize = new Size(0, (buttonHeight + verticalSpacing) + labelButtonSpacing);
                if (panel.Width < 300)
                {
                    panel.Width = 400;
                }
                var maxButtonsPerRow = panel.Width / (buttonWidth + horizontalSpacing);
                var i = 0;

                foreach (var control in panel.Controls.OfType<Button>())
                {
                    var col = i % maxButtonsPerRow;
                    var row = i / maxButtonsPerRow;
                    control.Size = new Size(buttonWidth, buttonHeight);
                    control.Location = new Point(col * (buttonWidth + horizontalSpacing),
                                            30 + labelButtonSpacing + row * (buttonHeight + verticalSpacing));
                    i++;
                }

            };
        }

        public static Label GenerateTopPanel(Panel panel, SqlDataReader reader, MainForm mainForm, SqlConnection cnx, Action<string, int> btnClickHandler)
        {
            // LABEL format
            var label = new Label();
            string labelDatabase;
            label.Font = new Font("Arial", 16, FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Dock = DockStyle.Top;
            label.Text = reader.GetName(1);
            labelDatabase = reader.GetName(1);
            
            
            if(label.Text != "Procesos" && label.Text != "Secciones") 
            {
                var backButton = new Button();
                backButton.Text = "Regresar";
                backButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                backButton.Size = new Size(75, 23);
                backButton.Location = new Point(panel.Width - backButton.Width - 10, 5);
                //// Add a click event handler to the button
                backButton.Click += (sender, e) =>
                {
                    btnClickHandler(backButton.Text, tagValue);
                    //mainForm.Procesos(cnx);
                };
                //backButton.Click += (sender, e) => btnClickHandler(btn.Text, Convert.ToInt32(btn.Tag));
                panel.Controls.Add(backButton);
            }
            return label;
        }

        public static void GenerateBottomPanel() 
        {
        }
        public static void UpdatePanelSizeAndPosition(Panel panel, Size clientSize)
        {
            panel.Dock = DockStyle.Fill;
        }
    }

}
