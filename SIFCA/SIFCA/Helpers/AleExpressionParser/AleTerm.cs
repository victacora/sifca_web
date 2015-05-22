using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using AleProjects.Text;

namespace AleProjects.AleLexer.AleParser
{

    public enum AleTermType { Unknown, Atom, Operation, Variable }


    public struct AleTermResult
    {
        public const int ERROR_OK = 0;
        public const int ERROR_GENERAL = 10;
        public const int ERROR_EVALUATION = 20;
        public const int ERROR_NULLREFERENCE = 30;
        public const int ERROR_UNKNOWNELEMENT = 1000;
        public const int ERROR_UNKNOWNVARIABLE = 110;
        public const int ERROR_UNKNOWNFUNCTION = 111;
        public const int ERROR_UNKNOWNPROPERTY = 112;
        public const int ERROR_UNKNOWNMETHOD = 113;
        public const int ERROR_INCOMPATIBLETYPES = 200;
        public const int ERROR_INVALIDPARAMETERS = 300;
        public const int ERROR_INVALIDINDEXES = 310;
        public const int ERROR_INVALIDKEYS = 311;
        public const int ERROR_KEYNOTFOUND = 400;
        public const int ERROR_INDEXOUTOFBOUNDS = 410;

        private object _Value;
        private int _ErrorCode;
        private int _ErrorPos;

        public object Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                _ErrorCode = ERROR_OK;
                _ErrorPos = 0;
            }
        }

        public int ErrorCode
        {
            get { return _ErrorCode; }
            set
            {
                if (value != ERROR_OK) _Value = null; else _ErrorPos = 0;
                _ErrorCode = value;
            }
        }
        public int ErrorPos
        {
            get { return _ErrorPos; }
            set
            {
                if (_ErrorCode == ERROR_OK)
                {
                    _ErrorCode = ERROR_GENERAL;
                    _Value = null;
                }
                _ErrorPos = value;
            }
        }

        public AleTermResult(object value)
        {
            _Value = value;
            _ErrorCode = ERROR_OK;
            _ErrorPos = 0;
        }

        public bool SetError(int errCode, int errPos = 0)
        {
            Value = null;
            ErrorCode = errCode;
            if (errCode != ERROR_OK) ErrorPos = errPos; else ErrorPos = 0;
            return true;
        }

        public string ErrorMessage()
        {
            switch (ErrorCode)
            {
                case ERROR_OK:
                    return "Ok";
                case ERROR_GENERAL:
                    return "General error";
                case ERROR_EVALUATION:
                    return "Evaluation error";
                case ERROR_UNKNOWNELEMENT:
                    return "Unknown element";
                case ERROR_UNKNOWNVARIABLE:
                    return "Unknown variable";
                case ERROR_UNKNOWNFUNCTION:
                    return "Unknown function";
                case ERROR_INCOMPATIBLETYPES:
                    return "Incompatible types";
                case ERROR_INVALIDPARAMETERS:
                    return "Invalid parameters";
                case ERROR_INVALIDINDEXES:
                    return "Invalid index(es)";
                case ERROR_UNKNOWNPROPERTY:
                    return "Unknown property";
                case ERROR_UNKNOWNMETHOD:
                    return "Unknown method";
                case ERROR_KEYNOTFOUND:
                    return "Key not found";
                case ERROR_INDEXOUTOFBOUNDS:
                    return "Index out of bounds";
                default:
                    return "Error with code " + ErrorCode.ToString();
            }
        }

        public override string ToString()
        {
            return Value != null ? Value.ToString() : "";
        }
    }


    public class AleTermEvaluateArgs
    {
        private object _Result;
        private bool _HasBeenEvaluated;

        public string Name { get; set; }
        public int NameHash { get; set; }
        public object Instance { get; set; }
        public List<object> Indexes { get; set; }

        public object Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
                _HasBeenEvaluated = true;
            }
        }

        public int ErrorCode { get; protected set; }
        public int ErrorPos { get; protected set; }

        public bool HasBeenEvaluated
        {
            get { return _HasBeenEvaluated; }
        }

        public AleTermEvaluateArgs()
        {
            Name = "";
            NameHash = 0;
            Instance = null;
            Indexes = null;
            _Result = null;
            ErrorCode = AleTermResult.ERROR_OK;
            ErrorPos = 0;
            _HasBeenEvaluated = false;
        }

        public AleTermEvaluateArgs(string name, int nameHash, object instance, List<object> indexes)
        {
            Name = name;
            NameHash = nameHash;
            Instance = instance;
            Indexes = indexes;
            _Result = null;
            ErrorCode = AleTermResult.ERROR_OK;
            ErrorPos = 0;
            _HasBeenEvaluated = false;
        }

        public bool SetError(int errCode, int errPos)
        {
            _Result = null;
            ErrorCode = errCode;
            if (errCode != AleTermResult.ERROR_OK) ErrorPos = errPos; else ErrorPos = 0;
            return true;
        }
    }


    public class AleTermAssignArgs
    {
        public string Name { get; set; }
        public int NameHash { get; set; }
        public object Instance { get; set; }
        public List<object> Indexes { get; set; }
        public object Value { get; set; }
        public int ErrorCode { get; protected set; }
        public int ErrorPos { get; protected set; }

        public AleTermAssignArgs()
        {
            Name = "";
            NameHash = 0;
            Instance = null;
            Indexes = null;
            Value = null;
            ErrorCode = AleTermResult.ERROR_OK;
            ErrorPos = 0;
        }

        public AleTermAssignArgs(string name, int nameHash, object instance, List<object> indexes, object value)
        {
            Name = name;
            NameHash = nameHash;
            Instance = instance;
            Indexes = indexes;
            Value = value;
            ErrorCode = AleTermResult.ERROR_OK;
            ErrorPos = 0;
        }

        public bool SetError(int errCode, int errPos)
        {
            ErrorCode = errCode;
            if (errCode != AleTermResult.ERROR_OK) ErrorPos = errPos; else ErrorPos = 0;
            return true;
        }
    }


    public delegate void TermEvaluate(AleTerm term, AleTermEvaluateArgs e);


    public delegate void TermAssign(AleTerm term, AleTermAssignArgs e);


    public class AleTerm
    {
        private AleTermType _Type;
        private int _Hash;
        private object _Value;
        private AleTerm _Parent;
        private AleToken _Token;
        private List<AleTerm> _Elements;
        private AleExpressionParser _Parser;
        private AleOperation _Operation;

        public AleTerm()
        {
            _Type = AleTermType.Unknown;
            _Hash = 0;
            _Value = null;
            _Parent = null;
            _Token = null;
            _Elements = null;
            _Parser = null;
            _Operation = null;
        }

        public AleTerm(AleTermType type, AleExpressionParser parser)
        {
            _Type = type;
            _Hash = 0;
            _Value = null;
            _Parent = null;
            _Token = null;
            _Elements = null;
            _Parser = parser;
            _Operation = null;
        }

        public AleTermType TypeOfTerm
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Value = null;
                    _Hash = 0;
                    _Elements = null;
                    _Operation = null;
                    _Type = value;
                }
            }
        }

        public string TypeOfTermName
        {
            get
            {
                switch (_Type)
                {
                    case AleTermType.Unknown:
                        return "Unknown";
                    case AleTermType.Atom:
                        return "Atom";
                    case AleTermType.Operation:
                        return "Operation";
                    case AleTermType.Variable:
                        return "Variable";
                    default:
                        return "";
                }
            }
        }

        public int HashCode
        {
            get { return _Hash; }
            protected set { _Hash = value; }
        }

        public object Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                _Hash = _Value != null ? _Value.GetHashCode() : 0;
            }
        }

        public AleTerm Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        public AleToken Token
        {
            get { return _Token; }
            set { _Token = value; }
        }

        public List<AleTerm> Elements
        {
            get { return _Elements; }
            set { _Elements = value; }
        }

        public AleExpressionParser Parser
        {
            get { return _Parser; }
            set { _Parser = value; }
        }

        public AleOperation Operation
        {
            get { return _Operation; }
            set
            {
                if (_Type == AleTermType.Operation) _Operation = value;
            }
        }

        public int Count
        {
            get
            {
                return _Elements != null ? _Elements.Count : 0;
            }
        }

        public AleTerm Root
        {
            get
            {
                AleTerm T = this;
                while (T.Parent != null) T = T.Parent;
                return T;
            }
        }

        public AleTerm this[int index]
        {
            get
            {
                return (_Elements != null ? _Elements[index] : null);
            }
        }

        public List<Tuple<int, string>> Variables
        {
            get
            {
                List<Tuple<int, string>> res = new List<Tuple<int, string>>();
                GetVariablesList(res);
                return res;
            }
        }

        public override int GetHashCode()
        {
            return _Hash;
        }

        private void GetVariablesList(List<Tuple<int, string>> res)
        {
            if (TypeOfTerm == AleTermType.Variable)
            {
                string s = _Value.ToString();
                int n = res.Count;

                for (int i = 0; i < n; i++)
                    if (res[i].Item1 == _Hash && res[i].Item2 == s) return;

                res.Add(new Tuple<int, string>(_Hash, s));
            }
            else if (TypeOfTerm == AleTermType.Operation)
            {
                if (_Operation != null && _Operation.IsClassOperator)
                {
                    AleTerm T = _Elements[0];
                    while (T.Operation != null && T.Operation.IsClassOperator) T = T._Elements[0];
                    T.GetVariablesList(res);
                    T = _Elements[1];
                    if (T.TypeOfTerm == AleTermType.Operation) T.GetVariablesList(res);
                }
                else if (_Elements != null)
                {
                    int n = _Elements.Count;
                    for (int i = 0; i < n; i++)
                        if (_Elements[i] != null) _Elements[i].GetVariablesList(res);
                }
            }
        }

        public bool IsConstant()
        {
            if (TypeOfTerm == AleTermType.Atom) return true;

            if (_Operation != null)
                if (_Operation.IsOperator)
                {
                    if (_Operation.OperationType == AleOperationType.MemberAccess || _Operation.OperationType == AleOperationType.ElementAccess) return Elements[0].IsConstant(); ;
                    return Elements[0].IsConstant() & Elements[1].IsConstant();
                }
                else if (_Operation.OperationType == AleOperationType.InitList || _Operation.OperationType == AleOperationType.ObjectConst) return true;

            return false;
        }

        public virtual bool UserEvaluate(string name, int nameHash, object instance, List<object> indexes, out AleTermResult result, TermEvaluate userEvaluate)
        {
            result = new AleTermResult();
            AleTermEvaluateArgs e = new AleTermEvaluateArgs(name, nameHash, instance, indexes);

            if (_Parser != null)
            {
                _Parser.EvaluateStandardProperties(this, e);
                if (e.ErrorCode != AleTermResult.ERROR_OK && result.SetError(e.ErrorCode, e.ErrorPos)) return false;
                if (e.HasBeenEvaluated)
                {
                    result.Value = e.Result;
                    return true;
                }
            }

            if (userEvaluate == null && result.SetError(AleTermResult.ERROR_UNKNOWNELEMENT, _Token.StartInOrigin)) return false;

            userEvaluate(this, e);
            if (e.ErrorCode != AleTermResult.ERROR_OK && result.SetError(e.ErrorCode, e.ErrorPos)) return false;
            result.Value = e.Result;
            return true;
        }

        public virtual bool UserAssign(string name, int nameHash, object instance, List<object> indexes, ref AleTermResult value, TermAssign userAssign)
        {
            if (userAssign == null && value.SetError(AleTermResult.ERROR_UNKNOWNELEMENT, _Token.StartInOrigin)) return false;

            AleTermAssignArgs e = new AleTermAssignArgs(name, nameHash, instance, indexes, value.Value);

            userAssign(this, e);
            if (e.ErrorCode != AleTermResult.ERROR_OK && value.SetError(e.ErrorCode, e.ErrorPos)) return false;
            return true;
        }

        public static TypeCode ObjectType(object obj)
        {
            if (obj != null) return Type.GetTypeCode(obj.GetType()); else return TypeCode.Object;
        }

        public static TypeCode OperationType(object a, object b)
        {
            if (a == null) return TypeCode.Object;

            TypeCode type_a = Type.GetTypeCode(a.GetType());
            TypeCode type_b = Type.GetTypeCode((b ?? a).GetType());

            if (type_a == TypeCode.String || type_a == TypeCode.Char || type_b == TypeCode.String || type_b == TypeCode.Char ||
                a is StringBuilder || b is StringBuilder) return TypeCode.String;
            else if (type_a == TypeCode.Single || type_a == TypeCode.Double || type_b == TypeCode.Single || type_b == TypeCode.Double) return TypeCode.Double;
            else if (type_a == TypeCode.Decimal || type_b == TypeCode.Decimal) return TypeCode.Decimal;
            else if (type_a == TypeCode.UInt64 || type_b == TypeCode.UInt64) return TypeCode.UInt64;
            else if (type_a == TypeCode.Int64 || type_b == TypeCode.Int64) return TypeCode.Int64;
            else if (type_a == TypeCode.UInt32 || type_b == TypeCode.UInt32) return TypeCode.UInt32;
            else if (type_a == TypeCode.Int32 || type_a == TypeCode.Byte || type_a == TypeCode.SByte || type_a == TypeCode.Int16 || type_a == TypeCode.UInt16) return TypeCode.Int32;
            else if (type_a == type_b) return type_a;
            else return TypeCode.Object;
        }

        public static TypeCode MostAppropriateType(object obj)
        {
            if (obj == null) return TypeCode.Object;
            if (obj is StringBuilder) return TypeCode.String;

            TypeCode type_obj = Type.GetTypeCode(obj.GetType());

            switch (type_obj)
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    return TypeCode.Double;

                case TypeCode.UInt64:
                    UInt64 ui64 = Convert.ToUInt64(obj);
                    if (ui64 <= Byte.MaxValue) return TypeCode.Byte;
                    if (ui64 <= UInt16.MaxValue) return TypeCode.UInt16;
                    if (ui64 <= UInt32.MaxValue) return TypeCode.UInt32;
                    return TypeCode.UInt64;

                case TypeCode.Int64:
                    Int64 i64 = Convert.ToInt64(obj);
                    if (i64 >= Byte.MinValue && i64 <= Byte.MaxValue) return TypeCode.Byte;
                    if (i64 >= SByte.MinValue && i64 <= SByte.MaxValue) return TypeCode.SByte;
                    if (i64 >= UInt16.MinValue && i64 <= UInt16.MaxValue) return TypeCode.UInt16;
                    if (i64 >= Int16.MinValue && i64 <= Int16.MaxValue) return TypeCode.Int16;
                    if (i64 >= UInt32.MinValue && i64 <= UInt32.MaxValue) return TypeCode.UInt32;
                    if (i64 >= Int32.MinValue && i64 <= Int32.MaxValue) return TypeCode.Int32;
                    return TypeCode.Int64;

                case TypeCode.UInt32:
                    UInt32 ui32 = Convert.ToUInt32(obj);
                    if (ui32 <= Byte.MaxValue) return TypeCode.Byte;
                    if (ui32 <= UInt16.MaxValue) return TypeCode.UInt16;
                    return TypeCode.UInt32;

                case TypeCode.Int32:
                    Int32 i32 = Convert.ToInt32(obj);
                    if (i32 >= Byte.MinValue && i32 <= Byte.MaxValue) return TypeCode.Byte;
                    if (i32 >= SByte.MinValue && i32 <= SByte.MaxValue) return TypeCode.SByte;
                    if (i32 >= UInt16.MinValue && i32 <= UInt16.MaxValue) return TypeCode.UInt16;
                    if (i32 >= Int16.MinValue && i32 <= Int16.MaxValue) return TypeCode.Int16;
                    return TypeCode.Int32;

                case TypeCode.UInt16:
                    UInt32 ui16 = Convert.ToUInt16(obj);
                    if (ui16 <= Byte.MaxValue) return TypeCode.Byte;
                    return TypeCode.UInt16;

                case TypeCode.Int16:
                    Int32 i16 = Convert.ToInt16(obj); ;
                    if (i16 >= Byte.MinValue && i16 <= Byte.MaxValue) return TypeCode.Byte;
                    if (i16 >= SByte.MinValue && i16 <= SByte.MaxValue) return TypeCode.SByte;
                    return TypeCode.Int16;
            }

            return type_obj;
        }

        public static bool ValidForOperationType(object obj, TypeCode optype)
        {
            if (optype == TypeCode.String || optype == TypeCode.Object) return true;

            TypeCode type_obj = MostAppropriateType(obj);

            switch (optype)
            {
                case TypeCode.Double:
                    return type_obj == TypeCode.Double || type_obj == TypeCode.Single || type_obj == TypeCode.Decimal ||
                           type_obj == TypeCode.UInt64 || type_obj == TypeCode.Int64 ||
                           type_obj == TypeCode.UInt32 || type_obj == TypeCode.Int32 ||
                           type_obj == TypeCode.UInt16 || type_obj == TypeCode.Int16 ||
                           type_obj == TypeCode.Byte || type_obj == TypeCode.SByte;

                case TypeCode.Decimal:
                    return type_obj == TypeCode.Decimal ||
                           type_obj == TypeCode.UInt64 || type_obj == TypeCode.Int64 ||
                           type_obj == TypeCode.UInt32 || type_obj == TypeCode.Int32 ||
                           type_obj == TypeCode.UInt16 || type_obj == TypeCode.Int16 ||
                           type_obj == TypeCode.Byte || type_obj == TypeCode.SByte;

                case TypeCode.UInt64:
                    return type_obj == TypeCode.UInt64 || type_obj == TypeCode.UInt32 || type_obj == TypeCode.UInt16 || type_obj == TypeCode.Byte;

                case TypeCode.Int64:
                    return type_obj == TypeCode.Int64 ||
                           type_obj == TypeCode.UInt32 || type_obj == TypeCode.Int32 ||
                           type_obj == TypeCode.UInt16 || type_obj == TypeCode.Int16 ||
                           type_obj == TypeCode.Byte || type_obj == TypeCode.SByte;

                case TypeCode.UInt32:
                    return type_obj == TypeCode.UInt32 || type_obj == TypeCode.UInt16 || type_obj == TypeCode.Byte;

                case TypeCode.Int32:
                    return type_obj == TypeCode.Int32 || type_obj == TypeCode.UInt16 || type_obj == TypeCode.Int16 || type_obj == TypeCode.Byte || type_obj == TypeCode.SByte;

                case TypeCode.UInt16:
                    return type_obj == TypeCode.UInt16 || type_obj == TypeCode.Byte;

                case TypeCode.Int16:
                    return type_obj == TypeCode.Int16 || type_obj == TypeCode.Byte || type_obj == TypeCode.SByte;
            }

            return optype == type_obj;
        }

        public bool AssignValue(AleTerm assignTo, ref AleTermResult value, TermEvaluate userEvaluate, TermAssign userAssign)
        {
            switch (assignTo.TypeOfTerm)
            {
                case AleTermType.Variable:
                    return UserAssign(assignTo.Value.ToString(), assignTo.HashCode, null, null, ref value, userAssign);

                case AleTermType.Operation:
                    if (assignTo.Operation != null)
                    {
                        AleTermResult a = new AleTermResult();
                        AleTermResult b = new AleTermResult();

                        if (assignTo.Operation.IsClassOperator)
                        {
                            if (!assignTo[0].Evaluate(out a, userEvaluate, userAssign) && value.SetError(a.ErrorCode, a.ErrorPos)) return false;
                            return assignTo[1].TypeOfTerm == AleTermType.Variable && UserAssign(assignTo[1].Value.ToString(), assignTo[1].HashCode, a.Value, null, ref value, userAssign);
                        }
                        else if (assignTo.Operation.IsIndexOperator)
                        {
                            List<object> list;

                            if (!assignTo[0].Evaluate(out a, userEvaluate, userAssign) && value.SetError(a.ErrorCode, a.ErrorPos)) return false;

                            int n = assignTo[1].Count;
                            list = new List<object>();

                            for (int i = 0; i < n; i++)
                            {
                                if (assignTo[1][i] != null)
                                {
                                    if (!assignTo[1][i].Evaluate(out b, userEvaluate, userAssign) && value.SetError(b.ErrorCode, b.ErrorPos)) return false;
                                }
                                else b.Value = null;
                                list.Add(b.Value);
                            }

                            return UserAssign(null, 0, a.Value, list, ref value, userAssign);
                        }
                    }
                    break;
            }

            value.SetError(AleTermResult.ERROR_GENERAL, assignTo.Token.StartInOrigin);
            return false;
        }

        public bool Evaluate(out AleTermResult result, TermEvaluate userEvaluate = null, TermAssign userAssign = null)
        {
            result = new AleTermResult();

            if (TypeOfTerm == AleTermType.Atom || TypeOfTerm == AleTermType.Unknown)
            {
                result.Value = _Value;
                return true;
            }

            AleTermResult res = new AleTermResult();

            try
            {
                if (TypeOfTerm == AleTermType.Variable) return UserEvaluate(Value.ToString(), HashCode, null, null, out result, userEvaluate);

                if ((_Operation == null || _Operation.IsClassMethod) && result.SetError(AleTermResult.ERROR_UNKNOWNFUNCTION, Token.StartInOrigin)) return false;

                OperationEvalParameters evalParams = new OperationEvalParameters();
                evalParams.userEvaluate = userEvaluate;
                evalParams.userAssign = userAssign;

                if (_Operation.IsOperator) return _Operation.Evaluator(this, ref evalParams, ref result);

                int m = _Operation.ParametersCount;
                int n = _Elements == null ? 0 : _Elements.Count;
                evalParams.ActualParamsCount = n;

                if (m > 0 && m < 4)
                {
                    if (n > 0 && _Elements[0] != null)
                    {
                        if (!_Elements[0].Evaluate(out res, userEvaluate, userAssign) && result.SetError(res.ErrorCode, res.ErrorPos)) return false;
                        if (!ValidForOperationType(res.Value, _Operation.Parameters[0].Item1) &&
                            result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, _Elements[0].Token.StartInOrigin)) return false;
                        evalParams.FirstParam = res.Value;
                    }
                    else evalParams.FirstParam = _Operation.Parameters[0].Item2;

                    if (n > 1 && _Elements[1] != null)
                    {
                        if (!_Elements[1].Evaluate(out res, userEvaluate, userAssign) && result.SetError(res.ErrorCode, res.ErrorPos)) return false;
                        if (!ValidForOperationType(res.Value, _Operation.Parameters[1].Item1) &&
                            result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, _Elements[1].Token.StartInOrigin)) return false;
                        evalParams.SecondParam = res.Value;
                    }
                    else if (m > 1) evalParams.SecondParam = _Operation.Parameters[1].Item2;

                    if (n > 2 && _Elements[2] != null)
                    {
                        if (!_Elements[2].Evaluate(out res, userEvaluate, userAssign) && result.SetError(res.ErrorCode, res.ErrorPos)) return false;
                        if (!ValidForOperationType(res.Value, _Operation.Parameters[2].Item1) &&
                            result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, _Elements[2].Token.StartInOrigin)) return false;
                        evalParams.ThirdParam = res.Value;
                    }
                    else if (m > 2) evalParams.ThirdParam = _Operation.Parameters[2].Item2;
                }
                else if (m > 3)
                {
                    object obj;

                    evalParams.Parameters = new List<object>(m);
                    for (int i = 0; i < m; i++)
                        if (i < n && _Elements[i] != null)
                        {
                            if (!_Elements[i].Evaluate(out res, userEvaluate, userAssign) && result.SetError(res.ErrorCode, res.ErrorPos)) return false;
                            obj = res.Value;
                            if (!ValidForOperationType(obj, _Operation.Parameters[i].Item1) &&
                                result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, _Elements[i].Token.StartInOrigin)) return false;
                            evalParams.Parameters.Add(obj);
                        }
                        else evalParams.Parameters.Add(_Operation.Parameters[i].Item2);
                }

                return _Operation.Evaluator(this, ref evalParams, ref result);

            }
            catch
            {
                result.SetError(AleTermResult.ERROR_EVALUATION, Token.StartInOrigin);
                return false;
            }

        }

        public virtual string DebugPrint(int indent = 0)
        {
            string res;

            if (_Value != null) res = _Value.ToString(); else res = "<null>";
            res = new string(' ', indent * 4) + "Type=" + TypeOfTermName + " : Value=" + res + " : Elements=" + Count.ToString() + " : Operation=" + (_Operation != null ? "yes" : "no") +
                  " : Parent=" + (Parent == null ? "null" : Parent.Value.ToString());

            if (_Elements != null)
            {
                int n = _Elements.Count;
                for (int i = 0; i < n; i++)
                    if (_Elements[i] != null) res += "\u000d\u000a" + new string(' ', indent * 4 + 4) + (i + 1).ToString() + ": " + _Elements[i].DebugPrint(indent + 1).TrimStart();
                    else res += "\u000d\u000a" + new string(' ', indent * 4 + 4) + (i + 1).ToString() + ": <null>";
            }

            return res;
        }

    }

}