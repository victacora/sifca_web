using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


namespace AleProjects.Text
{
        
    public static class AleString
    {
        public const int ALE_STR_INTEGERNUMBER = 1;
        public const int ALE_STR_FLOATNUMBER = 2;
        public const int ALE_STR_HEXNUMBER = 3;

        public const uint ALE_STR_HASALPHACHAR = 0x00000001;
        public const uint ALE_STR_HASNUMCHAR = 0x00000002;
        public const uint ALE_STR_HASALPHANUMCHAR = ALE_STR_HASALPHACHAR + ALE_STR_HASNUMCHAR;
        public const uint ALE_STR_ALPHANUMFIRST = 4;
        public const uint ALE_STR_ALPHANUMLAST = 8;


        public static uint HasAlphaNumChar(string text)
        {
            if (String.IsNullOrEmpty(text)) return 0;

            int[] textUnits = StringInfo.ParseCombiningCharacters(text);
            int n = textUnits.Length;
            int j;
            char c;
            uint res = 0;
            UnicodeCategory uc;

            for (int i = 0; i < n; i++)
            {
                j = textUnits[i];
                c = text[j];

                if (Char.IsLetter(c) || c == '_')
                {
                    res |= ALE_STR_HASALPHACHAR;
                    if (i == 0) res |= ALE_STR_ALPHANUMFIRST; 
                    else if (i + 1 == n) res |= ALE_STR_ALPHANUMLAST;
                }
                else if (Char.IsDigit(c))
                {
                    res |= ALE_STR_HASNUMCHAR;
                    if (i == 0) res |= ALE_STR_ALPHANUMFIRST; 
                    else if (i + 1 == n) res |= ALE_STR_ALPHANUMLAST;
                }
                else if (Char.IsHighSurrogate(c))
                {
                    uc = CharUnicodeInfo.GetUnicodeCategory(text, j);
                    if (uc == UnicodeCategory.UppercaseLetter || uc == UnicodeCategory.LowercaseLetter ||
                        uc == UnicodeCategory.OtherLetter || uc == UnicodeCategory.TitlecaseLetter)
                    {
                        res |= ALE_STR_HASALPHACHAR;
                        if (i == 0) res |= ALE_STR_ALPHANUMFIRST; 
                        else if (i + 1 == n) res |= ALE_STR_ALPHANUMLAST;
                    }
                }

            }

            return res;
        }

        public static bool IsCharAlphaNum(char c)
        {
            return (Char.IsLetterOrDigit(c) || c == '_');
        }

        public static bool IsCharAlphaNum(string text, int pos)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(text, pos);

            return (uc == UnicodeCategory.UppercaseLetter ||
                    uc == UnicodeCategory.LowercaseLetter ||
                    uc == UnicodeCategory.TitlecaseLetter ||
                    uc == UnicodeCategory.DecimalDigitNumber ||
                    text[pos] == '_');

        }

        public static bool IsCharAlpha(char c)
        {
            return (Char.IsLetter(c) || c == '_');
        }

        public static bool IsCharAlpha(string text, int pos)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(text, pos);

            return (uc == UnicodeCategory.UppercaseLetter || 
                    uc == UnicodeCategory.LowercaseLetter || 
                    uc == UnicodeCategory.TitlecaseLetter ||
                    text[pos] == '_');

        }

        // returns character next to character c if c is in even position in Text
        public static char CoupledChar(string text, char c)
        {
            int Hi = text.Length - 1;
            int k = 0;
            int i;

            while (true)
            {
                i = text.IndexOf(c, k);
                if (i < 0 || i == Hi) return '\u0000';
                if (i % 2 == 0) break;
                k = i + 1;
            }

            return text[i + 1];
        }

        // Pos - is an index of opening quote in SText
        // CClose - is a closing quote of text constant
        // BufSize - actual size of buffer for text constant (\n, \u0xffff ets are one char)
        // Flags - when zero SkipTextConst ignores '\'-escapes
        // returns index of next char after closing quote or -1 if error
        public static int SkipTextConst(string text, int pos, char cClose, out int bufSize, bool allowEsc, bool allowCrLf = false)
        {
            int i, j, n, bs, Hi;
            uint u;
            char c;
            string hex = "1234567890abcdefABCDEF";

            bufSize = 0;
            bs = 0;
            Hi = text.Length - 1;
            if (pos > Hi || pos < 0) return -1;

            i = pos + 1;
            while (i <= Hi)
            {
                c = text[i];
                if (c == '\u000d' && !allowCrLf) return -1;  //end of line and not closed constant => error

                if (c == cClose)
                {
                    j = i + 1;
                    while (j <= Hi && text[j] == cClose) j++;
                    n = j - i;
                    bs += n / 2;
                    n = n % 2;

                    if (j > Hi && n == 0) return -1;
                    if (n != 0)
                    {
                        bufSize = bs;
                        return j - pos;
                    }
                    i = j;
                }
                else if (c == '\\' && allowEsc)     // escapes like '\n'
                {
                    i++;
                    bs++;
                    c = text[i];
                    n = Hi - i;
                    switch (c)
                    {
                        case 'U':
                            i++;
                            j = i;
                            n = i + 8 - 1;
                            if (n > Hi) n = Hi;
                            while (i <= n && hex.IndexOf(text[i]) >= 0) i++;
                            n = i - j;
                            if (n != 8) return -1;
                            u = uint.Parse(text.Substring(j, n), NumberStyles.HexNumber);
                            if (u > 0x10FFFF || (u >= 0xD800 && u <= 0xDFFF)) return -1;
                            break;
                        case 'u':
                        case 'x':
                            i++;
                            j = i;
                            n = i + 4 - 1;
                            if (n > Hi) n = Hi;
                            while (i <= n && hex.IndexOf(text[i]) >= 0) i++;
                            n = i - j;
                            if (n == 0 || (c == 'u' && n != 4)) return -1;
                            break;
                        case 'n':
                        case 't':
                        case '\"':
                        case '\'':
                        case '\\':
                        case 'r':
                        case 'b':
                        case 'f':
                        case 'a':
                        case 'v':
                        case 'e':
                            if (n < 0) return -1;
                            i++;
                            break;
                        default:
                            if (c == cClose)
                            {
                                if (n < 0) return -1;
                                i++;
                                break;
                            }
                            return -1;
                    }
                }
                else
                {
                    i++;
                    bs++;
                }
            }

            return -1;
        }

        // Pos - index of first char in text constant (next char after opening quote)
        // CClose - is a closing quote of text constant
        // Count - is equal to BufSize parameter of SkipTextConst (see above)
        // Flags - when zero TextConstValue ignores '\'-escapes
        public static string TextConstValue(string text, int pos, char cClose, int count, bool allowEsc)
        {
            int i, j, k, Hi;
            uint u;
            char c;
            StringBuilder res;
            string hex = "1234567890abcdefABCDEF";

            Hi = text.Length - 1;
            if (pos < 0 || pos > Hi || count <= 0 || pos + count - 1 > Hi || cClose == '\u0000') return "";

            res = new StringBuilder(count);
            j = pos;

            for (i = 0; i < count; i++)
            {
                c = text[j];

                if (c == cClose)
                {
                    res.Append(c);
                    j++;
                }
                else if (c == '\\' && allowEsc)     // escapes like '\n'
                {
                    j++;
                    c = text[j];
                    switch (c)
                    {
                        case 'U':
                            j++;
                            u = uint.Parse(text.Substring(j, 8), NumberStyles.HexNumber);
                            if (u >= 0x10000)
                            {
                                u -= 0x10000;
                                res.Append((char)(u / 0x400 + 0xD800));
                                res.Append((char)(u % 0x400 + 0xDC00));
                            }
                            else res.Append((char)u);
                            j += 7;
                            break;
                        case 'u':
                            j++;
                            res.Append((char)ushort.Parse(text.Substring(j, 4), NumberStyles.HexNumber));
                            j += 3;
                            break;
                        case 'x':
                            j++;
                            k = j;
                            while (j <= Hi && hex.IndexOf(text[j]) >= 0) j++;
                            res.Append((char)ushort.Parse(text.Substring(k, j - k), NumberStyles.HexNumber));
                            j--;
                            break;
                        case 'n':
                            res.Append('\u000a');
                            break;
                        case 'r':
                            res.Append('\u000d');
                            break;
                        case 't':
                            res.Append('\u0009');
                            break;
                        case '\"':
                            res.Append('\"');
                            break;
                        case '\'':
                            res.Append('\'');
                            break;
                        case '\\':
                            res.Append('\\');
                            break;
                        case '0':
                            res.Append('\u0000');
                            break;
                        case 'b':
                            res.Append('\u0008');
                            break;
                        case 'f':
                            res.Append('\u000c');
                            break;
                        case 'a':
                            res.Append('\u0007');
                            break;
                        case 'v':
                            res.Append('\u000b');
                            break;
                        default:
                            res.Append(c == cClose ? cClose : ' ');
                            break;
                    }

                }
                else
                {
                    res.Append(c);
                }

                j++;   
            }

            
            return res.ToString();
        }

        // Pos - index of first char in Text representing number
        // NumType - type of number, see ALE_STR_xxx constant
        // returns index of char after number
        public static int SkipStandardNumber(string text, int pos, out int numType)
        {
            int i, j, Hi;
            uint ntype;
            char c;
            string hex = "1234567890abcdefABCDEF";

            numType = 0;
            Hi = text.Length - 1;
            if (pos > Hi || pos < 0) return -1;

            c = text[pos];
            if (c < '0' || c > '9') return 0;

            if (c != '0' || pos >= Hi - 1 || text[pos + 1] != 'x')  // not a hex number
            {
                i = pos + 1;
                ntype = 0;

                while (i <= Hi)
                {
                    c = text[i];
                    if (c == '.')
                    {
                        if (ntype != 0 || i == Hi || text[i + 1] < '0' || text[i + 1] > '9') break;
                        ntype |= 1;
                    }
                    else if (c == 'E' || c == 'e')
                    {
                        if (ntype > 1 || i == Hi) break;
                        j = i + 1;
                        c = text[j];
                        if (c == '+' || c == '-')
                        {
                            if (j == Hi) break;
                            j++;
                            c = text[j];
                        }
                        if (c < '0' || c > '9') break;
                        ntype |= 2;
                        i = j;
                    }
                    else if (c < '0' || c > '9') break;

                    i++;
                }
                if (ntype != 0) numType = ALE_STR_FLOATNUMBER; else numType = ALE_STR_INTEGERNUMBER;
            }
            else
            {
                i = pos + 2;
                while (i <= Hi && hex.IndexOf(text[i]) >= 0) i++;

                if (i == pos + 2)
                {
                    numType = ALE_STR_INTEGERNUMBER;
                    i = pos + 1;
                }
                else numType = ALE_STR_HEXNUMBER;
            }


            return i - pos;
        }

        public static int SkipLine(string text, int pos)
        {
            int Len = text.Length;
            if (pos < 0) pos = 0;
            if (pos >= Len) return Len;

            int i = text.IndexOf('\u000d', pos);
            if (i >= 0)
            {
                i++;
                if (i < Len && text[i] == '\u000a') i++;
            }
            else i = Len;

            return i;
        }

        // compares text like 
        // t = 'word1      word2   word3 word4   word5' and what = 'word1 word2 word3'. 
        // words in 'what' should be divided by only one space character
        // words in 'text' may be divided by any whitespace character in any count
        // these two strings above are equal and function returns index of next character after last compared word in 'text' (position of ' word4 in 'text')
        // or -1 if strings are not equal
        public static int CompareWords(string text, int pos, string what, StringComparison comparison = StringComparison.Ordinal)
        {
            int Hi = text.Length - 1;
            int WLen = what.Length;
            if (pos < 0 || pos > Hi || WLen == 0 || WLen > Hi - pos + 1) return -1;

            int i = pos;
            int j = 0;
            int n, res;

            while (true)
            {
                n = what.IndexOf(' ', j);
                if (n < 0) n = WLen;
                n -= j;
                res = String.Compare(text, i, what, j, n, comparison);
                if (res != 0) return -1;

                i += n;
                j += n + 1;
                if (j >= WLen) break;

                n = i;
                while (i <= Hi && Char.IsWhiteSpace(text[i])) i++;
                if (i == n || i > Hi) return -1;
            }

            return i;
        }

    }
}
