﻿using SimplyTyped.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SimplyTyped.Query
{
    internal class ExpressionParser
    {
        private PrimitiveAttributeSerializer _serializer = new PrimitiveAttributeSerializer();
        private static readonly Dictionary<ExpressionType, string> OperationMap = new Dictionary<ExpressionType, string>
        {
            [ExpressionType.Equal] = "=",
            [ExpressionType.NotEqual] = "!=",
            [ExpressionType.AndAlso] = "AND",
            [ExpressionType.OrElse] = "OR",
            [ExpressionType.GreaterThan] = ">",
            [ExpressionType.GreaterThanOrEqual] = ">=",
            [ExpressionType.LessThan] = "<",
            [ExpressionType.LessThanOrEqual] = "<="
        };

        internal string ParseBinaryExpression(BinaryExpression exp)
        {
            return ParseExpression(exp);
        }
        internal string ExtractMemberName(MemberExpression exp) => exp.Member.Name;

        private string ParseExpression(BinaryExpression exp)
        {
            var operation = exp.NodeType;

            if (!OperationMap.ContainsKey(operation))
                throw new NotSupportedException($"Operation {operation} is not supported");

            var leftStr = ParseExpression((dynamic)exp.Left);
            var rightStr = ParseExpression((dynamic)exp.Right);

            return $"({leftStr} {OperationMap[operation]} {rightStr})";
        }
        private string ParseExpression(MemberExpression exp)
        {
            var memberName = ExtractMemberName(exp);
            return $"`{memberName}`";
        }
        private string ParseExpression(ConstantExpression exp)
        {
            return $"'{QueryEncodingUtility.EncodeValue(_serializer.Serialize(exp.Value))}'";
        }
        private string ParseExpression(Expression exp) //unknown
        {
            throw new NotSupportedException($"Expression of type {exp.GetType().Name} are not supported");
        }
    }
}
