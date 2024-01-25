/*=======================================================================================================================
   Unit Name: ComponentButton.cs
   Purpose  : Button with image. Used in component panel of workbench in Relay Simulator.
   Author   : Juha Koivukorpi
   Date     : 20.05.2021
   TODO     : Make component multipurpose. It's almost identical with ToolbarButton.cs
=======================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace RelaySim
{
    class ComponentButton : Button
    {
        // Constructor without image
        public ComponentButton()
        {
            InitButton();
        }            
        
        // Constructor with image
        public ComponentButton(string Picture)
        {
            PictureBox ButtonImage = new PictureBox("Resources/" + Picture + ".png",0,0);
            this.Content = ButtonImage;
            InitButton();
        }

        // Set button dimensions
        private void InitButton()
        {
            this.Width = 54;
            this.Height = 54;

        }

        // Set image for button
        public void SetImage(string Picture)
        {
            PictureBox ButtonImage = new PictureBox("Resources/" + Picture + ".png", 0, 0);
            this.Content = ButtonImage;
        }

    }
}
