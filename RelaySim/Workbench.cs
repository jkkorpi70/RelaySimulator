/*=======================================================================================================================
   Unit Name: Workbench.cs
   Purpose  : Interface and base for all components and classes in relay simulator
   Author   : Juha Koivukorpi
   Date     : 20.05.2021
=======================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Threading;


namespace RelaySim
{
    class Workbench : Canvas
    {
        private readonly static string PATH = "Resources/";
        private string SavedFileName = "";
        private string CurrentDir = System.IO.Directory.GetCurrentDirectory();
        private double SaveTime; // Time for save cursor to tell user that file is saved
        private readonly static DispatcherTimer CursorTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) }; // Timer for cursor while saving file

        private readonly static PictureBox BackGroungImage = new PictureBox(PATH + "board10x10.png", 310, 50); // Background image for circuit
        private readonly static CircuitBoard CBoard = new CircuitBoard();
        
        // Components in build panel
        private readonly static TextBox TBButton = new TextBox();           // Textbox for push button in build component panel
        private readonly static TextBlock TBButtonText = new TextBlock();
        private readonly static TextBox TBRelay = new TextBox();            // Textbox for relay...
        private readonly static TextBlock TBRelayText = new TextBlock();
        private readonly static TextBox TBTime = new TextBox() { Text = "5" };  // Time for timr relay. Default 5 secs
        private readonly static TextBlock TBTimeText = new TextBlock();
        private readonly static TextBox TBLight = new TextBox();            // Text for light bulb...
        private readonly static TextBlock TBLightText = new TextBlock();
        
        // Menu bar buttons
        private readonly static ToolbarButton BtnEdit = new ToolbarButton("Edit_nappi");
        private readonly static ToolbarButton BtnRun = new ToolbarButton("Run_nappi");
        private readonly static ToolbarButton BtnNew = new ToolbarButton("Uusi_nappi");
        private readonly static ToolbarButton BtnLoad = new ToolbarButton("Avaa_nappi");
        private readonly static ToolbarButton BtnSave = new ToolbarButton("Tallenna_nappi");
        private readonly static ToolbarButton BtnSaveAs = new ToolbarButton("Tall.nim_nappi");
        private readonly static ToolbarButton BtnInfo = new ToolbarButton("Info_nappi");
        private readonly static PictureBox LightEdit = new PictureBox(PATH + "led_on.png",93,52);
        private readonly static PictureBox LightRun = new PictureBox(PATH + "led_off.png",93,82);
        private readonly static TextBox CommentBox = new TextBox();
        private readonly static AboutBox1 Abox = new AboutBox1();

        private readonly static Label LblSize = new Label();
        private readonly static Button Btn10x10 = new Button();
        private readonly static Button Btn15x15 = new Button();

        

        public Workbench()
        {
            // Define own parameters
            this.Margin = new Thickness(0, 0, 0, 0);
            this.Background = new SolidColorBrush(Color.FromRgb(235,235,235));
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            
            // Define children - background image and circuit board
            this.Children.Add(BackGroungImage);
            this.Children.Add(CBoard);
            CBoard.Margin = new Thickness(360, 100, 0, 0);
            CBoard.Width = 500;
            CBoard.Height = 500;

            CursorTimer.Tick += OnCursorTimer;

            // Create all buttons
            CreateButtons();
            CreateTextBoxes();
            CreateModeButtons();
            CreateFileButtons();
        }

        // Set circuit board size to 10x10 or 15x15
        public void SetBoard(int size) 
        {
            if (size == 10)
            {
                CBoard.Width = 500;
                CBoard.Height = 500;
                BackGroungImage.SetResourceImage("board10x10");
                Application.Current.MainWindow.Width = 950;
                Application.Current.MainWindow.Height = 700;
            }
            if (size == 15)
            {
                BackGroungImage.SetResourceImage("board15x15");
                CBoard.Width = 750;
                CBoard.Height = 750;
                Application.Current.MainWindow.Width = 1200;
                Application.Current.MainWindow.Height = 950;

                double sWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double sHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                Application.Current.MainWindow.Left = (sWidth / 2) - (1200 / 2);
                Application.Current.MainWindow.Top = (sHeight / 2) - (950 / 2);
                
            }
        }

        // Creates tools tool bar buttons
        private void CreateFileButtons()
        {
            BtnNew.Margin = new Thickness(10, 2, 0, 0);
            this.Children.Add(BtnNew);
            BtnNew.Click += BtnNew_Click;

            BtnLoad.Margin = new Thickness(94, 2, 0, 0);
            this.Children.Add(BtnLoad);
            BtnLoad.Click += BtnLoad_Click;

            BtnSave.Margin = new Thickness(178, 2, 0, 0);
            this.Children.Add(BtnSave);
            BtnSave.Click += BtnSave_Click;

            BtnSaveAs.Margin = new Thickness(262, 2, 0, 0);
            this.Children.Add(BtnSaveAs);
            BtnSaveAs.Click += BtnSaveAs_Click;

            BtnInfo.Margin = new Thickness(346, 2, 0, 0);
            this.Children.Add(BtnInfo);
            BtnInfo.Click += BtnInfo_Click;

            LblSize.HorizontalContentAlignment = HorizontalAlignment.Left;
            LblSize.VerticalAlignment = VerticalAlignment.Top;
            LblSize.Margin = new Thickness(430, 0, 0, 0);
            LblSize.FontSize = 16;
            LblSize.Content = "Piirikaavion koko:";
            this.Children.Add(LblSize);

            Btn10x10.Margin = new Thickness(570, 2, 0, 0);
            Btn10x10.Height = 27;
            Btn10x10.Content = "10x10";
            
            this.Children.Add(Btn10x10);
            Btn10x10.Click += ChangeSize10x10;

            Btn15x15.Margin = new Thickness(610, 2, 0, 0);
            Btn15x15.Height = 27;
            Btn15x15.Content = "15x15";
            this.Children.Add(Btn15x15);
            Btn15x15.Click += ChangeSize15x15;

        }

        // Set circuit board size to 10x10
        private void ChangeSize10x10(object sender, RoutedEventArgs e)
        {
            if (CBoard.GetBoardSize() == 15) // If size was 15x15, ask user before changing size
            {
                if (MessageBox.Show("Kaikki komponentit poistetaan. Haluatko jatkaa?", "Piirikaavio pienennetään!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SetEditMode();
                    CBoard.SetBoardSize(10);
                    CBoard.ClearCircuit();
                    CBoard.Children.Clear();
                    SetBoard(10);
                }
            }
        }

        // Set circuit board size to 15x15
        private void ChangeSize15x15(object sender, RoutedEventArgs e)
        {
            if (CBoard.GetBoardSize() == 10) // If size was 10x10, ask user before changing size
            {
                if (MessageBox.Show("Komponentteja ei poisteta. Haluatko jatkaa?", "Piirikaaviota suurennetaan!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SetEditMode();
                    CBoard.SetBoardSize(15);
                    CBoard.FillWhenExpanded();
                    SetBoard(15);
                }
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------
        // OPTIONS
        //---------------------------------------------------------------------------------------------------------------------------

        // Hour glass cursor was set in save function. Return it to normal when set time has passed
        private void OnCursorTimer(Object sender, EventArgs e)
        {
            SaveTime -= 0.5;
            if (SaveTime <= 0)
            {
                Cursor = Cursors.Arrow;
                CursorTimer.Stop();
            }
        }
        
        //---------------------------------------------------------------------------------------------------------------------------
        // Create a new empty file
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Haluatko jatkaa?", "Piirikaavio tyhjennetään!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                CBoard.ClearCircuit();
                SetEditMode();
                SavedFileName = "";
            }
        }

        // Load existing file
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode();
         
            OpenFileDialog OpenDlg = new OpenFileDialog { Filter = "rsf files(*.rsf) | *.rsf" };
            //OpenDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            OpenDlg.InitialDirectory = CurrentDir;
            OpenDlg.ShowDialog();
            string OFile = OpenDlg.FileName;
            CurrentDir = OpenDlg.Title;
            //OpenDlg.Dispose();
            if (OFile == "") { return; }
            CBoard.Children.Clear();
            SetBoard(CBoard.LoadBoard(OFile));
            SavedFileName = OFile;
            CommentBox.Text = CBoard.CommentText;
         }


        private void BtnSave_Click(object sender, RoutedEventArgs e) { SetEditMode(); SaveDataToFile(); } // Save file
        private void BtnSaveAs_Click(object sender, RoutedEventArgs e) { SetEditMode(); SaveAsDataToFile(); } // SaveAs file (new name)
        
        private void BtnInfo_Click(object sender, RoutedEventArgs e) { Abox.Show(); } // Info about program

        // Save data to existing file
        private void SaveDataToFile()
        {
            String SFile = SavedFileName;
            if (SFile == "") SaveAsDataToFile();
            else
            {
                SaveTime = 1;
                Cursor = Cursors.Wait;
                CursorTimer.Start();
                CBoard.SaveBoard(SFile);
            }
        }

        // Save data to new file
        private void SaveAsDataToFile()
        {
            SaveFileDialog SaveDlg = new SaveFileDialog { DefaultExt = "rsf", Filter = "rsf files(*.rsf) | *.rsf" };
            SaveDlg.ShowDialog();
            string SFile = SaveDlg.FileName;
            if (SFile == "") return;
            //SaveDlg.Dispose();
            SavedFileName = SFile;
            CBoard.SaveBoard(SFile);
        }

        //---------------------------------------------------------------------------------------------
        // Creates run and edit buttons
        private void CreateModeButtons()
        {
            BtnEdit.Margin = new Thickness(10, 50, 0, 0);
            this.Children.Add(BtnEdit);
            BtnEdit.Click += BtnEdit_Click;

            this.Children.Add(LightEdit);

            BtnRun.Margin = new Thickness(10, 80, 0, 0);
            this.Children.Add(BtnRun);
            BtnRun.Click += BtnRun_Click;

            this.Children.Add(LightRun);

            CBoard.SetMode(0);
        }

        // Create all textboxes in edit panel
        private void CreateTextBoxes()
        {
            TBButtonText.Margin = new Thickness(20,310,0,0);
            TBButtonText.Text = "Painonapin tunnus: S";
            this.Children.Add(TBButtonText);

            TBButton.Margin = new Thickness(135, 310, 0, 0);
            TBButton.Width = 20;
            TBButton.Text = "0";
            this.Children.Add(TBButton);
            TBButton.TextChanged += TBButton_Changed;

            TBRelayText.Margin = new Thickness(20, 370, 0, 0);
            TBRelayText.Text = "Releen tunnus:        K";
            this.Children.Add(TBRelayText);

            TBRelay.Margin = new Thickness(135, 370, 0, 0);
            TBRelay.Width = 20;
            TBRelay.Text = "1";
            this.Children.Add(TBRelay);
            TBRelay.TextChanged += TBRelay_Changed;

            TBTimeText.Margin = new Thickness(20, 425, 0, 0);
            TBTimeText.Text = "Aikarele (sek):";
            this.Children.Add(TBTimeText);

            TBTime.Margin = new Thickness(100, 425, 0, 0);
            TBTime.Width = 20;
            this.Children.Add(TBTime);
            TBTime.TextChanged += TBTime_Changed;

            TBLightText.Margin = new Thickness(20,490,0,0);
            TBLightText.Text = "Lamppu:";
            this.Children.Add(TBLightText);

            TBLight.Margin = new Thickness(100, 490, 0, 0);
            TBLight.Width = 20;
            TBLight.Text = "1";
            this.Children.Add(TBLight);
            TBLight.TextChanged += TBLight_Changed;

            CommentBox.Margin = new Thickness(20,540,0,0);
            CommentBox.Width = 274;
            CommentBox.Height = 110;
            CommentBox.AcceptsReturn = true;
            CommentBox.AppendText("Kirjoita kommentti.");
            this.Children.Add(CommentBox);
            CommentBox.TextChanged += CommentBox_TextChanged;
            
        }

        // Box for adding own comments about circuit
        private void CommentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CBoard.CommentText = CommentBox.Text;
        }

        // Create all component buttons in edit panel
        private void CreateButtons()
        {
            //Cross shaped line / wire
            ComponentButton CB0 = new ComponentButton("line_uldr") { Margin = new Thickness(130, 50, 0, 0) };
            this.Children.Add(CB0);
            CB0.Click += CB0_Click;

            // T-shaped lines / wires
            ComponentButton CB1 = new ComponentButton("line_ldr") { Margin = new Thickness(240, 50, 0, 0) };
            this.Children.Add(CB1);
            CB1.Click += CB1_Click;

            ComponentButton CB2 = new ComponentButton("line_ulr") { Margin = new Thickness(185, 105, 0, 0) };
            this.Children.Add(CB2);
            CB2.Click += CB2_Click;

            ComponentButton CB3 = new ComponentButton("line_udr") { Margin = new Thickness(185, 50, 0, 0) };
            this.Children.Add(CB3);
            CB3.Click += CB3_Click;

            ComponentButton CB4 = new ComponentButton("line_uld") { Margin = new Thickness(240, 105, 0, 0) };
            this.Children.Add(CB4);
            CB4.Click += CB4_Click;

            // Line / wires - Corners
            ComponentButton CB5 = new ComponentButton("line_dr") { Margin = new Thickness(185, 170, 0, 0) };
            this.Children.Add(CB5);
            CB5.Click += CB5_Click;

            ComponentButton CB6 = new ComponentButton("line_ur") { Margin = new Thickness(185, 225, 0, 0) };
            this.Children.Add(CB6);
            CB6.Click += CB6_Click;

            ComponentButton CB7 = new ComponentButton("line_ld") { Margin = new Thickness(240, 170, 0, 0) };
            this.Children.Add(CB7);
            CB7.Click += CB7_Click;

            ComponentButton CB8 = new ComponentButton("line_ul") { Margin = new Thickness(240, 225, 0, 0) };
            this.Children.Add(CB8);
            CB8.Click += CB8_Click;

            // Line / wires - Straght lines
            ComponentButton CB9 = new ComponentButton("line_ud") { Margin = new Thickness(130, 170, 0, 0) };
            this.Children.Add(CB9);
            CB9.Click += CB9_Click;

            ComponentButton CB10 = new ComponentButton("line_lr") { Margin = new Thickness(130, 225, 0, 0) };
            this.Children.Add(CB10);
            CB10.Click += CB10_Click;

            // Push buttons. Normally close (NC) and normally open (NO)
            ComponentButton CB11 = new ComponentButton("btn_NC_c") { Margin = new Thickness(185, 290, 0, 0) };
            this.Children.Add(CB11);
            CB11.Click += CB11_Click;

            ComponentButton CB12 = new ComponentButton("btn_NO_o") { Margin = new Thickness(240, 290, 0, 0) };
            this.Children.Add(CB12);
            CB12.Click += CB12_Click;

            // Relay contacts. Normally close (NC) and normally open (NO)
            ComponentButton CB13 = new ComponentButton("contact_NC_c") { Margin = new Thickness(185, 355, 0, 0) };
            this.Children.Add(CB13);
            CB13.Click += CB13_Click;

            ComponentButton CB14 = new ComponentButton("contact_NO_o") { Margin = new Thickness(240, 355, 0, 0) };
            this.Children.Add(CB14);
            CB14.Click += CB14_Click;

            // Relay coils. Normal, on timer and off timer
            ComponentButton CB15 = new ComponentButton("coil_norm") { ToolTip = "Normaali rele", Margin = new Thickness(130, 410, 0, 0) };
            this.Children.Add(CB15);
            CB15.Click += CB15_Click;

            ComponentButton CB16 = new ComponentButton("coil_ontimer") { ToolTip = "Vetohidastettu rele", Margin = new Thickness(185, 410, 0, 0) };
            this.Children.Add(CB16);
            CB16.Click += CB16_Click;

            ComponentButton CB17 = new ComponentButton("coil_offtimer") { ToolTip = "Päästöhidastettu rele", Margin = new Thickness(240, 410, 0, 0) };
            this.Children.Add(CB17);
            CB17.Click += CB17_Click;
            
            // Light bulbs. Green, yellow and red
            ComponentButton CB18 = new ComponentButton("light_g_off") { ToolTip = "Vihreä lamppu", Margin = new Thickness(130, 475, 0, 0) };
            this.Children.Add(CB18);
            CB18.Click += CB18_Click;

            ComponentButton CB19 = new ComponentButton("light_y_off") { ToolTip = "Keltainen lamppu", Margin = new Thickness(185, 475, 0, 0) };
            this.Children.Add(CB19);
            CB19.Click += CB19_Click;

            ComponentButton CB20 = new ComponentButton("light_r_off") { ToolTip = "Punainen lamppu", Margin = new Thickness(240, 475, 0, 0) };
            this.Children.Add(CB20);
            CB20.Click += CB20_Click;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------
        // When a component (above) is selected, set it as current component - component which is added to circuit board when it's clicked on selected position
        //-------------------------------------------------------------------------------------------------------------------------------------------------
        // Cross
        private void CB0_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_uldr"); }
        // T-shaped
        private void CB1_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_ldr"); }
        private void CB2_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_ulr"); }
        private void CB3_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_udr"); }
        private void CB4_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_uld"); }
        // Corners
        private void CB5_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_dr"); }
        private void CB6_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_ur"); }
        private void CB7_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_ld"); }
        private void CB8_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_ul"); }
        // Straigth lines
        private void CB9_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_ud"); }
        private void CB10_Click(object sender, RoutedEventArgs e) { CBoard.SetComponent("line_lr"); }
        // Buttons
        private void CB11_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentBtn("btn_NC_c", TBButton.Text); }
        private void CB12_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentBtn("btn_NO_o", TBButton.Text); }
        // Contacts
        private void CB13_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentContact("contact_NC_c", TBRelay.Text); }
        private void CB14_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentContact("contact_NO_o", TBRelay.Text); }
        // Coil
        private void CB15_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentCoil("coil_norm", TBRelay.Text, Convert.ToDouble(TBTime.Text)); }
        private void CB16_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentCoil("coil_ontimer", TBRelay.Text, Convert.ToDouble(TBTime.Text)); }
        private void CB17_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentCoil("coil_offtimer", TBRelay.Text, Convert.ToDouble(TBTime.Text)); }
        // Lights
        private void CB18_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentLight("light_g_off", TBLight.Text); }
        private void CB19_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentLight("light_y_off", TBLight.Text); }
        private void CB20_Click(object sender, RoutedEventArgs e) { CBoard.SetComponentLight("light_r_off", TBLight.Text); }

        // Mode buttons - Edit and Run
        private void BtnEdit_Click(object sender, RoutedEventArgs e) { SetEditMode();}

        private void BtnRun_Click(object sender, RoutedEventArgs e) { SetRunMode(); }

        // Push button id number is changed on edit panel
        private void TBButton_Changed(object sender, TextChangedEventArgs e)
        {
            try
            {
                int temp = Convert.ToInt16(TBButton.Text);
                if (temp < 0 || temp > 9) { MessageBox.Show("Lukuarvon pitää olla väliltä 0-9.", "Virheellinen Syöte!"); temp = 0; TBButton.Text = "0"; }
                CBoard.SetButtonID(temp);
            }
            catch { MessageBox.Show("Kenttään kelpaa vain lukuarvot", "Virheellinen Syöte!"); TBButton.Text = "0"; }
        }

        // Relay id number is changed on edit panel
        private void TBRelay_Changed(object sender, TextChangedEventArgs e)
        {
            try
            {
                int temp = Convert.ToInt16(TBRelay.Text);
                if (temp < 1 || temp > 9) { MessageBox.Show("Lukuarvon pitää olla väliltä 1-9.", "Virheellinen Syöte!"); temp = CBoard.FreeRelay; TBRelay.Text = Convert.ToString(CBoard.FreeRelay); }
                CBoard.SetRelayID(temp);
            }
            catch { MessageBox.Show("Kenttään kelpaa vain lukuarvot", "Virheellinen Syöte!"); TBRelay.Text = Convert.ToString(CBoard.FreeRelay); }
        }

        // Time relay time is changed on edit panel
        private void TBTime_Changed(object sender, TextChangedEventArgs e)
        {
            try { CBoard.SetRelayTime(Convert.ToDouble(TBTime.Text));}
            catch { MessageBox.Show("Kenttään kelpaa vain lukuarvot", "Virheellinen Syöte!"); TBTime.Text = "1";}
        }
        
        // light bulb number is changed on edit panel
        private void TBLight_Changed(object sender, TextChangedEventArgs e)
        {
            try
            {
                int temp = Convert.ToInt16(TBLight.Text);
                if (temp < 1 || temp > 9) { MessageBox.Show("Lukuarvon pitää olla väliltä 1-9.", "Virheellinen Syöte!"); temp = 1; TBLight.Text = "1"; }
                CBoard.SetLightID(temp);
            }
            catch { MessageBox.Show("Kenttään kelpaa vain lukuarvot", "Virheellinen Syöte!"); TBLight.Text = Convert.ToString(CBoard.FreeLight); }
        }

        // Enter to edit mode
        private void SetEditMode()
        {
            CBoard.SetMode(0);
            LightEdit.SetResourceImage("led_on");
            LightRun.SetResourceImage("led_off");
            TBButton.IsEnabled = true;
            TBRelay.IsEnabled = true;
            TBTime.IsEnabled = true;
        }

        // Enter to run mode
        private void SetRunMode()
        {
            CBoard.SetMode(1);
            LightEdit.SetResourceImage("led_off");
            LightRun.SetResourceImage("led_on");
            TBButton.IsEnabled = false;
            TBRelay.IsEnabled = false;
            TBTime.IsEnabled = false;
        }
    }
}