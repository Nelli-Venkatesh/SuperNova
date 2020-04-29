using Newtonsoft.Json;
using Pirate_Cell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Dynamic;
using System.IO;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Data;
using System.Web;

namespace SuperNova
{
    public class captcha
    {
        public static string machineKey1 = string.Empty;
        public static string machineKey2 = string.Empty;
        public static int expiryTime = 60;
        private static Timer aTimer;

        static string ConnectionString = string.Empty;

        // Create a new database:
        public static void initialize(int captchaExpiry = 0)
        {
            try
            {
                if (captchaExpiry > 0)
                    expiryTime = captchaExpiry;
                machineKey1 = Random_Data_Generator(16);
                machineKey2 = Random_Data_Generator(16);
                
                //Create a local database for the captcha
                createMiniDb();

                // Create a timer with a 15 minutes interval.
                aTimer = new Timer(1000 * 60 * 15);

                // Hook up the Elapsed event for the timer.
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static void  createMiniDb()
        {
            string path = HttpContext.Current.Server.MapPath("superNova");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string filePath = path + "\\Stack.db";
            if (!File.Exists(filePath))
                SQLiteConnection.CreateFile(filePath);
            ConnectionString = string.Format(@"Data Source={0};Version=3;New=False;Compress=True;cache=shared;", path.Replace("\\", "/") + "/Stack.db");

            string tableExistsQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='captchaTable';";
            using (SQLiteConnection sqlite_con = new SQLiteConnection(ConnectionString, true))
            {
                try
                {
                    sqlite_con.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(tableExistsQuery, sqlite_con))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.HasRows)
                            {
                                string Createsql = "CREATE TABLE captchaTable (captchaValue VARCHAR(10), captchaChiper VARCHAR(300),  validity TIMESTAMP   DEFAULT CURRENT_TIMESTAMP);";
                                using (SQLiteCommand cmd1 = new SQLiteCommand(Createsql, sqlite_con))
                                {
                                    cmd1.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    sqlite_con.Close();
                    sqlite_con.Dispose();
                }
                catch (Exception ex)
                {
                    sqlite_con.Close();
                    sqlite_con.Dispose();
                }
            }
        }

        //Timer to delete the captchas in the database for every 15 minutes
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                using (SQLiteConnection sqlite_con = new SQLiteConnection(ConnectionString, true))
                {
                    sqlite_con.Open();
                    string Createsql = "DELETE from captchaTable WHERE validity <= Datetime('now', '-1 minutes', 'localtime');";
                    using (SQLiteCommand cmd = new SQLiteCommand(Createsql, sqlite_con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    sqlite_con.Close();
                    sqlite_con.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //Captcha Methods
        public static string randomString(int i = 6)
        {
            try
            {
                if (i > 6)
                    throw new IndexOutOfRangeException("String length sould be less than 6 digits and greater than 2 digits");
                else if (i < 2)
                    throw new IndexOutOfRangeException("String length sould be less than 6 digits and greater than 2 digits");
                return Random_Data_Generator(i);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //To Generate the image base-64 string 
        public static string encodedImage(string captcha_data)
        {
            try
            {
                if (string.IsNullOrEmpty(captcha_data))
                    throw new NullReferenceException("captcha data should not be null");
                return captcha_generator(captcha_data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //To Generate the chiper text for the generated captcha
        public static string encodedCipher(string captcha_data)
        {

            try
            {
                if (string.IsNullOrEmpty(captcha_data))
                    throw new NullReferenceException("captcha data should not be null");
                return captcha_code_generator(captcha_data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //verify the captcha
        public static bool verify(string cipher_data, string client_captcha)
        {
            try
            {
                if (string.IsNullOrEmpty(cipher_data))
                    throw new NullReferenceException("Cipher data should not be null");
                if (string.IsNullOrEmpty(client_captcha))
                    throw new NullReferenceException("client captcha data should not be null");
                return captcha_verify(cipher_data, client_captcha);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Core Modules

        private static bool captcha_verify(string captchaChiper, string client_captcha)
        {
            try
            {
                string image_data = cipher._Decrypt(captchaChiper, machineKey1, machineKey2);
                CaptchaModel rootobj = JsonConvert.DeserializeObject<CaptchaModel>(image_data);
                if (!expiry_time_check(rootobj.expiry_time))
                    return false;
                if (client_captcha != rootobj.captcha_data)
                    return false;
                string captchaCheckQuery = "SELECT * FROM captchaTable WHERE captchaChiper='" + captchaChiper + "' ORDER BY validity ASC ;";
                using (SQLiteConnection sqlite_con = new SQLiteConnection(ConnectionString, true))
                {
                    try
                    {
                        sqlite_con.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(captchaCheckQuery, sqlite_con))
                        {
                            using (SQLiteDataReader rdr = cmd.ExecuteReader())
                            {
                                if (rdr.HasRows)
                                {
                                    sqlite_con.Close();
                                    sqlite_con.Dispose();
                                    return true;
                                }
                            }
                        }
                        sqlite_con.Close();
                        sqlite_con.Dispose();
                    }
                    catch (Exception ex)
                    {
                        sqlite_con.Close();
                        sqlite_con.Dispose();
                        return false;

                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool expiry_time_check(string date)
        {
            try
            {
                DateTime expiry_date = DateTime.Parse(date);
                DateTime current_date = DateTime.UtcNow;
                int date_difference = Convert.ToInt32((expiry_date - current_date).TotalSeconds);
                if (date_difference > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool storeCaptcha(string captchaValue, string captchaChiper)
        {
            try
            {
                if (string.IsNullOrEmpty(captchaValue))
                    throw new NullReferenceException("Cipher data should not be null");
                if (string.IsNullOrEmpty(captchaChiper))
                    throw new NullReferenceException("client chiper data should not be null");


                using (SQLiteConnection sqlite_con = new SQLiteConnection(ConnectionString, true))
                {
                    try
                    {
                        sqlite_con.Open();
                        string captchaInsertQuery = "INSERT INTO captchaTable (captchaValue, captchaChiper) VALUES('" + captchaValue + "', '" + captchaChiper + "'); ";
                        using (SQLiteCommand cmd1 = new SQLiteCommand(captchaInsertQuery, sqlite_con))
                        {
                            cmd1.ExecuteNonQuery();
                        }
                        sqlite_con.Close();
                        sqlite_con.Dispose();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        sqlite_con.Close();
                        sqlite_con.Dispose();
                        return false;

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string captcha_code_generator(string captcha_data)
        {
            try
            {
                dynamic obj_data = new ExpandoObject();
                obj_data.expiry_time = DateTime.UtcNow.AddSeconds(expiryTime).ToString();
                obj_data.captcha_data = captcha_data;
                string final_data = JsonConvert.SerializeObject(obj_data);
                string captchaChiper = cipher._Encrypt(final_data, machineKey1, machineKey2);
                while (storeCaptcha(captcha_data, captchaChiper))
                {
                    return captchaChiper;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string captcha_generator(string captcha_data)
        {
            try
            {
                captcha_data = captcha_data.ToUpper();
                const int iHeight = 100;
                const int iWidth = 350;
                var oRandom = new Random();
                int[] aFontEmSizes = { 40, 45 };
                string[] aFontNames = {
                    //"Comic Sans MS",
                    "Arial",
                    "Times New Roman",
                    "Georgia",
                    "Verdana",
                    "Geneva"
                };

                FontStyle[] aFontStyles =
                {
                FontStyle.Bold,
                FontStyle.Italic,
                FontStyle.Regular,
                FontStyle.Strikeout,
                FontStyle.Underline
            };
                HatchStyle[] aHatchStyles =
                {
                HatchStyle.BackwardDiagonal, HatchStyle.Cross, HatchStyle.DashedDownwardDiagonal, HatchStyle.DashedHorizontal,
                HatchStyle.DashedUpwardDiagonal, HatchStyle.DashedVertical, HatchStyle.DiagonalBrick, HatchStyle.DiagonalCross,
                HatchStyle.Divot, HatchStyle.DottedDiamond, HatchStyle.DottedGrid, HatchStyle.ForwardDiagonal, HatchStyle.Horizontal,
                HatchStyle.HorizontalBrick, HatchStyle.LargeCheckerBoard, HatchStyle.LargeConfetti, HatchStyle.LargeGrid,
                HatchStyle.LightDownwardDiagonal, HatchStyle.LightHorizontal, HatchStyle.LightUpwardDiagonal, HatchStyle.LightVertical,
                HatchStyle.Max, HatchStyle.Min, HatchStyle.NarrowHorizontal, HatchStyle.NarrowVertical, HatchStyle.OutlinedDiamond,
                HatchStyle.Plaid, HatchStyle.Shingle, HatchStyle.SmallCheckerBoard, HatchStyle.SmallConfetti, HatchStyle.SmallGrid,
                HatchStyle.SolidDiamond, HatchStyle.Sphere, HatchStyle.Trellis, HatchStyle.Vertical, HatchStyle.Wave, HatchStyle.Weave,
                HatchStyle.WideDownwardDiagonal, HatchStyle.WideUpwardDiagonal, HatchStyle.ZigZag
            };


                //Creates an output Bitmap
                var oOutputBitmap = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                var oGraphics = Graphics.FromImage(oOutputBitmap);
                oGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                //Create a Drawing area
                var oRectangleF = new RectangleF(0, 0, iWidth, iHeight);

                //Draw background (Lighter colors RGB 100 to 255)
                Brush oBrush = new HatchBrush(HatchStyle.Percent50, Color.Gainsboro, Color.White); //new HatchBrush(aHatchStyles[oRandom.Next(aHatchStyles.Length - 1)], Color.Gainsboro, Color.White);//HatchBrush(aHatchStyles[oRandom.Next(aHatchStyles.Length - 1)], Color.FromArgb((oRandom.Next(100, 255)), (oRandom.Next(100, 255)), (oRandom.Next(100, 255))), Color.White);

                oGraphics.FillRectangle(oBrush, oRectangleF);

                var oMatrix = new Matrix();
                int i;
                for (i = 0; i <= captcha_data.Length - 1; i++)
                {
                    oMatrix.Reset();
                    int iChars = captcha_data.Length;
                    int x = (iWidth / (iChars + 1) * i) + 30;  // adjust block width of every character
                    const int y = iHeight / 4; // adjust block height of every charachter

                    //Rotate text Random
                    //oMatrix.RotateAt(oRandom.Next(-40, 40), new PointF(x, y));

                    oGraphics.Transform = oMatrix;

                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    //Draw the letters with Randon Font Type, Size and Color
                    oGraphics.DrawString
                    (
                    //Text
                    captcha_data.Substring(i, 1),
                    //Random Font Name and Style
                    new Font(aFontNames[oRandom.Next(aFontNames.Length - 1)], aFontEmSizes[oRandom.Next(aFontEmSizes.Length - 1)], aFontStyles[oRandom.Next(aFontStyles.Length - 1)]),

                   //Random Color (Darker colors RGB 0 to 100)
                   new SolidBrush(Color.FromArgb(oRandom.Next(0, 100), oRandom.Next(0, 100), oRandom.Next(0, 100))),
                   x, //put text at fixed width
                    y  //oRandom.Next(10, 40)  // put text at random height
                    );
                    oGraphics.ResetTransform();
                }


                // context.Response.ContentType = "image/JPEG";

                //render image 
                //oOutputBitmap.Save(captcha_data + ".jpg", ImageFormat.Png);


                MemoryStream ms = new MemoryStream();
                oOutputBitmap.Save(ms, ImageFormat.Jpeg);
                string final_val = Convert.ToBase64String(ms.GetBuffer());

                //dispose everything, we do not need them any more.
                oOutputBitmap.Dispose();
                oGraphics.Dispose();
                //  Console.WriteLine();

                return final_val;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string Random_Data_Generator(int length = 6)
        {
            try
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789abcdefghijklmnopqrstuvwxyz";
                string result = "";
                var rand = new Random();
                for (int i = 0; i < length; i++)
                {
                    result += chars[rand.Next(chars.Length)];
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
    public class CaptchaModel
    {
        public string expiry_time { get; set; }
        public string captcha_data { get; set; }
    }

}
