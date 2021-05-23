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
using System.Data.Common;

namespace MapDataPorcess
{
    public partial class InsertMapData : Form
    {

        static readonly string singleSqlSelect = "SELECT Tile FROM main.TilesData WHERE id = (SELECT id FROM main.Tiles WHERE X={0} AND Y={1} AND Zoom={2} AND Type={3})";
        static readonly string singleSqlInsert = "INSERT INTO main.Tiles(X, Y, Zoom, Type, CacheTime) VALUES(@p1, @p2, @p3, @p4, @p5)";
        static readonly string singleSqlInsertLast = "INSERT INTO main.TilesData(id, Tile) VALUES((SELECT last_insert_rowid()), @p1)";
        static readonly int type = 713551964;//谷歌地图

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
                //遍历所有文件
                string[] files = Directory.GetFiles(textBox1.Text, "*.jpg", SearchOption.AllDirectories);

                int count = 0;
                int total = files.Length;
                int x=0, y=0, z=0;
                foreach (string file in files)
                {
                    string[] facter = file.Split('\\');
                    {
                        if (facter.Length >= 3)
                        {
                            var y_jpg = facter[facter.Length - 1];
                            y = Convert.ToInt32(y_jpg.Split('.')[0]);
                            x = Convert.ToInt32(facter[facter.Length - 2]);
                            z = Convert.ToInt32(facter[facter.Length - 3]);
                        }
                    }

                    string sourceFile = textBox2.Text;
                    if (sourceFile != "")
                    {
                        using (SQLiteConnection connection = new SQLiteConnection())
                        {
                            connection.ConnectionString = string.Format("Data Source=\"{0}\";Page Size=32768", sourceFile);
                            connection.Open();
                            if (connection.State == System.Data.ConnectionState.Open)
                            {
                                using (SQLiteCommand test_cmd = new SQLiteCommand(string.Format( singleSqlSelect,x, y, z, type), connection))
                                {
                                    using (SQLiteDataReader reader = test_cmd.ExecuteReader())
                                    {
                                        if (!reader.Read())
                                        {
                                            using (DbTransaction tr = connection.BeginTransaction())
                                            {
                                                try
                                                {
                                                    using (DbCommand cmd = connection.CreateCommand())
                                                    {
                                                        cmd.Transaction = tr;
                                                        cmd.CommandText = singleSqlInsert;

                                                        cmd.Parameters.Add(new SQLiteParameter("@p1", x));
                                                        cmd.Parameters.Add(new SQLiteParameter("@p2", y));
                                                        cmd.Parameters.Add(new SQLiteParameter("@p3", z));
                                                        cmd.Parameters.Add(new SQLiteParameter("@p4", type));
                                                        cmd.Parameters.Add(new SQLiteParameter("@p5", DateTime.Now));

                                                        cmd.ExecuteNonQuery();
                                                    }

                                                    using (DbCommand cmd = connection.CreateCommand())
                                                    {
                                                        cmd.Transaction = tr;
                                                        if (!File.Exists(file))
                                                        {
                                                            continue ;//文件不存在
                                                        }
                                                        byte[] data = File.ReadAllBytes(file);

                                                        cmd.CommandText = singleSqlInsertLast;
                                                        cmd.Parameters.Add(new SQLiteParameter("@p1", data));

                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    tr.Commit();
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("PutImageToCache: " + ex.ToString());
                                                    tr.Rollback();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine(file);
                    count++;
                    progressBar1.Value = count / total * 1000;
                    label1.Text = string.Format("{0}/{1}", count, total);
                }
            }
            else
            {
                MessageBox.Show("检查文件地址");
            }
        }
    }
}
