using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Drawing;


namespace ImageUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        String filePath;
        String fileName;
        byte[] imageData;
        // maxImageSize ne kadar büyük olursa, o kadar büyük dosya boyutu yükleyebiliyoruz. Burada 1MB olarak aldım
        int maxImageSize=1000000; 
        
        SqlConnection conn = new SqlConnection(@"Server= .\; Integrated Security= true; Database= piknik_7_mayis");
        //SqlConnection conn = new SqlConnection(@"Server= .\; Integrated Security= true; Database= ImageStorage");
        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            // Sadece kapalı ise açması için bu scope'u kullandım, yoksa veritabanına üst üste bağlantı açmaya çalışıyor
            if (conn.State == ConnectionState.Closed) 
            {
                conn.Open();
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            Nullable<bool> result = dialog.ShowDialog();
            if (result==true)
            {
                filePath = dialog.FileName;
                BrowseTextBox.Text = filePath;
                fileName = dialog.SafeFileName;
                ImageBox1.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"UPDATE marketler SET picture5=@imageData WHERE name='UYSAL MARKET';";
            //cmd.CommandText = @"INSERT INTO ImageTable(fileName, imageData) VALUES(@fileName, @imageData)";
            //cmd.Parameters.Add("@fileName", SqlDbType.NChar, 50);
            cmd.Parameters.Add("@imageData", SqlDbType.VarBinary, 10000000);
            cmd.Prepare();

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes(maxImageSize);

            //cmd.Parameters["@filename"].Value = fileName;
            cmd.Parameters["@imageData"].Value = imageData;
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Upload Successful!", "Successful!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Upload failed!", "Failed!", MessageBoxButton.OK, MessageBoxImage.Error );
            }
            conn.Close();

        }

        private void ShowImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("SELECT imageData FROM ImageTable WHERE filename='" + imageNameBox.Text.ToString() + "'", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                
                byte[] picData = reader.GetValue(0) as byte[];
                if (picData != null)
                {
                    MemoryStream ms = new MemoryStream(picData);
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(ms);
                    BitmapSource bmsource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    ImageBox2.Source = bmsource;
                    ImageBox2.Stretch = Stretch.UniformToFill;

                }
                
            }
            conn.Close();
            
        }

        private void imageNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key ==Key.Enter)
            {
                //PerformClick Metodu yerine WPF'de bunu kullanabiliyoruz
                ShowImageButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); 
            }
        }
        


    }
}
