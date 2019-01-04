using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;



namespace yazlabProje3
{
    public partial class Form1 : Form
    {
    
        RGBVideo video1;
        int gosterilenFrame = 0;
        int count = 1;
        bool acilanDosya = false;
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 25;

        }

        private void erwerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void dosyaAcBtn_Click(object sender, EventArgs e)
        {

            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "YUV dosyası (*.yuv)|*.yuv";
           
            byte[] fileData;
            openFileDialog.ShowDialog();
            Form2 ayarlar = new Form2();
            ayarlar.ShowDialog();
            acilanDosya = true;
            video1 = new RGBVideo(ayarlar.video1);
            try
            {
                fileData = File.ReadAllBytes(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("dosya okunamadı" + ex.Message, "Error", MessageBoxButtons.OK);
                return;
            }

            int width = video1.genislik, height = video1.yukseklik;
            video1 = RGBConvert.rgbDonustur(fileData, width, height, video1.Format);
            pictureBox1.Image = video1.Source[0];

        }
    
        public static int DefaultWidth { get; set; } = 176;
        public static int DefaultHeight { get; set; } = 144;

        public static YUVFormat DefaultFormat { get; set; } = YUVFormat.YUV420;

        public enum YUVFormat
        {
            YUV444 = 444,
            YUV422 = 422,
            YUV420 = 420
        }
        private void dosyaKaydetBtn_Click(object sender, EventArgs e)
        {
          
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.ShowDialog();
                for (int i = 0; i < video1.Frame; i++)
                {
                    video1.Source[i].Save(saveFileDialog.FileName + " (" + (i + 1) + ").bmp");
                }
              
            
        }

      
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Image = video1.Source[count];
            count++;
            if (count==video1.Source.Length)
            {
                count = 0;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }

    public class RGBVideo
    {
        public Bitmap[] Source { get; set; }
        public Form1.YUVFormat Format { get; set; }
        public int Frame { get; set; }
        public int genislik { get; set; }
        public int yukseklik { get; set; }

        public RGBVideo() : this(null, Form1.DefaultFormat, 0, Form1.DefaultWidth, Form1.DefaultHeight)
        { }

        public RGBVideo(Form1.YUVFormat format, int genislik, int yukseklik) : this(null, format, 0, genislik, yukseklik)
        { }

        public RGBVideo(Form1.YUVFormat format, int frame, int genislik, int yukseklik) : this(new Bitmap[frame], format, frame, genislik, yukseklik)
        { }

        public RGBVideo(Bitmap[] source, Form1.YUVFormat format, int frame, int width, int height)
        {
            Source = source;
            Format = format;
            Frame = frame;
            genislik = width;
            yukseklik = height;
        }

        public RGBVideo(RGBVideo nesne1)
        {
            Source = nesne1.Source;
            Format = nesne1.Format;
            Frame = nesne1.Frame;
            genislik = nesne1.genislik;
            yukseklik = nesne1.yukseklik;
        }
    }
    public class RGBConvert
    {
        public static RGBVideo rgbDonustur(byte[] data, int genislik, int yukseklik, Form1.YUVFormat format)
        {
            if (data.Length <= 0 || data == null) return null;
            switch (format)
            {
                case Form1.YUVFormat.YUV444:
                    return yuv444Donustur(data, genislik, yukseklik);
                case Form1.YUVFormat.YUV422:
                    return yuv422Donustur(data, genislik, yukseklik);
                case Form1.YUVFormat.YUV420:
                    return yuv420Donustur(data, genislik, yukseklik);
                default:
                    return null;
            }
        }

        private static RGBVideo yuv420Donustur(byte[] data, int genislik, int yukseklik)
        {
            int pixelSayisi = genislik * yukseklik,
                frame = (data.Length * 2) / (pixelSayisi * 3),
                ySayisi = pixelSayisi,
                uSayisi = pixelSayisi / 4,
                vCount = uSayisi,
                byteCount = pixelSayisi * 3 / 2;
            RGBVideo video = new RGBVideo(Form1.YUVFormat.YUV420, frame, genislik, yukseklik);
            byte[] rgbData = new byte[ySayisi * 3];
            for (int f = 0, j = 0; f < frame; f++, j = 0)
            {
                byte[] y = new byte[ySayisi], u = new byte[uSayisi], v = new byte[vCount];
                for (int p = 0, i = 0; p < ySayisi; p++)
                {
 y[i++] = data[f * byteCount + j++];
                }
                   
                for (int p = 0, i = 0; p < uSayisi; p++)
                {
  u[i++] = data[f * byteCount + j++];
                }
                  
                for (int p = 0, i = 0; p < vCount; p++)
                {
   v[i++] = data[f * byteCount + j++];
                }
                 
                for (int d = 0; d < ySayisi * 3; d++)
                {
  rgbData[d] = y[d / 3];
                }
                  

                Bitmap bitmap = new Bitmap(genislik, yukseklik, PixelFormat.Format24bppRgb);
                BitmapData bitmapData = bitmap.LockBits(
                           new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                           ImageLockMode.WriteOnly, bitmap.PixelFormat);
                Marshal.Copy(rgbData, 0, bitmapData.Scan0, rgbData.Length);
                bitmap.UnlockBits(bitmapData);

                video.Source[f] = bitmap;
            }

            return video;
        }

        private static RGBVideo yuv422Donustur(byte[] data, int genislik, int yukseklik)
        {
            int pixelSayisi = genislik * yukseklik,
                frame = data.Length / (pixelSayisi * 2),
                ySayisi = pixelSayisi,
                uSayisi = ySayisi / 2,
                vSayisi = uSayisi,
                byteCount = pixelSayisi * 2;

            RGBVideo video = new RGBVideo(Form1.YUVFormat.YUV422, frame, genislik, yukseklik);
            byte[] rgbData = new byte[ySayisi * 3];
            for (int f = 0, j = 0; f < frame; f++, j = 0)
            {
            
                byte[] y = new byte[ySayisi], u = new byte[uSayisi], v = new byte[vSayisi];
                for (int p = 0, i = 0; p < ySayisi; p++)
                    y[i++] = data[f * byteCount + j++];
                for (int p = 0, i = 0; p < uSayisi; p++)
                    u[i++] = data[f * byteCount + j++];
                for (int p = 0, i = 0; p < vSayisi; p++)
                    v[i++] = data[f * byteCount + j++];
                for (int d = 0; d < ySayisi * 3; d++)
                    rgbData[d] = y[d / 3];

                Bitmap bitmap = new Bitmap(genislik, yukseklik, PixelFormat.Format24bppRgb);
                BitmapData bitmapData = bitmap.LockBits(
                           new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                           ImageLockMode.WriteOnly, bitmap.PixelFormat);
                Marshal.Copy(rgbData, 0, bitmapData.Scan0, rgbData.Length);
                bitmap.UnlockBits(bitmapData);

                video.Source[f] = bitmap;
            }

            return video;
        }

        private static RGBVideo yuv444Donustur(byte[] data, int genislik, int yukseklik)
        {
            int pixelCount = genislik * yukseklik;
            int frame = data.Length / (pixelCount * 3);
            int ySayisi = pixelCount;
            int uSayisi = ySayisi;
            int vSayisi = ySayisi;
            int byteCount = pixelCount * 3;

            RGBVideo video = new RGBVideo(Form1.YUVFormat.YUV444, frame, genislik, yukseklik);
            byte[] rgbData = new byte[ySayisi * 3];
            for (int f = 0, j = 0; f < frame; f++, j = 0)
            {
               
                byte[] y = new byte[ySayisi], u = new byte[uSayisi], v = new byte[vSayisi];
                for (int p = 0, i = 0; p < ySayisi; p++)
                    y[i++] = data[f * byteCount + j++];
                for (int p = 0, i = 0; p < uSayisi; p++)
                    u[i++] = data[f * byteCount + j++];
                for (int p = 0, i = 0; p < vSayisi; p++)
                    v[i++] = data[f * byteCount + j++];
                for (int d = 0; d < ySayisi * 3; d++)
                    rgbData[d] = y[d / 3];

                Bitmap bitmap = new Bitmap(genislik, yukseklik, PixelFormat.Format24bppRgb);
                BitmapData bitmapData = bitmap.LockBits(
                           new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                           ImageLockMode.WriteOnly, bitmap.PixelFormat);
                Marshal.Copy(rgbData, 0, bitmapData.Scan0, rgbData.Length);
                bitmap.UnlockBits(bitmapData);

                video.Source[f] = bitmap;
            }

            return video;
        }
    }
}
