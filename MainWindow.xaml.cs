using DevSnip.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevSnip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GlobalHotKeys GlobalHotKeys;
        private double collapsedTop;
        private Terminal Terminal;

        public MainWindow()
        {
            InitializeComponent();
            
            //set full screen width
            //this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Left = 0;
            collapsedTop = -1 * this.Height + 3;

            inputCMD.Text = "";
            OutPut.Text = "";
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //register global hokey hooks
            GlobalHotKeys = new GlobalHotKeys();
            GlobalHotKeys.registerHotkeys(this,SlideToggle);

            //terminal
            Terminal = new Terminal(WriteOutPut);
            //Terminal.StartProcess("C:\\Program Files\\Git\\git-bash.exe","",false);
            Terminal.StartProcess();
            inputCMD.Focus();

            //start collapsed
            //SlideToggle();
        }

        protected override void OnClosed(EventArgs e)
        {
            //remove the hotkey hooks
            GlobalHotKeys.removeHokeys();
            base.OnClosed(e);
        }

        private void Quit()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void SlideToggle()
        {
            var collapsed = this.Top < 0;
            if (collapsed)
            {
                this.Top = 0;
            } else
            {
                this.Top = collapsedTop;
            }
            inputCMD.Focus();
        }
        
        private void inputCMD_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                string input = inputCMD.Text;
                if (input == "quit" || input == "exit") Quit();
                WriteOutPut(inputCMD.Text,"");
                inputCMD.Text = "";
                Terminal.sendCommand(input);
            }
        }

        private void WriteOutPut(string output,string extraInfo)
        {
            OutPut.Text += output + Environment.NewLine;
            //scroll down
            //ScrollViewer parent = (ScrollViewer)VisualTreeHelper.GetParent(OutPut);
            //parent.ScrollToBottom();
            OutPutScrollViewer.ScrollToBottom();
            extra.Text = extraInfo;
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            Terminal.StartProcess("C:\\Program Files\\Git\\git-bash.exe", "",false);
        }


        private void Recieve(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

        }

    }
}
