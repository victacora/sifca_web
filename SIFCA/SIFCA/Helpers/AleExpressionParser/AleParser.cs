using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using AleProjects.Text;


namespace AleProjects.AleLexer.AleParser
{

    public class SemanticsValidateEventArgs : EventArgs
    {
        public AleTerm Term { get; set; }

        public SemanticsValidateEventArgs(AleTerm term)
        {
            Term = term;
        }
    }


    public partial class AleExpressionParser : AleSimpleLexer
    {
        public const uint OPTION_ALLOWMULTIDIMINDEXES = 0x00010000;
        public const uint OPTION_ALLOWEMPTYINDEX = 0x00020000;
        public const uint OPTION_STRICTINDEXES = 0x00040000;
        public const uint OPTION_ALLOWEMPTYPARAMS = 0x00100000;
        public const uint OPTION_ALLOWUNDECLAREDOPERATIONS = 0x00400000;
        public const uint OPTION_DEFAULT = OPTION_STRICTSYNTAX + OPTION_ALLOWMULTIDIMINDEXES + OPTION_STRICTINDEXES + OPTION_ALLOWEMPTYPARAMS;

        public const string DEFAULT_PAIRSEPARATOR = ":";
        public const string DEFAULT_KEYVALUESEPARATOR = "=>";
        public const string DEFAULT_ARRAYBRACKETS = "[]";
        public const string DEFAULT_INITLISTBRACKETS = "{}";

        public const int OPERATIONS_STANDARDSET = 1;
        public const int OPERATIONS_CLIKESET = 2;

        public const string OPERATION_NAME_INITLIST = "AleProjects.AleParser.InitList";
        public const string OPERATION_NAME_OBJECTCONST = "AleProjects.AleParser.ObjectConst";
        public const string OPERATION_NAME_INDEX = "AleProjects.AleParser.Index";
        public const string OPERATION_NAME_INDEXOPERATOR = "AleProjects.AleParser.IndexOperator";

        public const int ERROR_INVALIDOPERATOR = 100;
        public const int ERROR_UNKNOWNOPERATOR = 110;
        public const int ERROR_INVALIDVARIABLE = 200;
        public const int ERROR_INVALIDINDEX = 300;
        public const int ERROR_INVALIDOPERATION = 400;
        public const int ERROR_UNKNOWNOPERATION = 410;
        public const int ERROR_INVALIDPARAMETERS = 420;
        public const int ERROR_INVALIDINITLIST = 500;
        public const int ERROR_INVALIDOBJECT = 600;
        public const int ERROR_INVALIDNAME = 700;     

        private List<AleOperation> _Operations;
        private string _ArrayBrackets;
        private string _InitListBrackets;
        private char _VarPrefix;
        private Dictionary<string, object> _Constants;

        private AleOperation _IndexOperator;
        private AleOperation _InitListOperation;
        private AleOperation _ObjectConstOperation;
        private AleOperation _IndexOperation;
        private AleOperation _KeyValueOperation;
        private AleOperation _PropertyValueOperation;

        protected string _listSeparator;
        protected string _keyvalueSeparator;
        protected string _pairSeparator;

        public event EventHandler<SemanticsValidateEventArgs> SemanticsValidate;

        public AleExpressionParser()
            : base()
        {
            Options = OPTION_DEFAULT;
            Brackets = "";
            Quotes = "\"\"\''";
            ListSeparator = DEFAULT_LISTSEPARATOR;
            KeyValueSeparator = DEFAULT_KEYVALUESEPARATOR;
            PairSeparator = DEFAULT_PAIRSEPARATOR;
            _listSeparator = ListSeparator;
            _keyvalueSeparator = KeyValueSeparator;
            _pairSeparator = PairSeparator;
            _Operations = null;
            _ArrayBrackets = DEFAULT_ARRAYBRACKETS;
            _InitListBrackets = DEFAULT_INITLISTBRACKETS;
            _VarPrefix = '\0';
            _Constants = null;

            _IndexOperator = new AleOperation(OPERATION_NAME_INDEXOPERATOR, 20, AleOperation.OPERATOR_YFX, AleOperationType.ElementAccess)
            {
                Evaluator = BuiltinIndexOperation
            };

            _InitListOperation = new AleOperation(OPERATION_NAME_INITLIST, AleOperationType.InitList)
            {
                Evaluator = BuiltinInitListOperation
            };

            _ObjectConstOperation = new AleOperation(OPERATION_NAME_OBJECTCONST, AleOperationType.ObjectConst)
            {
                Evaluator = BuiltinObjectConstOperation
            };

            _IndexOperation = new AleOperation(OPERATION_NAME_INDEX, AleOperationType.Index);
            // this operation doesn't require evaluator

            _KeyValueOperation = new AleOperation(_keyvalueSeparator, AleOperationType.KeyValue)
            {
                Evaluator = BuiltinKeyValueOperation
            };

            _PropertyValueOperation = new AleOperation(_pairSeparator, AleOperationType.PropertyValue)
            {
                Evaluator = BuiltinPropertyValueOperation
            };

        }

        public AleExpressionParser(string text)
            : base(text)
        {
            Options = OPTION_DEFAULT;
            Brackets = "";
            Quotes = "\"\"\'\'";
            ListSeparator = DEFAULT_LISTSEPARATOR;
            KeyValueSeparator = DEFAULT_KEYVALUESEPARATOR;
            PairSeparator = DEFAULT_PAIRSEPARATOR;
            _listSeparator = ListSeparator;
            _keyvalueSeparator = KeyValueSeparator;
            _pairSeparator = PairSeparator;
            _Operations = null;
            _ArrayBrackets = DEFAULT_ARRAYBRACKETS;
            _InitListBrackets = DEFAULT_INITLISTBRACKETS;
            _VarPrefix = '\0';
            _Constants = null;

            _IndexOperator = new AleOperation(OPERATION_NAME_INDEXOPERATOR, 20, AleOperation.OPERATOR_YFX, AleOperationType.ElementAccess)
            {
                Evaluator = BuiltinIndexOperation
            };

            _InitListOperation = new AleOperation(OPERATION_NAME_INITLIST, AleOperationType.InitList)
            {
                Evaluator = BuiltinInitListOperation
            };

            _ObjectConstOperation = new AleOperation(OPERATION_NAME_OBJECTCONST, AleOperationType.ObjectConst)
            {
                Evaluator = BuiltinObjectConstOperation
            };

            _IndexOperation = new AleOperation(OPERATION_NAME_INDEX, AleOperationType.Index);
            // this operation doesn't require evaluator

            _KeyValueOperation = new AleOperation(_keyvalueSeparator, AleOperationType.KeyValue)
            {
                Evaluator = BuiltinKeyValueOperation
            };

            _PropertyValueOperation = new AleOperation(_pairSeparator, AleOperationType.PropertyValue)
            {
                Evaluator = BuiltinPropertyValueOperation
            };

        }

        // properties

        protected List<AleOperation> Operations
        {
            get { return _Operations; }
            set { _Operations = value; }
        }

        public string ArrayBrackets
        {
            get { return _ArrayBrackets; }
            set { _ArrayBrackets = String.IsNullOrEmpty(value) ? "" : value.Trim(); }
        }

        public string InitListBrackets
        {
            get { return _InitListBrackets; }
            set { _InitListBrackets = String.IsNullOrEmpty(value) ? "" : value.Trim(); }
        }

        public char ArrayOpeningBracket
        {
            get
            {
                if (!String.IsNullOrEmpty(_ArrayBrackets)) return _ArrayBrackets[0]; else return '\0';
            }
        }

        public char ArrayClosingBracket
        {
            get
            {
                if (!String.IsNullOrEmpty(_ArrayBrackets) && _ArrayBrackets.Length > 1) return _ArrayBrackets[1]; else return '\0';
            }
        }

        public char InitListOpeningBracket
        {
            get
            {
                if (!String.IsNullOrEmpty(_InitListBrackets)) return _InitListBrackets[0]; else return '\0';
            }
        }

        public char InitListClosingBracket
        {
            get
            {
                if (!String.IsNullOrEmpty(_InitListBrackets) && _InitListBrackets.Length > 1) return _InitListBrackets[1]; else return '\0';
            }
        }

        public char VarPrefix
        {
            get { return _VarPrefix; }
            set { _VarPrefix = value; }
        }

        public Dictionary<string, object> Constants
        {
            get { return _Constants; }
            set { _Constants = value; }
        }


        //methods

        protected override void SetOptions(uint options)
        {
            _Options = options & (0xffffffff - OPTION_MINUSASPARTOFNUMBER);
        }

        protected override bool ProcessPairs(List<AleToken> tokens)
        {
            if (String.IsNullOrEmpty(PairSeparator)) return true;

            int n = tokens.Count;
            int k = -1;

            for (int i = 0; i < n; i++)
            {
                if (tokens[i].TypeOfToken == AleTokenType.Pair)
                {
                    if ((k >= 0 || i == 0 || i == n - 1) && SetError(tokens[i].StartInOrigin, ERROR_INVALIDOBJECT)) return false;
                    k = i;
                }
                else if (tokens[i].TypeOfToken == AleTokenType.KeyValue)
                {
                    if (k >= 0 && SetError(tokens[i].StartInOrigin, ERROR_INVALIDOBJECT)) return false;
                    return true;
                }
            }

            if (k >= 0)
            {
                if (k != 1 && SetError(tokens[0].StartInOrigin, ERROR_INVALIDOBJECT)) return false;

                List<AleToken> subelements = new List<AleToken>();
                AleToken T;

                subelements.Add(CreateToken(AleTokenType.PairLeftPart, tokens[0].StartInOrigin, tokens[0].LengthInOrigin, container: subelements));
                subelements.Add(CreateToken(AleTokenType.PairRightPart, tokens[2].StartInOrigin, tokens[n - 1].StartInOrigin + tokens[n - 1].LengthInOrigin - tokens[2].StartInOrigin, container: subelements));

                tokens[1].SubElements = subelements;
                tokens[1].StartInOrigin = tokens[0].StartInOrigin;
                tokens[1].LengthInOrigin = tokens[n - 1].StartInOrigin + tokens[n - 1].LengthInOrigin - tokens[0].StartInOrigin; ;

                subelements[0].SubElements = new List<AleToken>();
                
                tokens[0].Container = subelements[0].SubElements;
                subelements[0].SubElements.Add(tokens[0]);

                subelements[1].SubElements = new List<AleToken>(n - 2);
                for (int i = 2; i < n; i++)
                {
                    tokens[i].Container = subelements[1].SubElements;
                    subelements[1].SubElements.Add(tokens[i]);
                }

                T = tokens[1];
                tokens.Clear();
                tokens.Add(T);
                if (!UserValidate(T, tokens)) return false;
            }

            return true;
        }

        protected override bool ProcessKeyValues(List<AleToken> tokens)
        {
            if (String.IsNullOrEmpty(KeyValueSeparator)) return true;

            int n = tokens.Count;
            int k = -1;

            for (int i = 0; i < n; i++)
            {
                if (tokens[i].TypeOfToken == AleTokenType.KeyValue)
                {
                    if ((k >= 0 || i == 0 || i == n - 1) && SetError(tokens[i].StartInOrigin, ERROR_INVALIDKEYVALUE)) return false;
                    k = i;
                }
                else if (tokens[i].TypeOfToken == AleTokenType.Pair)
                {
                    if (k >= 0 && SetError(tokens[i].StartInOrigin, ERROR_INVALIDINITLIST)) return false;
                    return true;
                }
            }

            if (k >= 0)
            {
                List<AleToken> subelements = new List<AleToken>();
                AleToken T;

                subelements.Add(CreateToken(AleTokenType.KeyValueKey, tokens[0].StartInOrigin, tokens[k - 1].StartInOrigin + tokens[k - 1].LengthInOrigin - tokens[0].StartInOrigin, container: subelements));
                subelements.Add(CreateToken(AleTokenType.KeyValueValue, tokens[k + 1].StartInOrigin, tokens[n - 1].StartInOrigin + tokens[n - 1].LengthInOrigin - tokens[k + 1].StartInOrigin, container: subelements));

                tokens[k].SubElements = subelements;
                tokens[k].StartInOrigin = tokens[0].StartInOrigin;
                tokens[k].LengthInOrigin = tokens[n - 1].StartInOrigin + tokens[n - 1].LengthInOrigin - tokens[0].StartInOrigin;

                subelements[0].SubElements = new List<AleToken>(k);
                for (int i = 0; i < k; i++)
                {
                    tokens[i].Container = subelements[0].SubElements;
                    subelements[0].SubElements.Add(tokens[i]);
                }
                subelements[1].SubElements = new List<AleToken>(n - k);
                for (int i = k + 1; i < n; i++)
                {
                    tokens[i].Container = subelements[1].SubElements;
                    subelements[1].SubElements.Add(tokens[i]);
                }

                T = tokens[k];
                tokens.Clear();
                tokens.Add(T);
                if (!UserValidate(T, tokens)) return false;
            }

            return true;
        }

        protected int FindOperation(string name, AleToken token, uint flags, bool classMethod = false)
        {
            bool ignoreCase = (flags & OPTION_IGNORECASE) != 0;
            bool found = false;

            int hash = ignoreCase ? name.ToUpper().GetHashCode() : name.GetHashCode();
            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            int n = _Operations != null ? _Operations.Count : 0;
            int i = 0;
            int j, k, m;

            while (i < n && _Operations[i].IsOperator) i++;

            while (i < n)
            {
                if (hash == _Operations[i].HashCode && String.Compare(name, _Operations[i].Name, comparison) == 0 && _Operations[i].IsClassMethod == classMethod)
                {
                    found = true;
                    m = token.SubElements != null ? token.SubElements[0].SubElementsCount : 0;
                    k = _Operations[i].ParametersCount;

                    if (m <= k)
                    {
                        for (j = m; j < k; j++)
                            if (_Operations[i].Parameters[j].Item2 == null) break;

                        if (j < k)
                        {
                            found = false;
                            i++;
                            continue;
                        }

                        for (j = 0; j < m; j++)
                            if (_Operations[i].Parameters[j].Item2 == null && token.SubElements[0].SubElements[j].SubElements == null) break;

                        if (j < m)
                        {
                            found = false;
                            i++;
                            continue;
                        }

                        return i;
                    }
                }

                i++;
            }

            return -1 - (found ? 1 : 0);
        }

        protected int FindOperator(ref int pos, uint flags)
        {
            int n = _Operations.Count;
            int k = pos - 1;
            int len = _Text.Length - _TextUnits[pos];
            int olen;

            bool ignoreCase = (flags & OPTION_IGNORECASE) != 0;
            StringComparison comparison;

            bool bAlphaBefore = (k >= _Start) && AleString.IsCharAlphaNum(_Text, _TextUnits[k]);
            bool bAlpha = AleString.IsCharAlpha(_Text, _TextUnits[pos]);
            bool bAlphaAfter;

            char c = _Text[_TextUnits[pos]];
            if (ignoreCase) c = Char.ToUpper(c, CultureInfo.CurrentCulture);
          
            for (int i = 0; i < n && Operations[i].IsOperator; i++)
            {
                olen = _Operations[i].Name.Length;
                if (olen > len) continue;

                if (ignoreCase && bAlpha)
                {
                    if (c != Char.ToUpper(_Operations[i].Name[0], CultureInfo.CurrentCulture)) continue;
                }
                else if (c != _Operations[i].Name[0]) continue;

                comparison = (_Operations[i].Flags & AleOperation.OPERATOR_OPTION_HASALPHANUMCHAR) != 0 && ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

                if ((_Operations[i].Flags & AleOperation.OPERATOR_OPTION_HASWHITESPACE) != 0)
                {
                    k = AleString.CompareWords(_Text, _TextUnits[pos], _Operations[i].Name, comparison);
                    if (k < 0) continue;
                    olen = k - _TextUnits[pos];
                    k = NextTextUnit(pos, olen);
                }
                else
                {
                    k = String.Compare(_Text, _TextUnits[pos], _Operations[i].Name, 0, olen, comparison);
                    if (k != 0) continue;
                    k = NextTextUnit(pos, olen);
                }

                if (olen == CharsBetweenUnits(pos, k))
                {
                    if ((_Operations[i].Flags & AleOperation.OPERATOR_OPTION_HASALPHANUMCHAR) != 0 && (flags & OPTION_STRICTSYNTAX) != 0)
                    {
                        bAlphaAfter = (k < _TextUnits.Length) && (AleString.IsCharAlphaNum(_Text, _TextUnits[k]));
                        if (
                            (bAlphaBefore && (_Operations[i].Flags & AleOperation.OPERATOR_OPTION_ALPHANUMFIRST) != 0) ||
                            (bAlphaAfter && (_Operations[i].Flags & AleOperation.OPERATOR_OPTION_ALPHANUMLAST) != 0)
                           ) continue;
                    }

                    pos = k;
                    return i;
                }

            }

            return -1;
        }

        protected bool ProcessIndexes(ref int pos, AleToken token, uint flags)
        {
            List<AleToken> res = null;
            string ls = ListSeparator;
            string kvs = KeyValueSeparator;
            string ps = PairSeparator;
            int i, j;

            ListSeparator = _listSeparator;
            KeyValueSeparator = "";
            PairSeparator = "";

            flags |= (OPTION_ENDOFEXPRESSIONREQUIRED + OPTION_FORCELIST) | Options;
            flags &= (0xffffffff - OPTION_ALLOWEMPTYLISTMEMBER);

            i = ProcessText(pos, out res, out j, new string[] { ArrayClosingBracket.ToString() }, flags);
            if (i < 0)
            {
                if (ErrorCode == ERROR_NOEXPRESSIONENDING || ErrorCode == ERROR_UNEXPECTEDEND) SetError(pos - 1, ERROR_NOCLOSINGBRACKET, true);
                if (ErrorCode == ERROR_INVALIDLIST) SetError(pos - 1, ERROR_INVALIDINDEX, true);
                return false;
            }
            if (j != RETURN_TEXTEND && SetError(pos - 1, ERROR_NOCLOSINGBRACKET, true)) return false;

            if (res.Count != 0)
            {
                if ((flags & OPTION_ALLOWMULTIDIMINDEXES) == 0 && res[0].SubElementsCount > 1 && SetError(pos - 1, ERROR_INVALIDINDEX, true)) return false;
                token.SubElements = res;
            }
            else if ((flags & OPTION_ALLOWEMPTYINDEX) == 0 && SetError(pos - 1, ERROR_INVALIDINDEX, true)) return false;
         
            pos = i;
            ListSeparator = ls;
            KeyValueSeparator = kvs;
            PairSeparator = ps;
            return true;
        }

        protected bool ProcessParameters(ref int pos, AleToken token, uint flags)
        {
            List<AleToken> list = null;
            string ls = ListSeparator;
            string kvs = KeyValueSeparator;
            string ps = PairSeparator;
            int i, k;

            ListSeparator = _listSeparator;
            KeyValueSeparator = "";
            PairSeparator = "";

            flags |= (OPTION_ENDOFEXPRESSIONREQUIRED + OPTION_ALLOWEMPTYLISTMEMBER + OPTION_FORCELIST) | Options;
            if ((flags & OPTION_ALLOWEMPTYPARAMS) == 0) flags &= (0xffffffff - OPTION_ALLOWEMPTYLISTMEMBER);

            i = ProcessText(pos + 1, out list, out k, new string[] { ClosingParenthesis.ToString() }, flags);
            if (i < 0)
            {
                if (ErrorCode == ERROR_NOEXPRESSIONENDING || ErrorCode == ERROR_UNEXPECTEDEND) SetError(pos, ERROR_NOCLOSINGBRACKET, true);
                if (ErrorCode == ERROR_INVALIDLIST) SetError(pos, ERROR_INVALIDOPERATION, true);
                return false;
            }
            if (k != RETURN_TEXTEND && SetError(pos, ERROR_NOCLOSINGBRACKET, true)) return false;

            if (list.Count != 0)
            {
                token.SubElements = list;
                if ((flags & OPTION_ALLOWEMPTYLISTMEMBER) == 0)
                    for (k = 0; k < list.Count; k++)
                        if (list[k].SubElements == null && SetError(i, ERROR_INVALIDOPERATION, true)) return false;
            }

            pos = i;
            ListSeparator = ls;
            KeyValueSeparator = kvs;
            PairSeparator = ps;
            return true;
        }

        protected override AleToken CreateToken(AleTokenType tokenType, int start, int length, string value = "", List<AleToken> container = null)
        {
            AleOperationToken O;

            switch (tokenType)
            {
                case AleTokenType.Operator:
                case AleTokenType.Operation:
                    O = new AleOperationToken(tokenType, start, length, null, this);
                    O.Value = value;
                    O.Container = container;
                    return O;
                case AleTokenType.IndexToken:
                    O = new AleOperationToken(AleTokenType.IndexToken, start, length, _IndexOperation, this);
                    O.Value = value;
                    O.Container = container;
                    return O;
                case AleTokenType.KeyValue:
                    O = new AleOperationToken(AleTokenType.KeyValue, start, length, _KeyValueOperation, this);
                    O.Value = _keyvalueSeparator;
                    O.Container = container;
                    return O;
                case AleTokenType.Pair:
                    O = new AleOperationToken(AleTokenType.Pair, start, length, _PropertyValueOperation, this);
                    O.Value = _pairSeparator;
                    O.Container = container;
                    return O;
                case AleTokenType.InitList:
                    O = new AleOperationToken(AleTokenType.InitList, start, length, _InitListOperation, this);
                    O.Value = OPERATION_NAME_INITLIST;
                    O.Container = container;
                    return O;
                case AleTokenType.ObjectConst:
                    O = new AleOperationToken(AleTokenType.ObjectConst, start, length, _ObjectConstOperation, this);
                    O.Value = OPERATION_NAME_OBJECTCONST;
                    O.Container = container;
                    return O;
                default:
                    return base.CreateToken(tokenType, start, length, value, container);
            }
        }

        protected override uint UserProcessText(ref int pos, List<AleToken> tokens, string[] textEndItems, int unrecognizedPos, uint flags)
        {
            int i, j, Hi;
            AleTokenType tokenType = AleTokenType.NullToken;
            AleToken E;
            string S;
            char C = _Text[_TextUnits[pos]];

            //first call
            if ((flags & OPTION_FIRSTCALL) != 0)
            {

                // custom parentheses processing
                if (C == OpeningParenthesis && TextUnitSize(pos) == 1)
                {
                    List<AleToken> list;
                    string ls = ListSeparator;
                    string kvs = KeyValueSeparator;
                    string ps = PairSeparator;

                    ListSeparator = "";
                    KeyValueSeparator = "";
                    PairSeparator = "";

                    i = ProcessText(pos + 1, out list, out j, new string[] { ClosingParenthesis.ToString() }, (flags | OPTION_ENDOFEXPRESSIONREQUIRED | Options) & OPTION_OPTIONS);
                    if (i < 0)
                    {
                        if (ErrorCode == ERROR_NOEXPRESSIONENDING || ErrorCode == ERROR_UNEXPECTEDEND) SetError(pos, ERROR_NOCLOSINGBRACKET, true);
                        return USER_PROCESS_ERROR;
                    }
                    if (j != RETURN_TEXTEND && SetError(pos, ERROR_NOCLOSINGBRACKET, true)) return USER_PROCESS_ERROR;
                    if (list.Count == 0 && SetError(pos, ERROR_SYNTAX, true)) return USER_PROCESS_ERROR;

                    E = CreateToken(AleTokenType.Parentheses, _TextUnits[pos], CharsBetweenUnits(pos, i), container: tokens);
                    E.SubElements = list;
                    tokens.Add(E);

                    pos = i;
                    ListSeparator = ls;
                    KeyValueSeparator = kvs;
                    PairSeparator = ps;
                    return USER_PROCESS_NEW;
                }
                // custom parentheses processing

                return USER_PROCESS_NOACTION;
            } 
            //** first call

            //operators
            j = pos;
            if (_Operations != null && (i = FindOperator(ref pos, flags)) >= 0)
            {
                E = CreateToken(AleTokenType.Operator, _TextUnits[j], CharsBetweenUnits(j, pos), _Operations[i].Name, tokens);
                (E as AleOperationToken).Operation = _Operations[i];
                tokens.Add(E);
                if (!UserValidate(E, tokens)) return USER_PROCESS_ERROR;

                return USER_PROCESS_NEW;
            }

            //variables, atoms and structures
            if (AleString.IsCharAlpha(_Text, _TextUnits[pos]) || (C == _VarPrefix && TextUnitSize(pos) == 1))
            {
                int type = 0;

                Hi = _TextUnits.Length - 1;
                i = pos + 1;

                if (C == _VarPrefix)
                {
                    if ((i > Hi || !AleString.IsCharAlpha(_Text, _TextUnits[i])) && SetError(pos, ERROR_INVALIDNAME, true)) return USER_PROCESS_ERROR;
                    i++;
                }

                while (i <= Hi && AleString.IsCharAlphaNum(_Text, _TextUnits[i])) i++;
                j = CharsBetweenUnits(pos, i);

                if (C != _VarPrefix)
                {
                    if (i <= Hi && _Text[_TextUnits[i]] == OpeningParenthesis && TextUnitSize(i) == 1) type = 3;
                    else if (_VarPrefix == '\0')
                    {
                        if (_Constants != null && _Constants.ContainsKey(_Text.Substring(_TextUnits[pos], j))) type = 2; else type = 1;
                    }
                    else if (tokens.Count > 0 && tokens.Last() is AleOperationToken && (tokens.Last() as AleOperationToken).Operation.IsClassOperator) type = 1; else type = 2;
                }
                else
                {
                    if (tokens.Count > 0 && tokens.Last() is AleOperationToken && (tokens.Last() as AleOperationToken).Operation.IsClassOperator &&
                        SetError(pos, ERROR_INVALIDNAME, true)) return USER_PROCESS_ERROR;
                    type = 1;
                }

                switch (type)
                {
                    case 1: //variable
                        E = CreateToken(AleTokenType.Variable, _TextUnits[pos], j, container: tokens);
                        pos = i;
                        break;

                    case 2: //atom
                        E = CreateToken(AleTokenType.TextAtom, _TextUnits[pos], j, container: tokens);
                        pos = i;
                        break;

                    case 3: //operation
                        S = _Text.Substring(_TextUnits[pos], j);
                        E = CreateToken(AleTokenType.Operation, _TextUnits[pos], j, null, tokens);

                        j = pos;
                        pos = i;
                        if (!ProcessParameters(ref pos, E, flags)) return USER_PROCESS_ERROR;
                        E.LengthInOrigin = CharsBetweenUnits(j, pos);

                        int k = FindOperation(S, E, flags, tokens.Count > 0 && tokens.Last() is AleOperationToken && (tokens.Last() as AleOperationToken).Operation.IsClassOperator);
                        if ((k < -1 || (k < 0 && (flags & OPTION_ALLOWUNDECLAREDOPERATIONS) == 0)) &&
                            SetError(E.StartInOrigin, ERROR_INVALIDOPERATION)) return USER_PROCESS_ERROR;
                        E.Value = S;
                        if (k >= 0) (E as AleOperationToken).Operation = _Operations[k];

                        break;

                    default:
                        SetError(pos, ERROR_SYNTAX, true);
                        return USER_PROCESS_ERROR;
                }

                tokens.Add(E);
                if (!UserValidate(E, tokens)) return USER_PROCESS_ERROR;
                return USER_PROCESS_NEW;
            }

            //indexers
            if (C == ArrayOpeningBracket && TextUnitSize(pos) == 1)
            {
                if (tokens.Count == 0 && SetError(pos, ERROR_SYNTAX, true)) return USER_PROCESS_ERROR;

                E = tokens.Last();
                if (E.StartInOrigin + E.LengthInOrigin != _TextUnits[pos] && (flags & OPTION_STRICTINDEXES) != 0 && 
                    SetError(pos, ERROR_SYNTAX, true)) return USER_PROCESS_ERROR;

                E = CreateToken(AleTokenType.Operator, _TextUnits[pos], 1, C.ToString(), tokens);
                (E as AleOperationToken).Operation = _IndexOperator;
                tokens.Add(E);
                if (!UserValidate(E, tokens)) return USER_PROCESS_ERROR;

                pos++;
                i = pos;

                E = CreateToken(AleTokenType.IndexToken, _TextUnits[pos], 0, OPERATION_NAME_INDEX, tokens);
                if (!ProcessIndexes(ref pos, E, flags)) return USER_PROCESS_ERROR;
                E.LengthInOrigin = CharsBetweenUnits(i, pos);
                tokens.Add(E);
                if (!UserValidate(E, tokens)) return USER_PROCESS_ERROR;
                return USER_PROCESS_NEW;
            }

            //initialization list
            if (C == InitListOpeningBracket && TextUnitSize(pos) == 1)
            {
                List<AleToken> list;
                string ls = ListSeparator;
                string kvs = KeyValueSeparator;
                string ps = PairSeparator;

                ListSeparator = _listSeparator;
                KeyValueSeparator = _keyvalueSeparator;
                PairSeparator = _pairSeparator;

                i = ProcessText(pos + 1, out list, out j, new string[] { InitListClosingBracket.ToString() },
                                (flags & (0xffffffff - OPTION_ALLOWEMPTYLISTMEMBER)) | OPTION_ENDOFEXPRESSIONREQUIRED | OPTION_FORCELIST);
                
                if (i < 0)
                {
                    if (ErrorCode == ERROR_NOEXPRESSIONENDING || ErrorCode == ERROR_UNEXPECTEDEND) SetError(pos, ERROR_NOCLOSINGBRACKET, true);
                    return USER_PROCESS_ERROR;
                }
                if (j != RETURN_TEXTEND && SetError(pos, ERROR_NOCLOSINGBRACKET, true)) return USER_PROCESS_ERROR;
                if ((list == null || list.Count == 0) && SetError(pos, ERROR_INVALIDINITLIST, true)) return USER_PROCESS_ERROR;

                tokenType = AleTokenType.InitList;
                foreach (AleToken T in list[0].SubElements)
                    if (T.SubElements[0].TypeOfToken == AleTokenType.Pair)
                    {
                        tokenType = AleTokenType.ObjectConst;
                        break;
                    }

                E = CreateToken(tokenType, _TextUnits[pos], CharsBetweenUnits(pos, i), container: tokens);
                if (list.Count != 0) E.SubElements = list;
                (E as AleOperationToken).Operation = tokenType == AleTokenType.InitList ? _InitListOperation : _ObjectConstOperation;
                tokens.Add(E);
                if (!UserValidate(E, tokens)) return USER_PROCESS_ERROR;

                pos = i;
                ListSeparator = ls;
                KeyValueSeparator = kvs;
                PairSeparator = ps;
                return USER_PROCESS_NEW;

            }

            return USER_PROCESS_NOACTION;
        }

        private struct OperatorData
        {
            public uint DefinedAssociativity;
            public uint PossibleAssociativity;
            public int TokenIndex;

            public OperatorData(int tokenIndex, uint possibleAssociativity)
            {
                DefinedAssociativity = 0;
                PossibleAssociativity = possibleAssociativity;
                TokenIndex = tokenIndex;
            }
        }

        private bool OnlyOneBit(uint i)
        {
            while (i != 0)
            {
                if ((i & 1) != 0) return (i & 0xfffffffe) == 0;
                i >>= 1;
            }

            return false;
        }

        private uint FirstBitValue(uint i)
        {
            byte j = 0;

            while (i != 0)
            {
                if ((i & 1) != 0) return (uint)1 << j;
                i >>= 1;
                j++;
            }

            return 0;
        }

        public new string ErrorMessage()
        {
            switch (ErrorCode)
            {
                case ERROR_INVALIDOPERATOR:
                    return "Invalid operator";
                case ERROR_INVALIDVARIABLE:
                    return "Invalid variable";
                case ERROR_INVALIDINDEX:
                    return "Invalid array index";
                case ERROR_INVALIDOPERATION:
                    return "Invalid operation";
                case ERROR_INVALIDPARAMETERS:
                    return "Invalid parameters";
                case ERROR_INVALIDINITLIST:
                    return "Invalid initialization list";
                case ERROR_INVALIDOBJECT:
                    return "Invalid object";
                case ERROR_INVALIDNAME:
                    return "Invalid name";
                default:
                    return base.ErrorMessage();
            }
        }

        protected bool ToRPN(List<AleToken> tokens, List<AleToken> rpnList)
        {
            int i, j, k, n, m, lo, hi, OpsCount;
            uint a;
            bool bOk = true;

            if (tokens == null)
            {
                rpnList.Add(null);
                return true;
            }

            n = tokens.Count;
            if (n == 0) return true;

            SetError(-1, ERROR_OK);
            OpsCount = 0;

            for (i = 0; i < n; i++)
            {
                if (tokens[i].TypeOfToken == AleTokenType.Operator)
                {
                    OpsCount++;
                    bOk = true;
                }
                else
                {
                    if (!bOk && SetError(tokens[i].StartInOrigin, ERROR_SYNTAX)) return false;
                    bOk = false;
                }
            }


            if (OpsCount != 0)
            {
                OperatorData[] OpList = new OperatorData[OpsCount];

                j = 0;
                for (i = 0; i < n; i++)
                    if (tokens[i].TypeOfToken == AleTokenType.Operator)
                        OpList[j++] = new OperatorData(i, (tokens[i] as AleOperationToken).Operation.Associativity);

                hi = OpsCount - 1;

                //*** strictly prefix operators
                //and check - prefix operators must go in descending order of precedence
                k = -1;

                for (i = 0; i <= hi; i++)
                    if (OpList[i].TokenIndex == 0 || (i > 0 && OpList[i - 1].TokenIndex == OpList[i].TokenIndex - 1))
                    {
                        a = OpList[i].PossibleAssociativity & AleOperation.OPERATOR_PREFIX;
                        OpList[i].DefinedAssociativity = a;
                        OpList[i].PossibleAssociativity = 0;
                        if (a == 0 && SetError(tokens[OpList[i].TokenIndex].StartInOrigin, ERROR_INVALIDOPERATOR)) return false;
                        k = i;
                    }
                    else break;

                // prefix operators precedence check
                for (i = 0; i < k; i++)
                {
                    m = AleOperation.CompareOperators((tokens[OpList[i].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[i].DefinedAssociativity,
                                                      (tokens[OpList[i + 1].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[i + 1].DefinedAssociativity);
                    if (m != 1 && SetError(tokens[OpList[i].TokenIndex].StartInOrigin, ERROR_INVALIDOPERATOR)) return false;
                }

                //*** strictly postfix operators
                //and check - postfix operators must go in ascending order of precedence
                k++;
                n = hi + 1;

                for (i = hi; i >= k; i--)
                    if (OpList[i].TokenIndex == tokens.Count - 1 || (i < hi && OpList[i].TokenIndex == OpList[i + 1].TokenIndex - 1))
                    {
                        a = OpList[i].PossibleAssociativity & AleOperation.OPERATOR_POSTFIX;
                        OpList[i].DefinedAssociativity = a;
                        OpList[i].PossibleAssociativity = 0;
                        if (a == 0 && SetError(tokens[OpList[i].TokenIndex].StartInOrigin, ERROR_INVALIDOPERATOR)) return false;
                        n = i;
                    }
                    else break;

                // postfix operators precedence check
                for (i = hi; i > n; i--)
                {
                    m = AleOperation.CompareOperators((tokens[OpList[i].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[i].DefinedAssociativity,
                                                        (tokens[OpList[i - 1].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[i - 1].DefinedAssociativity);
                    if (m != 1 && SetError(tokens[OpList[i].TokenIndex].StartInOrigin, ERROR_INVALIDOPERATOR)) return false;
                }

                //*** strictly infix operators
                // also check for prefix operators with terminal on left side and postfix with terminal on right side
                n--;

                for (i = k; i <= n; i++)
                {
                    a = AleOperation.OPERATOR_PREFIX + AleOperation.OPERATOR_INFIX + AleOperation.OPERATOR_POSTFIX;

                    if ((i == 0 && OpList[i].TokenIndex != 0) || (i > 0 && OpList[i - 1].TokenIndex != OpList[i].TokenIndex - 1)) a &= AleOperation.OPERATOR_INFIX + AleOperation.OPERATOR_POSTFIX;
                    if ((i == hi && OpList[i].TokenIndex != tokens.Count - 1) || (i < hi && OpList[i].TokenIndex != OpList[i + 1].TokenIndex - 1)) a &= AleOperation.OPERATOR_PREFIX + AleOperation.OPERATOR_INFIX;

                    a &= OpList[i].PossibleAssociativity;
                    if (OnlyOneBit(a))
                    {
                        OpList[i].DefinedAssociativity = a;
                        OpList[i].PossibleAssociativity = 0;
                    }
                    else OpList[i].PossibleAssociativity = a;

                    if (a == 0 && SetError(tokens[OpList[i].TokenIndex].StartInOrigin, ERROR_INVALIDOPERATOR)) return false;
                }

                // at this point all leftmost (prefix) and rightmost (postfix) operators in expression are checked
                // all strictly infix operatorsare also checked

                lo = k;
                hi = n;
                i = lo;

                while (i <= hi)
                {
                    n = i;
                    while (n < hi && OpList[n].TokenIndex + 1 == OpList[n + 1].TokenIndex) n++;

                    for (j = i; j <= n; j++)
                        if (OpList[j].PossibleAssociativity != 0)
                        {
                            a = FirstBitValue(OpList[j].PossibleAssociativity);
                            OpList[j].PossibleAssociativity -= a;
                            OpList[j].DefinedAssociativity = a;
                        }


                    //loop of enumeration of all possible positions of operators with multiple assocciativities
                    do
                    {
                        bOk = true;
                        //checks - only one infix operator with highest precedence in group of operators
                        //       - precedence of postfix operators on left side must grow from left to right
                        //       - precedence of prefix operators on right side must grow from right to left
                        k = -1;

                        for (j = i; j <= n && bOk; j++)
                            if ((OpList[j].DefinedAssociativity & AleOperation.OPERATOR_INFIX) != 0)
                            {
                                if (k >= 0) bOk = false;
                                k = j;
                            }

                        if (k < 0) bOk = false;

                        if (bOk)
                        {
                            if (k > i)
                            {
                                m = AleOperation.CompareOperators((tokens[OpList[k - 1].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[k - 1].DefinedAssociativity,
                                                                    (tokens[OpList[k].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[k].DefinedAssociativity);
                                bOk = (m == 2);
                            }

                            if (bOk && k < n)
                            {
                                m = AleOperation.CompareOperators((tokens[OpList[k].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[k].DefinedAssociativity,
                                                                    (tokens[OpList[k + 1].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[k + 1].DefinedAssociativity);
                                bOk = (m == 1);
                            }
                        }

                        if (bOk)
                        {
                            // postfix operators on left side
                            for (j = k - 1; j > i && bOk; j--)
                            {
                                m = AleOperation.CompareOperators((tokens[OpList[j].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[j].DefinedAssociativity,
                                                                    (tokens[OpList[j - 1].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[j - 1].DefinedAssociativity);
                                if (m != 1) bOk = false;
                            }

                            if (bOk)
                            {
                                // prefix operators on right side
                                for (j = k + 1; j < n && bOk; j++)
                                {
                                    m = AleOperation.CompareOperators((tokens[OpList[j].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[j].DefinedAssociativity,
                                                                         (tokens[OpList[j + 1].TokenIndex] as AleOperationToken).Operation.Precedence, OpList[j + 1].DefinedAssociativity);
                                    if (m != 1) bOk = false;
                                }
                            }
                        }

                        // iterating through all possible values of 'PossibleAssoc' where it is nonzero
                        // when every 'PossibleAssoc' == 0 and bOk == false - it is an error of operators consistency in expression
                        if (!bOk)
                        {
                            k = -1;
                            for (j = i; j <= n; j++)
                            {
                                if (OpList[j].PossibleAssociativity != 0)
                                {
                                    k = j;
                                    a = FirstBitValue(OpList[j].PossibleAssociativity);
                                    OpList[j].PossibleAssociativity -= a;
                                    OpList[j].DefinedAssociativity = a;
                                    break;
                                }
                            }
                            if (k < 0 && SetError(tokens[OpList[i].TokenIndex].StartInOrigin, ERROR_INVALIDOPERATOR)) return false;
                        }

                    } while (!bOk);

                    for (j = i; j <= n; j++) OpList[j].PossibleAssociativity = 0;
                    i = n + 1;

                } //*** while (i <= hi)


                for (i = 0; i < OpsCount; i++) (tokens[OpList[i].TokenIndex] as AleOperationToken).Associativity = OpList[i].DefinedAssociativity;

                OpList = null;
            } //*** (OpsCount != 0)


            Stack<AleOperationToken> OpStack = new Stack<AleOperationToken>(OpsCount);
            AleOperationToken O;

            i = 0;
            n = tokens.Count;

            // Dijkstra's "Shunting Yard" algorithm

            for (i = 0; i < n; i++)
            {
                if (tokens[i].TypeOfToken == AleTokenType.Operator)
                {
                    while (OpStack.Count != 0)
                    {
                        O = OpStack.Peek();
                        m = AleOperation.CompareOperators(O.Operation.Precedence, O.Associativity, (tokens[i] as AleOperationToken).Operation.Precedence, (tokens[i] as AleOperationToken).Associativity);
                        if (m == 2) rpnList.Add(OpStack.Pop()); else break;
                    }
                    OpStack.Push((tokens[i] as AleOperationToken));
                }
                else if (tokens[i].TypeOfToken == AleTokenType.Parentheses)
                {
                    if (!ToRPN(tokens[i].SubElements, rpnList)) return false;
                }
                else
                {
                    m = tokens[i].SubElementsCount;
                    if (m != 0)
                    {
                        if (tokens[i].TypeOfToken == AleTokenType.ListToken)
                        {
                            for (j = 0; j < m; j++)
                                if (!ToRPN(tokens[i].SubElements[j].SubElements, rpnList)) return false;
                        }
                        else if (tokens[i].TypeOfToken == AleTokenType.KeyValue || tokens[i].TypeOfToken == AleTokenType.Pair)
                        {
                            for (j = 0; j < m; j++)
                                if (!ToRPN(tokens[i].SubElements[j].SubElements, rpnList)) return false;
                            rpnList.Add(tokens[i]);
                        }
                        else
                        {
                            if (!ToRPN(tokens[i].SubElements, rpnList)) return false;
                            rpnList.Add(tokens[i]);
                        }
                    }
                    else rpnList.Add(tokens[i]);

                }
            }

            while (OpStack.Count != 0) rpnList.Add(OpStack.Pop());
            OpStack = null;

            return true;
        }

        public List<AleToken> ToReversePolishNotation(List<AleToken> tokens)
        {
            SetError(-1, ERROR_OK);

            int m = tokens.Count;
            int n = m;
            for (int i = 0; i < m; i++) n += tokens[i].SubElementsTotalCount();

            if (n <= 0) return null;

            List<AleToken> res = new List<AleToken>(n);

            if (!ToRPN(tokens, res)) return null;

            return res;
        }

        protected virtual AleTerm CreateTerm(AleTermType type)
        {
            return new AleTerm(type, this);
        }

        protected virtual bool UserSemanticsValidate(AleTerm t)
        {
            SemanticsValidateEventArgs e = new SemanticsValidateEventArgs(t);
            EventHandler<SemanticsValidateEventArgs> handler = SemanticsValidate;
            if (handler != null)
            {
                handler(this, e);
                if (ErrorCode != ERROR_OK) return false;
            }

            return true;
        }

        public virtual bool CheckSemantics(AleTerm t)
        {
            if (t == null || t.TypeOfTerm == AleTermType.Atom) return true;

            // term without operation description
            if (t.Operation == null) return UserSemanticsValidate(t);

            int i, j;
            AleTokenType tokenType;

            if (t.Operation.IsClassOperator)
            {
                // checks right part of class operator
                tokenType = t[1].Token.TypeOfToken;
                if (tokenType != AleTokenType.Variable && tokenType != AleTokenType.TextAtom && tokenType != AleTokenType.Operation &&
                    SetError(t.Token.StartInOrigin, ERROR_INVALIDOPERATOR)) return false;
                i = t.Token.Container.IndexOf(t.Token) + 1;
                if ((i >= t.Token.Container.Count || t.Token.Container[i] != t[1].Token) && 
                    SetError(t.Token.StartInOrigin, ERROR_INVALIDOPERATOR)) return false;
            }
            else if (t.Operation.IsIndexOperator)
            {
                // numbers can't be indexed
                tokenType = t[0].Token.TypeOfToken;
                if ((tokenType == AleTokenType.DecimalNumber || tokenType == AleTokenType.FloatNumber || tokenType == AleTokenType.HexNumber) &&
                    SetError(t.Token.StartInOrigin, ERROR_INVALIDINDEX)) return false;
            }
            else if (t.Operation.OperationType == AleOperationType.InitList || t.Operation.OperationType == AleOperationType.ObjectConst)
            {
                // initlist contains only single values and/or key=>values pairs
                // object (json-like) contains only "propertyName":value pairs
                j = 0;
                foreach (AleTerm Term in t.Elements)
                {
                    if (Term.Operation == null) j |= 1;
                    else if (Term.Operation.IsKeyValue) j |= 2;
                    else if (Term.Operation.IsPropertyValue) j |= 4;

                    if (j > 4 && SetError(t.Token.StartInOrigin, ERROR_INVALIDOBJECT)) return false;
                }

                foreach (AleTerm Term in t.Elements)
                    if (Term.Operation != null && Term.Operation.IsPropertyValue && Term[0].Token.TypeOfToken != AleTokenType.TextConstant &&
                        Term[0].Token.TypeOfToken != AleTokenType.TextAtom &&
                        SetError(t.Token.StartInOrigin, ERROR_INVALIDINITLIST)) return false;
            }
            else if (t.Operation.IsIndex)
            {
                // empty array indexes can be only in left side of assignment operator '=' (but not '+=', '-=', etc)
                // and only most right index can be empty
                if (t.Elements == null)
                {
                    AleTerm T1 = t.Parent;
                    if (T1 != null && (T1 = T1.Parent) != null && T1.Operation != null && T1.Operation.IsIndexOperator &&
                        SetError(t.Token.StartInOrigin, ERROR_INVALIDINDEX)) return false;

                    T1 = t.Parent;
                    if ((T1 == null || (T1 = T1.Parent) == null || T1.Operation == null || T1.Operation.OperationType != AleOperationType.Assignment) &&
                        SetError(t.Token.StartInOrigin, ERROR_INVALIDINDEX)) return false;
                }
            }
            else if (t.Operation.IsAssignOperator || t.Operation.IsIncrementOperator)
            {
                // only the variable, array element or class property can be in left side of assignment operator
                // or can be operand of the increment/decrement operator
                AleTerm Term = t[0];
                if (Term.TypeOfTerm != AleTermType.Variable &&
                    (
                     Term.Operation == null ||
                     (!Term.Operation.IsIndexOperator && (!Term.Operation.IsClassOperator || Term[1].TypeOfTerm != AleTermType.Variable)) || Term[0].IsConstant()
                    ) &&
                    SetError(t.Token.StartInOrigin, ERROR_INVALIDOPERATOR)
                   ) return false;
            }
            else return UserSemanticsValidate(t);

            return true;
        }

        public new int Tokenize(out List<AleToken> result, int start = 0)
        {
            _listSeparator = ListSeparator;
            _keyvalueSeparator = KeyValueSeparator;
            _pairSeparator = PairSeparator;
            ListSeparator = "";
            KeyValueSeparator = "";
            PairSeparator = "";

            int i = base.Tokenize(out result, start);

            ListSeparator = _listSeparator;
            KeyValueSeparator = _keyvalueSeparator;
            PairSeparator = _pairSeparator;

            return i;
        }

        public AleTerm Parse(List<AleToken> tokens = null)
        {
            if (tokens == null)
            {
                Tokenize(out tokens);
                if (ErrorCode != ERROR_OK) return null;
            }

            List<AleToken> rpn = ToReversePolishNotation(tokens);
            if (rpn == null) return null;

            int i = rpn.Count;
            int n;

            AleTerm Term = null;
            AleTerm T = null;

            Stack<AleTerm> TermStack = new Stack<AleTerm>(i);
            List<AleTerm> TermsList = new List<AleTerm>(i);
            object v;

            i--;

            while (i >= 0)
            {
                n = 0;
                if (TermStack.Count != 0) T = TermStack.Peek(); else T = null;

                if (rpn[i] != null)
                {
                    switch (rpn[i].TypeOfToken)
                    {
                        case AleTokenType.Operation:
                            Term = CreateTerm(AleTermType.Operation);
                            if (IgnoreCase) Term.Value = rpn[i].Value.ToUpper(); else Term.Value = rpn[i].Value;
                            Term.Operation = (rpn[i] as AleOperationToken).Operation;
                            if (rpn[i].SubElements != null) n = rpn[i].SubElements[0].SubElementsCount;
                            if (n > 0) Term.Elements = new List<AleTerm>(n);
                            break;

                        case AleTokenType.Operator:
                            Term = CreateTerm(AleTermType.Operation);
                            if (IgnoreCase) Term.Value = rpn[i].Value.ToUpper(); else Term.Value = rpn[i].Value;
                            Term.Operation = (rpn[i] as AleOperationToken).Operation;
                            n = (rpn[i] as AleOperationToken).IsBinaryOperator ? 2 : 1;
                            Term.Elements = new List<AleTerm>(n);
                            break;

                        case AleTokenType.Variable:
                            Term = CreateTerm(AleTermType.Variable);
                            if (VarPrefix != '\0' && rpn[i].Value[0] == VarPrefix)
                                if (IgnoreCase) Term.Value = rpn[i].Value.Substring(1).ToUpper(); else Term.Value = rpn[i].Value.Substring(1);
                            else 
                                if (IgnoreCase) Term.Value = rpn[i].Value.ToUpper(); else Term.Value = rpn[i].Value;
                            break;

                        case AleTokenType.KeyValue:
                        case AleTokenType.Pair:
                            Term = CreateTerm(AleTermType.Operation);
                            if (IgnoreCase) Term.Value = rpn[i].Value.ToUpper(); else Term.Value = rpn[i].Value;
                            Term.Operation = (rpn[i] as AleOperationToken).Operation;
                            n = 2;
                            Term.Elements = new List<AleTerm>(n);
                            break;

                        case AleTokenType.InitList:
                        case AleTokenType.ObjectConst:
                        case AleTokenType.IndexToken:
                            Term = CreateTerm(AleTermType.Operation);
                            if (IgnoreCase) Term.Value = rpn[i].Value.ToUpper(); else Term.Value = rpn[i].Value;
                            Term.Operation = (rpn[i] as AleOperationToken).Operation;

                            if (rpn[i].SubElements != null)
                            {
                                n = rpn[i].SubElements[0].SubElementsCount;
                                Term.Elements = new List<AleTerm>(n);
                            }

                            break;

                        default:
                            Term = CreateTerm(AleTermType.Atom);
                            if (rpn[i].TypeOfToken == AleTokenType.TextAtom && _Constants != null && _Constants.TryGetValue(rpn[i].Value, out v)) Term.Value = v; else Term.Value = rpn[i].ToObject();
                            break;

                    } //switch

                    Term.Token = rpn[i];
                }
                else Term = null;

                TermsList.Add(Term);

                if (T != null)
                {
                    while (true)
                    {
                        if (T.TypeOfTerm == AleTermType.Operation && T.Elements != null && T.Elements.Count != T.Elements.Capacity)
                        {
                            T.Elements.Add(Term);
                            if (T.Elements.Count == T.Elements.Capacity) T.Elements.Reverse();
                            if (Term != null && Term.TypeOfTerm == AleTermType.Operation) TermStack.Push(Term);
                            break;
                        }
                        TermStack.Pop();
                        if (TermStack.Count == 0) break;
                        T = TermStack.Peek();
                    }

                    if (Term != null) Term.Parent = T;
                    if (TermStack.Count == 0) break;
                }
                else
                {
                    T = Term;
                    if (n == 0) break;
                    TermStack.Push(Term);
                }

                i--;
            } //while

            // T after loop contains function result
            while (TermStack.Count != 0) T = TermStack.Pop();

            n = TermsList.Count();

            for (i = 0; i < n; i++)
                if (!CheckSemantics(TermsList[i])) return null;

            return T;
        }

    }

}