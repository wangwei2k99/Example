using System;
using System.Collections.Generic;
using System.Reflection;

namespace EntityClassForeach
{
    class Program
    {
        static void Main(string[] args)
        {
            List<EntityClass> list = new List<EntityClass>();
            TypeFuncs typeFuncs = new TypeFuncs();
            typeFuncs.Dicdefinition();
            for (int i = 0; i < 50; i++)
            {
                EntityClass entityClass = new EntityClass();
                Type type = entityClass.GetType();
                PropertyInfo[] pis = type.GetProperties();
                foreach (PropertyInfo p in pis)
                {
                    string typeName = p.PropertyType.Name;
                    typeFuncs.Dic[typeName](p, entityClass, i);
                }
                list.Add(entityClass);
            }
        }
    }
}
