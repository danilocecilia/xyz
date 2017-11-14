using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using tms.Base.VO;

namespace tms.Base.Essentials
{
    public static class Tools
    {
        #region Alerts

        /// <summary>
        /// Exibe um Alert no navegador de acordo com os parãmetros passados
        /// </summary>
        /// <param name="p"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void Alert(System.Web.UI.Page p, string title, string message)
        {
            string script = "<script>alert('" + message + "');</script>";
            p.ClientScript.RegisterClientScriptBlock(p.GetType(), title, script);
        }

        public static void AlertAjax(System.Web.UI.Page p, string title, string message)
        {
            string script = "alert('" + message + "');";
            System.Web.UI.ScriptManager.RegisterStartupScript(p.Page, p.Page.GetType(), title, script, true);
        }

        #endregion

        #region Redirect

        public static void RedirectToSource()
        {
            string source = HttpContext.Current.Request["source"] != null ? HttpContext.Current.Request["Source"].ToString() : "/";
            HttpContext.Current.Response.Redirect(source);
        }

        public static void RedirectToSessionSource()
        {
            string source = HttpContext.Current.Session["source"] != null ? HttpContext.Current.Session["Source"].ToString() : "/";

            if (source.ToUpper().Contains(".ASHX"))
                source = "/";

            HttpContext.Current.Response.Redirect(source);
        }

        public static void RedirectJavascript(System.Web.UI.Page p, string url)
        {
            string script = "<script>window.location.href='" + url + "';</script>";
            p.ClientScript.RegisterClientScriptBlock(p.GetType(), "", script);
        }

        public static string GetCurrentUrl()
        {
            string url = HttpContext.Current.Request.Url.ToString();

            if (url.Substring(0, 5).ToUpper() == "HTTP:")
                url = "https:" + url.Substring(5, url.Length - 5);

            return url;
        }

        public static string GetCurrentUrl(VOConfiguration configuration)
        {
            string url = HttpContext.Current.Request.Url.ToString();

            if (configuration.ProtocolType == VOConfiguration.ConfigurationProtocolType.HTTPS)
            {
                if (url.Substring(0, 5).ToUpper() == "HTTP:")
                    url = "https:" + url.Substring(5, url.Length - 5);
            }

            return url;
        }

        #endregion

        #region Criptography

        /// <summary>
        /// Criptografa Strings
        /// </summary>
        /// <param name="paramentro"></param>
        /// <returns></returns>
        public static string Cripto(string paramentro)
        {
            byte[] byt = Encoding.UTF8.GetBytes(paramentro);
            string parametroCriptografado = Convert.ToBase64String(byt, Base64FormattingOptions.None);

            return parametroCriptografado;
        }

        /// <summary>
        /// Descriptografa Strings
        /// </summary>
        /// <param name="parametro"></param>
        /// <returns></returns>
        public static string DeCripto(string parametro)
        {
            string aaa = string.Empty;

            try
            {
                byte[] kkk = Convert.FromBase64String(parametro);
                Encoding enc = System.Text.Encoding.ASCII;
                aaa = enc.GetString(kkk);
            }
            catch
            {
            }

            return aaa;
        }

        /// <summary>
        /// Returns the MD5 cyfer from a string input. Used to validated codhsh from MSX DCS
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5Hash(string input)
        {
            string hash = string.Empty;

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                hash = sBuilder.ToString();
            }

            return hash;
        }

        /// <summary>
        /// Encrypt parameters to be used in QueryString
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string UrlEncrypt(string parameter)
        {
            string passPhrase = "Pas5pr@se";
            string saltValue = "s@1tValue";
            string hashAlgorithm = "MD5";
            int passwordIterations = 2;
            string initVector = "@1B2c3D4e5F6g7H8";
            int keySize = 256;

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(parameter);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherTextBytes);
            return Cripto(cipherText);
        }

        /// <summary>
        /// Dencrypt parameters to be used in QueryString
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string UrlDecrypt(string parameter)
        {
            if (parameter == null || parameter == "null" || string.IsNullOrEmpty(parameter) || parameter == "undefined")
                return null;

            string passPhrase = "Pas5pr@se";
            string saltValue = "s@1tValue";
            string hashAlgorithm = "MD5";
            int passwordIterations = 2;
            string initVector = "@1B2c3D4e5F6g7H8";

            int keySize = 256;
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] cipherTextBytes = Convert.FromBase64String(DeCripto(parameter));
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            return plainText;
        }

        public static string GetTimeStamp()
        {
            return ((int)Math.Truncate((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)).ToString();
        }

        public static string ToMD5(string str)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static string FormatKeyword(string keyword)
        {
            keyword = keyword.ToLower().Replace("á", "a")
                               .Replace("é", "e")
                               .Replace("í", "i")
                               .Replace("ó", "o")
                               .Replace("ú", "u");

            List<string> parameters = keyword.Split(" ".ToCharArray()).ToList();

            keyword = string.Empty;

            foreach (var p in parameters)
            {
                keyword += p + "+";
            }

            keyword += "+0";


            return keyword;
        }

        public static string ToSHA1(string str)
        {
            return string.Join("", (new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(str))).Select(x => x.ToString("X2")).ToArray());
        }

        #endregion

        #region Resources

        /// <summary>
        /// Retorna o corresponde do Resource informado, de acordo com a Culture instalada / selecionada
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static string GetResource(string resource)
        {
            Assembly assembly = System.Web.HttpContext.Current.GetType().Assembly;
            System.Resources.ResourceManager resman = new System.Resources.ResourceManager(resource, assembly);
            return resman.BaseName;
        }

        /// <summary>
        /// Retorna o correspondente do Resource informado. Usado para chamadas via CS
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static string GetResourceByCodeBehind(string resource)
        {
            return HttpContext.GetGlobalResourceObject("TMS", resource).ToString();
        }

        public static string GetResource(string classKey, string resource, CultureInfo culture)
        {
            return HttpContext.GetGlobalResourceObject(classKey, resource, culture).ToString();
        }

        public static string GetDinamicResourceFromText(string str)
        {
            string aux = str;
            List<string> keys = null;

            bool shoudlContinue = true;
            int start = 0;
            int end = 0;
            int size = 0;

            while (shoudlContinue)
            {
                start = aux.IndexOf("[Resource(") + "[Resource(".Length;
                end = aux.IndexOf(")]");
                size = end - start;

                if (start <= -1 || end <= -1 || size < 0)
                    break;

                string key = aux.Substring(start, size);
                if (keys == null) keys = new List<string>();
                keys.Add(key);

                aux = aux.Substring(end + 2);
            }

            if (keys != null && keys.Count > 0)
            {
                foreach (string key in keys)
                {
                    str = str.Replace("[Resource(" + key + ")]", Tools.GetResourceByCodeBehind(key));
                }
            }

            return str;
        }

        #endregion

        #region Dates

        public static string ConvertDateByMonthName(DateTime date)
        {
            string month = string.Empty;

            switch (date.Month)
            {
                case 1:
                    month = "January";
                    break;

                case 2:
                    month = "February";
                    break;

                case 3:
                    month = "March";
                    break;

                case 4:
                    month = "April";
                    break;

                case 5:
                    month = "May";
                    break;

                case 6:
                    month = "June";
                    break;

                case 7:
                    month = "July";
                    break;

                case 8:
                    month = "August";
                    break;

                case 9:
                    month = "September";
                    break;

                case 10:
                    month = "October";
                    break;

                case 11:
                    month = "November";
                    break;

                case 12:
                    month = "December";
                    break;

                default:
                    month = "January";
                    break;
            }

            return date.Day.ToString() + " " + GetResourceByCodeBehind("of") + " " + GetResourceByCodeBehind(month) + " " + GetResourceByCodeBehind("of") + " " + date.Year.ToString();
        }

        public static string GetMonthName(int m)
        {
            string month = string.Empty;

            switch (m)
            {
                case 1:
                    month = "January";
                    break;

                case 2:
                    month = "February";
                    break;

                case 3:
                    month = "March";
                    break;

                case 4:
                    month = "April";
                    break;

                case 5:
                    month = "May";
                    break;

                case 6:
                    month = "June";
                    break;

                case 7:
                    month = "July";
                    break;

                case 8:
                    month = "August";
                    break;

                case 9:
                    month = "September";
                    break;

                case 10:
                    month = "October";
                    break;

                case 11:
                    month = "November";
                    break;

                case 12:
                    month = "December";
                    break;

                default:
                    month = "January";
                    break;
            }

            return month;
        }

        public static string GetAbbreviatedMonth(DateTime date)
        {
            string month = string.Empty;

            switch (date.Month)
            {
                case 1:
                    month = "Jan";
                    break;

                case 2:
                    month = "Fev";
                    break;

                case 3:
                    month = "Mar";
                    break;

                case 4:
                    month = "Abr";
                    break;

                case 5:
                    month = "Mai";
                    break;

                case 6:
                    month = "Jun";
                    break;

                case 7:
                    month = "Jul";
                    break;

                case 8:
                    month = "Ago";
                    break;

                case 9:
                    month = "Set";
                    break;

                case 10:
                    month = "Out";
                    break;

                case 11:
                    month = "Nov";
                    break;

                case 12:
                    month = "Dez";
                    break;

                default:
                    month = "Jan";
                    break;
            }

            return month;
        }

        public static string GetData(DateTime data)
        {
            return data.ToString("dd/MM/yyyy").Replace("-", "/");
        }

        public static string GetHora(DateTime data)
        {
            return data.ToString("HH");
        }

        public static string GetMinuto(DateTime data)
        {
            return data.ToString("mm");
        }

        public static string GetHoraMinuto(DateTime data)
        {
            return data.ToString("HH") + ":" + data.ToString("mm");
        }

        public static string GetHoraMinutoSegundo(DateTime data)
        {
            return data.ToString("HH:mm:ss");
        }

        public static int GetTotalMonthsFrom(this DateTime dt1, DateTime dt2)
        {
            DateTime earlyDate = (dt1 > dt2) ? dt2.Date : dt1.Date;
            DateTime lateDate = (dt1 > dt2) ? dt1.Date : dt2.Date;

            // Start with 1 month's difference and keep incrementing
            // until we overshoot the late date
            int monthsDiff = 1;
            while (earlyDate.AddMonths(monthsDiff) <= lateDate)
            {
                monthsDiff++;
            }

            return monthsDiff - 1;
        }

        public static string GetDateForQuery(DateTime d, bool ordem)
        {
            string data = d.Year + "-" + d.Month + "-" + d.Day;

            if (ordem)
                data += " 00:00:00.000";
            else
                data += " 23:59:59.999";

            return data;
        }

        #endregion

        #region Senhas

        /// <summary>
        /// Generates a new random passowrd
        /// </summary>
        /// <returns></returns>
        public static string GeneratePassword()
        {
            // Evitar o L minúsculo para não ser confundido com o número 1
            string caracter = "abcdefhijkmnopqrstuvxwyz123456789";
            string caracterEspecial = "!@#$%&*";

            // Converte em uma matriz de caracteres
            char[] letras = caracter.ToCharArray();
            char[] especiais = caracterEspecial.ToCharArray();

            // Embaralhando n vezes
            Embaralhar(ref letras, especiais, 5);

            // Juntando as partes e formando uma senha de 8 digitos e/ou caracteres
            string senha = new String(letras).Substring(0, 8);

            return senha;
        }

        /// <summary>
        /// Embaralha o vetor de caracteres n vezes informada
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <param name="vezes"></param>
        private static void Embaralhar(ref char[] array1, char[] array2, int vezes)
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            for (int i = 1; i <= vezes; i++)
            {
                for (int x = 1; x <= array1.Length; x++)
                {
                    Trocar(ref array1[rand.Next(0, array1.Length)], ref array1[rand.Next(0, array1.Length)]);
                }
            }

            array1[rand.Next(0, 7)] = array2[rand.Next(0, array2.Length)];
        }

        /// <summary>
        /// Método que troca os valores de dois caracteres informados
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private static void Trocar(ref char arg1, ref char arg2)
        {
            char strTemp = arg1;
            arg1 = arg2;
            arg2 = strTemp;
        }

        #endregion

        #region Strings

        public static string Split(string msg, int caracteres)
        {
            if (caracteres > msg.Length)
                caracteres = msg.Length;

            return msg.Substring(0, caracteres);
        }

        public static string PreventSqlInjection(string msg)
        {
            return msg.Replace("'", "").Trim();
        }

        public static string ValidateString(string msg)
        {
            return Regex.Replace(msg, "@[^\\w\\._]", "").Trim();
        }

        public static string RemoverAcentos(string str)
        {
            /** Troca os caracteres acentuados por não acentuados **/
            string[] acentos = new string[] { "ç", "Ç", "á", "é", "í", "ó", "ú", "ý", "Á", "É", "Í", "Ó", "Ú", "Ý", "à", "è", "ì", "ò", "ù", "À", "È", "Ì", "Ò", "Ù", "ã", "õ", "ñ", "ä", "ë", "ï", "ö", "ü", "ÿ", "Ä", "Ë", "Ï", "Ö", "Ü", "Ã", "Õ", "Ñ", "â", "ê", "î", "ô", "û", "Â", "Ê", "Î", "Ô", "Û" };
            string[] semAcento = new string[] { "c", "C", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "Y", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U", "a", "o", "n", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "A", "O", "N", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U" };

            for (int i = 0; i < acentos.Length; i++)
            {
                str = str.Replace(acentos[i], semAcento[i]);
            }

            /** Troca os caracteres especiais da string por "" **/
            string[] caracteresEspeciais = { "\\.", ",", "-", ":", "\\(", "\\)", "ª", "\\|", "\\\\", "°" };

            for (int i = 0; i < caracteresEspeciais.Length; i++)
            {
                str = str.Replace(caracteresEspeciais[i], "");
            }

            /** Troca os espaços no início por "" **/
            str = str.Replace("^\\s+", "");
            /** Troca os espaços no início por "" **/
            str = str.Replace("\\s+$", "");
            /** Troca os espaços duplicados, tabulações e etc por  " " **/
            str = str.Replace("\\s+", " ");

            return str;

        }

        public static string RemoverCaracteresEspeciais(string str)
        {
            #region Replace de Acentuação

            str = str.Replace("á", "%");
            str = str.Replace("à", "%");
            str = str.Replace("é", "%");
            str = str.Replace("í", "%");
            str = str.Replace("ó", "%");
            str = str.Replace("ú", "%");

            str = str.Replace("Á", "%");
            str = str.Replace("À", "%");
            str = str.Replace("É", "%");
            str = str.Replace("Í", "%");
            str = str.Replace("Ó", "%");
            str = str.Replace("Ú", "%");

            str = str.Replace("ã", "%");
            str = str.Replace("Ã", "%");
            str = str.Replace("õ", "%");
            str = str.Replace("Õ", "%");

            str = str.Replace("â", "%");
            str = str.Replace("Â", "%");
            str = str.Replace("ê", "%");
            str = str.Replace("Ê", "%");
            str = str.Replace("ô", "%");
            str = str.Replace("Ô", "%");

            str = str.Replace("ç", "%");
            str = str.Replace("Ç", "%");

            #endregion

            return str;
        }

        public static string ToTileCase(string str)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }

        #endregion

        #region Validations

        public static bool IsNumber(string value)
        {
            double numero = 0;

            if (double.TryParse(value, out numero))
                return true;
            else
                return false;
        }

        public static bool IsEmail(string value)
        {
            if (Regex.IsMatch(value, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"))
                return true;
            else
                return false;
        }

        #endregion

        #region Conversions

        #region Primitives Types

        public static int GetInt(string valor)
        {
            int id = 0;
            Int32.TryParse(valor, out id);
            return id;
        }

        public static float GetFloat(string valor)
        {
            float value = 0;
            float.TryParse(valor, out value);
            return value;
        }

        public static long GetLong(string valor)
        {
            long value = 0;
            long.TryParse(valor, out value);
            return value;
        }

        public static double GetDouble(string valor)
        {
            double value = 0;
            double.TryParse(valor, out value);
            return value;
        }

        public static DateTime GetDate(string date, string culture)
        {
            CultureInfo info = new CultureInfo(culture);

            DateTime d = DateTime.MinValue;
            DateTime.TryParse(date, info, DateTimeStyles.None, out d);

            return d;
        }

        public static DateTime GetDate(string date)
        {
            DateTime d = DateTime.MinValue;
            DateTime.TryParse(date, out d);

            return d;
        }

        public static List<DateTime> GetDate(ListItemCollection items, bool IsChecked)
        {
            if (items == null)
                return null;


            List<DateTime> dates = null;

            foreach (ListItem i in items)
            {
                if (i.Selected == IsChecked)
                {
                    DateTime d = GetDate(i.ToString());
                    if (d != DateTime.MinValue)
                    {
                        if (dates == null) dates = new List<DateTime>();
                        dates.Add(d);
                    }
                }
            }

            return dates;
        }

        public static bool GetBoolean(string valor)
        {
            bool value = false;
            bool.TryParse(valor, out value);
            return value;
        }

        #endregion

        #region Base to ExtendedVO - ExtendedVO to Base

        //public static VOCountry BaseExtensionConverter<T>(T i)
        //{
        //    object item;

        //    var propertyInfo = i.GetType().GetProperties();

        //    if (propertyInfo != null && propertyInfo.Count() > 0)
        //    {
        //        foreach(var p in propertyInfo)
        //        {
                    
        //        }
        //    }

        //    return null;
        //}

        public static T CastExamp1<T>(object input)
        {
            return (T)input;
        }

        public static T ConvertExamp1<T>(object input)
        {
            var type = input.GetType();
            
            

            return (T)Convert.ChangeType(input, typeof(T));
        }

        public static dynamic Cast(dynamic obj, Type castTo)
        {
            return Convert.ChangeType(obj, castTo);
        }

        public static T BaseExtensionConverter<T>(object i)
        {
            return (T)i;
        }

        #endregion

        #region ListItems

        public static ListItem GetListItem(int value)
        {
            return new ListItem(value.ToString(), value.ToString());
        }

        public static List<ListItem> GetListItem(List<int> value)
        {
            List<ListItem> items = null;

            foreach (int i in value)
            {
                if (items == null) items = new List<ListItem>();
                items.Add(GetListItem(i));
            }

            return items;
        }

        public static List<ListItem> GetListItem(List<int> value, bool IsChecked)
        {
            List<ListItem> items = null;

            foreach (int i in value)
            {
                ListItem item = GetListItem(i);
                item.Selected = IsChecked;

                if (items == null) items = new List<ListItem>();
                items.Add(item);
            }

            return items;
        }

        public static ListItem GetListItem(string value)
        {
            return new ListItem(value, value);
        }

        public static List<ListItem> GetListItem(List<string> value)
        {
            List<ListItem> items = null;

            foreach (string i in value)
            {
                if (items == null) items = new List<ListItem>();
                items.Add(GetListItem(i));
            }

            return items;
        }

        public static List<ListItem> GetListItem(List<string> value, bool IsChecked)
        {
            List<ListItem> items = null;

            foreach (string i in value)
            {
                ListItem item = GetListItem(i);
                item.Selected = IsChecked;

                if (items == null) items = new List<ListItem>();
                items.Add(item);
            }

            return items;
        }

        public static ListItem GetListItem(DateTime value, string dateFormat)
        {
            return new ListItem(value.ToString(dateFormat), value.ToString(dateFormat));
        }

        public static List<ListItem> GetListItem(List<DateTime> value, string dateFormat)
        {
            List<ListItem> items = null;

            foreach (DateTime i in value)
            {
                if (items == null) items = new List<ListItem>();
                items.Add(GetListItem(i, dateFormat));
            }

            return items;
        }

        public static List<ListItem> GetListItem(List<DateTime> value, string dateFormat, bool IsChecked)
        {
            List<ListItem> items = null;

            foreach (DateTime i in value)
            {
                ListItem item = GetListItem(i, dateFormat);
                item.Selected = IsChecked;

                if (items == null) items = new List<ListItem>();
                items.Add(item);
            }

            return items;
        }

        public static List<string> GetListItem(ListItemCollection items, bool IsChecked)
        {
            if (items == null)
                return null;

            List<string> list = null;

            foreach (ListItem i in items)
            {
                if (i.Selected == IsChecked)
                {
                    if (list == null) list = new List<string>();
                    list.Add(i.Value);
                }
            }

            return list;
        }

        #endregion

        #endregion

        #region CPF

        public static bool ValidarCPF(string cpf)
        {
            #region Sequencias repetidas

            switch (cpf)
            {
                case "00000000000":
                case "11111111111":
                case "22222222222":
                case "33333333333":
                case "44444444444":
                case "55555555555":
                case "66666666666":
                case "77777777777":
                case "88888888888":
                case "99999999999":
                    return false;
            }

            #endregion

            #region Verificando o tamanho da string

            if (cpf.Length != 11)
                return false;

            #endregion

            #region Variaveis

            cpf = cpf.Replace(".", "");
            cpf = cpf.Replace("-", "");

            if (string.IsNullOrEmpty(cpf))
                return false;

            int d1;
            int d2;
            int soma = 0;
            string digitado = string.Empty;
            string calculado = string.Empty;

            // Pesos para calcular o primeiro digito
            int[] peso1 = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            // Pesos para calcular o segundo digito
            int[] peso2 = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, };

            int[] n = new int[11];

            #endregion

            #region Verificando os digitos

            try
            {
                // Quebra cada digito do CPF
                n[0] = Convert.ToInt32(cpf.Substring(0, 1));
                n[1] = Convert.ToInt32(cpf.Substring(1, 1));
                n[2] = Convert.ToInt32(cpf.Substring(2, 1));
                n[3] = Convert.ToInt32(cpf.Substring(3, 1));
                n[4] = Convert.ToInt32(cpf.Substring(4, 1));
                n[5] = Convert.ToInt32(cpf.Substring(5, 1));
                n[6] = Convert.ToInt32(cpf.Substring(6, 1));
                n[7] = Convert.ToInt32(cpf.Substring(7, 1));
                n[8] = Convert.ToInt32(cpf.Substring(8, 1));
                n[9] = Convert.ToInt32(cpf.Substring(9, 1));
                n[10] = Convert.ToInt32(cpf.Substring(10, 1));
            }
            catch
            {
                return false;
            }

            // Calcula cada digito com seu respectivo peso
            for (int i = 0; i <= peso1.GetUpperBound(0); i++)
                soma += (peso1[i] * Convert.ToInt32(n[i]));

            // Pega o resto da divisao
            int resto = soma % 11;

            if (resto == 1 || resto == 0)
                d1 = 0;
            else
                d1 = 11 - resto;

            soma = 0;

            // Calcula cada digito com seu respectivo peso
            for (int i = 0; i <= peso2.GetUpperBound(0); i++)
                soma += (peso2[i] * Convert.ToInt32(n[i]));

            // Pega o resto da divisao
            resto = soma % 11;

            if (resto == 1 || resto == 0)
                d2 = 0;
            else
                d2 = 11 - resto;

            calculado = d1.ToString() + d2.ToString();
            digitado = n[9].ToString() + n[10].ToString();

            // Se os ultimos dois digitos calculados bater com
            // os dois ultimos digitos do cpf entao é válido
            if (calculado == digitado)
                return (true);
            else
                return (false);

            #endregion
        }

        #endregion

        #region Scripts

        public static void RegisterJavasCriptFile(System.Web.UI.Page p, string script)
        {
            p.ClientScript.RegisterClientScriptInclude(Guid.NewGuid().ToString(), script);
        }

        public static void RunJavaScript(System.Web.UI.Page p, string title, string script)
        {
            p.ClientScript.RegisterClientScriptBlock(p.GetType(), title, script);
        }

        public static void RunJavaScriptAjax(System.Web.UI.Page p, string title, string script)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(p.Page, p.Page.GetType(), title, script, true);
        }

        public static void RunJavaScriptFoundation(System.Web.UI.Page p)
        {
            string script = "$(document).ready(function () { $(document).foundation(); });";
            System.Web.UI.ScriptManager.RegisterStartupScript(p.Page, p.Page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        public static void RunJavaScriptReStartFilters(System.Web.UI.Page p)
        {
            string script = "reStartFilter();";
            System.Web.UI.ScriptManager.RegisterStartupScript(p.Page, p.Page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        public static string RunJavaScriptTinyMCE3(System.Web.UI.Page p, string control)
        {
            StringBuilder script = new StringBuilder();

            script.Append("tinymce.init({ ");
            script.Append("    selector: '#").Append(control).Append("', ");
            script.Append("    plugins: [ ");
            script.Append("        'advlist autolink lists link image charmap print preview hr anchor pagebreak', ");
            script.Append("        'searchreplace wordcount visualblocks visualchars code fullscreen', ");
            script.Append("        'insertdatetime media nonbreaking save table contextmenu directionality', ");
            script.Append("        'emoticons template paste textcolor colorpicker textpattern imagetools' ");
            script.Append("    ], ");
            script.Append("    toolbar1: 'insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image', ");
            script.Append("    toolbar2: 'print preview media | forecolor backcolor emoticons', ");
            script.Append("    image_advtab: true, ");
            script.Append("    templates: [ ");
            script.Append("      { title: 'Test template 1', content: 'Test 1' }, ");
            script.Append("      { title: 'Test template 2', content: 'Test 2' } ");
            script.Append("    ], ");
            script.Append("    content_css: [ ");
            script.Append("      '//fast.fonts.net/cssapi/e6dc9b99-64fe-4292-ad98-6974f93cd2a2.css', ");
            script.Append("      '//www.tinymce.com/css/codepen.min.css' ");
            script.Append("    ], ");
            script.Append("     menubar: true, ");
            script.Append("    theme: 'modern', ");
            script.Append("    encoding: 'xml' ");
            script.Append("}); ");
            script.Append("tinyMCE.triggerSave(); ");
            script.Append("window.setTimeout(function() { var ed = tinyMCE.get('").Append(control).Append("'); ed.setContent($('<div/>').html($('#").Append(control).Append("').val()).text());  }, 500);");
            return script.ToString();
        }

        public static void RunJavaScriptTinyMCE2(System.Web.UI.Page p, string control)
        {
            StringBuilder script = new StringBuilder();

            script.Append("tinymce.init({ ");
            script.Append("    selector: '#").Append(control).Append("', ");
            script.Append("    plugins: [ ");
            script.Append("        'advlist autolink lists link image charmap print preview anchor', ");
            script.Append("        'searchreplace visualblocks code fullscreen', ");
            script.Append("        'insertdatetime media table contextmenu paste code' ");
            script.Append("    ], ");
            script.Append("    toolbar: 'undo redo | insert | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image', ");
            script.Append("    content_css: [ ");
            script.Append("      '//fast.fonts.net/cssapi/e6dc9b99-64fe-4292-ad98-6974f93cd2a2.css', ");
            script.Append("      '//www.tinymce.com/css/codepen.min.css' ");
            script.Append("    ], ");
            script.Append("     menubar: true ");
            script.Append("}); ");

            script.Append("tinyMCE.triggerSave(); ");

            System.Web.UI.ScriptManager.RegisterStartupScript(p.Page, p.Page.GetType(), Guid.NewGuid().ToString(), script.ToString(), true);
        }

        public static void RunJavaScriptTinyMCE(System.Web.UI.Page p, string control)
        {
            StringBuilder script = new StringBuilder();

            script.Append("$(document).ready(function () { ");
            script.Append("tinymce.init({ ");
            script.Append("    selector: '#").Append(control).Append("', ");
            script.Append("    plugins: [ ");
            script.Append("        'advlist autolink lists link image charmap print preview hr anchor pagebreak', ");
            script.Append("        'searchreplace wordcount visualblocks visualchars code fullscreen', ");
            script.Append("        'insertdatetime media nonbreaking save table contextmenu directionality', ");
            script.Append("        'emoticons template paste textcolor colorpicker textpattern imagetools' ");
            script.Append("    ], ");
            script.Append("    toolbar1: 'insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image', ");
            script.Append("    toolbar2: 'print preview media | forecolor backcolor emoticons', ");
            script.Append("    image_advtab: true, ");
            script.Append("    templates: [ ");
            script.Append("      { title: 'Test template 1', content: 'Test 1' }, ");
            script.Append("      { title: 'Test template 2', content: 'Test 2' } ");
            script.Append("    ], ");
            script.Append("    content_css: [ ");
            script.Append("      '//fast.fonts.net/cssapi/e6dc9b99-64fe-4292-ad98-6974f93cd2a2.css', ");
            script.Append("      '//www.tinymce.com/css/codepen.min.css' ");
            script.Append("    ], ");
            script.Append("     menubar: true, ");
            script.Append("    theme: 'modern', ");
            script.Append("    encoding: 'xml' ");
            script.Append("}); ");
            script.Append("}); ");

            script.Append("tinyMCE.triggerSave();");

            System.Web.UI.ScriptManager.RegisterStartupScript(p.Page, p.Page.GetType(), Guid.NewGuid().ToString(), script.ToString(), true);
        }

        public static void RunJavaScriptTinyMCEForMobile(System.Web.UI.Page p, string control)
        {
            StringBuilder script = new StringBuilder();

            script.Append("$(document).ready(function () { ");
            script.Append("tinymce.init({ ");
            script.Append("    selector: '#").Append(control).Append("', ");
            script.Append("    plugins: [ ");
            script.Append("        'advlist autolink lists link image charmap print preview anchor', ");
            script.Append("        'searchreplace visualblocks code fullscreen', ");
            script.Append("        'insertdatetime table contextmenu paste code' ");
            script.Append("    ], ");
            script.Append("    toolbar1: 'undo redo | insert | styleselect | bold italic | alignleft aligncenter alignright alignjustify', ");
            script.Append("    templates: [ ");
            script.Append("      { title: 'Test template 1', content: 'Test 1' }, ");
            script.Append("      { title: 'Test template 2', content: 'Test 2' } ");
            script.Append("    ], ");
            script.Append("    content_css: [ ");
            script.Append("      '//fast.fonts.net/cssapi/e6dc9b99-64fe-4292-ad98-6974f93cd2a2.css', ");
            script.Append("      '//www.tinymce.com/css/codepen.min.css' ");
            script.Append("    ], ");
            script.Append("    theme: 'modern', ");
            script.Append("    encoding: 'xml' ");
            script.Append("}); ");
            script.Append("}); ");

            script.Append("tinyMCE.triggerSave();");

            System.Web.UI.ScriptManager.RegisterStartupScript(p.Page, p.Page.GetType(), Guid.NewGuid().ToString(), script.ToString(), true);
        }

        public static void RunJavaScriptSetPageTitle(System.Web.UI.Page p, string title)
        {
            string script = "$('#PageTitle').html('" + title + "');";
            System.Web.UI.ScriptManager.RegisterStartupScript(p.Page, p.Page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        public static void RunJavaScriptSetMaskTextBox(System.Web.UI.Page p, string control)
        {
            string script = "$('#" + control + "').maskMoney();";
            System.Web.UI.ScriptManager.RegisterStartupScript(p.Page, p.Page.GetType(), Guid.NewGuid().ToString(), script, true);
        }

        #endregion

        #region Tableau

        public static string GetTableauTicket()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://reporting.msxi-euro.com/trusted");

            var encoding = new UTF8Encoding();
            // TODO: Add Triumph credential and target site
            var postData = "username=triumph_adm" + "&target_site=Triumph";

            byte[] data = encoding.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        #endregion

        #region Post

        public static string Post(string url, string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var encoding = new UTF8Encoding();
            // TODO: Add MSX credential and target site
            var postData = parameters;

            byte[] data = encoding.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public static string BBBPost(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var xml = new WebClient().DownloadString(url);
                var doc = new XmlDocument();

                try
                {
                    doc.LoadXml(xml);
                }
                catch
                {
                }

                if (doc.ChildNodes != null && doc.ChildNodes.Count > 0)
                    return doc.ChildNodes[0].OuterXml;
            }

            return null;
        }

        #endregion

        #region General Types


        #endregion

        #region Serialization / Deserialization

        public static string Serialize<T>(T items)
        {
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, items);

            return writer.ToString();
        }

        public static T Deserialize<T>(string xml)
        {
            StringReader reader = new StringReader(xml);
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            return (T)serializer.Deserialize(reader);
        }

        #endregion

        #region Calendar

        #endregion

        #region Resize Image

        public static Bitmap ResizeImageAndUpload(System.IO.Stream newFile, double maxHeight, double maxWidth)
        {
            try
            {
                // Declare variable for the conversion
                float ratio;
                // Create variable to hold the image
                System.Drawing.Image thisImage = System.Drawing.Image.FromStream(newFile);
                // Get height and width of current image
                int width = (int)thisImage.Width;
                int height = (int)thisImage.Height;
                // Ratio and conversion for new size
                if (width > maxWidth)
                {
                    ratio = (float)width / (float)maxWidth;
                    width = (int)(width / ratio);
                    height = (int)(height / ratio);
                }
                // Ratio and conversion for new size
                if (height > maxHeight)
                {
                    ratio = (float)height / (float)maxHeight;
                    height = (int)(height / ratio);
                    width = (int)(width / ratio);
                }
                // Create "blank" image for drawing new image
                Bitmap outImage = new Bitmap(width, height);
                Graphics outGraphics = Graphics.FromImage(outImage);
                SolidBrush sb = new SolidBrush(System.Drawing.Color.White);
                // Fill "blank" with new sized image
                outGraphics.FillRectangle(sb, 0, 0, outImage.Width, outImage.Height);
                outGraphics.DrawImage(thisImage, 0, 0, outImage.Width, outImage.Height);
                sb.Dispose();
                outGraphics.Dispose();
                thisImage.Dispose();

                return outImage;



            }
            catch (Exception)
            {
                return null;
            }
        }


        #endregion
    }
}
