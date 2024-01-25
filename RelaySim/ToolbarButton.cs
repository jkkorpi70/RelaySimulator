/*=======================================================================================================================
   Unit Name: ToolbarButton.cs
   Purpose  : Button with image. Used in toolbar of workbench in Relay Simulator.
   Author   : Juha Koivukorpi
   Date     : 20.05.2021
   TODO     : Make component multipurpose. It's almost identical with ComponentButton.cs
=======================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RelaySim
{
    
    class ToolbarButton : Button
    {

        // Constructor without image
        public ToolbarButton()
        {
            InitButton();
        }

        // Constructor with image
        public ToolbarButton(string Picture)
        {
            PictureBox ButtonImage = new PictureBox("Resources/" + Picture + ".png", 0, 0);
            this.Content = ButtonImage;
            InitButton();
        }
        
        // Set button dimensions
        private void InitButton()
        {
            this.Width = 82;
            this.Height = 27;
        }

        // Set image for button
        public void SetImage(string Picture)
        {
            PictureBox ButtonImage = new PictureBox("Resources/" + Picture + ".png", 0, 0);
            this.Content = ButtonImage;
        }

    }
    
}
