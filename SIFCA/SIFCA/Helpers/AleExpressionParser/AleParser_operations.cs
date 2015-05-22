using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using AleProjects.Text;

namespace AleProjects.AleLexer.AleParser
{

    public partial class AleExpressionParser : AleSimpleLexer
    {

        protected bool BuiltinClassOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;

            if ((!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) || 
                (a.Value == null && result.SetError(AleTermResult.ERROR_NULLREFERENCE, term.Token.StartInOrigin))) return false;

            if (t2.TypeOfTerm != AleTermType.Operation)
            {
                Dictionary<string, object> dict = a.Value as Dictionary<string, object>;
                if (dict != null)
                {
                    object res;
                    if (dict.TryGetValue(t2.Value.ToString(), out res)) result.Value = res;
                    else result.SetError(AleTermResult.ERROR_UNKNOWNPROPERTY, term.Token.StartInOrigin);

                    return result.ErrorCode == AleTermResult.ERROR_OK;
                }

                return term.UserEvaluate(t2.Value.ToString(), t2.HashCode, a.Value, null, out result, parameters.userEvaluate);
            }

            if ((t2.Operation == null || !AleTerm.ValidForOperationType(a.Value, t2.Operation.InstanceTypeCode)) &&
                result.SetError(AleTermResult.ERROR_UNKNOWNMETHOD, t2.Token.StartInOrigin)) return false;

            OperationEvalParameters evalParams = new OperationEvalParameters();
            evalParams.ClassInstance = a.Value;

            int m = t2.Operation.ParametersCount;
            int n = t2.Elements == null ? 0 : t2.Elements.Count;

            if (m > 0 && m < 4)
            {
                if (n > 0 && t2[0] != null)
                {
                    if ((!t2[0].Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorCode)) ||
                        (!AleTerm.ValidForOperationType(a.Value, t2.Operation.Parameters[0].Item1) && 
                        result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, t2[0].Token.StartInOrigin))) return false;
                    evalParams.FirstParam = a.Value;
                }
                else evalParams.FirstParam = t2.Operation.Parameters[0].Item2;

                if (n > 1 && t2[1] != null)
                {
                    if ((!t2[1].Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorCode)) ||
                        (!AleTerm.ValidForOperationType(a.Value, t2.Operation.Parameters[1].Item1) &&
                        result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, t2[1].Token.StartInOrigin))) return false;
                    evalParams.SecondParam = a.Value;
                }
                else if (m > 1) evalParams.SecondParam = t2.Operation.Parameters[1].Item2;

                if (n > 2 && t2[2] != null)
                {
                    if ((!t2[2].Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorCode)) ||
                        (!AleTerm.ValidForOperationType(a.Value, t2.Operation.Parameters[2].Item1) &&
                         result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, t2[2].Token.StartInOrigin))) return false;
                    evalParams.ThirdParam = a.Value;
                }
                else if (m > 2) evalParams.ThirdParam = t2.Operation.Parameters[2].Item2;
            }
            else if (m > 3)
            {
                evalParams.Parameters = new List<object>(m);
                for (int i = 0; i < m; i++)
                    if (i < n && t2[i] != null)
                    {
                        if ((!t2[i].Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorCode)) ||
                            (!AleTerm.ValidForOperationType(a.Value, t2.Operation.Parameters[i].Item1) &&
                             result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, t2[i].Token.StartInOrigin))) return false;
                        evalParams.Parameters.Add(a.Value);
                    }
                    else evalParams.Parameters.Add(t2.Operation.Parameters[2].Item2);
            }

            return t2.Operation.Evaluator(t2, ref evalParams, ref result);
        }

        protected bool BuiltinIndexOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if ((!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) ||
                (a.Value == null && result.SetError(AleTermResult.ERROR_NULLREFERENCE, term.Token.StartInOrigin))) return false;

            AleTermResult b = new AleTermResult();
            List<object> indexes = new List<object>();
            int n = t2.Count;

            for (int i = 0; i < n; i++)
            {
                if (t2[i] != null)
                {
                    if (!t2[i].Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;
                }
                else b.Value = null;
                indexes.Add(b.Value);
            }

            if (indexes.Count == 1 && (Type.GetTypeCode(a.Value.GetType()) == TypeCode.String || a.Value is StringBuilder) && 
                AleTerm.ValidForOperationType(indexes[0], TypeCode.Int32))
            {
                result.Value = a.Value.ToString()[Convert.ToInt32(indexes[0])];
                return true;
            }

            Dictionary<object, object> dict = a.Value as Dictionary<object, object>;
            if (dict != null)
            {
                if (indexes.Count != 1 && result.SetError(AleTermResult.ERROR_INVALIDINDEXES, term.Token.StartInOrigin)) return false;
                object res;
                if (AleTerm.ValidForOperationType(indexes[0], TypeCode.Int32))
                    if (dict.TryGetValue(Convert.ToInt32(indexes[0]), out res)) result.Value = res; else result.Value = (int)0;
                else if (dict.TryGetValue(indexes[0], out res)) result.Value = res; else result.Value = (int)0;

                return true;
            }

            return term.UserEvaluate(null, 0, a.Value, indexes, out result, parameters.userEvaluate);
        }

        protected bool BuiltinKeyValueOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTermResult a, b;

            if (!term[0].Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            if (!term[1].Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;
            result.Value = new KeyValuePair<object, object>(a.Value, b.Value);
            return true;
        }

        protected bool BuiltinPropertyValueOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTermResult a, b;

            if (!term[0].Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            if (!term[1].Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;
            result.Value = new KeyValuePair<string, object>(a.Value.ToString(), b.Value);
            return true;
        }

        protected bool BuiltinInitListOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            Dictionary<object, object> initlist = new Dictionary<object, object>();
            AleTermResult a;
            object b;
            int key = 0;
            int n = term.Count;
            result.Value = null;

            for (int i = 0; i < n; i++)
            {
                if (!term[i].Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

                if (a.Value is KeyValuePair<object, object>)
                {
                    b = ((KeyValuePair<object, object>)a.Value).Key;
                    if (AleTerm.ValidForOperationType(b, TypeCode.Int32))
                    {
                        key = Convert.ToInt32(b);
                        initlist.Add(key, ((KeyValuePair<object, object>)a.Value).Value);
                        key++;
                    }
                    else initlist.Add(b, ((KeyValuePair<object, object>)a.Value).Value);
                }
                else
                {
                    initlist.Add(key, a);
                    key++;
                }
            }

            result.Value = initlist;
            return true;
        }

        protected bool BuiltinObjectConstOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            Dictionary<string, object> proplist = new Dictionary<string, object>(IgnoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture);
            AleTermResult a;
            int n = term.Count;

            for (int i = 0; i < n; i++)
            {
                if (!term[i].Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
                proplist.Add(((KeyValuePair<string, object>)a.Value).Key, ((KeyValuePair<string, object>)a.Value).Value);
            }

            result.Value = proplist;
            return true;
        }

        protected bool BuiltinAssignOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t2.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            result.Value = a.Value;
            return term.AssignValue(term[0], ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignINCOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.String:
                    result.Value = a.Value.ToString() + b.Value.ToString();
                    break;
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) + Convert.ToDouble(b.Value);
                    break;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) + Convert.ToDecimal(b.Value);
                    break;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) + Convert.ToUInt64(b.Value);
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) + Convert.ToInt64(b.Value);
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) + Convert.ToUInt32(b.Value);
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) + Convert.ToInt32(b.Value);
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignDECOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) + Convert.ToDouble(b.Value);
                    break;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) + Convert.ToDecimal(b.Value);
                    break;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) + Convert.ToUInt64(b.Value);
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) + Convert.ToInt64(b.Value);
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) + Convert.ToUInt32(b.Value);
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) + Convert.ToInt32(b.Value);
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignMULOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) * Convert.ToDouble(b.Value);
                    break;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) * Convert.ToDecimal(b.Value);
                    break;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) * Convert.ToUInt64(b.Value);
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) * Convert.ToInt64(b.Value);
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) * Convert.ToUInt32(b.Value);
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) * Convert.ToInt32(b.Value);
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignDIVOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) / Convert.ToDouble(b.Value);
                    break;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) / Convert.ToDecimal(b.Value);
                    break;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) / Convert.ToUInt64(b.Value);
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) / Convert.ToInt64(b.Value);
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) / Convert.ToUInt32(b.Value);
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) / Convert.ToInt32(b.Value);
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignREMOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) % Convert.ToDouble(b.Value);
                    break;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) % Convert.ToDecimal(b.Value);
                    break;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) % Convert.ToUInt64(b.Value);
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) % Convert.ToInt64(b.Value);
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) % Convert.ToUInt32(b.Value);
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) % Convert.ToInt32(b.Value);
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignANDOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) & Convert.ToUInt64(b.Value);
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) & Convert.ToInt64(b.Value);
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) & Convert.ToUInt32(b.Value);
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) & Convert.ToInt32(b.Value);
                    break;
                case TypeCode.Boolean:
                    result.Value = Convert.ToBoolean(a.Value) & Convert.ToBoolean(b.Value);
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignOROperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) | Convert.ToUInt64(b.Value);
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) | Convert.ToInt64(b.Value);
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) | Convert.ToUInt32(b.Value);
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) | Convert.ToInt32(b.Value);
                    break;
                case TypeCode.Boolean:
                    result.Value = Convert.ToBoolean(a.Value) | Convert.ToBoolean(b.Value);
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignXOROperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) ^ Convert.ToUInt64(b.Value);
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) ^ Convert.ToInt64(b.Value);
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) ^ Convert.ToUInt32(b.Value);
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) ^ Convert.ToInt32(b.Value);
                    break;
                case TypeCode.Boolean:
                    result.Value = Convert.ToBoolean(a.Value) ^ Convert.ToBoolean(b.Value);
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignSHLOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];
            int i = 0;

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, null);
            if (AleTerm.ValidForOperationType(b.Value, TypeCode.Int32)) i = Convert.ToInt32(b.Value); else optype = TypeCode.Object;

            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) << i;
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) << i;
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) << i;
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) << i;
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinAssignSHROperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];
            int i = 0;

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, null);
            if (AleTerm.ValidForOperationType(b.Value, TypeCode.Int32)) i = Convert.ToInt32(b.Value); else optype = TypeCode.Object;

            if (!AleTerm.ValidForOperationType(b.Value, optype) && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin)) return false;

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) >> i;
                    break;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) >> i;
                    break;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) >> i;
                    break;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) >> i;
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            return term.AssignValue(t1, ref result, parameters.userEvaluate, parameters.userAssign);
        }

        protected bool BuiltinINCOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, null);

            switch (optype)
            {
                case TypeCode.Double:
                    double x = Convert.ToDouble(a.Value) + 1.0;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToDouble(a.Value); else result.Value = x;
                    a.Value = x;
                    break;
                case TypeCode.Decimal:
                    Decimal d = Convert.ToDecimal(a.Value) + 1.0m;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToDecimal(a.Value); else result.Value = d;
                    a.Value = d;
                    break;
                case TypeCode.UInt64:
                    UInt64 ui64 = Convert.ToUInt64(a.Value) + 1;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToUInt64(a.Value); else result.Value = ui64;
                    a.Value = ui64;
                    break;
                case TypeCode.Int64:
                    Int64 i64 = Convert.ToInt64(a.Value) + 1;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToInt64(a.Value); else result.Value = i64;
                    a.Value = i64;
                    break;
                case TypeCode.UInt32:
                    UInt32 ui32 = Convert.ToUInt32(a.Value) + 1;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToUInt32(a.Value); else result.Value = ui32;
                    a.Value = ui32;
                    break;
                case TypeCode.Int32:
                    int i = Convert.ToInt32(a.Value) + 1;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToInt32(a.Value); else result.Value = i;
                    a.Value = i;
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            if (!term.AssignValue(t1, ref a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            return true;
        }

        protected bool BuiltinDECOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, null);

            switch (optype)
            {
                case TypeCode.Double:
                    double x = Convert.ToDouble(a.Value) - 1.0;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToDouble(a.Value); else result.Value = x;
                    a.Value = x;
                    break;
                case TypeCode.Decimal:
                    Decimal d = Convert.ToDecimal(a.Value) - 1.0m;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToDecimal(a.Value); else result.Value = d;
                    a.Value = d;
                    break;
                case TypeCode.UInt64:
                    UInt64 ui64 = Convert.ToUInt64(a.Value) - 1;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToUInt64(a.Value); else result.Value = ui64;
                    a.Value = ui64;
                    break;
                case TypeCode.Int64:
                    Int64 i64 = Convert.ToInt64(a.Value) - 1;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToInt64(a.Value); else result.Value = i64;
                    a.Value = i64;
                    break;
                case TypeCode.UInt32:
                    UInt32 ui32 = Convert.ToUInt32(a.Value) - 1;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToUInt32(a.Value); else result.Value = ui32;
                    a.Value = ui32;
                    break;
                case TypeCode.Int32:
                    int i = Convert.ToInt32(a.Value) - 1;
                    if ((term.Token as AleOperationToken).IsPrefixOperator) result.Value = Convert.ToInt32(a.Value); else result.Value = i;
                    a.Value = i;
                    break;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }

            if (!term.AssignValue(t1, ref a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            return true;
        }

        protected bool BuiltinBooleanOROperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            if (AleTerm.ObjectType(a.Value) == TypeCode.Boolean)
            {
                if (!(bool)a.Value)
                {
                    AleTerm t2 = term[1];
                    AleTermResult b;
                    if ((!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) ||
                        (AleTerm.ObjectType(b.Value) != TypeCode.Boolean && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin))) return false;
                    result.Value = (bool)b.Value;
                }
                else result.Value = true;

                return true;
            }

            result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
            return false;
        }

        protected bool BuiltinBooleanANDOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            if (AleTerm.ObjectType(a.Value) == TypeCode.Boolean)
            {
                if ((bool)a.Value)
                {
                    AleTerm t2 = term[1];
                    AleTermResult b;
                    if ((!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) ||
                        (AleTerm.ObjectType(b.Value) != TypeCode.Boolean && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin))) return false;
                    result.Value = (bool)b.Value;
                }
                else result.Value = false;

                return true;
            }

            result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
            return false;
        }

        protected bool BuiltinBooleanNOTOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];

            AleTermResult a;
            if ((!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) ||
                (AleTerm.ObjectType(a.Value) != TypeCode.Boolean && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin))) return false;
            result.Value = !(bool)a.Value;
            return true;
        }

        protected bool BuiltinXOROperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) ^ Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) ^ Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) ^ Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) ^ Convert.ToInt32(b.Value);
                    return true;
                case TypeCode.Boolean:
                    result.Value = Convert.ToBoolean(a.Value) ^ Convert.ToBoolean(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinOROperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) | Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) | Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) | Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) | Convert.ToInt32(b.Value);
                    return true;
                case TypeCode.Boolean:
                    result.Value = Convert.ToBoolean(a.Value) | Convert.ToBoolean(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinORShortCircuitOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a, b;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            if (AleTerm.ObjectType(a.Value) == TypeCode.Boolean)
            {
                if (!(bool)a.Value)
                {
                    if ((!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) ||
                        (AleTerm.ObjectType(b.Value) != TypeCode.Boolean && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin))) return false;
                    result.Value = (bool)b.Value;
                }
                else result.Value = true;

                return true;
            }

            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) | Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) | Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) | Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) | Convert.ToInt32(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinANDOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) & Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) & Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) & Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) & Convert.ToInt32(b.Value);
                    return true;
                case TypeCode.Boolean:
                    result.Value = Convert.ToBoolean(a.Value) & Convert.ToBoolean(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinANDShortCircuitOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a, b;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            if (AleTerm.ObjectType(a.Value) == TypeCode.Boolean)
            {
                if ((bool)a.Value)
                {
                    if ((!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) ||
                        (AleTerm.ObjectType(b.Value) != TypeCode.Boolean && result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin))) return false;
                    result.Value = (bool)b.Value;
                }
                else result.Value = false;

                return true;
            }

            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) & Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) & Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) & Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) & Convert.ToInt32(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinNOTOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, null);

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = ~Convert.ToUInt64(a.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = ~Convert.ToInt64(a.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = ~Convert.ToUInt32(a.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = ~Convert.ToInt32(a.Value);
                    return true;
                case TypeCode.Boolean:
                    result.Value = !(bool)a.Value;
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinBitNOTOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, null);

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = ~Convert.ToUInt64(a.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = ~Convert.ToInt64(a.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = ~Convert.ToUInt32(a.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = ~Convert.ToInt32(a.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinEqualOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.String:
                    result.Value = string.Compare(a.Value.ToString(), b.Value.ToString()) == 0;
                    return true;
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) == Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) == Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) == Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) == Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) == Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) == Convert.ToInt32(b.Value);
                    return true;
                case TypeCode.DateTime:
                    result.Value = Convert.ToDateTime(a.Value) == Convert.ToDateTime(b.Value);
                    return true;
                case TypeCode.Boolean:
                    result.Value = Convert.ToBoolean(a.Value) == Convert.ToBoolean(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinNotEqualOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.String:
                    result.Value = string.Compare(a.Value.ToString(), b.Value.ToString()) != 0;
                    return true;
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) != Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) != Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) != Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) != Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) != Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) != Convert.ToInt32(b.Value);
                    return true;
                case TypeCode.DateTime:
                    result.Value = Convert.ToDateTime(a.Value) != Convert.ToDateTime(b.Value);
                    return true;
                case TypeCode.Boolean:
                    result.Value = Convert.ToBoolean(a.Value) != Convert.ToBoolean(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinGTOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.String:
                    result.Value = string.Compare(a.Value.ToString(), b.Value.ToString()) > 0;
                    return true;
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) > Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) > Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) > Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) > Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) > Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) > Convert.ToInt32(b.Value);
                    return true;
                case TypeCode.DateTime:
                    result.Value = Convert.ToDateTime(a.Value) > Convert.ToDateTime(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinLTOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.String:
                    result.Value = string.Compare(a.Value.ToString(), b.Value.ToString()) < 0;
                    return true;
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) < Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) < Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) < Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) < Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) < Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) < Convert.ToInt32(b.Value);
                    return true;
                case TypeCode.DateTime:
                    result.Value = Convert.ToDateTime(a.Value) < Convert.ToDateTime(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinGTEOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.String:
                    result.Value = string.Compare(a.Value.ToString(), b.Value.ToString()) >= 0;
                    return true;
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) >= Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) >= Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) >= Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) >= Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) >= Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) >= Convert.ToInt32(b.Value);
                    return true;
                case TypeCode.DateTime:
                    result.Value = Convert.ToDateTime(a.Value) >= Convert.ToDateTime(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinLTEOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.String:
                    result.Value = string.Compare(a.Value.ToString(), b.Value.ToString()) <= 0;
                    return true;
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) <= Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) <= Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) <= Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) <= Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) <= Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) <= Convert.ToInt32(b.Value);
                    return true;
                case TypeCode.DateTime:
                    result.Value = Convert.ToDateTime(a.Value) <= Convert.ToDateTime(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinAddOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.String:
                    result.Value = a.Value.ToString() + b.Value.ToString();
                    return true;
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) + Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) + Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) + Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) + Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) + Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) + Convert.ToInt32(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinSubstractOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;

            AleTermResult b;
            if (term.Count == 2) // binary operator
            {
                AleTerm t2 = term[1];
                if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;
            }
            else
            {
                b = a;
                a.Value = 0;
            }

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) - Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) - Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) - Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) - Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) - Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) - Convert.ToInt32(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinMulOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) * Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) * Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) * Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) * Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) * Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) * Convert.ToInt32(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinDivOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) / Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) / Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) / Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) / Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) / Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) / Convert.ToInt32(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinRemOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);

            switch (optype)
            {
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(a.Value) % Convert.ToDouble(b.Value);
                    return true;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(a.Value) % Convert.ToDecimal(b.Value);
                    return true;
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) % Convert.ToUInt64(b.Value);
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) % Convert.ToInt64(b.Value);
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) % Convert.ToUInt32(b.Value);
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) % Convert.ToInt32(b.Value);
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinSHLOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];
            int i = 0;

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (AleTerm.ValidForOperationType(b.Value, TypeCode.Int32)) i = Convert.ToInt32(b.Value); else optype = TypeCode.Object;

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) << i;
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) << i;
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) << i;
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) << i;
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinSHROperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];
            int i = 0;

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            AleTermResult b;
            if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;

            TypeCode optype = AleTerm.OperationType(a.Value, b.Value);
            if (AleTerm.ValidForOperationType(b.Value, TypeCode.Int32)) i = Convert.ToInt32(b.Value); else optype = TypeCode.Object;

            switch (optype)
            {
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(a.Value) >> i;
                    return true;
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(a.Value) >> i;
                    return true;
                case TypeCode.UInt32:
                    result.Value = Convert.ToUInt32(a.Value) >> i;
                    return true;
                case TypeCode.Int32:
                    result.Value = Convert.ToInt32(a.Value) >> i;
                    return true;
                default:
                    result.SetError(AleTermResult.ERROR_INCOMPATIBLETYPES, term.Token.StartInOrigin);
                    return false;
            }
        }

        protected bool BuiltinNullCoalescingOperation(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            AleTerm t1 = term[0];
            AleTerm t2 = term[1];

            AleTermResult a;
            if (!t1.Evaluate(out a, parameters.userEvaluate, parameters.userAssign) && result.SetError(a.ErrorCode, a.ErrorPos)) return false;
            if (a.Value == null)
            {
                AleTermResult b;
                if (!t2.Evaluate(out b, parameters.userEvaluate, parameters.userAssign) && result.SetError(b.ErrorCode, b.ErrorPos)) return false;
                result.Value = b.Value;
            }
            else result.Value = a.Value;

            return true;
        }

        protected bool BuiltinSinFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Sin(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinCosFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Cos(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinTanFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Tan(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinSinhFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Sinh(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinCoshFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Cosh(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinTanhFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Tanh(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinASinFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Asin(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinACosFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Acos(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinATanFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Atan(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinExpFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Exp(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinLogFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Log(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinLog10Function(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Log10(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinSqrtFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Sqrt(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinPowFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Pow(Convert.ToDouble(parameters.FirstParam), Convert.ToDouble(parameters.SecondParam));
            return true;
        }

        protected bool BuiltinAbsFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Abs(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinCeilingFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Ceiling(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinFloorFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Floor(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinRoundFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            if (parameters.ActualParamsCount == 2)
            {
                int i = Convert.ToInt32(parameters.SecondParam);
                if (i > 15) i = 15;
                else if (i < 0) i = 0;
                result.Value = Math.Round(Convert.ToDouble(parameters.FirstParam), i);
            }
            else result.Value = Math.Round(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinTruncateFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = Math.Truncate(Convert.ToDouble(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinToStringMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            if (Type.GetTypeCode(parameters.ClassInstance.GetType()) == TypeCode.String) result.Value = parameters.ClassInstance; else result.Value = parameters.ClassInstance.ToString();
            return true;
        }

        protected bool BuiltinToStringFmtMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            string fmt = parameters.FirstParam.ToString();
            TypeCode typ = Type.GetTypeCode(parameters.ClassInstance.GetType());

            switch (typ)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    result.Value = Convert.ToInt64(parameters.ClassInstance).ToString(fmt);
                    break;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    result.Value = Convert.ToUInt64(parameters.ClassInstance).ToString(fmt);
                    break;
                case TypeCode.DateTime:
                    result.Value = Convert.ToDateTime(parameters.ClassInstance).ToString(fmt);
                    break;
                case TypeCode.Double:
                    result.Value = Convert.ToDouble(parameters.ClassInstance).ToString(fmt);
                    break;
                case TypeCode.Decimal:
                    result.Value = Convert.ToDecimal(parameters.ClassInstance).ToString(fmt);
                    break;
                default:
                    result.Value = parameters.ClassInstance.ToString();
                    break;
            }

            return true;
        }

        protected bool BuiltinStringFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            string S = parameters.FirstParam == null ? null : parameters.FirstParam.ToString();
            result.Value = String.IsNullOrEmpty(S) ? "" : new String(S[0], Convert.ToInt32(parameters.SecondParam));
            return true;
        }

        protected bool BuiltinCompareFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            string S1 = parameters.FirstParam == null ? null : parameters.FirstParam.ToString();
            string S2 = parameters.SecondParam == null ? null : parameters.SecondParam.ToString();
            StringComparison comparison = (bool)parameters.ThirdParam ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            result.Value = String.Compare(S1, S2, comparison);
            return true;
        }

        protected bool BuiltinIndexOfMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            if (parameters.FirstParam != null) result.Value = parameters.ClassInstance.ToString().IndexOf(parameters.FirstParam.ToString());
            else result.Value = -1;

            return true;
        }

        protected bool BuiltinIndexOf2ParamsMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            if (parameters.FirstParam != null)
            {
                string s = parameters.ClassInstance.ToString();
                int i = Convert.ToInt32(parameters.SecondParam);

                if (i < 0) i += s.Length;
                result.Value = s.IndexOf(parameters.FirstParam.ToString(), i);
            }
            else result.Value = -1;

            return true;
        }

        protected bool BuiltinSubstringMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            string s = parameters.ClassInstance.ToString();
            int i = Convert.ToInt32(parameters.FirstParam);
            if (i < 0) i += s.Length;
            else if (i >= s.Length) i = s.Length;
            result.Value = s.Substring(i);
            return true;
        }

        protected bool BuiltinSubstring2ParamsMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            string s = parameters.ClassInstance.ToString();
            int i = Convert.ToInt32(parameters.FirstParam);
            int n = Convert.ToInt32(parameters.SecondParam);
            if (i < 0) i += s.Length;
            else if (i >= s.Length) i = s.Length;
            if (i + n > s.Length) n = s.Length - i;
            result.Value = s.Substring(i, n);
            return true;
        }

        protected bool BuiltinReplaceMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            if (parameters.FirstParam != null)
            {
                string s = parameters.ClassInstance.ToString();
                string s1 = parameters.SecondParam != null ? parameters.SecondParam.ToString() : "";
                result.Value = s.Replace(parameters.FirstParam.ToString(), s1);
            }
            else result.Value = parameters.ClassInstance;

            return true;
        }

        protected bool BuiltinToUpperMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = parameters.ClassInstance.ToString().ToUpper();
            return true;
        }

        protected bool BuiltinToLowerMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = parameters.ClassInstance.ToString().ToLower();
            return true;
        }

        protected bool BuiltinTrimMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = parameters.ClassInstance.ToString().Trim();
            return true;
        }

        protected bool BuiltinTrimStartMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = parameters.ClassInstance.ToString().TrimStart();
            return true;
        }

        protected bool BuiltinTrimEndMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = parameters.ClassInstance.ToString().TrimEnd();
            return true;
        }

        protected bool BuiltinChrFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = (char)Convert.ToInt16(Convert.ToInt16(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinOrdFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            if (parameters.FirstParam != null) result.Value = (UInt16)parameters.FirstParam.ToString()[0]; else result.Value = 0;
            return true;
        }

        protected bool BuiltinNowFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = DateTime.Now;
            return true;
        }

        protected bool BuiltinDaysInMonthFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = DateTime.DaysInMonth(Convert.ToInt32(parameters.FirstParam), Convert.ToInt32(parameters.SecondParam));
            return true;
        }

        protected bool BuiltinDateTimeFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = new DateTime(Convert.ToInt32(parameters.Parameters[0]), 
                                    Convert.ToInt32(parameters.Parameters[1]), Convert.ToInt32(parameters.Parameters[2]),
                                    Convert.ToInt32(parameters.Parameters[3]), Convert.ToInt32(parameters.Parameters[4]), 
                                    Convert.ToInt32(parameters.Parameters[5]), Convert.ToInt32(parameters.Parameters[6]));
            return true;
        }

        protected bool BuiltinDateTimeTicksFunction(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            result.Value = new DateTime(Convert.ToInt64(parameters.FirstParam));
            return true;
        }

        protected bool BuiltinDateAddMethod(AleTerm term, ref OperationEvalParameters parameters, ref AleTermResult result)
        {
            DateTime dt = Convert.ToDateTime(parameters.ClassInstance);

            int i = Convert.ToInt32(parameters.Parameters[0]);
            if (i != 0) dt = dt.AddYears(i);
            i = Convert.ToInt32(parameters.Parameters[1]);
            if (i != 0) dt = dt.AddMonths(i);
            i = Convert.ToInt32(parameters.Parameters[2]);
            if (i != 0) dt = dt.AddDays(i);
            i = Convert.ToInt32(parameters.Parameters[3]);
            if (i != 0) dt = dt.AddHours(i);
            i = Convert.ToInt32(parameters.Parameters[4]);
            if (i != 0) dt = dt.AddMinutes(i);
            i = Convert.ToInt32(parameters.Parameters[5]);
            if (i != 0) dt = dt.AddSeconds(i);
            i = Convert.ToInt32(parameters.Parameters[6]);
            if (i != 0) dt = dt.AddMilliseconds(i);

            result.Value = dt;
            return true;
        }

        protected void InitStandardOperations()
        {
            // class operator
            AddOperation(new AleOperation(".", 20, AleOperation.OPERATOR_YFX, AleOperationType.MemberAccess)
            {
                Evaluator = BuiltinClassOperation
            });

            // assign operator
            AddOperation(new AleOperation(":=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.Assignment)
            {
                Evaluator = BuiltinAssignOperation
            });

            // OR operator
            AddOperation(new AleOperation(IgnoreCase ? "OR" : "or", 9200, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinORShortCircuitOperation
            });

            // AND operator
            AddOperation(new AleOperation(IgnoreCase ? "AND" : "and", 9200, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinANDShortCircuitOperation
            });

            // XOR operator
            AddOperation(new AleOperation(IgnoreCase ? "XOR" : "xor", 9100, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinXOROperation
            });

            // NOT operator
            AddOperation(new AleOperation(IgnoreCase ? "NOT" : "not", 1000, AleOperation.OPERATOR_FX)
            {
                Evaluator = BuiltinNOTOperation
            });

            // EQUAL "=" operator
            AddOperation(new AleOperation("=", 7500, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinEqualOperation
            });

            // NOT EQUAL "<>" operator
            AddOperation(new AleOperation("<>", 7500, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinNotEqualOperation
            });

            // GT ">" operator
            AddOperation(new AleOperation(">", 7000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinGTOperation
            });

            // GTE ">=" operator
            AddOperation(new AleOperation(">=", 7000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinGTEOperation
            });

            // LT "<" operator
            AddOperation(new AleOperation("<", 7000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinLTOperation
            });

            // LTE "<=" operator
            AddOperation(new AleOperation("<=", 7000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinLTEOperation
            });

            // Shift left "shl" operator
            AddOperation(new AleOperation(IgnoreCase ? "SHL" : "shl", 4000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinSHLOperation
            });

            // Shift left "shr" operator
            AddOperation(new AleOperation(IgnoreCase ? "SHR" : "shr", 4000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinSHROperation
            });

            // "+" operator
            AddOperation(new AleOperation("+", 5000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinAddOperation
            });

            // "-" operator
            AddOperation(new AleOperation("-", 5000, AleOperation.OPERATOR_YFX + AleOperation.OPERATOR_FX)
            {
                Evaluator = BuiltinSubstractOperation
            });

            // "*" operator
            AddOperation(new AleOperation("*", 4000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinMulOperation
            });

            // "/" operator
            AddOperation(new AleOperation("/", 4000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinDivOperation
            });

            // Remainder "mod" operator
            AddOperation(new AleOperation(IgnoreCase ? "MOD" : "mod", 3000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinRemOperation
            });

        }

        protected void InitCLikeOperations()
        {
            // class operator
            AddOperation(new AleOperation(".", 20, AleOperation.OPERATOR_YFX, AleOperationType.MemberAccess)
            {
                Evaluator = BuiltinClassOperation
            });

            // assign operator
            AddOperation(new AleOperation("=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.Assignment)
            {
                Evaluator = BuiltinAssignOperation
            });

            // assign "+=" operator
            AddOperation(new AleOperation("+=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignINCOperation
            });

            // assign "-=" operator
            AddOperation(new AleOperation("-=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignDECOperation
            });

            // assign "*=" operator
            AddOperation(new AleOperation("*=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignMULOperation
            });

            // assign "/=" operator
            AddOperation(new AleOperation("/=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignDIVOperation
            });

            // assign "%=" operator
            AddOperation(new AleOperation("%=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignREMOperation
            });

            // assign "&=" operator
            AddOperation(new AleOperation("&=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignANDOperation
            });

            // assign "|=" operator
            AddOperation(new AleOperation("|=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignOROperation
            });

            // assign "^=" operator
            AddOperation(new AleOperation("^=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignXOROperation
            });

            // assign ">>=" operator
            AddOperation(new AleOperation(">>=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignSHROperation
            });

            // assign "<<=" operator
            AddOperation(new AleOperation("<<=", 10000, AleOperation.OPERATOR_XFY, AleOperationType.CompoundAssignment)
            {
                Evaluator = BuiltinAssignSHLOperation
            });

            // inc "++" operator
            AddOperation(new AleOperation("++", 1000, AleOperation.OPERATOR_FX+AleOperation.OPERATOR_YF, AleOperationType.Increment)
            {
                Evaluator = BuiltinINCOperation
            });

            // dec "--" operator
            AddOperation(new AleOperation("--", 1000, AleOperation.OPERATOR_FX + AleOperation.OPERATOR_YF, AleOperationType.Increment)
            {
                Evaluator = BuiltinDECOperation
            });

            // null coalescing "??" operator
            AddOperation(new AleOperation("??", 9500, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinNullCoalescingOperation
            });

            // or "||" operator
            AddOperation(new AleOperation("||", 9200, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinBooleanOROperation
            });

            // and "&&" operator
            AddOperation(new AleOperation("&&", 9000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinBooleanANDOperation
            });

            // not "!" operator
            AddOperation(new AleOperation("!", 1000, AleOperation.OPERATOR_FX)
            {
                Evaluator = BuiltinBooleanNOTOperation
            });

            // or "|" operator
            AddOperation(new AleOperation("|", 8200, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinOROperation
            });

            // and "&" operator
            AddOperation(new AleOperation("&", 8000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinANDOperation
            });

            // xor "^" operator
            AddOperation(new AleOperation("^", 8100, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinXOROperation
            });

            // not "~" operator
            AddOperation(new AleOperation("~", 1000, AleOperation.OPERATOR_FX)
            {
                Evaluator = BuiltinNOTOperation
            });

            // EQUAL "==" operator
            AddOperation(new AleOperation("==", 7500, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinEqualOperation
            });

            // NOT EQUAL "!=" operator
            AddOperation(new AleOperation("!=", 7500, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinNotEqualOperation
            });

            // GT ">" operator
            AddOperation(new AleOperation(">", 7000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinGTOperation
            });

            // GTE ">=" operator
            AddOperation(new AleOperation(">=", 7000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinGTEOperation
            });

            // LT "<" operator
            AddOperation(new AleOperation("<", 7000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinLTOperation
            });

            // LTE "<=" operator
            AddOperation(new AleOperation("<=", 7000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinLTEOperation
            });

            // Shift left "shl" operator
            AddOperation(new AleOperation("<<", 6000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinSHLOperation
            });

            // Shift left "shr" operator
            AddOperation(new AleOperation(">>", 6000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinSHROperation
            });

            // "+" operator
            AddOperation(new AleOperation("+", 5000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinAddOperation
            });

            // "-" operator
            AddOperation(new AleOperation("-", 5000, AleOperation.OPERATOR_YFX + AleOperation.OPERATOR_FX)
            {
                Evaluator = BuiltinSubstractOperation
            });

            // "*" operator
            AddOperation(new AleOperation("*", 4000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinMulOperation
            });

            // "/" operator
            AddOperation(new AleOperation("/", 4000, AleOperation.OPERATOR_YFX)
            {
                Evaluator = BuiltinDivOperation
            });

            // Remainder "%" operator
            AddOperation(new AleOperation("%", 4000, AleOperation.OPERATOR_XFY)
            {
                Evaluator = BuiltinRemOperation
            });

        }

        public void InitOperations(int opset, bool addStdFunctions = true, bool clear = true)
        {
            if (_Operations != null && clear) _Operations.Clear();

            switch (opset)
            {
                case OPERATIONS_STANDARDSET:
                    InitStandardOperations();
                    break;
                case OPERATIONS_CLIKESET:
                    InitCLikeOperations();
                    break;
            }

            if (addStdFunctions)
            {
                AddOperation(new AleOperation(IgnoreCase ? "SIN" : "Sin")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinSinFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "COS" : "Cos")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinCosFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "TAN" : "Tan")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinTanFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "SINH" : "Sinh")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinSinhFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "COSH" : "Cosh")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinCoshFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "TANH" : "Tanh")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinTanhFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "ASIN" : "Asin")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinASinFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "ACOS" : "Acos")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinACosFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "ATAN" : "Atan")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinATanFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "EXP" : "Exp")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinExpFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "LOG" : "Log")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinLogFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "LOG10" : "Log10")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinLog10Function
                });

                AddOperation(new AleOperation(IgnoreCase ? "SQRT" : "Sqrt")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinSqrtFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "POW" : "Pow")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null), new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinPowFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "ABS" : "Abs")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinAbsFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "CEILING" : "Ceiling")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinCeilingFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "FLOOR" : "Floor")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinFloorFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "ROUND" : "Round")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null), new Tuple<TypeCode, object>(TypeCode.Int32, 0) },
                    Evaluator = BuiltinRoundFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "TRUNCATE" : "Truncate")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Double, null) },
                    Evaluator = BuiltinTruncateFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "TOSTRING" : "ToString")
                {
                    InstanceTypeCode = TypeCode.Object,
                    Evaluator = BuiltinToStringMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "TOSTRING" : "ToString")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.String, null) },
                    InstanceTypeCode=TypeCode.Object,
                    Evaluator = BuiltinToStringFmtMethod
                });

                /*
                 * 
                 * String functions and methods
                 * 
                 */
                AddOperation(new AleOperation(IgnoreCase ? "STRING" : "String")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.String, null), new Tuple<TypeCode, object>(TypeCode.Int32, null) },
                    Evaluator = BuiltinStringFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "COMPARE" : "Compare")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.String, null), new Tuple<TypeCode, object>(TypeCode.String, null), new Tuple<TypeCode, object>(TypeCode.Boolean, false) },
                    Evaluator = BuiltinCompareFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "INDEXOF" : "IndexOf")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.String, null) },
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinIndexOfMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "INDEXOF" : "IndexOf")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.String, null), new Tuple<TypeCode, object>(TypeCode.Int32, null) },
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinIndexOf2ParamsMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "SUBSTRING" : "Substring")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Int32, null) },
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinSubstringMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "SUBSTRING" : "Substring")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Int32, null), new Tuple<TypeCode, object>(TypeCode.Int32, null) },
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinSubstring2ParamsMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "REPLACE" : "Replace")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.String, null), new Tuple<TypeCode, object>(TypeCode.String, null) },
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinReplaceMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "TOUPPER" : "ToUpper")
                {
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinToUpperMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "TOLOWER" : "ToLower")
                {
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinToLowerMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "TRIM" : "Trim")
                {
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinTrimMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "TRIMSTART" : "TrimStart")
                {
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinTrimStartMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "TRIMEND" : "TrimEnd")
                {
                    InstanceTypeCode = TypeCode.String,
                    Evaluator = BuiltinTrimEndMethod
                });

                AddOperation(new AleOperation(IgnoreCase ? "CHR" : "chr")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Int16, null) },
                    Evaluator = BuiltinChrFunction
                });

                AddOperation(new AleOperation(IgnoreCase ? "ORD" : "ord")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.String, null) },
                    Evaluator = BuiltinOrdFunction
                });
                
                
                /*
                 * 
                 * DateTime operations
                 * 
                 */

                // Now() returns current date and time
                AddOperation(new AleOperation(IgnoreCase ? "NOW" : "Now")
                {
                    Evaluator = BuiltinNowFunction
                });

                // DaysInMonth() returns the number of days in the specified month and year
                AddOperation(new AleOperation(IgnoreCase ? "DAYSINMONTH" : "DaysInMonth")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Int32, null), new Tuple<TypeCode, object>(TypeCode.Int32, null) },
                    Evaluator = BuiltinDaysInMonthFunction
                });

                // DateTime(Year, Month, Day, Hours, Minutes, Seconds, Milliseconds) creates datetime value
                AddOperation(new AleOperation(IgnoreCase ? "DATETIME" : "DateTime")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Int32, null), new Tuple<TypeCode, object>(TypeCode.Int32, null),
                                                                       new Tuple<TypeCode, object>(TypeCode.Int32, null), new Tuple<TypeCode, object>(TypeCode.Int32, (int)0),
                                                                       new Tuple<TypeCode, object>(TypeCode.Int32, (int)0), new Tuple<TypeCode, object>(TypeCode.Int32, (int)0),
                                                                       new Tuple<TypeCode, object>(TypeCode.Int32, (int)0) },
                    Evaluator = BuiltinDateTimeFunction
                });

                // DateTime(ticks) creates datetime value
                AddOperation(new AleOperation(IgnoreCase ? "DATETIME" : "DateTime")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Int64, null) },
                    Evaluator = BuiltinDateTimeTicksFunction
                });

                // method Month returns month part of DateTime
                AddOperation(new AleOperation(IgnoreCase ? "ADD" : "Add")
                {
                    Parameters = new List<Tuple<TypeCode, object>>() { new Tuple<TypeCode, object>(TypeCode.Int32, (int)0), new Tuple<TypeCode, object>(TypeCode.Int32, (int)0),
                                                                       new Tuple<TypeCode, object>(TypeCode.Int32, (int)0), new Tuple<TypeCode, object>(TypeCode.Int32, (int)0),
                                                                       new Tuple<TypeCode, object>(TypeCode.Int32, (int)0), new Tuple<TypeCode, object>(TypeCode.Int32, (int)0),
                                                                       new Tuple<TypeCode, object>(TypeCode.Int32, (int)0) },
                    InstanceTypeCode = TypeCode.DateTime,
                    Evaluator = BuiltinDateAddMethod
                });

            }
        }

        public bool AddOperation(AleOperation operation)
        {
            if (operation == null) return false;

            if (_Operations == null) _Operations = new List<AleOperation>();

            StringComparison comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            int n = _Operations.Count;

            // class and index operator or index/initlist/objectconst/keyvalue/propertyvalue operation can be only one
            AleOperationType[] singleTypes = { AleOperationType.MemberAccess, AleOperationType.ElementAccess, AleOperationType.Index, AleOperationType.InitList,
                                                 AleOperationType.ObjectConst, AleOperationType.KeyValue, AleOperationType.PropertyValue};
            bool ofSingleType = Array.IndexOf(singleTypes, operation.OperationType) >= 0;

            for (int i = 0; i < n; i++)
                if (ofSingleType && Array.IndexOf(singleTypes, _Operations[i].OperationType) >= 0) return false;

            if (operation.IsOperator)
            {
                int l = operation.Name.Length;

                for (int i = 0; i < n; i++)
                {
                    if (_Operations[i].IsOperator)
                    {
                        if (_Operations[i].Name.Length < l)
                        {
                            _Operations.Insert(i, operation);
                            return true;
                        }
                        else if (String.Compare(Operations[i].Name, operation.Name, comparison) == 0)
                        {
                            _Operations[i] = operation;
                            return true;
                        }
                    }
                    else
                    {
                        _Operations.Insert(i, operation);
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                    if (Operations[i].IsClassMethod == operation.IsClassMethod && operation.ParametersCount == Operations[i].ParametersCount &&
                        String.Compare(Operations[i].Name, operation.Name, comparison) == 0)
                    {
                        Operations[i] = operation;
                        return true;
                    }
            }

            _Operations.Add(operation);
            return true;
        }

        public void RemoveOperation(string name, int paramCount, bool isClassMethod = false)
        {
            StringComparison comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            int n = 0;
            if (_Operations != null) n = _Operations.Count;

            for (int i = 0; i < n; i++)
                if (((paramCount < 0 && Operations[i].IsOperator) ||
                     (Operations[i].ParametersCount == paramCount && Operations[i].IsClassMethod == isClassMethod)) &&
                    String.Compare(Operations[i].Name, name, comparison) == 0)
                {
                    Operations.RemoveAt(i);
                    n--;
                }
        }

        public void EvaluateStandardProperties(AleTerm term, AleTermEvaluateArgs e)
        {
            if (e.Instance == null) return;

            TypeCode typ = Type.GetTypeCode(e.Instance.GetType());

            switch (typ)
            {
                case TypeCode.String:
                    if ((e.NameHash == 1212501634 && !IgnoreCase && e.Name == "Length") || (e.NameHash == 1677114114 && IgnoreCase && e.Name == "LENGTH")) e.Result = e.Instance.ToString().Length;
                    break;

                case TypeCode.DateTime:
                    DateTime dt = Convert.ToDateTime(e.Instance);

                    if ((e.NameHash == 1272578850 && !IgnoreCase && e.Name == "Date") || (e.NameHash == 1672179650 && IgnoreCase && e.Name == "DATE")) e.Result = dt.Date;
                    else if ((e.NameHash == -422187236 && !IgnoreCase && e.Name == "Year") || (e.NameHash == -22586436 && IgnoreCase && e.Name == "YEAR")) e.Result = dt.Year;
                    else if ((e.NameHash == -1590538603 && !IgnoreCase && e.Name == "Month") || (e.NameHash == -1123828907 && IgnoreCase && e.Name == "MONTH")) e.Result = dt.Month;
                    else if ((e.NameHash == -1694433159 && !IgnoreCase && e.Name == "Day") || (e.NameHash == -1177709849 && IgnoreCase && e.Name == "DAY")) e.Result = dt.Day;
                    else if ((e.NameHash == 1191344073 && !IgnoreCase && e.Name == "Hour") || (e.NameHash == 1590944873 && IgnoreCase && e.Name == "HOUR")) e.Result = dt.Hour;
                    else if ((e.NameHash == 380522145 && !IgnoreCase && e.Name == "Minute") || (e.NameHash == 840940385 && IgnoreCase && e.Name == "MINUTE")) e.Result = dt.Minute;
                    else if ((e.NameHash == -792816040 && !IgnoreCase && e.Name == "Second") || (e.NameHash == -328203496 && IgnoreCase && e.Name == "SECOND")) e.Result = dt.Second;
                    else if ((e.NameHash == 1156562306 && !IgnoreCase && e.Name == "Millisecond") || (e.NameHash == -1069636026 && IgnoreCase && e.Name == "MILLISECOND")) e.Result = dt.Millisecond;
                    else if ((e.NameHash == -8743282 && !IgnoreCase && e.Name == "Ticks") || (e.NameHash == 457966350 && IgnoreCase && e.Name == "TICKS")) e.Result = dt.Ticks;

                    break;

                case TypeCode.Object:
                    if (e.Instance is Dictionary<object,object>)
                    {
                        Dictionary<object,object> array = e.Instance as Dictionary<object,object>;
                        if ((e.NameHash == 1212501634 && !IgnoreCase && e.Name == "Length") || (e.NameHash == 1677114114 && IgnoreCase && e.Name == "LENGTH")) e.Result = array.Count;
                    }
                    break;

            }

        }
    }

}