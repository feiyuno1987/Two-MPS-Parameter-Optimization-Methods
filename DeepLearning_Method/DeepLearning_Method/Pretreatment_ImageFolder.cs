/*     Batch preprocessing of image files for ML.Net image classification
*
* Version: 1.0
* Author:  Siyu Yu
* Email:   siyuyu @yangtzeu.edu.cn(or 573315294@qq.com)
* Date:    9 Dec 2020
* 
*/

using ConsoleProgressBar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DeepLearning_Method
{
    /// <summary>
    /// Batch preprocessing of image files for ML.Net image classification
    /// </summary>
    public class Pretreatment_ImageFolder
    {
        /// <summary>
        /// Batch preprocessing of image files for ML.Net image classification
        /// </summary>
        public static void Process(string path)
        {
            #region CopyImagesToFolder

            System.Console.WriteLine($"directory of realizations is : {path}");
            var filePaths = DirHelper.GetFileNames(path, "*.jpg", false);
            FileHelper.SortByName(ref filePaths);

            Dictionary<string, List<string>> prefix_filePaths = new Dictionary<string, List<string>>();

            for (int i = 0; i < filePaths.Length; i++)
            {
                string filePath = filePaths[i];
                string fileName = FileHelper.GetFileName(filePaths[i]);

                if (fileName.IndexOf("_") < 0)
                    continue;

                string Prefix = fileName.Substring(0, fileName.IndexOf("_"));
                if (prefix_filePaths.ContainsKey(Prefix))
                {
                    prefix_filePaths[Prefix].Add(filePath);
                }
                else
                {
                    prefix_filePaths.Add(Prefix, new List<string>() { filePath });
                }
            }

            Dictionary<int, string> idx_prefix = new Dictionary<int, string>();
            System.Console.WriteLine();
            System.Console.WriteLine("list of prefixs(column name:1 - serial number;2 - parameter value;3 - realizations number)");
            int counter = 1;
            foreach (var item in prefix_filePaths)
            {
                idx_prefix.Add(counter, item.Key);
                System.Console.WriteLine($"{counter}\t{item.Key}\t{item.Value.Count}");
                counter++;
            }

            System.Console.WriteLine("Select two adjacent serial numbers (for example, 1 2 or 4 5) from the prefix list.");
            string selected = System.Console.ReadLine();
            List<int> ids = selected.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(a => int.Parse(a)).ToList();

            System.Console.WriteLine("How many realizations are sampled at a time(one sampling experiments)?");
            System.Console.WriteLine("If the number of realizations is 300, it is recommended to take 50~100");
            int k = int.Parse(System.Console.ReadLine());

            System.Console.WriteLine("How many sampling experiments need to be done?");
            System.Console.WriteLine("it is recommended to take value in range of 30~80");
            int N = int.Parse(System.Console.ReadLine());
            Program.SamplingNumber = N;

            System.Console.WriteLine();
            System.Console.WriteLine("Clean up the previous data");
            if (!DirHelper.IsExistDirectory(Program.SavePath))
                DirHelper.CreateDir(Program.SavePath);
            else
                DirHelper.ClearDirectory(Program.SavePath);

            System.Threading.Thread.Sleep(100);

            System.Console.WriteLine();
            System.Console.WriteLine("Copy data file");
            //Take samples from the realizations
            for (int i = 0; i < N; i++)
            {
                string SamplingPath = @$"{Program.SavePath}\train{i}";
                Directory.CreateDirectory(SamplingPath);

                counter = 0;

                using (var pb = new ProgressBar())
                {
                    foreach (var idx in ids)
                    {
                        pb.Progress.Report(counter / ids.Count, "Copying now");

                        string prefix = idx_prefix[idx];
                        List<string> filePaths1 = prefix_filePaths[prefix];
                        filePaths1 = SortHelper.RandomSelect(filePaths1, k, new Random());
                        foreach (var filePath in filePaths1)
                        {
                            string filePath_new = string.Format("{0}\\{1}", SamplingPath, FileHelper.GetFileName(filePath));
                            File.Copy(filePath, filePath_new);
                        }
                    }
                }


                #region MakeTagFile

                var filePaths_temp = DirHelper.GetFileNames(SamplingPath);
                FileHelper.SortByName(ref filePaths_temp);

                MyDataTable myDT = new MyDataTable();
                myDT.AddColumn("FileName");
                myDT.AddColumn("Label");

                for (int j = 0; j < filePaths_temp.Length; j++)
                {
                    MyRow row = myDT.NewRow();
                    string fileName = FileHelper.GetFileName(filePaths_temp[j]);

                    if (fileName.IndexOf("_") < 0)
                        continue;

                    row["FileName"] = fileName;
                    row["Label"] = fileName.Substring(0, fileName.IndexOf("_"));
                    myDT.AddRow(row);
                }
                string TagFilePath = $"{Program.SavePath}\\train_tags{i}.tsv";
                myDT.WriteToTxt(TagFilePath, "\t", false);

                #endregion
            }

            #endregion
        }
    }
}
