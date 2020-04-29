using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pirate_Cell;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Configuration;

namespace SuperNova
{
    public class Utils
    {

        //Public Libraries

        //File Recovery Methods
        public static List<String> get_files(string path)
        {
            return Path_Search(path);
        }

        public static string get_JSON(string file)
        {
            return read_json_array(file);
        }

        //Data saving Methods
        public static bool log_data(dynamic data, string folder_name, string file_name)
        {
            return Data_Log(data, folder_name, file_name);
        }

        public static string Image_save(string base64, string salt_name, string output_image_path, string output_image_directory)
        {
            return Save_Image(base64, salt_name, output_image_path, output_image_directory);
        }

        public static string PDF_save(string base64, string salt_name, string output_image_path, string output_image_directory)
        {
            return Save_PDF(base64, salt_name, output_image_path, output_image_directory);
        }

        public static string Save_Video(byte[] base64, string salt_name, string output_image_path, string output_image_directory)
        {
            return Save_3GP(base64, salt_name, output_image_path, output_image_directory);
        }

        private static List<String> Path_Search(string path)
        {

            List<String> files = new List<String>();
            if (String.IsNullOrEmpty(path))
                throw new NullReferenceException("Path should not be null");
            try
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(path))
                {
                    files.AddRange(Path_Search(d));
                }
            }
            catch (System.Exception excpt)
            {
                // MessageBox.Show(excpt.Message);
                throw excpt;
            }

            return files;
        }

        private static string read_json_array(string file)
        {
            try
            {
                if (String.IsNullOrEmpty(file))
                    throw new NullReferenceException("File path should not be null");
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    string mid_json = json.Replace("}", "},");
                    string final_json = "{ \"data\": [" + mid_json.Remove(mid_json.Length - 1, 1) + "] }";
                    return final_json;
                }
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        private static string read_json(string file)
        {
            try
            {
                if (String.IsNullOrEmpty(file))
                    throw new NullReferenceException("File path should not be null");
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    return json;
                }
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }

        }

        private static bool Data_Log(dynamic data, string folder_name, string file_name)
        {
            if (data == null)
                throw new NullReferenceException("Data should not be null");
            if (string.IsNullOrEmpty(folder_name))
                throw new NullReferenceException("Folder should not be null");
            if (string.IsNullOrEmpty(file_name))
                throw new NullReferenceException("File name should not be null");
            bool return_flag = false;
            try
            {
                string strPath = folder_name + "\\" + DateTime.Now.ToString("MMddyyyy") + "\\" + DateTime.Now.ToString("HH").ToString();
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                string path2 = strPath + "\\" + file_name + DateTime.Now.ToString("yyyyMMddhhmmsstt");
                StreamWriter swLog = new StreamWriter(path2 + ".txt", true);
                swLog.WriteLine(data);
                swLog.Close();
                swLog.Dispose();
                return_flag = true;
                return return_flag;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
        }

        private static string Save_Image(string base64, string salt_name, string output_image_path, string output_image_directory)
        {
            if (string.IsNullOrEmpty(base64))
                throw new NullReferenceException("base64 string should not be null");
            if (string.IsNullOrEmpty(salt_name))
                throw new NullReferenceException("Salt should not be null");
            if (string.IsNullOrEmpty(output_image_path))
                throw new NullReferenceException("output image path should not be null");
            if (string.IsNullOrEmpty(output_image_directory))
                throw new NullReferenceException("output image directory should not be null");

            try
            {
                string salt = "DEFAULT";
                if (!string.IsNullOrEmpty(salt_name))
                {
                    salt = salt_name;
                }
                string directoryPath = output_image_directory;
                Random rn = new Random();
                //    string sha_value = SHA1(base64);
                string output_file_path = "";
                string path = output_image_path;
                if (base64 != "" && base64 != null)
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    if (generate_File(directoryPath + "\\" + salt + "_" + DateTime.Now.ToString("yyyyMMddhhmmsstt") + ".jpg", base64))
                    {
                        output_file_path = path + "/" + salt + "_" + DateTime.Now.ToString("yyyyMMddhhmmsstt") + ".jpg";
                    }
                    return output_file_path;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        private static string Save_PDF(string base64, string salt_name, string output_image_path, string output_image_directory)
        {

            if (string.IsNullOrEmpty(base64))
                throw new NullReferenceException("base64 string should not be null");
            if (string.IsNullOrEmpty(salt_name))
                throw new NullReferenceException("Salt should not be null");
            if (string.IsNullOrEmpty(output_image_path))
                throw new NullReferenceException("output image path should not be null");
            if (string.IsNullOrEmpty(output_image_directory))
                throw new NullReferenceException("output image directory should not be null");

            try
            {
                string salt = "DEFAULT";
                if (!string.IsNullOrEmpty(salt_name))
                {
                    salt = salt_name;
                }
                string directoryPath = output_image_directory;
                Random rn = new Random();
                //  string sha_value = SHA1(base64);
                string output_file_path = "";
                string path = output_image_path;
                if (base64 != "" && base64 != null)
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    if (generate_File(directoryPath + "\\" + salt + "_" + DateTime.Now.ToString("yyyyMMddhhmmsstt") + ".pdf", base64))
                    {
                        output_file_path = path + "/" + salt + "_" + DateTime.Now.ToString("yyyyMMddhhmmsstt") + ".pdf";
                    }
                    return output_file_path;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        private static string Save_3GP(byte[] bytes, string salt_name, string output_image_path, string output_image_directory)
        {

            if (bytes == null)
                throw new NullReferenceException("base64 string should not be null");
            if (string.IsNullOrEmpty(salt_name))
                throw new NullReferenceException("Salt should not be null");
            if (string.IsNullOrEmpty(output_image_path))
                throw new NullReferenceException("output image path should not be null");
            if (string.IsNullOrEmpty(output_image_directory))
                throw new NullReferenceException("output image directory should not be null");

            try
            {
                string salt = "WA";
                if (!string.IsNullOrEmpty(salt_name))
                {
                    salt = salt_name;
                }
                string directoryPath = output_image_directory;
                Random rn = new Random();
                //  string sha_value = SHA1(bytes.ToString());
                string output_file_path = "";
                string path = output_image_path;
                if (bytes != null)
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    if (generate_video_File(directoryPath + "\\" + salt + "_" + DateTime.Now.ToString("yyyyMMddhhmmsstt") + ".3gp", bytes))
                    {
                        output_file_path = path + "/" + salt + "_" + DateTime.Now.ToString("yyyyMMddhhmmsstt") + ".3gp";
                    }
                    return output_file_path;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        private static bool generate_File(string file_path, string data)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(data);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    FileStream fs = new FileStream(file_path, FileMode.Create);
                    ms.WriteTo(fs);
                    ms.Close();
                    fs.Close();
                    fs.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static bool generate_video_File(string file_path, byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    FileStream fs = new FileStream(file_path, FileMode.Create);
                    ms.WriteTo(fs);
                    ms.Close();
                    fs.Close();
                    fs.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }

}
