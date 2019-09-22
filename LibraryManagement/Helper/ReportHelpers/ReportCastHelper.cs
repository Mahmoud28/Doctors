using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Seagull.Core.Helper
{
    public static class ReportCastHelper
    {
        #region Enum
        public static Type BaseType(Type oType)
        {
            //#### If the passed oType is valid, .IsValueType and is logicially nullable, .Get(its)UnderlyingType
            if (oType != null &&
                oType.IsValueType &&
                oType.IsGenericType &&
                oType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(oType);
            }

            //#### Else the passed oType was null or was not logicially nullable, so simply return the passed oType
            return oType;
        }

        // Method to Execute Query
        public static IEnumerable<dynamic> Execute()
        {
            var result = new IDataRecord[] { new DummyRecord(0), new DummyRecord(1), new DummyRecord(2) };
            foreach (var record in result)
                yield return new DataRecordDynamicWrapper(record);
        }

        public class DataRecordDynamicWrapper : DynamicObject, ICustomTypeDescriptor
        {
            private readonly IDataRecord _dataRecord;
            private PropertyDescriptorCollection _properties;

            public DataRecordDynamicWrapper(IDataRecord dataRecord)
            {
                _dataRecord = dataRecord;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = _dataRecord[binder.Name];
                return result != null;
            }

            AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
                return AttributeCollection.Empty;
            }

            string ICustomTypeDescriptor.GetClassName()
            {
                return _dataRecord.GetType().Name;
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return _dataRecord.GetType().Name;
            }

            TypeConverter ICustomTypeDescriptor.GetConverter()
            {
                return new TypeConverter();
            }

            EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
                return null;
            }

            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
                return null;
            }

            object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
                throw new NotSupportedException();
            }

            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                return EventDescriptorCollection.Empty;
            }

            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
                return EventDescriptorCollection.Empty;
            }

            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
                if (_properties == null)
                    _properties = GenerateProperties();
                return _properties;
            }

            private PropertyDescriptorCollection GenerateProperties()
            {
                var count = _dataRecord.FieldCount;
                var properties = new PropertyDescriptor[count];

                for (var i = 0; i < count; i++)
                    properties[i] = new DataRecordProperty(i, _dataRecord.GetName(i), _dataRecord.GetFieldType(i));

                return new PropertyDescriptorCollection(properties);
            }

            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
                if (attributes != null && attributes.Length == 0)
                    return ((ICustomTypeDescriptor)this).GetProperties();
                return PropertyDescriptorCollection.Empty;
            }

            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
            {
                return _dataRecord;
            }

            private sealed class DataRecordProperty : PropertyDescriptor
            {
                private static readonly Attribute[] NoAttributes = new Attribute[0];

                private readonly int _ordinal;
                private readonly Type _type;

                public DataRecordProperty(int ordinal, string name, Type type)
                    : base(name, NoAttributes)
                {
                    _ordinal = ordinal;
                    _type = type;
                }

                public override bool CanResetValue(object component)
                {
                    return false;
                }

                public override object GetValue(object component)
                {
                    var wrapper = ((DataRecordDynamicWrapper)component);
                    return wrapper._dataRecord.GetValue(_ordinal);
                }

                public override void ResetValue(object component)
                {
                    throw new NotSupportedException();
                }

                public override void SetValue(object component, object value)
                {
                    throw new NotSupportedException();
                }

                public override bool ShouldSerializeValue(object component)
                {
                    return true;
                }

                public override Type ComponentType
                {
                    get { return typeof(IDataRecord); }
                }

                public override bool IsReadOnly
                {
                    get { return true; }
                }

                public override Type PropertyType
                {
                    get { return _type; }
                }
            }
        }

        internal sealed class DummyRecord : IDataRecord
        {
            private readonly int _id;

            public DummyRecord(int id)
            {
                _id = id;
            }

            public string GetName(int i)
            {
                return "Property" + i;
            }

            public string GetDataTypeName(int i)
            {
                return "String";
            }

            public Type GetFieldType(int i)
            {
                return typeof(string);
            }

            public object GetValue(int i)
            {
                return "Value_" + _id + "_" + i;
            }

            public int GetValues(object[] values)
            {
                return 3;
            }

            public int GetOrdinal(string name)
            {
                if (name.StartsWith("Property"))
                    return int.Parse(name.Remove(0, "Property".Length));
                return -1;
            }

            public bool GetBoolean(int i)
            {
                return false;
            }

            public byte GetByte(int i)
            {
                return default(byte);
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                throw new NotSupportedException();
            }

            public char GetChar(int i)
            {
                return default(char);
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                throw new NotSupportedException();
            }

            public Guid GetGuid(int i)
            {
                return default(Guid);
            }

            public short GetInt16(int i)
            {
                return default(short);
            }

            public int GetInt32(int i)
            {
                return default(int);
            }

            public long GetInt64(int i)
            {
                return default(long);
            }

            public float GetFloat(int i)
            {
                return default(float);
            }

            public double GetDouble(int i)
            {
                return default(double);
            }

            public string GetString(int i)
            {
                return (string)GetValue(i);
            }

            public decimal GetDecimal(int i)
            {
                return default(decimal);
            }

            public DateTime GetDateTime(int i)
            {
                return default(DateTime);
            }

            public IDataReader GetData(int i)
            {
                throw new NotSupportedException();
            }

            public bool IsDBNull(int i)
            {
                return false;
            }

            public int FieldCount
            {
                get { return 3; }
            }

            object IDataRecord.this[int i]
            {
                get { return GetValue(i); }
            }

            object IDataRecord.this[string name]
            {
                get { return GetValue(GetOrdinal(name)); }
            }
        }
        #endregion
        #region Helpers
        public static DataTable EnumToDataTable<T>(this IEnumerable<T> l_oItems)
        {
            var firstItem = l_oItems.FirstOrDefault();
            if (firstItem == null)
                return new DataTable();

            DataTable oReturn = new DataTable(TypeDescriptor.GetClassName(firstItem));
            object[] a_oValues;
            int i;

            var properties = TypeDescriptor.GetProperties(firstItem);

            foreach (PropertyDescriptor property in properties)
            {
                oReturn.Columns.Add(property.Name, BaseType(property.PropertyType));
            }

            //#### Traverse the l_oItems
            foreach (T oItem in l_oItems)
            {
                //#### Collect the a_oValues for this loop
                a_oValues = new object[properties.Count];

                //#### Traverse the a_oProperties, populating each a_oValues as we go
                for (i = 0; i < properties.Count; i++)
                    a_oValues[i] = properties[i].GetValue(oItem);

                //#### .Add the .Row that represents the current a_oValues into our oReturn value
                oReturn.Rows.Add(a_oValues);
            }

            //#### Return the above determined oReturn value to the caller
            return oReturn;
        }
        public static DataTable ToDataTable<T>( dynamic items)
        {

            DataTable dtDataTable = new DataTable();
            if (items.Count == 0) return dtDataTable;

            ((IEnumerable)items[0]).Cast<dynamic>().Select(p => p.Name).ToList().ForEach(col => { dtDataTable.Columns.Add(col); });

            ((IEnumerable)items).Cast<dynamic>().ToList().
                ForEach(data =>
                {
                    DataRow dr = dtDataTable.NewRow();
                    ((IEnumerable)data).Cast<dynamic>().ToList().ForEach(Col => { dr[Col.Name] = Col.Value; });
                    dtDataTable.Rows.Add(dr);
                });
            return dtDataTable;
        }
        public static DataTable ToDataTable(this IEnumerable<dynamic> items)
        {
            var data = items.ToArray();
            if (data.Count() == 0) return null;

            var dt = new DataTable();
            foreach (var key in ((IDictionary<string, object>)data[0]).Keys)
            {
                dt.Columns.Add(key);
            }
            foreach (var d in data)
            {
                dt.Rows.Add(((IDictionary<string, object>)d).Values.ToArray());
            }
            return dt;
        }
        public static DataTable ConvertToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        public static object ToType<T>(this object obj, T type)
        {
            //create instance of T type object:
            object tmp = Activator.CreateInstance(Type.GetType(type.ToString()));

            //loop through the properties of the object you want to covert:          
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                try
                {
                    //get the value of property and try to assign it to the property of T type object:
                    tmp.GetType().GetProperty(pi.Name).SetValue(tmp, pi.GetValue(obj, null), null);
                }
                catch (Exception ex)
                {
                    //Logging.Log.Error(ex);
                }
            }
            //return the T type object:         
            return tmp;
        }
        public static object ToNonAnonymousList<T>(this List<T> list, Type t)
        {

            //define system Type representing List of objects of T type:
            var genericType = typeof(List<>).MakeGenericType(t);

            //create an object instance of defined type:
            var l = Activator.CreateInstance(genericType);

            //get method Add from from the list:
            MethodInfo addMethod = l.GetType().GetMethod("Add");

            //loop through the calling list:
            foreach (T item in list)
            {

                //convert each object of the list into T object 
                //by calling extension ToType<T>()
                //Add this object to newly created list:
                addMethod.Invoke(l, new object[] { item.ToType(t) });
            }

            //return List of T objects:
            return l;
        }
        #endregion
    }
}