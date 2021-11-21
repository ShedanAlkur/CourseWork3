using CourseWork3.Patterns;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CourseWork3.Parser
{
    class Parser
    {
        List<Type> parsedTypes = new List<Type>()
        {

        };

        public void Init()
        {
            foreach (var type in parsedTypes)
            {
                var fields = type.GetFields(System.Reflection.BindingFlags.Public);
                var props = type.GetProperties(System.Reflection.BindingFlags.Public);

                foreach(var field in fields)
                {
                    var attribs = field.GetCustomAttributes(typeof(FieldOrPropertyPatternAttribute), false);
                    if (attribs.Length > 0) continue;
                    var name = field.Name;
                    

                    var attrib = (FieldOrPropertyPatternAttribute)attribs[0];
                    if (attrib.Type.HasFlag(FieldOrPropertyPatternType.StructSetter))
                    {
                        // создаем обобщенный метод
                        var method = typeof(ExpressionBuilder.ExpressionHelper).GetMethod(nameof(ExpressionBuilder.ExpressionHelper.CreateSetter));
                        var genericMethod = method.MakeGenericMethod(type, field.FieldType);

                        // создамем обобщенный тип для action<,>
                        var actionType = typeof(Action<>).MakeGenericType(type, field.FieldType);


                        var setter = (Action<object, object>)genericMethod.CreateDelegate(actionType);
                    }
                }
            }
        }
    }
}
