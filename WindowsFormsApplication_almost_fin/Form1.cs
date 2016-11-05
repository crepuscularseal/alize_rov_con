using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;

using System.IO.Ports;

using System.Diagnostics;

namespace WindowsFormsApplication_almost_fin
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        string xy_solar;  //xy平面的運動形式
        string x_power_handle; //x向推力的持續
        string z_power_handle; //z向推力的持續

        SerialPort port;

        public Form1()
        {
            InitializeComponent();
            init();
        }
       
         //打開PORT，並測試，為PORT溝通之物
        private void init()
        {
            port = new SerialPort();
            port.PortName = "COM3";
            port.BaudRate = 9600;
            
            try 
            {
                port.Open();
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }

        //外框載入，選擇視訊鏡頭輸入源
        private void Form1_Load(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo device in videoDevices)
            {
                comboBox1.Items.Add(device.Name); 
            }
            comboBox1.SelectedIndex = 0;
            
            videoSource = new VideoCaptureDevice();
        }

        //確定哪個為我們要的視訊鏡頭輸入源
        private void button1_Click(object sender, EventArgs e)
        {
            if (videoSource.IsRunning)
            {
                videoSource.Stop();
                pictureBox1.Image = null;
                pictureBox1.Invalidate();
            }
            else 
            {
                videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
                //設定新的 frame event handler
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                videoSource.Start();
            }
        }

        //訊號源框架，將載入物設為點陣圖檔
        private void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;
        }

        //關閉圖源的時機
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource.IsRunning)
                videoSource.Stop();
            port.WriteLine("5,0,0");
        }
        
        
        //以下為控制xy_solar的，即控制xy平面運動模式的程式--------是為向量

        //BTU 2 為按下一個RADIO 按鍵後，再按下BTU2 輸出訊息 
        //XY平面的第一種操控法-滑鼠選擇
        private void button2_Click(object sender, EventArgs e)
        {
            Get_xy_dir(radioButton1);
            Get_xy_dir(radioButton2);
            Get_xy_dir(radioButton3);
            Get_xy_dir(radioButton4);
            Get_xy_dir(radioButton5);
            Get_xy_dir(radioButton6);
            Get_xy_dir(radioButton7);
            Get_xy_dir(radioButton8);
            Get_xy_dir(radioButton9);
        }

        private void Get_xy_dir(RadioButton rdobutton)
        {
            if (rdobutton.Checked)
            {
                // MessageBox.Show(rdobutton.Text.ToString());
                //port.WriteLine(rdobutton.ToString());
                xy_solar = rdobutton.Text.ToString();
                port.WriteLine(xy_solar + "," + x_power_handle + "," + z_power_handle);
            }
        }

        //XY平面的第二種控法-鍵盤
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                iniarbt();
                radioButton2.Checked = true;
                Get_xy_dir(radioButton2);
            }
            if (e.KeyCode == Keys.A)
            {
                iniarbt();
                radioButton4.Checked = true;
                Get_xy_dir(radioButton4);
            }
            if (e.KeyCode == Keys.S)
            {
                iniarbt();
                radioButton8.Checked = true;
                Get_xy_dir(radioButton8);
            }
            if (e.KeyCode == Keys.D)
            {
                iniarbt();
                radioButton6.Checked = true;
                Get_xy_dir(radioButton6);
            }
            
            //左後和右後
            if (e.KeyCode == Keys.Z)
            {
                iniarbt();
                radioButton7.Checked = true;
                Get_xy_dir(radioButton7);
            }
            if (e.KeyCode == Keys.X)
            {
                iniarbt();
                radioButton9.Checked = true;
                Get_xy_dir(radioButton9);
            }
            //左後和右後
            if (e.KeyCode == Keys.Q)
            {
                iniarbt();
                radioButton1.Checked = true;
                Get_xy_dir(radioButton1);
            }
            if (e.KeyCode == Keys.E)
            {
                iniarbt();
                radioButton3.Checked = true;
                Get_xy_dir(radioButton3);
            }


            //水平停機
            if (e.KeyCode == Keys.P)
            {
                iniarbt();
                radioButton5.Checked = true;
                Get_xy_dir(radioButton5);
            }
        }

        //將全盤先淨空
        private void iniarbt()
        {
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            radioButton7.Checked = false;
            radioButton8.Checked = false;
            radioButton9.Checked = false;
        }

        //深淺推力的量值
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if(port.IsOpen)
            {
                z_power_handle = var_z_trackBar1.Value.ToString();
                port.WriteLine(xy_solar+","+x_power_handle+","+ z_power_handle);
                vertical_now.Text = "vertical =" + var_z_trackBar1.Value.ToString();
             }
        }

        //用CHECK BOX 和鍵盤的852處理定力
        //一次推以50為準

        
        
        //水平推力的量值
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (port.IsOpen)
            {
                x_power_handle  = var_x_trackBar2.Value.ToString();
                port.WriteLine(xy_solar + "," + x_power_handle + "," + z_power_handle);
                horizon_now.Text = "horizon =" + var_x_trackBar2.Value.ToString();
            }
        }


        //攝影程式呼叫的按鍵
        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Users\user\Desktop\RCControl\Remote Camera Control");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var_z_trackBar1.Value = 0;
            z_power_handle = var_z_trackBar1.Value.ToString();
            port.WriteLine(xy_solar + "," + x_power_handle + "," + z_power_handle);
            vertical_now.Text = "vertical =" + var_z_trackBar1.Value.ToString();

        }
    }
}
