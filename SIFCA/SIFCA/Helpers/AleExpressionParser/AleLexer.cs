using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using AleProjects.Text;

namespace AleProjects.AleLexer
{

    public enum AleTokenType
    {
        NullToken, UnknownText, DecimalNumber, FloatNumber, HexNumber, TextConstant, TextAtom, DateToken, 
        Variable, Operator, Operation, Parentheses, Brackets, ListToken, ListElement,
        Pair, PairLeftPart, PairRightPart, KeyValue, KeyValueKey, KeyValueValue, InitList, ObjectConst, IndexToken
    }

    public class AleToken
    {
        private AleTokenType _Type;
        private int _StartInOrigin;
        private int _LengthInOrigin;
        private string _Value;

        private List<AleToken> _SubElements;
        private List<AleToken> _Container;
        private AleSimpleLexer _Lexer;

        public AleToken() : base() { }

        public AleToken(AleTokenType type, int start, int length, AleSimpleLexer lexer = null)
        {
            _Type = type;
            _StartInOrigin = start;
            _LengthInOrigin = length;
            _SubElements = null;
            _Container = null;
            _Lexer = lexer;
            _Value = "";
        }

        public AleTokenType TypeOfToken
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public int StartInOrigin
        {
            get { return _StartInOrigin; }
            set { _StartInOrigin = value; }
        }

        public int LengthInOrigin
        {
            get { return _LengthInOrigin; }
            set { _LengthInOrigin = value; }
        }

        public string Value
        {
            get { return _Value; }
            set { if (value == null) _Value = ""; else _Value = value; }
        }

        public List<AleToken> SubElements
        {
            get { return _SubElements; }
            set { _SubElements = value; }
        }

        public AleToken this[int index]
        {
            get
            {
                return (_SubElements != null ? _SubElements[index] : null);
            }
        }

        public List<AleToken> Container
        {
            get { return _Container; }
            set { _Container = value; }
        }

        public AleSimpleLexer Lexer
        {
            get { return _Lexer; }
            set { _Lexer = value; }
        }

        public virtual string Name()
        {
            switch (_Type)
            {
                case AleTokenType.UnknownText:
                    return "UnknownText";
                case AleTokenType.DecimalNumber:
                    return "Integer";
                case AleTokenType.FloatNumber:
                    return "Float";
                case AleTokenType.HexNumber:
                    return "Hexadecimal";
                case AleTokenType.TextConstant:
                    return "TextConstant";
                case AleTokenType.TextAtom:
                    return "Atom";
                case AleTokenType.DateToken:
                    return "Date";
                case AleTokenType.Operation:
                    return "Operation";
                case AleTokenType.Operator:
                    return "Operator";
                case AleTokenType.Parentheses:
                    return "Parentheses";
                case AleTokenType.Brackets:
                    return "Brackets";
                case AleTokenType.ListToken:
                    return "List";
                case AleTokenType.ListElement:
                    return "ListElement";
                case AleTokenType.Pair:
                    return "Pair";
                case AleTokenType.KeyValue:
                    return "Key-value";
                case AleTokenType.Variable:
                    return "Variable";
                case AleTokenType.InitList:
                    return "Initialization list";
                case AleTokenType.ObjectConst:
                    return "Object";
                case AleTokenType.IndexToken:
                    return "Index";
                default:
                    return "Token(type=" + _Type.ToString() + ")";
            }
        }

        public int SubElementsCount
        {
            get
            {
                if (_SubElements == null) return 0; else return _SubElements.Count;
            }
        }

        public int SubElementsTotalCount()
        {
            if (_SubElements == null) return 0;
            int m = _SubElements.Count;
            int n = m;

            for (int i = 0; i < m; i++) n += _SubElements[i].SubElementsTotalCount();

            return n;
        }

        public virtual object ToObject()
        {
            object res = null;
            UInt64 res_ui64;
            Int64 res_i64;
            Double res_x;

            switch (_Type)
            {
                case AleTokenType.HexNumber:
                    try
                    {
                        res_ui64 = Convert.ToUInt64(_Value, 16);
                        if (res_ui64 <= Byte.MaxValue) res = Convert.ToByte(res_ui64);
                        else if (res_ui64 <= UInt16.MaxValue) res = Convert.ToUInt16(res_ui64);
                        else if (res_ui64 <= UInt32.MaxValue) res = Convert.ToUInt32(res_ui64);
                        else res = res_ui64;
                    }
                    catch
                    {
                        res = Double.NaN;
                    }
                    break;

                case AleTokenType.DecimalNumber:
                    try
                    {
                        res_i64 = Convert.ToInt64(_Value, 10);
                        if (res_i64 >= Byte.MinValue && res_i64 <= Byte.MaxValue) res = Convert.ToByte(res_i64);
                        else if (res_i64 >= SByte.MinValue && res_i64 <= SByte.MaxValue) res = Convert.ToSByte(res_i64);
                        else if (res_i64 >= UInt16.MinValue && res_i64 <= UInt16.MaxValue) res = Convert.ToUInt16(res_i64);
                        else if (res_i64 >= Int16.MinValue && res_i64 <= Int16.MaxValue) res = Convert.ToInt16(res_i64);
                        else if (res_i64 >= UInt32.MinValue && res_i64 <= UInt32.MaxValue) res = Convert.ToUInt32(res_i64);
                        else if (res_i64 >= Int32.MinValue && res_i64 <= Int32.MaxValue) res = Convert.ToInt32(res_i64);
                        else res = res_i64;
                    }
                    catch
                    {
                        try
                        {
                            res_ui64 = Convert.ToUInt64(_Value, 10);
                            if (res_ui64 <= Byte.MaxValue) res = Convert.ToByte(res_ui64);
                            else if (res_ui64 <= UInt16.MaxValue) res = Convert.ToUInt16(res_ui64);
                            else if (res_ui64 <= UInt32.MaxValue) res = Convert.ToUInt32(res_ui64);
                            else res = res_ui64;
                        }
                        catch
                        {
                            try
                            {
                                res_x = Convert.ToDouble(_Value);
                            }
                            catch
                            {
                                res_x = Double.NaN;
                            }
                            res = res_x;
                        }
                    }

                    break;

                case AleTokenType.FloatNumber:
                    try
                    {
                        res_x = Convert.ToDouble(_Value, new NumberFormatInfo());
                    }
                    catch
                    {
                        res_x = Double.NaN;
                    }
                    res = res_x;
                    break;

                default:
                    res = _Value;
                    break;
            }

            return res;
        }

        public virtual string DebugPrint(int indent = 0)
        {
            string res = new string(' ', indent * 4) + "Name=" + Name() + ": Start=" + StartInOrigin.ToString() + " : Length=" + _LengthInOrigin.ToString() + " : Elements=" + SubElementsCount.ToString() + " : Value=" + (Value == null ? "<null>" : Value) + " : SubElements=" + (_SubElements != null ? "<object>" : "<null>");
            int n = SubElementsCount;
            for (int i = 0; i < n; i++) res += "\u000d\u000a" + _SubElements[i].DebugPrint(indent + 1);

            return res;
        }
    }


    public class TokenValidateEventArgs : EventArgs
    {
        AleToken _Token;
        int _Position;
        List<AleToken> _TokensList;

        public AleToken Token
        {
            get { return _Token; }
            set { _Token = value; }
        }

        public int Position
        {
            get { return _Position; }
            set { _Position = value; }
        }

        public List<AleToken> TokensList
        {
            get { return _TokensList; }
            set { _TokensList = value; }
        }

        public TokenValidateEventArgs(AleToken token, List<AleToken> tokens, int pos)
        {
            _Token = token;
            _Position = pos;
            _TokensList = tokens;
        }
    }


    public class AleSimpleLexer
    {
        public const uint OPTION_ALLOWUNRECOGNIZEDTEXT = 0x00000001;
        public const uint OPTION_IGNORECASE = 0x00000002;
        public const uint OPTION_STRICTSYNTAX = 0x00000004;
        public const uint OPTION_MINUSASPARTOFNUMBER = 0x00000008;
        public const uint OPTION_ALLOWEMPTYLISTMEMBER = 0x00000010;
        public const uint OPTION_NOESCAPESINCONST = 0x00000020;
        public const uint OPTION_ENDOFEXPRESSIONREQUIRED = 0x00000040;
        public const uint OPTION_FORCELIST = 0x00000080;
        public const uint OPTION_FIRSTCALL = 0x80000000;
        public const uint OPTION_OPTIONS = 0x0fffffff;
        public const uint OPTION_DEFAULT = OPTION_ALLOWUNRECOGNIZEDTEXT + OPTION_STRICTSYNTAX + OPTION_MINUSASPARTOFNUMBER + OPTION_ALLOWEMPTYLISTMEMBER;

        public const int ERROR_OK = 0;
        public const int ERROR_UNKNOWN = 2;
        public const int ERROR_INTERNAL = 3;
        public const int ERROR_RECURSION = 4;
        public const int ERROR_INVALIDOPTIONS = 5;
        public const int ERROR_SYNTAX = 6;
        public const int ERROR_UNRECOGNIZEDTEXT = 7;
        public const int ERROR_UNEXPECTEDEND = 8;
        public const int ERROR_NOEXPRESSIONENDING = 9;
        public const int ERROR_NOCLOSINGBRACKET = 10;
        public const int ERROR_INVALIDLIST = 20;
        public const int ERROR_INVALIDPAIR = 30;
        public const int ERROR_INVALIDKEYVALUE = 40;
        public const int ERROR_INVALIDNUMBER = 50;
        public const int ERROR_INVALIDTEXTCONST = 60;
        public const int ERROR_INVALIDDATE = 70;                         // for future

        public const char DEFAULT_VERBATIMSTRINGCHAR = '@';
        public const string DEFAULT_QUOTES = "''\"\"“”‘’«»";
        public const string DEFAULT_PARENTHESES = "()";
        public const string DEFAULT_BRACKETS = "[]{}";
        public const string DEFAULT_LISTSEPARATOR = ",";
        public const string DEFAULT_KEYVALUESEPARATOR = ":";
        public const string DEFAULT_BLOCKCOMMENT = "//";
        public const string DEFAULT_STREAMCOMMENTSTART = "/*";
        public const string DEFAULT_STREAMCOMMENTEND = "*/";

        protected const uint USER_PROCESS_ERROR = 0xffffffff;
        protected const uint USER_PROCESS_NOACTION = 0x00000000;
        protected const uint USER_PROCESS_STOP = 0x00000001;
        protected const uint USER_PROCESS_NEWPOS = 0x00000002;
        protected const uint USER_PROCESS_ADDUNRECOGNIZED = 0x00000004;
        protected const uint USER_PROCESS_NEW = USER_PROCESS_NEWPOS + USER_PROCESS_ADDUNRECOGNIZED;
        protected const uint USER_PROCESS_TEXTEND = USER_PROCESS_STOP + USER_PROCESS_NEWPOS + USER_PROCESS_ADDUNRECOGNIZED;

        protected const int RETURN_ENDOFSTRING = 0;
        protected const int RETURN_GLOBALTEXTEND = 1;
        protected const int RETURN_TEXTEND = 2;
        protected const int RETURN_USER = 3;


        protected string _Text;
        protected int[] _TextUnits;
        protected int _Start;
        protected uint _Options;
        private uint _InternalFlags;
        private int _Error;
        private int _ErrorPos;
        protected uint _RecursionLevel;
        protected uint _MaxRecursionLevel;

        private char _VerbatimStringChar;
        private string _Quotes;
        private string _Parentheses;
        private string _Brackets;
        private string _EndOfExpression;
        private string _ListSep;
        private string _PairSep;
        private string _KeyValueSep;
        private string _BlockComment;
        private string _StreamCommentStart;
        private string _StreamCommentEnd;
        private List<string> _EndOfExpressionItems;

        public event EventHandler<TokenValidateEventArgs> TokenValidate;

        public AleSimpleLexer()
        {
            _Text = "";
            _TextUnits = StringInfo.ParseCombiningCharacters(_Text);
            _Options = OPTION_DEFAULT;
            _Error = 0;
            _ErrorPos = -1;
            _VerbatimStringChar = DEFAULT_VERBATIMSTRINGCHAR;
            _Quotes = DEFAULT_QUOTES;
            _Parentheses = DEFAULT_PARENTHESES;
            _Brackets = DEFAULT_BRACKETS;
            _ListSep = DEFAULT_LISTSEPARATOR;
            _PairSep = "";
            _KeyValueSep = DEFAULT_KEYVALUESEPARATOR;
            _BlockComment = DEFAULT_BLOCKCOMMENT;
            _StreamCommentStart = DEFAULT_STREAMCOMMENTSTART;
            _StreamCommentEnd = DEFAULT_STREAMCOMMENTEND;
            _EndOfExpression = "";
            _EndOfExpressionItems = new List<string>();
            _RecursionLevel = 0;
            _MaxRecursionLevel = 1500;
        }

        public AleSimpleLexer(string text)
        {
            _Text = text;
            _TextUnits = StringInfo.ParseCombiningCharacters(_Text);
            _Options = OPTION_DEFAULT;
            _Error = 0;
            _ErrorPos = -1;
            _VerbatimStringChar = DEFAULT_VERBATIMSTRINGCHAR;
            _Quotes = DEFAULT_QUOTES;
            _Parentheses = DEFAULT_PARENTHESES;
            _Brackets = DEFAULT_BRACKETS;
            _ListSep = DEFAULT_LISTSEPARATOR;
            _PairSep = "";
            _KeyValueSep = DEFAULT_KEYVALUESEPARATOR;
            _BlockComment = DEFAULT_BLOCKCOMMENT;
            _StreamCommentStart = DEFAULT_STREAMCOMMENTSTART;
            _StreamCommentEnd = DEFAULT_STREAMCOMMENTEND;
            _EndOfExpression = "";
            _EndOfExpressionItems = new List<string>();
            _RecursionLevel = 0;
            _MaxRecursionLevel = 1500;
        }

        // properties

        protected uint RecursionLevel
        {
            get { return _RecursionLevel; }
            set { _RecursionLevel = value; }
        }

        protected uint InternalFlags
        {
            get { return _InternalFlags; }
            set { _InternalFlags = value; }
        }

        protected int TextUnitsCount
        {
            get { return _TextUnits.Length; }
        }

        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                _TextUnits = StringInfo.ParseCombiningCharacters(_Text);
                SetError(-1, ERROR_OK);
            }
        }

        public uint Options
        {
            get { return _Options; }
            set { SetOptions(value); }
        }

        public int ErrorCode
        {
            get { return _Error; }
            protected set { _Error = value; }
        }

        public int ErrorPosition
        {
            get { return _ErrorPos; }
            protected set { _ErrorPos = value; }
        }

        public uint MaxRecursionLevel
        {
            get { return _MaxRecursionLevel; }
            set { _MaxRecursionLevel = value; }
        }

        public bool IgnoreCase
        {
            get { return (_Options & OPTION_IGNORECASE) != 0; }
            set
            {
                if (value) _Options |= OPTION_IGNORECASE; else _Options &= 0xffffffff - OPTION_IGNORECASE;
            }
        }

        public int ErrorLine
        {
            get { return ErrorPosToLine(_ErrorPos); }
        }

        public int ErrorCol
        {
            get { return ErrorPosToCol(_ErrorPos); }
        }

        public char VerbatimStringChar
        {
            get { return _VerbatimStringChar; }
            set { _VerbatimStringChar = value; }
        }

        public string Quotes
        {
            get { return _Quotes; }
            set { _Quotes = String.IsNullOrEmpty(value) ? "" : value.Trim(); }
        }

        public string Parentheses
        {
            get { return _Parentheses; }
            set { _Parentheses = String.IsNullOrEmpty(value) ? "" : value.Trim(); }
        }

        public char OpeningParenthesis
        {
            get
            {
                if (_Parentheses.Length != 0) return _Parentheses[0]; else return '\u0000';
            }
        }

        public char ClosingParenthesis
        {
            get
            {
                if (_Parentheses.Length > 1) return _Parentheses[1]; else return '\u0000';
            }
        }

        public string Brackets
        {
            get { return _Brackets; }
            set { _Brackets = String.IsNullOrEmpty(value) ? "" : value.Trim(); }
        }

        public string ListSeparator
        {
            get { return _ListSep; }
            set
            {
                if (value != null) _ListSep = value.Trim(); else _ListSep = "";
                uint i = AleString.HasAlphaNumChar(_ListSep);
                _InternalFlags &= (0xffffffff - AleString.ALE_STR_HASALPHANUMCHAR);
                _InternalFlags |= i;
            }
        }

        public string PairSeparator
        {
            get { return _PairSep; }
            set
            {
                if (value != null) _PairSep = value.Trim(); else _PairSep = "";
                uint i = AleString.HasAlphaNumChar(_PairSep) << 4;
                _InternalFlags &= (0xffffffff - (AleString.ALE_STR_HASALPHANUMCHAR << 4));
                _InternalFlags |= i;
            }
        }

        public string KeyValueSeparator
        {
            get { return _KeyValueSep; }
            set
            {
                if (value != null) _KeyValueSep = value.Trim(); else _KeyValueSep = "";
                uint i = AleString.HasAlphaNumChar(_KeyValueSep) << 8;
                _InternalFlags &= (0xffffffff - (AleString.ALE_STR_HASALPHANUMCHAR << 8));
                _InternalFlags |= i;
            }
        }

        public string BlockComment
        {
            get { return _BlockComment; }
            set
            {
                if (value != null) _BlockComment = value.Trim(); else _BlockComment = "";
                uint i = AleString.HasAlphaNumChar(_BlockComment) << 12;
                _InternalFlags &= (0xffffffff - (AleString.ALE_STR_HASALPHANUMCHAR << 12));
                _InternalFlags |= i;
            }
        }

        public string StreamCommentStart
        {
            get { return _StreamCommentStart; }
            set
            {
                if (value != null) _StreamCommentStart = value.Trim(); else _StreamCommentStart = "";
                uint i = AleString.HasAlphaNumChar(_StreamCommentStart) << 16;
                _InternalFlags &= (0xffffffff - (AleString.ALE_STR_HASALPHANUMCHAR << 16));
                _InternalFlags |= i;
            }
        }

        public string StreamCommentEnd
        {
            get { return _StreamCommentEnd; }
            set
            {
                if (value != null) _StreamCommentEnd = value.Trim(); else _StreamCommentEnd = "";
                uint i = AleString.HasAlphaNumChar(_StreamCommentEnd) << 20;
                _InternalFlags &= (0xffffffff - (AleString.ALE_STR_HASALPHANUMCHAR << 20));
                _InternalFlags |= i;
            }
        }

        public string EndOfExpression
        {
            get { return _EndOfExpression; }
            set
            {
                string S = String.IsNullOrEmpty(value) ? "" : value.Trim();
                if (S == _EndOfExpression) return;

                _EndOfExpression = "";
                _EndOfExpressionItems.Clear();

                int Hi = S.Length - 1;
                int i = 0;
                int j, n;

                while (i <= Hi)
                {
                    if (S[i] == '"' || S[i] == '\'')
                    {
                        j = AleString.SkipTextConst(S, i, S[i], out n, true);
                        if (j >= 0) j += i;
                        if (j < 0 || (j <= Hi && S[j] != ' '))
                        {
                            _EndOfExpressionItems.Clear();
                            return;
                        }
                        _EndOfExpressionItems.Add(AleString.TextConstValue(S, i + 1, S[i], n, (_Options & OPTION_NOESCAPESINCONST) == 0));
                    }
                    else
                    {
                        j = i;
                        while (j <= Hi && S[j] != ' ') j++;
                        _EndOfExpressionItems.Add(S.Substring(i, j - i));
                    }

                    i = j + 1;
                }

                _EndOfExpression = S;
            }
        }

        //methods

        protected virtual void SetOptions(uint options)
        {
            _Options = options;
        }

        protected virtual bool ProcessPairs(List<AleToken> tokens)
        {
            if (_PairSep.Length == 0) return true;

            int n = tokens.Count;
            int i = 0;
            List<AleToken> subelements;

            while (i < n)
            {
                if (tokens[i].TypeOfToken == AleTokenType.Pair)
                {
                    if (i == 0 || i == n - 1 || tokens[i + 1].TypeOfToken == AleTokenType.Pair)
                    {
                        SetError(tokens[i].StartInOrigin, ERROR_INVALIDPAIR);
                        return false;
                    }

                    subelements = new List<AleToken>();
                    subelements.Add(tokens[i - 1]);
                    subelements.Add(tokens[i + 1]);
                    subelements[0].Container = subelements;
                    subelements[1].Container = subelements;
                    tokens.RemoveAt(i - 1);
                    i--; // because of delete
                    tokens.RemoveAt(i + 1);
                    tokens[i].StartInOrigin = subelements[0].StartInOrigin;
                    tokens[i].LengthInOrigin = subelements[1].StartInOrigin + subelements[1].LengthInOrigin - tokens[i].StartInOrigin;
                    tokens[i].SubElements = subelements;
                    if (!UserValidate(tokens[i], tokens, i)) return false;
                    n -= 2;
                }

                i++;
            }

            return true;
        }

        protected virtual bool ProcessKeyValues(List<AleToken> tokens)
        {
            if (_KeyValueSep.Length == 0) return true;

            int n = tokens.Count;
            int i = 0;
            List<AleToken> subelements;

            while (i < n)
            {
                if (tokens[i].TypeOfToken == AleTokenType.KeyValue)
                {
                    if (i == 0 || i == n - 1 || tokens[i - 1].TypeOfToken == AleTokenType.KeyValue || tokens[i + 1].TypeOfToken == AleTokenType.KeyValue)
                    {
                        SetError(tokens[i].StartInOrigin, ERROR_INVALIDKEYVALUE);
                        return false;
                    }

                    subelements = new List<AleToken>();
                    subelements.Add(tokens[i - 1]);
                    subelements.Add(tokens[i + 1]);
                    subelements[0].Container = subelements;
                    subelements[1].Container = subelements;
                    tokens.RemoveAt(i - 1);
                    i--; // because of delete
                    tokens.RemoveAt(i + 1);
                    tokens[i].StartInOrigin = subelements[0].StartInOrigin;
                    tokens[i].LengthInOrigin = subelements[1].StartInOrigin + subelements[1].LengthInOrigin - tokens[i].StartInOrigin;
                    tokens[i].SubElements = subelements;
                    if (!UserValidate(tokens[i], tokens, i)) return false;
                    n -= 2;
                }

                i++;
            }

            return true;
        }

        private bool IsNumber(int pos, uint flags)
        {
            int i = _TextUnits[pos];
            char c = _Text[i];

            if ((c < '0' || c > '9') && (c != '-' || (flags & OPTION_MINUSASPARTOFNUMBER) == 0)) return false;

            if (c == '-')
            {
                if (pos == _TextUnits.Length - 1) return false;
                c = _Text[i + 1];
                if (c < '0' || c > '9') return false;
            }

            return true;
        }

        private int IsListSeparator(int pos, uint flags)
        {
            int Len = _ListSep.Length;
            int i = _TextUnits[pos];
            if (Len == 0 || i + Len > _Text.Length) return -1;

            int res;
            bool ignoreCase = (_InternalFlags & AleString.ALE_STR_HASALPHACHAR) != 0 && (flags & OPTION_IGNORECASE) != 0;
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if ((!ignoreCase && _Text[i] != _ListSep[0]) || String.Compare(_Text, i, _ListSep, 0, Len, comparison) != 0) return -1;

            res = NextTextUnit(pos, Len);
            if (Len != CharsBetweenUnits(pos, res)) return -1;

            if ((_InternalFlags & AleString.ALE_STR_HASALPHANUMCHAR) == 0 || (flags & OPTION_STRICTSYNTAX) == 0) return res;

            if (pos > _Start && AleString.IsCharAlphaNum(_Text, _TextUnits[pos - 1]) && AleString.IsCharAlphaNum(_ListSep, 0)) return -1;
            if (res < _TextUnits.Length && AleString.IsCharAlphaNum(_Text, _TextUnits[res]) && AleString.IsCharAlphaNum(_Text, _TextUnits[res - 1])) return -1;

            return res;
        }

        private int IsPairSeparator(int pos, uint flags)
        {
            int Len = _PairSep.Length;
            int i = _TextUnits[pos];
            if (Len == 0 || i + Len > _Text.Length) return -1;

            int res;
            bool ignoreCase = ((_InternalFlags >> 4) & AleString.ALE_STR_HASALPHACHAR) != 0 && (flags & OPTION_IGNORECASE) != 0;
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if ((!ignoreCase && _Text[i] != _PairSep[0]) || String.Compare(_Text, i, _PairSep, 0, Len, comparison) != 0) return -1;

            res = NextTextUnit(pos, Len);
            if (Len != CharsBetweenUnits(pos, res)) return -1;

            if (((_InternalFlags >> 4) & AleString.ALE_STR_HASALPHANUMCHAR) == 0 || (flags & OPTION_STRICTSYNTAX) == 0) return res;

            if (pos > _Start && AleString.IsCharAlphaNum(_Text, _TextUnits[pos - 1]) && AleString.IsCharAlphaNum(_PairSep, 0)) return -1;
            if (res < _TextUnits.Length && AleString.IsCharAlphaNum(_Text, _TextUnits[res]) && AleString.IsCharAlphaNum(_Text, _TextUnits[res - 1])) return -1;

            return res;
        }

        private int IsKeyValueSeparator(int pos, uint flags)
        {
            int Len = _KeyValueSep.Length;
            int i = _TextUnits[pos];
            if (Len == 0 || i + Len > _Text.Length) return -1;

            int res;
            bool ignoreCase = ((_InternalFlags >> 8) & AleString.ALE_STR_HASALPHACHAR) != 0 && (flags & OPTION_IGNORECASE) != 0;
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if ((!ignoreCase && _Text[i] != _KeyValueSep[0]) || String.Compare(_Text, i, _KeyValueSep, 0, Len, comparison) != 0) return -1;

            res = NextTextUnit(pos, Len);
            if (Len != CharsBetweenUnits(pos, res)) return -1;

            if (((_InternalFlags >> 8) & AleString.ALE_STR_HASALPHANUMCHAR) == 0 || (flags & OPTION_STRICTSYNTAX) == 0) return res;

            if (pos > _Start && AleString.IsCharAlphaNum(_Text, _TextUnits[pos - 1]) && AleString.IsCharAlphaNum(_KeyValueSep, 0)) return -1;
            if (res < _TextUnits.Length && AleString.IsCharAlphaNum(_Text, _TextUnits[res]) && AleString.IsCharAlphaNum(_Text, _TextUnits[res - 1])) return -1;

            return res;
        }

        private int IsBlockComment(int pos, uint flags)
        {
            int Len = _BlockComment.Length;
            int i = _TextUnits[pos];
            if (Len == 0 || i + Len > _Text.Length) return -1;

            int res;
            bool ignoreCase = ((_InternalFlags >> 12) & AleString.ALE_STR_HASALPHACHAR) != 0 && (flags & OPTION_IGNORECASE) != 0;
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if ((!ignoreCase && _Text[i] != _BlockComment[0]) || String.Compare(_Text, i, _BlockComment, 0, Len, comparison) != 0) return -1;

            res = NextTextUnit(pos, Len);
            if (Len != CharsBetweenUnits(pos, res)) return -1;

            if (((_InternalFlags >> 12) & AleString.ALE_STR_HASALPHANUMCHAR) == 0 || (flags & OPTION_STRICTSYNTAX) == 0) return res;

            if (pos > _Start && AleString.IsCharAlphaNum(_Text, _TextUnits[pos - 1]) && AleString.IsCharAlphaNum(_BlockComment, 0)) return -1;
            if (res < _TextUnits.Length && AleString.IsCharAlphaNum(_Text, _TextUnits[res]) && AleString.IsCharAlphaNum(_Text, _TextUnits[res - 1])) return -1;

            return res;
        }

        private int IsStreamComment(int pos, uint flags)
        {
            int Len = _StreamCommentStart.Length;
            int i = _TextUnits[pos];
            if (Len == 0 || i + Len > _Text.Length) return -1;

            int res;
            bool ignoreCase = ((_InternalFlags >> 16) & AleString.ALE_STR_HASALPHACHAR) != 0 && (flags & OPTION_IGNORECASE) != 0;
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if ((!ignoreCase && _Text[i] != _StreamCommentStart[0]) || String.Compare(_Text, i, _StreamCommentStart, 0, Len, comparison) != 0) return -1;

            res = NextTextUnit(pos, Len);
            if (Len != CharsBetweenUnits(pos, res)) return -1;

            if (((_InternalFlags >> 16) & AleString.ALE_STR_HASALPHANUMCHAR) == 0 || (flags & OPTION_STRICTSYNTAX) == 0) return res;

            if (pos > _Start && AleString.IsCharAlphaNum(_Text, _TextUnits[pos - 1]) && AleString.IsCharAlphaNum(_StreamCommentStart, 0)) return -1;

            return res;
        }

        private int IsTextEnd(int pos, uint flags, out int Ret, string[] textEndItems)
        {
            int i, l, n, m, res, Hi, Len;
            string SEnd;

            Ret = RETURN_ENDOFSTRING;

            Hi = _TextUnits.Length - 1;
            if (pos < 0 || pos > Hi) return -1;

            StringComparison comparison = (flags & OPTION_IGNORECASE) != 0 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            if (textEndItems != null) m = textEndItems.Length; else m = 0;
            n = m + _EndOfExpressionItems.Count;
            res = -1;

            for (i = 0; i < n; i++)
            {
                if (i < m) SEnd = textEndItems[i]; else SEnd = _EndOfExpressionItems[i - m];
                Len = SEnd.Length;
                l = _TextUnits[pos];
                if (l + Len > _Text.Length) continue;

                res = AleString.CompareWords(_Text, l, SEnd, comparison);
                if (res < 0) continue;

                l = res - l;
                res = NextTextUnit(pos, l);
                if (l != CharsBetweenUnits(pos, res)) return -1;

                if (
                    (flags & OPTION_STRICTSYNTAX) == 0 ||
                    (
                     (pos == _Start || !AleString.IsCharAlphaNum(_Text, _TextUnits[pos - 1]) || !AleString.IsCharAlphaNum(SEnd, 0)) &&
                     (res > Hi || !AleString.IsCharAlphaNum(_Text, _TextUnits[res]) || !AleString.IsCharAlphaNum(_Text, _TextUnits[res - 1]))
                    )
                   )
                {
                    if (i < m) Ret = RETURN_TEXTEND; else Ret = RETURN_GLOBALTEXTEND;
                    break;
                }

                res = -1;
            }

            return res;
        }

        protected int NextTextUnit(int pos, int len)
        {
            int Hi = _TextUnits.Length - 1;
            if (pos < 0 || pos > Hi || len <= 0) return -1;

            int k = _TextUnits[pos] + len - 1;
            if (k >= _Text.Length) return Hi + 1;

            int i = pos;
            while (i <= Hi && _TextUnits[i] <= k) i++;

            return i;
        }

        protected int TextUnitSize(int pos)
        {
            if (pos == _TextUnits.Length - 1) return _Text.Length - _TextUnits[pos];
            return _TextUnits[pos + 1] - _TextUnits[pos];
        }

        protected int CharsBetweenUnits(int pos1, int pos2)
        {
            int i;

            if (pos2 >= _TextUnits.Length) i = _Text.Length; else i = _TextUnits[pos2];
            return i - _TextUnits[pos1];
        }

        protected int TextUnitByCharIndex(int charIndex)
        {
            if (charIndex < 0 || charIndex >= _Text.Length) return -1;
            if (_TextUnits.Length == _Text.Length) return charIndex;

            int i = 0;
            int l = 0;
            int r = _TextUnits.Length - 1;

            while (l != r)
            {
                i = l + (r - l) / 2;
                if (_TextUnits[i] == charIndex) return i;
                if (_TextUnits[i] > charIndex) r = i; else l = i + 1;
            }

            if (_TextUnits[l] > charIndex) l--;
            return l;
        }

        protected virtual bool UserValidate(AleToken t, List<AleToken> tokens, int pos = -1)
        {
            if (pos < 0) pos = tokens.Count - 1;
            TokenValidateEventArgs e = new TokenValidateEventArgs(t, tokens, pos);
            EventHandler<TokenValidateEventArgs> handler = TokenValidate;
            if (handler != null)
            {
                handler(this, e);
                if (_Error != ERROR_OK) return false;
            }

            return true;
        }

        protected virtual bool CheckOptions(uint flags)
        {
            return true;
        }

        protected virtual AleToken CreateToken(AleTokenType tokenType, int start, int length, string value = "", List<AleToken> container = null)
        {
            AleToken Token = new AleToken(tokenType, start, length, this);
            Token.Container = container;

            switch (tokenType)
            {
                case AleTokenType.UnknownText:
                case AleTokenType.DecimalNumber:
                case AleTokenType.FloatNumber:
                case AleTokenType.HexNumber:
                case AleTokenType.TextAtom:
                case AleTokenType.DateToken:
                case AleTokenType.Operation:
                case AleTokenType.Operator:
                case AleTokenType.Variable:
                    Token.Value = _Text.Substring(start, length);
                    break;
                case AleTokenType.TextConstant:
                    Token.Value = value;
                    break;
                case AleTokenType.Parentheses:
                case AleTokenType.Brackets:
                    Token.Value = _Text.Substring(start, 1);
                    break;
                case AleTokenType.ListToken:
                case AleTokenType.ListElement:
                    Token.Value = _ListSep;
                    break;
                case AleTokenType.KeyValue:
                    Token.Value = _KeyValueSep;
                    break;
                case AleTokenType.Pair:
                    Token.Value = _PairSep;
                    break;
                default:
                    Token.Value = value;
                    break;
            }

            return Token;
        }

        protected virtual uint UserProcessText(ref int pos, List<AleToken> tokens, string[] textEndItems, int unrecognizedPos, uint flags)
        {
            return USER_PROCESS_NOACTION;
        }

        protected int ProcessText(int start, out List<AleToken> result, out int ret, string[] textEndItems, uint flags)
        {
            int SaveStart, Pos, i, l, k, m, unrec, Hi;
            AleTokenType tokenType = AleTokenType.NullToken;
            uint ures;
            char c;
            List<AleToken> list = null, subelements = null;
            List<AleToken> tokens = new List<AleToken>();
            AleToken E;
            bool EndIsReached = false;

            ret = RETURN_ENDOFSTRING;
            result = null;

            Hi = _TextUnits.Length - 1;
            if (start < 0 && SetError(0, ERROR_INTERNAL)) return -1;
            if (start > Hi && SetError(Hi, ERROR_UNEXPECTEDEND, true)) return -1;
            if (!CheckOptions(flags) && SetError(0, ERROR_INVALIDOPTIONS)) return -1;

            if (_RecursionLevel > _MaxRecursionLevel && SetError(start, ERROR_RECURSION, true)) return -1;
            _RecursionLevel++;

            SaveStart = _Start;
            _Start = start;
            Pos = start;

            i = Pos;
            unrec = i;

            while (Pos <= Hi) //main loop
            {
                i = Pos;

                //user processing
                m = tokens.Count;
                ures = UserProcessText(ref Pos, tokens, textEndItems, unrec, flags + OPTION_FIRSTCALL);
                if (ures == USER_PROCESS_ERROR) return -1;
                if ((ures & USER_PROCESS_ADDUNRECOGNIZED) != 0)
                {
                    k = i - 1;
                    while (k > unrec && Char.IsWhiteSpace(_Text, _TextUnits[k])) k--;
                    if (k >= unrec) tokens.Insert(m, CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, k + 1), container: tokens));
                }
                if (ures != USER_PROCESS_NOACTION) unrec = Pos;
                if ((ures & USER_PROCESS_STOP) != 0)
                {
                    EndIsReached = true;
                    ret = RETURN_USER;
                    break;
                }
                if ((ures & USER_PROCESS_NEWPOS) != 0) continue;

                //*** Whitespaces
                if (Char.IsWhiteSpace(_Text, _TextUnits[Pos]))
                {
                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));
                    while (Pos <= Hi && Char.IsWhiteSpace(_Text, _TextUnits[Pos])) Pos++;
                    unrec = Pos;
                    continue;
                }

                //*** Text end
                k = IsTextEnd(Pos, flags, out ret, textEndItems);
                if (k > 0)
                {
                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));
                    Pos = k;
                    unrec = Pos;
                    EndIsReached = true;
                    break;
                }

                c = _Text[_TextUnits[Pos]];

                //*** Number
                if (IsNumber(Pos, flags))
                {
                    k = Pos;
                    if (c == '-') k++;
                    l = AleString.SkipStandardNumber(_Text, _TextUnits[k], out m);
                    if (l <= 0 && SetError(i, ERROR_INVALIDNUMBER, true)) return -1;

                    if (m == AleString.ALE_STR_INTEGERNUMBER) tokenType = AleTokenType.DecimalNumber;
                    else if (m == AleString.ALE_STR_HEXNUMBER) tokenType = AleTokenType.HexNumber;
                    else tokenType = AleTokenType.FloatNumber;

                    k = NextTextUnit(k, l);
                    if (c == '-') l++;
                    Pos = k;

                    if (CharsBetweenUnits(i, k) == l) // no non-space mark (diacritic) after last digit
                    {
                        k = 0;
                        if ((_Options & OPTION_STRICTSYNTAX) != 0)
                        {
                            if (Pos <= Hi && AleString.IsCharAlphaNum(_Text, _TextUnits[Pos])) k = 1; //alphanumeric char or '_' after number?
                            if (i > _Start && AleString.IsCharAlphaNum(_Text, _TextUnits[i - 1])) k += 2; //alphanumeric char or '_' before number?
                        }
                    }
                    else k = 1; // non-space mark (diacritic) after last digit makes unrecognized text instead of number

                    if (k != 0)
                    {
                        if ((flags & OPTION_ALLOWUNRECOGNIZEDTEXT) == 0 && SetError(unrec, ERROR_UNRECOGNIZEDTEXT, true)) return -1;
                        while (Pos <= Hi && AleString.IsCharAlphaNum(_Text, _TextUnits[Pos])) Pos++;
                        continue;
                    }

                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));
                    E = CreateToken(tokenType, _TextUnits[i], CharsBetweenUnits(i, Pos), container: tokens);
                    tokens.Add(E);
                    if (!UserValidate(E, tokens)) return -1;
                    unrec = Pos;
                    continue;
                }

                //*** Text constant in quotes
                k = TextUnitSize(Pos);
                if (c != _VerbatimStringChar || k != 1 || _VerbatimStringChar == '\0') c = AleString.CoupledChar(_Quotes, c);
                if (c != '\0' && k == 1)
                {
                    if (c == _VerbatimStringChar)
                    {
                        if (Pos == Hi && SetError(i, ERROR_INVALIDTEXTCONST, true)) return -1;
                        Pos++;
                        c = AleString.CoupledChar(_Quotes, _Text[_TextUnits[Pos]]);
                        if (c == '\0' && SetError(i, ERROR_INVALIDTEXTCONST, true)) return -1;
                    }

                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));

                    l = AleString.SkipTextConst(_Text, _TextUnits[Pos], c, out m, ((flags & OPTION_NOESCAPESINCONST) == 0) && (Pos == i), Pos != i);
                    if (l < 0 && SetError(i, ERROR_INVALIDTEXTCONST, true)) return -1;
                    k = NextTextUnit(Pos, l);
                    // non-space combining mark (diacritic) after closing quote produces an error
                    if (CharsBetweenUnits(Pos, k) != l && SetError(i, ERROR_INVALIDTEXTCONST, true)) return -1;

                    E = CreateToken(AleTokenType.TextConstant, _TextUnits[i], CharsBetweenUnits(i, k),
                                    AleString.TextConstValue(_Text, _TextUnits[Pos] + 1, c, m, ((_Options & OPTION_NOESCAPESINCONST) == 0 && Pos == i)), tokens);
                    tokens.Add(E);
                    if (!UserValidate(E, tokens)) return -1;
                    Pos = k;
                    unrec = Pos;
                    continue;
                }

                //*** List members separator
                k = IsListSeparator(Pos, flags);
                if (k > 0)
                {
                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));
                    if (tokens.Count == 0 && (flags & OPTION_ALLOWEMPTYLISTMEMBER) == 0 && SetError(Pos, ERROR_INVALIDLIST, true)) return -1;
                    Pos = k;

                    if (!ProcessPairs(tokens)) return -1;
                    if (!ProcessKeyValues(tokens)) return -1;

                    if (list == null)
                    {
                        list = new List<AleToken>();
                        E = CreateToken(AleTokenType.ListElement, _TextUnits[start], CharsBetweenUnits(start, i), container: list);
                        if (tokens.Count > 0) E.SubElements = tokens;
                        list.Add(E);
                    }
                    else
                    {
                        E = list[list.Count - 1];
                        E.LengthInOrigin = _TextUnits[i] - E.StartInOrigin;
                        if (tokens.Count > 0) E.SubElements = tokens;
                    }

                    E = CreateToken(AleTokenType.ListElement, _TextUnits[i], 0, container: list);
                    list.Add(E);
                    tokens = new List<AleToken>();
                    unrec = Pos;
                    continue;
                }

                //*** Parentheses and brackets
                c = _Text[_TextUnits[Pos]];
                if (c != OpeningParenthesis)
                {
                    c = AleString.CoupledChar(_Brackets, _Text[_TextUnits[Pos]]);
                    tokenType = AleTokenType.Brackets;
                }
                else
                {
                    c = ClosingParenthesis;
                    tokenType = AleTokenType.Parentheses;
                }

                if (c != '\u0000' && TextUnitSize(Pos) == 1)
                {
                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));
                    Pos++;
                    if (Pos > Hi && SetError(i, ERROR_NOCLOSINGBRACKET, true)) return -1;
                    Pos = ProcessText(Pos, out subelements, out m, new string[] { c.ToString() }, flags | OPTION_ENDOFEXPRESSIONREQUIRED);
                    if (Pos < 0)
                    {
                        if (ErrorCode == ERROR_NOEXPRESSIONENDING) SetError(i, ERROR_NOCLOSINGBRACKET, true);
                        return -1;
                    }
                    if (m != RETURN_TEXTEND && SetError(i, ERROR_NOCLOSINGBRACKET, true)) return -1;
                    E = CreateToken(tokenType, _TextUnits[i], CharsBetweenUnits(i, Pos), container: tokens);
                    E.SubElements = subelements;
                    tokens.Add(E);
                    unrec = Pos;
                    continue;
                }

                //*** Block comment like '//' (until CRLF or CR or until the end of the text
                k = IsBlockComment(Pos, flags);
                if (k > 0)
                {
                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));
                    Pos = k;
                    k = _TextUnits[Pos];
                    Pos = NextTextUnit(Pos, AleString.SkipLine(_Text, k) - k);
                    unrec = Pos;
                    continue;
                }

                //*** Stream comment like '/* */'
                k = IsStreamComment(Pos, flags);
                if (k > 0)
                {
                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));
                    Pos = k;

                    k = _TextUnits[Pos];
                    m = _TextUnits.Length;

                    while (k < _Text.Length)
                    {
                        l = _Text.IndexOf(_StreamCommentEnd, k, ((flags & OPTION_IGNORECASE) != 0 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
                        if (l < 0) break;
                        i = l - _TextUnits[Pos] + _StreamCommentEnd.Length;
                        m = NextTextUnit(Pos, i);

                        if (CharsBetweenUnits(Pos, m) == i &&
                            (
                             ((_InternalFlags >> 20) & AleString.ALE_STR_HASALPHANUMCHAR) == 0 || (flags & OPTION_STRICTSYNTAX) == 0 ||
                             m > Hi || !AleString.IsCharAlphaNum(_Text, _TextUnits[m]) || !AleString.IsCharAlphaNum(_Text, _TextUnits[m - 1])
                            )) break;

                        m = _TextUnits.Length;
                        k += i;
                    }

                    Pos = m;
                    unrec = Pos;
                    continue;
                }

                //*** Key-value
                k = IsKeyValueSeparator(i, flags);
                if (k > 0)
                {
                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));
                    Pos = k;
                    tokens.Add(CreateToken(AleTokenType.KeyValue, _TextUnits[i], CharsBetweenUnits(i, Pos), container: tokens));
                    unrec = Pos;
                    continue;
                }

                //*** Pair
                k = IsPairSeparator(i, flags);
                if (k > 0)
                {
                    if (unrec != i) tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, i), container: tokens));
                    Pos = k;
                    tokens.Add(CreateToken(AleTokenType.Pair, _TextUnits[i], CharsBetweenUnits(i, Pos), container: tokens));
                    unrec = Pos;
                    continue;
                }

                //user processing
                m = tokens.Count;
                ures = UserProcessText(ref Pos, tokens, textEndItems, unrec, flags);
                if (ures == USER_PROCESS_ERROR) return -1;
                if ((ures & USER_PROCESS_ADDUNRECOGNIZED) != 0)
                {
                    k = i - 1;
                    while (k > unrec && Char.IsWhiteSpace(_Text, _TextUnits[k])) k--;
                    if (k >= unrec) tokens.Insert(m, CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, k + 1), container: tokens));
                }
                if (ures != USER_PROCESS_NOACTION) unrec = Pos;
                if ((ures & USER_PROCESS_STOP) != 0)
                {
                    EndIsReached = true;
                    ret = RETURN_USER;
                    break;
                }
                if ((ures & USER_PROCESS_NEWPOS) != 0) continue;


                if ((flags & OPTION_ALLOWUNRECOGNIZEDTEXT) == 0 && SetError(unrec, ERROR_UNRECOGNIZEDTEXT, true)) return -1;
                Pos++;
            } //main loop


            if (Pos != unrec)
            {
                if ((flags & OPTION_ALLOWUNRECOGNIZEDTEXT) == 0 && SetError(unrec, ERROR_UNRECOGNIZEDTEXT, true)) return -1;
                tokens.Add(CreateToken(AleTokenType.UnknownText, _TextUnits[unrec], CharsBetweenUnits(unrec, Pos), container: tokens));
            }

            if (!EndIsReached && (flags & OPTION_ENDOFEXPRESSIONREQUIRED) != 0 && SetError(Pos - 1, ERROR_NOEXPRESSIONENDING, true)) return -1;

            if (!ProcessPairs(tokens)) return -1;
            if (!ProcessKeyValues(tokens)) return -1;

            if (list != null || ((flags & OPTION_FORCELIST) != 0 && tokens.Count != 0))
            {
                if (tokens.Count == 0 && (flags & OPTION_ALLOWEMPTYLISTMEMBER) == 0 && SetError(i, ERROR_INVALIDLIST, true)) return -1;

                if (list == null)
                {
                    list = new List<AleToken>();
                    E = CreateToken(AleTokenType.ListElement, _TextUnits[start], 0, container: list);
                    list.Add(E);
                }
                else E = list[list.Count - 1];

                if (EndIsReached) E.LengthInOrigin = _TextUnits[i] - E.StartInOrigin;
                else
                    if (Pos < _TextUnits.Length) E.LengthInOrigin = _TextUnits[Pos] - E.StartInOrigin; else E.LengthInOrigin = _Text.Length - E.StartInOrigin;
                if (tokens.Count > 0) E.SubElements = tokens;

                tokens = new List<AleToken>();
                k = list[0].StartInOrigin;
                E = CreateToken(AleTokenType.ListToken, k, E.StartInOrigin + E.LengthInOrigin - k, container: tokens);
                E.SubElements = list;
                tokens.Add(E);
            }

            result = tokens;
            SetError(-1, ERROR_OK);
            _RecursionLevel--;
            _Start = SaveStart;
            return Pos;
        }

        public bool SetError(int errPos, int errCode, bool textUnit = false)
        {
            if (errPos > 0)
            {
                if (textUnit)
                    if (errPos >= _TextUnits.Length) _ErrorPos = _Text.Length; else _ErrorPos = _TextUnits[errPos];
                else _ErrorPos = errPos;
            }
            else _ErrorPos = errPos;

            _Error = errCode;
            return true;
        }

        public string ErrorMessage()
        {
            switch (_Error)
            {
                case ERROR_OK:
                    return "OK";
                case ERROR_UNKNOWN:
                    return "Unknown error";
                case ERROR_INTERNAL:
                    return "Internal error";
                case ERROR_RECURSION:
                    return "Recursion is too deep";
                case ERROR_INVALIDOPTIONS:
                    return "Invalid options";
                case ERROR_SYNTAX:
                    return "General syntax error";
                case ERROR_UNRECOGNIZEDTEXT:
                    return "Couldn't recognize text";
                case ERROR_UNEXPECTEDEND:
                    return "Unexpected end of text";
                case ERROR_NOEXPRESSIONENDING:
                    return "No text end";
                case ERROR_NOCLOSINGBRACKET:
                    return "No closing parenthesis or bracket";
                case ERROR_INVALIDLIST:
                    return "Invalid list";
                case ERROR_INVALIDKEYVALUE:
                    return "Invalid key-value";
                case ERROR_INVALIDPAIR:
                    return "Invalid pair";
                case ERROR_INVALIDNUMBER:
                    return "Invalid number";
                case ERROR_INVALIDTEXTCONST:
                    return "Invalid text const";
                case ERROR_INVALIDDATE:
                    return "Invalid date";
                default:
                    return "Error with code: " + _Error.ToString();
            }
        }

        public int ErrorPosToLine(int errPos)
        {
            if (errPos < 0 || errPos >= _Text.Length) return 0;

            int res = 1;
            int i = 0;

            while (true)
            {
                i = AleString.SkipLine(_Text, i);
                if (i > errPos) break;
                res++;
            }

            return res;
        }

        public int ErrorPosToCol(int errPos)
        {
            if (errPos < 0 || errPos >= _Text.Length) return 0;

            int i = errPos;
            int res = 1;

            while (i >= 0 && _Text[i] != '\u000a' && _Text[i] != '\u000d')
            {
                i--;
                res++;
            }
            if (i != 0) res--;
            return res;
        }
        
        public int Tokenize(out List<AleToken> result, int start = 0)
        {
            int ret;

            result = null;
            if (start < 0) start = 0;
            if (start >= _Text.Length && SetError(0, ERROR_UNEXPECTEDEND)) return -1;

            _RecursionLevel = 0;
            _Start = TextUnitByCharIndex(start);

            int i = ProcessText(_Start, out result, out ret, null, (_Options & OPTION_OPTIONS));

            if (i >= 0)
                if (i < _TextUnits.Length) i = _TextUnits[i]; else i = _Text.Length;

            return i;
        }

        public static bool TryToMakeSimpleList(List<AleToken> tokens)
        {
            if (tokens == null || tokens.Count == 0 || tokens[0].TypeOfToken != AleTokenType.ListToken) return false;

            AleToken T = tokens[0];
            int n = T.SubElementsCount;

            for (int i = 0; i < n; i++)
                if (T[i].SubElementsCount > 1) return false;

            tokens.Clear();
            if (tokens.Capacity < n) tokens.Capacity = n;

            for (int i = 0; i < n; i++)
                if (T[i].SubElementsCount == 1) tokens.Add(T[i][0]); else tokens.Add(new AleToken(AleTokenType.NullToken, 0, -1, T.Lexer));

            return true;
        }
    }

}