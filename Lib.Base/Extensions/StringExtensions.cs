using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Lib.Base
{
    public static class StringExtensions
    {
        #region 

        /// <summary>
        /// 
        /// </summary>
        private static readonly Regex DoubleByteRegex = new Regex("[^\x00-\xff]");

        /// <summary>
        /// 
        /// </summary>
        private static readonly Regex CaseRegex = new Regex("[A-H]|[M-Z]");

        /// <summary>
        /// , 12,1.5
        /// </summary>
        /// <param name="s"></param>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static int GetStringLength(this string s, out Char[] nums)
        {
            double num = 0;
            nums = s.ToCharArray();
            for (var i = 0; i < nums.Length; i++)
            {
                var m = DoubleByteRegex.Match(nums[i].ToString());
                if (m.Success)
                {
                    num += 2;
                }
                else
                {
                    var m2 = CaseRegex.Match(nums[i].ToString());
                    if (m2.Success)
                    {
                        num += 1.5;
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            return (int)num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <param name="substitute">，“...”，</param>
        /// <returns></returns>
        public static string SubCutString(this string s, int length, string substitute = "...")
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            //
            Char[] arr;
            //
            if (GetStringLength(s, out arr) <= length)
            {
                return s;
            }
            //substitute
            if (string.IsNullOrEmpty(substitute))
            {
                length -= Encoding.UTF8.GetBytes("...").Length;
            }
            else
            {
                length -= Encoding.UTF8.GetBytes(substitute).Length;
            }
            //
            double byteLength = 0;
            //
            var subBuilder = new StringBuilder();
            //(1.5)
            for (var i = 0; i < arr.Length; i++)
            {
                //
                var doubleByteMatch = DoubleByteRegex.Match(arr[i].ToString());
                if (doubleByteMatch.Success)
                {
                    byteLength += 2;
                }
                else
                {
                    //
                    var caseMatch = CaseRegex.Match(arr[i].ToString());
                    if (caseMatch.Success)
                    {
                        byteLength += 1.5;
                    }
                    else
                    {
                        byteLength++;
                    }
                }
                if (byteLength > length)
                {
                    break;
                }
                subBuilder.Append(arr[i]);
            }
            subBuilder.Append(substitute);
            return subBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <param name="substitute">，“...”，</param>
        /// <returns></returns>
        public static string SubCutString2(this string s, int length, string substitute = "...")
        {
            var ascii = new ASCIIEncoding();
            var tempLen = 0;
            var tempString = "";
            var tempS = ascii.GetBytes(s);
            for (var i = 0; i < tempS.Length; i++)
            {
                if (tempS[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
                try
                {
                    tempString += s.Substring(i, 1);
                }
                catch
                {
                    break;
                }
                if (tempLen > length)
                {
                    break;
                }
            }
            var mybyte = Encoding.Default.GetBytes(s);
            if (mybyte.Length > length)
            {
                tempString += substitute;
            }
            return tempString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="delLength"></param>
        /// <returns></returns>
        public static string TrimEndString(this string s, int delLength)
        {
            if (s == null)
                return null;

            if (s.Length <= delLength)
                return "";

            return s.Substring(0, s.Length - delLength);
        }

        public static string TrimEndString(this string s, string endStr)
        {
            if (!s.EndsWith(endStr))
                return s;

            return s.TrimEndString(endStr.Length);
        }

        public static string TrimStr(this string srcStr, string startStr, string endStr)
        {
            RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline)
                                             | RegexOptions.IgnoreCase);
            string reg = $"(?<={startStr})[\\w\\W]*?(?={endStr})";
            string ret = Regex.Match(srcStr, reg, options).Value;
            return ret;
        }

        public static string TrimLastChar(this string s)
        {
            if (s == null)
                return null;

            if (s.Length > 0)
                return s.Substring(0, s.Length - 1);

            return s;
        }

        public static string RemoveFirstLine(this string s)
        {
            if (s == null)
                return null;

            var r = s.IndexOf("\r", StringComparison.Ordinal);
            var n = s.IndexOf("\n", StringComparison.Ordinal);

            var max = Math.Max(r, n);
            if (max != -1)
                return s.Substring(max + 1);

            return s;
        }

        #endregion

        #region 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speed">（）</param>
        /// <returns></returns>
        public static string ConvertDownloadStr(this double speed)
        {
            var mStrSize = "";
            var factSize = speed;
            if (factSize < 1024.00)
            {
                mStrSize = factSize.ToString("F2") + " Byte/s";
            }
            else if (factSize >= 1024.00 && factSize < 1048576)
            {
                mStrSize = (factSize / 1024.00).ToString("F2") + " K/s";
            }
            else if (factSize >= 1048576 && factSize < 1073741824)
            {
                mStrSize = (factSize / 1024.00 / 1024.00).ToString("F2") + " M/s";
            }
            else if (factSize >= 1073741824)
            {
                mStrSize = (factSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G/s";
            }
            return mStrSize;
        }

        #endregion

        #region 

        /// <summary>
        ///  (SBC case)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(this string input)
        {
            var c = input.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65535 && c[i] < 65375)
                {
                    c[i] = (char)(c[i] - 65248);
                }
            }
            return new string(c);
        }

        /// <summary>
        /// (SBC case)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(this string input)
        {
            //：
            var c = input.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                {
                    c[i] = (char)(c[i] + 65248);
                }
            }
            return new string(c);
        }

        #endregion

        #region 

        /// <summary> 
        /// cnStr 
        /// </summary> 
        /// <param name="cnStr"></param> 
        /// <returns></returns> 
        public static string GetSpellCode(this string cnStr)
        {
            var strTemp = string.Empty;
            var iLen = cnStr.Length;
            for (var i = 0; i <= iLen - 1; i++)
            {
                strTemp += GetCharSpellCode(cnStr.Substring(i, 1));
            }
            return strTemp;
        }

        /// <summary> 
        /// ， 
        /// </summary> 
        /// <param name="cnChar"></param> 
        /// <returns></returns> 
        private static string GetCharSpellCode(this string cnChar)
        {
            var zw = Encoding.Default.GetBytes(cnChar);
            //， 
            if (zw.Length == 1)
            {
                return cnChar.ToUpper();
            }
            //
            int i1 = zw[0];
            int i2 = zw[1];
            long iCnChar = i1 * 256 + i2;
            //expresstion 
            //table of the constant list 
            // 'A'; //45217..45252 
            // 'B'; //45253..45760 
            // 'C'; //45761..46317 
            // 'D'; //46318..46825 
            // 'E'; //46826..47009 
            // 'F'; //47010..47296 
            // 'G'; //47297..47613 

            // 'H'; //47614..48118 
            // 'J'; //48119..49061 
            // 'K'; //49062..49323 
            // 'L'; //49324..49895 
            // 'M'; //49896..50370 
            // 'N'; //50371..50613 
            // 'O'; //50614..50621 
            // 'P'; //50622..50905 
            // 'Q'; //50906..51386 

            // 'R'; //51387..51445 
            // 'S'; //51446..52217 
            // 'T'; //52218..52697 
            //U,V 
            // 'W'; //52698..52979 
            // 'X'; //52980..53640 
            // 'Y'; //53689..54480 
            // 'Z'; //54481..55289 

            // iCnChar match the constant 
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }
            if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }
            if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            return ("?");
        }

        #endregion

        #region 

        /// <summary>
        /// pattern
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsMatch(this string input, string pattern)
        {
            return !input.IsNullOrEmpty() && Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// pattern
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string Match(this string input, string pattern)
        {
            return input.IsNullOrEmpty()
                       ? string.Empty
                       : Regex.Match(input, pattern).Value;
        }

        /// <summary>
        /// ，
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string Replace(this string input, string pattern, string replacement)
        {
            return input.IsNullOrEmpty()
                       ? string.Empty
                       : Regex.Replace(input, pattern, replacement);
        }

        /// <summary>
        /// ，
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string Replace(this string input, string pattern, string replacement, RegexOptions options)
        {
            return input.IsNullOrEmpty()
                       ? string.Empty
                       : Regex.Replace(input, pattern, replacement, options);
        }

        /// <summary>
        /// ，
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceStartToEnd(this string input, string start, string end, string replacement)
        {
            return input.Replace("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", replacement, RegexOptions.Multiline | RegexOptions.Singleline);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns> 
        public static string GetStartToEnd(this string input, string start, string end)
        {
            var rg = new Regex("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(input).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchNumeric(this string input)
        {
            return input.IsMatch(@"^[-]?\d+[.]?\d*$");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchLetter(this string input)
        {
            return input.IsMatch(@"^[A-Za-z]+$");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchNumericAndLetter(this string input)
        {
            return input.IsMatch(@"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchNumericAndLetterAndSign(this string input)
        {
            //，\.
            return input.IsMatch(@"^[A-Za-z0-9\.]+$");
        }

        /// <summary>
        /// IP()
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchIpAddressWithOutPort(this string input)
        {
            return input.IsMatch(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
        }

        #endregion

        #region 

        /// <summary>
        /// Int32
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsInt32(this string input)
        {
            Int32 i;
            return Int32.TryParse(input, out i);
        }

        /// <summary>
        /// Int64（long）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsInt64(this string input)
        {
            Int64 i;
            return Int64.TryParse(input, out i);
        }

        /// <summary>
        /// DateTime
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string input)
        {
            DateTime dateTime;
            return DateTime.TryParse(input, out dateTime);
        }

        /// <summary>
        /// Boolean
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsBoolean(this string input)
        {
            Boolean b;
            return Boolean.TryParse(input, out b);
        }

        /// <summary>
        /// Decimal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string input)
        {
            decimal de;
            return decimal.TryParse(input, out de);
        }

        /// <summary>
        /// Double
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsDouble(this string input)
        {
            double d;
            return double.TryParse(input, out d);
        }

        /// <summary>
        /// Base64
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(this string input)
        {
            return Convert.FromBase64String(input);
        }

        /// <summary>
        /// Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmailAddress(this string email)
        {
            return email.IsMatch(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        }

        #endregion

        #region 

        /// <summary>
        /// （。）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToCamel(this string input)
        {
            if (input.IsNullOrEmpty())
            {
                return input;
            }
            return input[0].ToString().ToLower() + input.Substring(1);
        }

        /// <summary>
        /// （。）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToPascal(this string input)
        {
            if (input.IsNullOrEmpty())
            {
                return input;
            }
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        #endregion

        #region 

        /// <summary>
        ///  MD5 。，，。
        /// </summary>
        public static string ComputeMd5Hash(this string str)
        {
            var hash = str;
            if (str != null)
            {
                var md5 = new MD5CryptoServiceProvider();
                byte[] data = Encoding.ASCII.GetBytes(str);
                data = md5.ComputeHash(data);
                hash = "";
                for (var i = 0; i < data.Length; i++)
                {
                    hash += data[i].ToString("x2");
                }
            }
            return hash;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>-1 a ;0 ;1 b </returns>
        public static int CompareToBase16(String a, String b)
        {
            var isAempty = string.IsNullOrEmpty(a);
            var isBempty = string.IsNullOrEmpty(b);
            if (isAempty && isBempty)
                return 0;

            if (isAempty)
                return 1;

            if (isBempty)
                return -1;

            if (a.ToLower().IndexOf("0x", StringComparison.InvariantCulture) >= 0)
            {
                a = a.Substring(2);
            }
            if (b.ToLower().IndexOf("0x", StringComparison.InvariantCulture) >= 0)
            {
                b = b.Substring(2);
            }
            var aa = Convert.ToInt64(a, 16);
            var bb = Convert.ToInt64(b, 16);
            if (aa > bb)
            {
                return -1;
            }
            if (aa == bb)
            {
                return 0;
            }
            return 1;
        }

        #endregion

        #region 

        /// <summary>
        /// Indicates whether the specified string is null or an Empty string.
        /// </summary>
        /// <param name="s">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        ///  System.String 。(，)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToTrim(this string s)
        {
            return s.IsNullOrEmpty()
                       ? string.Empty
                       : s.Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="strchar"></param>
        /// <returns></returns>
        public static string DelLastChar(this string s, string strchar)
        {
            return s.Substring(0, s.LastIndexOf(strchar));
        }

        /// <summary>
        /// ，
        /// </summary>
        /// <param name="strA">A</param>
        /// <param name="strB">B</param>
        /// <returns></returns>
        public static bool CompareWith(this string strA, string strB)
        {
            return string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 1;
        }

        /// <summary>
        /// "abc\0\0" => "abc" 
        /// </summary>
        public static string TrimEndZero(this string s)
        {
            if (s == null)
            {
                return null;
            }

            var indexOf = s.IndexOf('\0');
            if (indexOf == -1)
            {
                return s;
            }

            return s.Substring(0, indexOf);
        }

        public static string SubStringLeft(this string s, int leftLength)
        {
            if (s == null)
                return null;

            if (s.Length < leftLength)
                return s;

            return s.Substring(0, leftLength);
        }

        #endregion

    }
}