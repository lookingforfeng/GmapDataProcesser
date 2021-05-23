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
using System.Data.SQLite;

namespace MapDataPorcess
{
    public partial class InsertMapData : Form
    {
        public InsertMapData()
        {
            InitializeComponent();
        }

        //打开图片文件位置
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            };
        }

        //打开数据库位置
        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog2.FileName;
            };
        }

        //开始导入
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                try
                {
                    //遍历所有文件
                    string[] files = Directory.GetFiles(textBox1.Text, "*.jpg", SearchOption.AllDirectories);

                    int count = 0;
                    int total = files.Length;
                    foreach (string file in files)
                    {
                        string[] facter = file.Split('/');

                        //string sourceFile = textBox2.Text;
                        //using (SQLiteConnection cn1 = new SQLiteConnection())
                        //{
                        //    cn1.ConnectionString = string.Format("Data Source=\"{0}\";Page Size=32768", sourceFile);
                        //    cn1.Open();
                        //    if (cn1.State == System.Data.ConnectionState.Open)
                        //    {
                        //        //using (SQLiteCommand test_cmd = new SQLiteCommand(string.Format(
                        //        //    "SELECT id FROM Tiles WHERE X={0} AND Y={1} AND Zoom={2} AND Type={3};",
                        //        //    rd.GetInt32(1), rd.GetInt32(2), rd.GetInt32(3), rd.GetInt32(4)), cn2))
                        //        //{
                        //        //    using (SQLiteDataReader reader = test_cmd.ExecuteReader())
                        //        //    {
                        //        //        if (!rd2.Read())
                        //        //        {
                        //        //            add.Add(id);
                        //        //        }
                        //        //    }
                        //        //}
                        //    }
                        //}
                        Console.WriteLine(file);
                        count++;
                        progressBar1.Value = count / total * 1000;
                        label1.Text = string.Format("{0}/{1}", count, total);


                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("检查文件地址");
                }

            }
            else
            {
                MessageBox.Show("检查文件地址");
            }
        }
    }
}
