using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace YisinTick
{
    public class DynamicConsoleWriter : DynamicObject
    {
        protected string first = "";
        protected string last = "";
        public int Count
        {
            get
            {
                return 2;
            }
        }
        public override bool TryBinaryOperation(BinaryOperationBinder binder,
                             object arg, out object result)
        {
            bool success = false;
            if (binder.Operation == System.Linq.Expressions.ExpressionType.Add)
            {
                Console.WriteLine("I have to think about that");
                success = true;
            }
            result = this;
            return success;
        }
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            bool success = false;
            if (binder.Operation == System.Linq.Expressions.ExpressionType.Increment)
            {
                Console.WriteLine("I will do it later");
                success = true;
            }
            result = this;
            return success;
        }
        public override bool TryGetIndex(GetIndexBinder binder,
                        object[] indexes, out object result)
        {
            result = null;
            if ((int)indexes[0] == 0)
            {
                result = first;
            }
            else if ((int)indexes[0] == 1)
            {
                result = last;
            }
            return true;
        }
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if ((int)indexes[0] == 0)
            {
                first = (string)value;
            }
            else if ((int)indexes[0] == 1)
            {
                last = (string)value;
            }
            return true;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            bool success = false;
            result = null;
            if (name == "last")
            {
                result = last;
                success = true;
            }
            else if (name == "first")
            {
                result = first;
                success = true;
            }
            return success;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string name = binder.Name.ToLower();
            bool success = false;
            if (name == "last")
            {
                last = (string)value;
                success = true;
            }
            else if (name == "first")
            {
                first = (string)value;
                success = true;
            }
            return success;
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder,
                        object[] args, out object result)
        {
            string name = binder.Name.ToLower();
            bool success = false;
            result = true;
            if (name == "writelast")
            {
                Console.WriteLine(last);
                success = true;
            }
            else if (name == "writefirst")
            {
                Console.WriteLine(first);
                success = true;
            }
            return success;
        }
    }
}
