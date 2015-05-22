using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;


namespace AleProjects.AleLexer.AleParser
{

    public class AleOperationToken : AleToken
    {
        private AleOperation _Operation;
        private uint _Associativity;

        public AleOperationToken() : base() { }

        public AleOperationToken(AleTokenType type, int start, int length, AleOperation op, AleSimpleLexer lexer = null)
            : base(type, start, length, lexer)
        {
            _Associativity = 0;
            _Operation = op;
        }

        public AleOperation Operation
        {
            get { return _Operation; }
            set { _Operation = value; }
        }

        public uint Associativity
        {
            get { return _Associativity; }
            set { _Associativity = value; }
        }

        public bool IsBinaryOperator
        {
            get
            {
                return (_Associativity & AleOperation.OPERATOR_BINARY) != 0;
            }
        }

        public bool IsPrefixOperator
        {
            get
            {
                return (_Associativity & AleOperation.OPERATOR_PREFIX) != 0;
            }
        }

        public bool IsPostfixOperator
        {
            get
            {
                return (_Associativity & AleOperation.OPERATOR_POSTFIX) != 0;
            }
        }
    
    }
}
