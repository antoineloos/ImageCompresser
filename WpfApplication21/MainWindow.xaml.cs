using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication21
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap courantimg;
        private OpenFileDialog openFileDialog1;
        private BackgroundWorker bgw;
        private List<string> listpath;
        private Int64 nivCompr;
        private string outputDir;

        public MainWindow()
        {
            InitializeComponent();
            PaletteHelper tmp = new PaletteHelper();
            tmp.ReplacePrimaryColor("blue");
            tmp.ReplaceAccentColor("amber");
            outputDir = outputtxt.Text = "Compressed";
            openFileDialog1 = new OpenFileDialog();
            listpath = new List<string>();
            bgw = new BackgroundWorker();
            bgw.ProgressChanged +=bgw_ProgressChanged;
            bgw.RunWorkerCompleted +=bgw_RunWorkerCompleted;
            bgw.DoWork +=bgw_DoWork;
            bgw.WorkerReportsProgress = true;
            nivCompr = 50L;
        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgb.Value = e.ProgressPercentage;
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            var cmpt = 0;
 	        foreach (String file in listpath) 
                {
                   
                    try
                    {
                       // Get a bitmap.
                        Bitmap bmp1 = new Bitmap(file);
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                        // Create an Encoder object based on the GUID
                        // for the Quality parameter category.
                        System.Drawing.Imaging.Encoder myEncoder =
                            System.Drawing.Imaging.Encoder.Quality;

                        // Create an EncoderParameters object.
                        // An EncoderParameters object has an array of EncoderParameter
                        // objects. In this case, there is only one
                        // EncoderParameter object in the array.
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);

                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, nivCompr);
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        bmp1.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + outputDir + "\\" + System.IO.Path.GetFileName(file), jpgEncoder, myEncoderParameters);
                        bgw.ReportProgress((int)((double)cmpt / (double)listpath.Count() * 100f));

                        bmp1.Dispose();
                        

                        
                    }
                    
                    catch (Exception ex)
                    {
                       MessageBox.Show(ex.ToString());
                    }
                    cmpt++;
                }
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dlg.IsOpen = true;
            outputtxt.IsEnabled = true;
            outputDir = "Compressed";
            listpath.Clear();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (outputtxt.Text == "")
            {
                outputtxt.Text = outputDir = "Compressed";
            };
            
                InitializeOpenFileDialog();
                var res = openFileDialog1.ShowDialog();
                if (res != null && res == true)
                {
                    listpath.AddRange(openFileDialog1.FileNames.AsEnumerable<string>());



                    if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + outputDir))
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + outputDir);
                    }
                    outputtxt.IsEnabled = false;
                    bgw.RunWorkerAsync();
                }
            
        }

        private void InitializeOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "My Image Browser";
        }



        private ImageCodecInfo GetEncoder(ImageFormat format)
            {

                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.FormatID == format.Guid)
                    {
                        return codec;
                    }
                }
                return null;
            }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            nivCompr = Convert.ToInt64((int)e.NewValue);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dlg.IsOpen = false;
            prgb.Value = 0;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            outputDir = ((TextBox)e.Source).Text;
            
        }
    }
}
