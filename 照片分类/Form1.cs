using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// 将指定文件夹中的图片文件，按拍摄日期放到相应文件夹中，每个月建立一个文件夹，以2005-07这样的形式保存
/// </summary>

namespace 照片分类
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            string path;
            if ((path = ChooseFolderPath()) == "")
            {
                /* do nothing */
            }
            else
            {
                textBox1.Text = path;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            DealWithFile(this.textBox1.Text);
        }


        public void DealWithFile(string targetDir)
        {
            string temp = "";

            List<string> listPath = new List<string> { };
            List<string[]> listDirfiles = new List<string[]> { };

            foreach (string fileName in Directory.GetFiles(targetDir))
            {
                try
                {
                    temp = GetTakePicDate(fileName);
                }
                catch (Exception e) { temp = "error"; }


                if (temp != "error" && temp != "")
                {
                    string directoryName = Path.GetDirectoryName(fileName);//目录名
                    string fileNameOnly = Path.GetFileName(fileName);  //文件名
                    string newDirName =Path.Combine(directoryName,temp.Substring(0, 7)); //新目录名（加了日期）
                    string newFullFileName = Path.Combine(newDirName, fileNameOnly);   //新文件名


                    listPath.Add(newDirName);  //新目录名入LIST
                    listDirfiles.Add(new string[] { fileName, newFullFileName });  //旧文件名（含路径）和新文件（含路径）入LIST
                }
            }

            //建立日期目录
            listPath = listPath.Distinct().ToList();
            foreach (string path in listPath)
            {
                Directory.CreateDirectory(path);
            }

            //移动所有图片
            foreach (string[] deldui in listDirfiles)
            {
                File.Move(deldui[0], deldui[1]);
            }

            MessageBox.Show("OK");
        }

        /// <summary>
        /// 选择指定目录，并将目录值返回
        /// </summary>
        /// <returns></returns>
        public string ChooseFolderPath()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.Desktop;
            fbd.ShowNewFolderButton = true;
            fbd.Description = "请选择目录";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                return (fbd.SelectedPath.ToString());
            }
            else
            {
                return ("");
            }
        }

        /// <summary>
        /// 获取指定图片的EXIF信息，返回图片的拍摄日期
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetTakePicDate(string fileName)

        {
            Encoding ascii = Encoding.ASCII;

            string picDate;


            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            Image image = Image.FromStream(stream, true, false);


            foreach (PropertyItem p in image.PropertyItems)

            {
                /* 获取拍摄日期时间 */

                if (p.Id == 0x9003) /* 0x0132 最后更新时间 */

                {
                    stream.Close();


                    picDate = ascii.GetString(p.Value);

                    if ((!"".Equals(picDate)) && picDate.Length >= 10)

                    {
                        /* 拍摄日期 */

                        picDate = picDate.Substring(0, 10);

                        picDate = picDate.Replace(":", "-");

                        return (picDate);
                    }
                }
            }

            stream.Close();

            return ("");
        }
    }
}
