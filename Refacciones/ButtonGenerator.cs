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
    internal class ButtonGenerator
    {
        static int tagValue = 0;
        public static Label GenerateButtons(SqlConnection cnx, Panel panel, SqlDataReader reader, Action<string, int> btnClickHandler)
        {
            // LABEL format
            var label = new Label();
            string labelDatabase;
            label.Font = new Font("Arial", 16, FontStyle.Bold);
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Dock = DockStyle.Top;
            label.Text = reader.GetName(1);
            labelDatabase = reader.GetName(1);
            panel.Controls.Add(label);

            // Create a new BUTTON for each row and add it to the panel
            while (reader.Read())
            {
                var btn = new Button { Text = reader.GetString(1), Tag = reader.GetInt32(0) };
                //var id = reader.GetInt32(0);
                panel.Controls.Add(btn);
                btn.Click += (sender, e) => {
                    tagValue = Convert.ToInt32(btn.Tag);
                    btnClickHandler(btn.Text, tagValue);
                };
                btn.Click += (sender, e) => btnClickHandler(btn.Text, Convert.ToInt32(btn.Tag));
            }

            // Recalculate the size and position of each button
            var buttonWidth = 100;
            var buttonHeight = 50;
            var horizontalSpacing = 10; // horizontal spacing between buttons
            var verticalSpacing = 20; // vertical spacing between buttons
            var labelButtonSpacing = 40; // spacing between label and buttons

            panel.Resize += (sender, e) =>
            {
              
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
                                            label.Bottom + labelButtonSpacing + row * (buttonHeight + verticalSpacing));
                    i++;
                }
            };
            return label;
        }
      
        public static void UpdatePanelSizeAndPosition(Panel panel, Size clientSize)
        {
            panel.Dock = DockStyle.Top;
            panel.Dock = DockStyle.Fill;
        }
    }

}
