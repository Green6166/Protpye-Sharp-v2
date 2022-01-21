using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using DrawBehindDesktopIcons;


namespace Protpyesharpv2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            this.Visible = true;
            button4.Enabled = false;
            button2.Enabled = true;
        }

        private void TextBox1_valuechanged(object sender,EventArgs e)
        {
            DrawBehindDesktopIcons.Program.SetwallpaperinDBDI(textBox1.ToString());


        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "photo files (*.gif)|*.jpg|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file

                    var filenam = openFileDialog1.FileName;
                    listBox2.Items.Add(openFileDialog1.FileName);
                    listBox1.Items.Add(numericUpDown1.Value);
                    pictureBox1.ImageLocation = openFileDialog1.FileName;

            }   };
        }



        public void Openfile()
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            listBox2.Items.RemoveAt(-1);
            listBox1.Items.RemoveAt(-1);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            button4.Enabled = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            Setwallpaper();
        }

        public void Setwallpaper()
        {
            int pointer = 0;

            while (button4.Enabled is true)
            {
                foreach (object item in listBox2.Items)
                {
                    pictureBox1.Image = Image.FromFile(item.ToString());
                    DrawBehindDesktopIcons.Program.SetwallpaperinDBDI(item.ToString());
                    // timer1.Interval = Convert.ToInt32(listBox1.Items[pointer].ToString());
                    //  timer1.Start();
                    // while (timer1.Interval != 0)
                    //  {

                    // }
                    pointer += 1;

                    if (pointer > listBox1.Items.Count) { button4.Enabled = false; button1.Enabled = true; button2.Enabled = true; button3.Enabled = true; }

                }

            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            timer1.Stop();

        }

        private void Timer1_tick(object sender, EventArgs e)
        {
            timer1.Stop();







        }
    }
}
