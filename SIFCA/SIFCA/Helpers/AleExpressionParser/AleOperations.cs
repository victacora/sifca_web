using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using AleProjects.Text;


namespace AleProjects.AleLexer.AleParser
{

    public struct OperationEvalParameters
    {
        public object FirstParam;
        public object SecondParam;
        public object ThirdParam;
        public List<object> Parameters;
        public int ActualParamsCount;
        public object ClassInstance;
        public TermEvaluate userEvaluate;
        public TermAssign userAssign;
    }


    public delegate bool EvaluateOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result);


    public enum AleOperationType
    {
        Evaluation, Assignment, CompoundAssignment, Increment, MemberAccess, ElementAccess, Index, InitList, ObjectConst, KeyValue, PropertyValue
    }


    public class AleOperation
    {
        public const uint OPERATOR_FX = 0x00000001;
        public const uint OPERATOR_FY = 0x00000002;
        public const uint OPERATOR_YFX = 0x00000004;
        public const uint OPERATOR_XFY = 0x000000010;
        public const uint OPERATOR_YF = 0x000000020;
        public const uint OPERATOR_XF = 0x000000040;

        public const uint OPERATOR_PREFIX = OPERATOR_FX + OPERATOR_FY;
        public const uint OPERATOR_INFIX = OPERATOR_YFX + OPERATOR_XFY;
        public const uint OPERATOR_POSTFIX = OPERATOR_YF + OPERATOR_XF;

        public const uint OPERATOR_UNARY = OPERATOR_PREFIX + OPERATOR_POSTFIX;
        public const uint OPERATOR_BINARY = OPERATOR_INFIX;

        public const uint OPERATOR_OPTION_HASALPHACHAR = AleString.ALE_STR_HASALPHACHAR;
        public const uint OPERATOR_OPTION_HASNUMCHAR = AleString.ALE_STR_HASNUMCHAR;
        public const uint OPERATOR_OPTION_HASALPHANUMCHAR = OPERATOR_OPTION_HASALPHACHAR + OPERATOR_OPTION_HASNUMCHAR;
        public const uint OPERATOR_OPTION_ALPHANUMFIRST = AleString.ALE_STR_ALPHANUMFIRST;
        public const uint OPERATOR_OPTION_ALPHANUMLAST = AleString.ALE_STR_ALPHANUMLAST;
        public const uint OPERATOR_OPTION_HASWHITESPACE = 0x00000010;


        private string _Name;
        private int _Hash;
        private int _Precedence;
        private uint _Associativity;
        private AleOperationType _OperationType;
        private uint _Flags;
        private TypeCode _InstanceTypeCode;

        public string Name
        {
            get { return _Name; }
        }

        public int HashCode
        {
            get { return _Hash; }
        }

        public int Precedence
        {
            get { return _Precedence; }
        }

        public uint Associativity
        {
            get { return _Associativity; }
        }

        public AleOperationType OperationType
        {
            get { return _OperationType; }
        }

        public uint Flags
        {
            get
            {
                return _Flags;
            }
            protected set
            {
                _Flags = value;
            }
        }

        public TypeCode InstanceTypeCode
        {
            get
            {
                return _InstanceTypeCode;
            }
            set
            {
                if (_Associativity == 0) _InstanceTypeCode = value;
            }
        }

        public List<Tuple<TypeCode, object>> Parameters { get; set; }

        public EvaluateOperation Evaluator { get; set; }

        public int ParametersCount
        {
            get
            {
                return Parameters == null? 0: Parameters.Count;
            }
        }

        public bool IsOperator
        {
            get
            {
                return _Associativity != 0;
            }
        }

        public bool IsAssignOperator
        {
            get
            {
                return _Associativity != 0 && (_OperationType==AleOperationType.Assignment || _OperationType==AleOperationType.CompoundAssignment);
            }
        }

        public bool IsIncrementOperator
        {
            get
            {
                return _Associativity != 0 && _OperationType == AleOperationType.Increment;
            }
        }

        public bool IsClassOperator
        {
            get
            {
                return _Associativity != 0 && _OperationType == AleOperationType.MemberAccess;
            }
        }

        public bool IsIndexOperator
        {
            get
            {
                return _Associativity != 0 && _OperationType == AleOperationType.ElementAccess;
            }
        }

        public bool IsIndex
        {
            get
            {
                return _Associativity == 0 && _OperationType == AleOperationType.Index;
            }
        }

        public bool IsInitList
        {
            get
            {
                return _Associativity == 0 && _OperationType == AleOperationType.InitList;
            }
        }

        public bool IsObjectConst
        {
            get
            {
                return _Associativity == 0 && _OperationType == AleOperationType.ObjectConst;
            }
        }

        public bool IsKeyValue
        {
            get
            {
                return _Associativity == 0 && _OperationType == AleOperationType.KeyValue;
            }
        }

        public bool IsPropertyValue
        {
            get
            {
                return _Associativity == 0 && _OperationType == AleOperationType.PropertyValue;
            }
        }

        public bool IsClassMethod
        {
            get
            {
                return _InstanceTypeCode != TypeCode.Empty;
            }
        }

        // creates operator
        public AleOperation(string name, int precedence, uint associativity, AleOperationType type = AleOperationType.Evaluation)
        {
            string s = name.Trim();
            if (!CheckOperatorName(s)) throw new System.ArgumentException("Invalid operator name");

            if ((associativity == 0) ||
                ((associativity & OPERATOR_PREFIX) == OPERATOR_PREFIX) ||
                ((associativity & OPERATOR_POSTFIX) == OPERATOR_POSTFIX) ||
                ((associativity & OPERATOR_INFIX) == OPERATOR_INFIX)) throw new System.ArgumentException("Invalid operator associativity");

            AleOperationType[] validTypes = { AleOperationType.Evaluation, AleOperationType.Assignment, AleOperationType.CompoundAssignment, AleOperationType.Increment, AleOperationType.MemberAccess, AleOperationType.ElementAccess };
            if (Array.IndexOf(validTypes, type) < 0) throw new System.ArgumentException("Invalid operation type");

            _Name = s;
            _Hash = _Name.GetHashCode();
            _Associativity = associativity;
            _Precedence = precedence;
            _InstanceTypeCode = TypeCode.Empty;
            _OperationType = type;
            _Flags = AleString.HasAlphaNumChar(name) | (name.IndexOf(' ') >= 0 ? OPERATOR_OPTION_HASWHITESPACE : 0);
            Parameters = null;
            Evaluator = null;
        }

        // creates function or method
        public AleOperation(string name)
        {
            string s = name.Trim();

            if (!CheckFunctionName(s)) throw new System.ArgumentException("Invalid function name");

            _Name = s;
            _Hash = _Name.GetHashCode();
            _Associativity = 0;
            _Precedence = 0;
            _InstanceTypeCode = TypeCode.Empty;
            _OperationType = AleOperationType.Evaluation;
            _Flags = AleString.HasAlphaNumChar(name);
            Parameters = null;
            Evaluator = null;
        }

        // creates specific operation like index, initlist, objectconst ets.
        public AleOperation(string name, AleOperationType type)
        {
            string s = name.Trim();

            int n = s.Length;
            for (int i = 0; i < n; i++)
                if (Char.IsWhiteSpace(s, i)) throw new System.ArgumentException("Invalid operation name");

            AleOperationType[] validTypes = { AleOperationType.Index, AleOperationType.InitList, AleOperationType.ObjectConst, AleOperationType.KeyValue, AleOperationType.PropertyValue };
            if (Array.IndexOf(validTypes, type) < 0) throw new System.ArgumentException("Invalid operation type");

            _Name = s;
            _Hash = _Name.GetHashCode();
            _Associativity = 0;
            _Precedence = 0;
            _InstanceTypeCode = TypeCode.Empty;
            _OperationType = type;
            _Flags = AleString.HasAlphaNumChar(name);
            Parameters = null;
            Evaluator = null;
        }

        private bool CheckOperatorName(string name)
        {
            if (String.IsNullOrEmpty(name) || Char.IsDigit(name, 0)) return false;

            string InvalidChars = "()[]{}\'\"\u0009\u000a\u000d";
            string elem;

            int n = name.Length; 
            int i = 0;
            int m;

            while (i < n)
            {
                elem = StringInfo.GetNextTextElement(name, i);
                m = elem.Length;
                if (InvalidChars.IndexOf(elem[0]) >= 0 || (Char.IsWhiteSpace(elem, 0) && elem[0] != ' ') ||
                    (elem[0] == ' ' && (m != 1 || (i + m < n && name[i + m] == ' ')))) return false;
                i += m;
            }

            i++;
            return true;
        }

        private bool CheckFunctionName(string name)
        {
            if (String.IsNullOrEmpty(name) || !AleString.IsCharAlpha(name, 0)) return false;

            int[] textUnits = StringInfo.ParseCombiningCharacters(name);
            int n = textUnits.Length;

            for (int i = 1; i < n; i++)
                if (!AleString.IsCharAlphaNum(name, textUnits[i])) return false;

            return true;
        }

        public static int CompareOperators(int precedenceLeft, uint assocLeft, int precedenceRight, uint assocRight)
        {
            if (precedenceLeft < precedenceRight) return 2;
            if (precedenceLeft > precedenceRight) return 1;
            if (assocRight == OPERATOR_YFX || assocRight == OPERATOR_YF || assocRight == OPERATOR_FX) return 2;
            if (assocLeft == OPERATOR_XFY || assocLeft == OPERATOR_XF || assocLeft == OPERATOR_FY) return 1;
            if ((assocRight & OPERATOR_INFIX) != 0) return 2;
            if ((assocLeft & OPERATOR_INFIX) != 0) return 1;
            return 2;
        }

        public override string ToString()
        {
            if (IsOperator)
            {
                StringBuilder a = new StringBuilder(25);

                if ((_Associativity & OPERATOR_FX) != 0) a.Append("FX+");
                if ((_Associativity & OPERATOR_FY) != 0) a.Append("FY+");
                if ((_Associativity & OPERATOR_XFY) != 0) a.Append("XFY+");
                if ((_Associativity & OPERATOR_YFX) != 0) a.Append("YFX+");
                if ((_Associativity & OPERATOR_XF) != 0) a.Append("XF+");
                if ((_Associativity & OPERATOR_YF) != 0) a.Append("YF+");
                a.Length--;

                return "[" + Name + ", Precedence=" + Precedence.ToString() + ", Associativity=" + a + ", Flags=" + Convert.ToString(_Flags,2) + "]";
            }

            return "[" + Name + ", ParamCount=" + ParametersCount.ToString() + ", Flags=" + Convert.ToString(_Flags, 2) + "]";
        }
    }


}
