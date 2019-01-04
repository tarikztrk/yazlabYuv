using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yazlabProje3
{
    public partial class Form2 : Form
    {
        public RGBVideo video1;
        public Form2()
        {

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Form1.YUVFormat format = (comboBox1.SelectedItem.ToString() == "yuv444") ? Form1.YUVFormat.YUV444 : (comboBox1.SelectedItem.ToString() == "yuv422") ? Form1.YUVFormat.YUV422 : Form1.YUVFormat.YUV420;
            int width = 0, height = 0;

            if (Int32.TryParse(textBox2.Text, out width))
            {
                MessageBox.Show("değer atama başarılı", "", MessageBoxButtons.OK);
            }
            if (!Int32.TryParse(textBox1.Text, out height))
            {
                MessageBox.Show("değer atama başarılı", "", MessageBoxButtons.OK);
            }

            video1 = new RGBVideo(format, width, height);
            this.Close();


        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = Form1.DefaultHeight.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = Form1.DefaultWidth.ToString();
        }
    }
}
