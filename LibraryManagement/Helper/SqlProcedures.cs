using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Seagull.Core.Helper
{
    public static class SqlProcedures
    {
        #region Procedures
        //This Method Used To Map Data Dynamicly (Reflection)
        private static List<T> MapToList<T>(this DbDataReader dr)
        {
            var objList = new List<T>();
            var props = typeof(T).GetRuntimeProperties();
            //will return true while there is data to be read.
            while (dr.Read())
            {
                var colMapping = dr.GetColumnSchema()
                              .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                              .ToDictionary(key => key.ColumnName.ToLower());

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        T obj = Activator.CreateInstance<T>();
                        foreach (var prop in props)
                        {
                            var val =
                              dr.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                            prop.SetValue(obj, val == DBNull.Value ? null : val);
                        }
                        objList.Add(obj);
                    }
                }
            }
            return objList;
        }

        //This Method Used Execute Stored Procedure
        public static List<T> ExecuteStoredProcAsync<T>(this DbContext _context, string storedProcName, List<ProcedureParameter> Parameters)
        {
            #region Call Procedure First
            DbCommand command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = storedProcName;
            command.CommandType = System.Data.CommandType.StoredProcedure;
            #endregion

            #region Prepare Procedure Parameters
            foreach (var y in Parameters)
            {
                if (string.IsNullOrEmpty(command.CommandText))
                    throw new InvalidOperationException(
                      "Call LoadStoredProc before using this method");
                var param = command.CreateParameter();
                param.ParameterName = y.paramName;
                param.Value = y.paramValue;
                command.Parameters.Add(param);
            }
            #endregion

            #region execute the command
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        try
                        {
                            return reader.MapToList<T>();
                        }
                        catch (Exception e)
                        {
                            reader.Close();
                            throw (e);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
            #endregion
        }
        #endregion

    }
    public class ProcedureParameter
    {
        public string paramName { get; set; }
        public object paramValue { get; set; }
    }
}
