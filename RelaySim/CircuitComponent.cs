/*=======================================================================================================================
   Unit Name: CircuitComponent.cs
   Purpose  : Contains some properties of the components which are used in Circuitboard.cs. Derived from PictureBox, which
              shows wanted image.
   Author   : Juha Koivukorpi
   Date     : 20.05.2021
   TODO     : Something could be moved to CircuitInfo.cs
=======================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RelaySim
{
    class CircuitComponent : PictureBox
    {
        public string Component { get; } // wire, button, relaycoil, contact
        public string ComponentID { get; } // eg. S0, K1 ...

        public int IsConnectedUp { get; }
        public int IsConnectedLeft { get; }
        public int IsConnectedDown { get; }
        public int IsConnectedRight { get; }
        public int IsConductive { get; set; } // 1 = conductive (NC), 0 = not conductive (NO)
        public double DelayTime { get; set; } // Time for timed relays. Set 0 for normal relays
        public int IsConnectedPositive { get; set; } // 1 connected, 0 not connected
        public int IsConnectedNegative { get; set; } // 1 connected, 0 not connected
        private string componentfilename;
        public string ComponentFilelName
        {
            get { return componentfilename; }
            set { componentfilename = value; SetResourceImage(value); }
        }

        /// <summary>
        /// Creates Component with all attributes
        /// Directions indicate where component is connected
        /// </summary>
        public CircuitComponent(string component, string id, int up, int left, int down, int right, int conductive)
        {
            Component = component;
            ComponentID = id;
            IsConnectedUp = up;
            IsConnectedLeft = left;
            IsConnectedDown = down;
            IsConnectedRight = right;
            IsConductive = conductive; // Is component conductive in normal state
        }
        /// <summary>
        /// Creates WIRES with connections:  Set 1 if connected to direction and set 0 if not.
        /// </summary>
        public CircuitComponent(int ConnectUp, int ConnectLeft, int ConnectDown, int ConnectRight)
        {
            IsConnectedUp = ConnectUp;
            IsConnectedLeft = ConnectLeft;
            IsConnectedDown = ConnectDown;
            IsConnectedRight = ConnectRight;

            if (ConnectUp == 0 && ConnectLeft == 0 && ConnectDown == 0 && ConnectRight == 0)
            {
                Component = "line_empty"; // It's just an empty box
                IsConductive = 0;
                ComponentFilelName = "line_empty";
            }
            else
            {
                Component = "wire";
                IsConductive = 1;
            }
            ComponentID = " "; // Not any special component like button or relay
        }

        /// <summary>
        /// Creates button or contact:  NO = 0 or NC = 1. If valid type is not given, default set is contact and NO.
        /// NO = normal open, NC = normal closed
        /// </summary>
        public CircuitComponent(string component, int isConductive, string ID)
        {
            IsConnectedUp = 1;
            IsConnectedLeft = 0;
            IsConnectedDown = 1;
            IsConnectedRight = 0;
            string str = "NO";
            if (isConductive == 1) str = "NC";
            Component = component + str;
            IsConductive = isConductive;
            ComponentID = ID;

        }

        /// <summary>
        /// Creates component: RELAYCOIL. if time (sec) is 0, normal relay is created. If bigger, timerelay is created. 
        /// </summary>
        public CircuitComponent(double Time, string ID, string type)
        {
            IsConnectedUp = 1;
            IsConnectedLeft = 0;
            IsConnectedDown = 1;
            IsConnectedRight = 0;
            if (Time == 0) Component = "relaycoil";
            else Component = type + "coil";
            DelayTime = Time;
            ComponentID = ID;
            IsConductive = 1;

        }
        /// <summary>
        /// Creates component: LIGHT. g = green, y = yellow, r = red.
        /// </summary>
        public CircuitComponent(string ID, string type)
        {
            IsConnectedUp = 1;
            IsConnectedLeft = 0;
            IsConnectedDown = 1;
            IsConnectedRight = 0;
            ComponentID = ID;
            IsConductive = 1;
            if (type == "g") Component = "greenlight";
            if (type == "y") Component = "yellowlight";
            if (type == "r") Component = "redlight";
        }
    }
}
