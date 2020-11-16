using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EntityClassForeach
{
    class TypeFuncs
    {
        public delegate void TypeDelegate(PropertyInfo p, EntityClass entityClass, int i);
        private Dictionary<string, TypeDelegate> keyValuePairs = new Dictionary<string, TypeDelegate>();
        public Dictionary<string, TypeDelegate> Dic
        {
            get { return keyValuePairs; }
            set { keyValuePairs = value; }
        }

        public void Dicdefinition()
        {
            Dic.Add(typeof(decimal).Name, WriteDecimalValue);
            Dic.Add(typeof(double).Name, WriteDoubleValue);
            Dic.Add(typeof(float).Name, WriteFloatValue);
            Dic.Add(typeof(int).Name, WriteIntValue);
            Dic.Add(typeof(string).Name, WriteStringValue);
        }

        public void WriteDecimalValue(PropertyInfo p, EntityClass entityClass, int i)
        {
            p.SetValue(entityClass, Convert.ToDecimal(i));
        }

        public void WriteDoubleValue(PropertyInfo p, EntityClass entityClass, int i)
        {
            p.SetValue(entityClass, Convert.ToDouble(i));
        }

        public void WriteFloatValue(PropertyInfo p, EntityClass entityClass, int i)
        {
            p.SetValue(entityClass, Convert.ToSingle(i));
        }

        public void WriteIntValue(PropertyInfo p, EntityClass entityClass, int i)
        {
            p.SetValue(entityClass, i);
        }

        public void WriteStringValue(PropertyInfo p, EntityClass entityClass, int i)
        {
            p.SetValue(entityClass, Convert.ToString(i));
        }
        public void WriteDatatimeValue(PropertyInfo p, EntityClass entityClass, int i)
        {
            p.SetValue(entityClass, Convert.ToDateTime(i));
        }
    }
    #region 尝试接口实现委托,未完成
    //interface TypeMethod
    //{
    //    void WriteIntValue(PropertyInfo p, EntityClass entityClass, int i);
    //    void WriteStringValue(PropertyInfo p, EntityClass entityClass, int i);
    //    void WriteDoubleValue(PropertyInfo p, EntityClass entityClass, int i);
    //    void WriteDecimalValue(PropertyInfo p, EntityClass entityClass, int i);
    //    void WriteFloatValue(PropertyInfo p, EntityClass entityClass, int i);
    //}
    //public class TypeFuncs : TypeMethod
    //{
    //    public void WriteDecimalValue(PropertyInfo p, EntityClass entityClass, int i)
    //    {
    //        p.SetValue(entityClass, Convert.ToDecimal(i));
    //    }

    //    public void WriteDoubleValue(PropertyInfo p, EntityClass entityClass, int i)
    //    {
    //        p.SetValue(entityClass, Convert.ToDouble(i));
    //    }

    //    public void WriteFloatValue(PropertyInfo p, EntityClass entityClass, int i)
    //    {
    //        p.SetValue(entityClass, Convert.ToSingle(i));
    //    }

    //    public void WriteIntValue(PropertyInfo p, EntityClass entityClass, int i)
    //    {
    //        p.SetValue(entityClass, i);
    //    }

    //    public void WriteStringValue(PropertyInfo p, EntityClass entityClass, int i)
    //    {
    //        p.SetValue(entityClass, Convert.ToString(i));
    //    }
    //}
    #endregion
}
