using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DllCompare;

namespace DllCompareGui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1.Click += Button_Click;
            button2.Click += Button_Click;


        

        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;

            TextBox tb = textBox1;

            if (b == button2)
                tb = textBox2;


            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "DLLs |*.dll";
            d.Multiselect = false;

            if (d.ShowDialog() == DialogResult.OK)
            {
                tb.Text = d.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DllCompare.DllComparer.DifferenceType diff=0;

            if (checkBox1.Checked)
                diff |= DllComparer.DifferenceType.IgnoreHideBySig;
            if (checkBox2.Checked)
                diff |= DllComparer.DifferenceType.MissingMethods;
            if (checkBox3.Checked)
                diff |= DllComparer.DifferenceType.MissingTypes;
            if (checkBox4.Checked)
                diff |= DllComparer.DifferenceType.DifferentReturnType;
            if (checkBox5.Checked)
                diff |= DllComparer.DifferenceType.DifferentAccessLevel;

            if (Check())
            {
                DllComparer comp = new DllComparer(textBox1.Text, textBox2.Text);
                textBox3.Text = comp.GetResult(diff);
            }
        }

        private bool Check()
        {
            //Todo file exists etc...
            return true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string t = textBox2.Text;
            textBox2.Text = textBox1.Text;
            textBox1.Text = t;
        }

      
    }
}
