using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using AleProjects.AleLexer.AleParser;
using AleProjects.AleLexer;


namespace SIFCA_BLL
{
    public class ExpressionEvaluator
    {
        public AleExpressionParser AP = new AleExpressionParser();

        Random rnd = new Random();
        Dictionary<string, object> Variables;
        private bool ingnoreCase=false;
        private bool strictSyntax=false;

        public ExpressionEvaluator(bool ignoreCase, bool strictSyntax)
        {
            AP.InitOperations(AleExpressionParser.OPERATIONS_CLIKESET);
            AP.IgnoreCase = ignoreCase;
            this.ingnoreCase = ignoreCase;
            this.strictSyntax = strictSyntax;
            Variables = new Dictionary<string, object>(AP.IgnoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture);
            Variables.Add("S", "AleExpressionParser 1.0 beta");
        }


        private void OnSemanticsValidate(object sender, SemanticsValidateEventArgs e)
        {
            bool ignoreCase = AP.IgnoreCase;
            AleExpressionParser P = sender as AleExpressionParser;

            // We want for some reasons that "Rnd" can't be a variable name
            if ((e.Term.HashCode == -1028829980 && !ignoreCase && e.Term.Value.ToString() == "Rnd") ||
                (e.Term.HashCode == 1843313028 && ignoreCase && e.Term.Value.ToString() == "RND"))
            {
                AleTerm parent = e.Term.Parent;
                if (e.Term.TypeOfTerm == AleTermType.Variable && (parent == null || parent.Operation == null || !parent.Operation.IsClassOperator || e.Term != parent[1]))
                    P.SetError(e.Term.Token.StartInOrigin, AleExpressionParser.ERROR_INVALIDVARIABLE);
            }
        }

        // evaluation
        private void OnEvaluate(AleTerm term, AleTermEvaluateArgs e)
        {
            bool ignoreCase = AP.IgnoreCase;
            object res;

            if (e.Instance == null)
            {
                if (Variables.TryGetValue(e.Name, out res)) e.Result = res;
                else
                {
                    res = (int)0;
                    Variables.Add(e.Name, res);
                    e.Result = res;
                }
                return;
            }
        }

        // assignment
        private void OnAssign(AleTerm term, AleTermAssignArgs e)
        {
            bool ignoreCase = AP.IgnoreCase;

            if (e.Instance == null)
            {
                Variables[e.Name] = e.Value;
                return;
            }
            e.SetError(AleTermResult.ERROR_UNKNOWNELEMENT, term.Token.StartInOrigin);
        }

        private string replaceVarOnExpression(string expression,Dictionary<string, double> valores)
        {
            string expresionWithValues = expression;
            foreach (KeyValuePair<string,double> valor in valores)
            {
                expresionWithValues = expresionWithValues.Replace(valor.Key, (valor.Value > 0 ? valor.Value.ToString().Replace(",", ".") : "("+valor.Value.ToString().Replace(",", ".")+")"));
            }
            return expresionWithValues;
        }

        public AleTermResult EvaluateExpression(string expression,Dictionary<string,double> valores, out int result)
        {
            List<AleToken> list;
            AleTerm T = null;
            
            expression = replaceVarOnExpression(expression, valores);

            AP.Options = (AP.IgnoreCase ? AleExpressionParser.OPTION_IGNORECASE : 0) +
                AleExpressionParser.OPTION_ALLOWEMPTYLISTMEMBER +
                (strictSyntax ? AleExpressionParser.OPTION_STRICTSYNTAX : 0) +
                AleExpressionParser.OPTION_ALLOWEMPTYPARAMS +
                AleExpressionParser.OPTION_ALLOWEMPTYINDEX +
                AleExpressionParser.OPTION_ALLOWMULTIDIMINDEXES +
                AleExpressionParser.OPTION_STRICTINDEXES;

            AP.Text = expression;
            AP.VarPrefix ='\0';
            AP.EndOfExpression = "; 'Fin de la expresion' 'Parar aqui'";

            AP.Constants = new Dictionary<string, object>(AP.IgnoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture);
            AP.Constants.Add("true", true);
            AP.Constants.Add("false", false);
            AP.Constants.Add("PI", Math.PI);
            AP.Constants.Add("π", Math.PI);
            AP.Constants.Add("null", null);

            AP.SemanticsValidate += OnSemanticsValidate;

            int res = 0;
            result = 0;
            AleTermResult val=new AleTermResult();

            while (res < AP.Text.Length)
            {
                res = AP.Tokenize(out list, res);

                if (AP.ErrorCode == AleExpressionParser.ERROR_OK) T = AP.Parse(list);

                if (AP.ErrorCode != AleExpressionParser.ERROR_OK)
                {
                    result = 1;
                    break;
                }

                if (T != null)
                {
                    if (!T.Evaluate(out val, OnEvaluate, OnAssign))
                    {
                        result = 2;
                        return val;
                    }
                    else if (val.Value != null)
                    {
                        result = 3;
                        return val;
                    }
                }
            }
            return val;
        }
    }
}
