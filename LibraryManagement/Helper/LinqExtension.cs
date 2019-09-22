
using CodeBureau;
using Seagull.Helpers.WhereOperation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

/*
  Some helpful Linq extensions I've used with Unity.
  Original code from stackoverflow.com where user contributions are
  licensed under CC-BY-SA 3.0 with attribution required.
*/
namespace Seagull.Helpers.WhereOperation
{
    public enum WhereOperation
    {
        [StringValue("eq")]
        Equal,
        [StringValue("nq")]
        NotEqual,
        [StringValue("cn")]
        Contains,
        [StringValue("in")]
        In,
        [StringValue("gr")]
        greater,
        [StringValue("ls")]
        less
    }
}
namespace ExtensionMethods
{
    public static class LinqExtensions
    {
        /// <summary>Orders the sequence by specific column and direction.</summary>
        /// <param name="query">The query.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="ascending">if set to true [ascending].</param>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string sortColumn, string direction)
        {
            try
            {
                sortColumn = string.IsNullOrEmpty(sortColumn) ? "Id" : sortColumn;
                string methodName = string.Format("OrderBy{0}",
                    direction.ToLower() == "asc" ? "" : "descending");

                ParameterExpression parameter = Expression.Parameter(query.ElementType, "p");

                MemberExpression memberAccess = null;
                foreach (var property in sortColumn.Split('.'))
                    memberAccess = MemberExpression.Property
                       (memberAccess ?? (parameter as Expression), property);

                LambdaExpression orderByLambda = Expression.Lambda(memberAccess, parameter);

                MethodCallExpression result = Expression.Call(
                          typeof(Queryable),
                          methodName,
                          new[] { query.ElementType, memberAccess.Type },
                          query.Expression,
                          Expression.Quote(orderByLambda));

                return query.Provider.CreateQuery<T>(result);
            }
            catch (Exception e)
            {
                sortColumn = "Id";
                string methodName = string.Format("OrderBy{0}",
                    direction.ToLower() == "asc" ? "" : "descending");

                ParameterExpression parameter = Expression.Parameter(query.ElementType, "p");

                MemberExpression memberAccess = null;
                foreach (var property in sortColumn.Split('.'))
                    memberAccess = MemberExpression.Property
                       (memberAccess ?? (parameter as Expression), property);

                LambdaExpression orderByLambda = Expression.Lambda(memberAccess, parameter);

                MethodCallExpression result = Expression.Call(
                          typeof(Queryable),
                          methodName,
                          new[] { query.ElementType, memberAccess.Type },
                          query.Expression,
                          Expression.Quote(orderByLambda));

                return query.Provider.CreateQuery<T>(result);
            }
        }


        public static IQueryable<T> Where<T>(this IQueryable<T> query,
            object column, object value, WhereOperation operation)
        {
            if (string.IsNullOrEmpty(column.ToString()))
                return query;

            ParameterExpression parameter = Expression.Parameter(query.ElementType, "p"); //parameter.Type=Types

            MemberExpression memberAccess = null;
            foreach (var property in column.ToString().Split('.'))
                memberAccess = MemberExpression.Property
                   (memberAccess ?? (parameter as Expression), property);

            //change param value type
            //necessary to getting bool from string
            ConstantExpression filter = null;
            if (value.ToString().Split(',').Count() > 1)
            {
                foreach (var item in value.ToString().Split(','))
                {
                    Type t = Type.GetType(memberAccess.Type.ToString());
                    if (operation == WhereOperation.greater || operation == WhereOperation.less)
                    {
                        filter = Expression.Constant(DateTime.Parse(item.ToString()), typeof(DateTime?));
                    }
                    else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        if (String.IsNullOrEmpty(item.ToString()))
                            filter = null;
                        else
                            switch (t.GetGenericArguments()[0].Name)
                            {
                                case "DateTime":
                                    DateTime? _date = DateTime.Parse(item.ToString());
                                    filter = Expression.Constant(_date);
                                    break;
                                case "Int32":
                                    int? _int = int.Parse(item.ToString());
                                    filter = Expression.Constant(_int);
                                    break;
                            }
                    }
                    else

                        filter = Expression.Constant(Convert.ChangeType(item, t));
                }
            }
            else
            {
                try
                {
                    Type t = Type.GetType(memberAccess.Type.ToString());
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        if (String.IsNullOrEmpty(value.ToString()))
                            filter = null;
                        else
                            switch (t.GetGenericArguments()[0].Name)
                            {
                                case "DateTime":
                                    DateTime? _date = DateTime.Parse(value.ToString());
                                    filter = Expression.Constant(_date);
                                    break;
                                case "Int32":
                                    int? _int = int.Parse(value.ToString());
                                    filter = Expression.Constant(_int);
                                    break;
                                case "Boolean":
                                    bool? _bool = Boolean.Parse(value.ToString());
                                    filter = Expression.Constant(_bool);
                                    break;
                            }
                    }
                    else if (operation == WhereOperation.greater || operation == WhereOperation.less)
                    {
                        filter = Expression.Constant(DateTime.Parse(value.ToString()) , typeof(DateTime?));
                    }
                    
                    else

                        filter = Expression.Constant(Convert.ChangeType(value.ToString(), t));
                }
                catch (Exception e)
                {

                }
                

            }
            //switch operation
            Expression condition = null;
            LambdaExpression lambda = null;
            try
            {
                switch (operation)
                {
                    //equal ==
                    case WhereOperation.Equal:
                        condition = Expression.Equal(memberAccess, Expression.Convert(filter, memberAccess.Type));
                        lambda = Expression.Lambda(condition, parameter);
                        break;
                    //not equal !=
                    case WhereOperation.NotEqual:
                        condition = Expression.NotEqual(memberAccess, Expression.Convert(filter, memberAccess.Type)); //Expression.NotEqual(memberAccess, filter);
                        lambda = Expression.Lambda(condition, parameter);
                        break;
                    //string.Contains()
                    case WhereOperation.Contains:
                    case WhereOperation.In:
                        condition = Expression.Call(memberAccess,
                            typeof(string).GetMethod("Contains"),
                            Expression.Constant(value.ToString()));
                        lambda = Expression.Lambda(condition, parameter);
                        break;
                    case WhereOperation.greater:
                      // condition = Expression.GreaterThanOrEqual(memberAccess, filter);
                       condition = Expression.GreaterThanOrEqual(memberAccess, Expression.Convert(filter, memberAccess.Type)); 
                        lambda = Expression.Lambda(condition, parameter);
                        break;
                    case WhereOperation.less:
                       // condition = Expression.LessThanOrEqual(memberAccess, filter);
                         condition = Expression.LessThanOrEqual(memberAccess, Expression.Convert(filter, memberAccess.Type));
                        lambda = Expression.Lambda(condition, parameter);
                        break;
                }
            }
            catch (Exception e) { }


            MethodCallExpression result = Expression.Call(
                   typeof(Queryable), "Where",
                   new[] { query.ElementType },
                   query.Expression,
                   lambda);

            return query.Provider.CreateQuery<T>(result);
        }
        public class InnerExtension
        {
            public ConstantExpression GetValue(object value, Type info)
            {
                ConstantExpression filter = null;
               
                return filter;
            }
        }
    }
   
}