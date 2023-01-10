using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProiectD3_Final
{
    public partial class Form1 : Form
    {

        public string text = String.Empty;

        public class Config
        {

            public List<string> configuration = new List<string>();

            public string trace = String.Empty;

            public string output = String.Empty;
            public int frequency;
            public double latency;
            public double codeHitrate;
            public double dataHitrate;
            public double speculation_accuracy;



            public Config(string trace, int frequency, double latency, double codeHitrate, double dataHitrate,double speculation_accuracy)
            {
                this.trace = trace;
                this.frequency = frequency;
                this.latency = latency;
                this.dataHitrate = dataHitrate;
                this.codeHitrate = codeHitrate;
                this.speculation_accuracy = speculation_accuracy;
            }


            public void scrie(string output, int index)
            {
                string configuratie = String.Format("default_cfg{0}.xml", index);

                using (StreamWriter writer = new StreamWriter(configuratie))
                {
                    writer.Write(output);
                }
            }

        }
        public string citeste()
        {

            using (StreamReader reader = new StreamReader(@"C:\Users\RobertValentin\source\repos\ProiectD3-Final\ProiectD3-Final\default_cfg.txt"))
            {
                text = reader.ReadToEnd();
            }
            return text;

        }


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string citit = textBox1.Text;
            citeste();

            for (int i = 0; i < 20; i++)
            {
                Random rnd = new Random();
                int frec = rnd.Next(200, 900);
                double latency = rnd.Next(10, 30);
                double dataHitrate = rnd.NextDouble() * 0.09 + 0.9;
                double codeHitrate = rnd.NextDouble() * 0.09 + 0.9;
                double speculation_accuracy= rnd.NextDouble() * 0.490 + 0.500;


                Config obj = new Config(citit + ".tra", frec,latency,codeHitrate,dataHitrate,speculation_accuracy);


                obj.output = string.Format(text, obj.trace, obj.frequency,obj.latency,obj.codeHitrate,obj.dataHitrate,obj.speculation_accuracy);
              //  obj.configuration.Add(obj.output);
                obj.scrie(obj.output, i);

            }


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string copie;

            // System.Diagnostics.Process.Start("cmd.exe");
            for (int i = 0; i <= 20; i++)
            {
                System.Threading.Thread.Sleep(1000);
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                copie = String.Format(@"/c psatsim_con.exe default_cfg{0}.xml output{0}.xml", i);
                startInfo.Arguments = copie;
                p.StartInfo = startInfo;
                p.Start();

            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            var IPC = new List<double>();
            var power = new List<double>();

            for (int i = 0; i < 20; i++)
            {
                string filePath = string.Format(@"C:\Users\RobertValentin\source\repos\ProiectD3-Final\ProiectD3-Final\bin\Debug\output{0}.xml", i);

                foreach (string line in System.IO.File.ReadLines(filePath))
                {
                    var match = Regex.Match(line, "ipc=\"([0-9.]+)\"");
                    var matchPower = Regex.Match(line, "power=\"([0-9.]+)\">");
                    if (match.Success)
                    {
                        string value = match.Groups[1].Value;
                        double doubleValue = Convert.ToDouble(value);
                        IPC.Add(doubleValue);
                    }
                     if (matchPower.Success)
                    {

                        string value = matchPower.Groups[1].Value;
                        double doubleValue = Convert.ToDouble(value);
                        power.Add(doubleValue);

                    }
                }
            }

            IPC.Sort();
            power.Sort();

            richTextBox1.Text = richTextBox1.Text + "  IPC  " + "        POWER \n";
              foreach (var (item1, item2) in IPC.Zip(power, (i1, i2) => (i1, i2)))
              {
                  
                  richTextBox1.Text = richTextBox1.Text + ($"{item1},    {item2}") + "\n";
              }


            double maxIpc = IPC.Max();
            double maxPower = power.Max();
            double thresholdIPC = maxIpc*0.999;
            double thresholdPower = maxPower * 0.999;

            List<double> nonParetoIpc = IPC.Where(ipc => ipc < thresholdIPC).ToList();
            List<double> nonParetoPower = power.Where(powerr => powerr < thresholdPower).ToList();


            richTextBox2.Text = richTextBox2.Text+"Non-Pareto IPC"+"     Non-Pareto Power" +" \n";
           
            foreach (var (item1, item2) in nonParetoIpc.Zip(nonParetoPower, (i1, i2) => (i1, i2)))
            {

                richTextBox2.Text = richTextBox2.Text + ($"{item1},              {item2}") + "\n";
            }


        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
