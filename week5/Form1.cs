using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace week5
{
    public partial class Form1 : Form
    {
        public int itask=0;
        public List<double> data = new List<double>();
        public List<double> EnlargeData = new List<double>();
        public List<double> BaseData = new List<double>();

        public Form1()
        {
            InitializeComponent();
            openfile();                 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            openfile();
        }

        public void openfile()             //開啟檔案並show在picturebox
        {
            OpenFileDialog newfile = new OpenFileDialog();
            newfile.Filter = "All Files|*.*";
            if (newfile.ShowDialog() == DialogResult.OK)
            {
                string filepath = newfile.FileName;
                pictureBox1.Image = Image.FromFile(filepath);
            }
        }

        private void StartBTN_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            itask = 1;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            process();
        }
       
        private bool process()
        {
            switch (itask)
            {
                case 0:
                    break;
                case 1:
                    itask = 2;
                    Transfer_To_Data();            //將圖片轉數據並畫出
                    break;
                case 2:
                    timer1.Stop();
                    if (chart1.Series[0].AxisLabel != null) {
                        MessageBox.Show("1請輸入放大倍率!!");
                    }
                    StartBTN.Enabled = false;
                    StopBTN.Text = "繼續";
                    break;
                case 3:
                    itask = 4;
                    enlarge();                    //將數據放大並畫出
                    break;
                case 4:
                    timer1.Stop();
                    MessageBox.Show("是否扣除基線");
                    StopBTN.Text = "繼續";
                    break;
                case 5:
                    itask = 0;
                    StopBTN.Text = "暫停";
                    break;
            }
            return true; 
        }

        private void StopBTN_Click(object sender, EventArgs e)
        {
            if(itask == 2)
            {
                if (EnlargeTXB.Text == "")
                    MessageBox.Show("請輸入放大倍率!!");
                else
                {
                    itask = 3;
                    timer1.Start();
                    StopBTN.Text = "暫停";
                }
                    
            }
            
            if (itask == 4)
            {
                double basedata=0, totaldata=0;
                if (checkbox.Checked)
                {
                   for(int i = 0; i < 51; i++)          //計算基線值
                    {
                        totaldata += EnlargeData[i];
                    }
                    basedata = totaldata / 50;
                    for(int j=0; j < EnlargeData.Count; j++)
                    {
                        BaseData.Add(EnlargeData[j] - basedata);
                    }
                    chart1.Series[0].Points.Clear();
                    chart1.Series[0].Points.AddXY(0, 0);
                    for (int i = 0; i < BaseData.Count; i++)
                    {
                        chart1.Series[0].Points.AddXY(i, BaseData[i]);
                    }
                    itask = 5;
                    timer1.Start();
                }
            }
        }
        public void Transfer_To_Data()                       //將圖片轉灰階並畫出波形
        {
            int gray;
            double total = 0;
            Bitmap bitmap = new Bitmap(pictureBox1.Image);
            for (int x = 0; x < bitmap.Width; x++)
            {
                total = 0;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    gray = (
                    bitmap.GetPixel(x, y).R +
                    bitmap.GetPixel(x, y).G +
                    bitmap.GetPixel(x, y).B) / 3;
                    Color color = Color.FromArgb(gray, gray, gray);
                    bitmap.SetPixel(x, y, color);
                    total += gray;
                }
                data.Add(total / 960);
            }
            chart1.ChartAreas[0].AxisY.Maximum = 260;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            for (int i = 0; i < data.Count; i++)
            {
                chart1.Series[0].Points.AddXY(i, data[i]);
            }
        }
        public void enlarge()     //將數據放大並畫出
        {
            for (int i = 0; i < data.Count; i++)
            {
                EnlargeData.Add(data[i] * double.Parse(EnlargeTXB.Text));
            }
            chart1.Series[0].Points.Clear();
            chart1.Series[0].Points.AddXY(0, 0);
            for (int i = 0; i < EnlargeData.Count; i++)
            {
                chart1.Series[0].Points.AddXY(i, EnlargeData[i]);
            }
        }

        private void ExitBTN_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
