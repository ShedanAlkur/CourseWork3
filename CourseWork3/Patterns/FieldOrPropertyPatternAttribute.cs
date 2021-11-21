using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork3.Patterns
{
    /// <summary>
    /// 123
    /// </summary>
    [Flags]
    public enum FieldOrPropertyPatternType
    {
        /// <summary>
        /// У поля или свойства может быть установлено значение.
        /// </summary>
        StructSetter = 1 << 0,
        /// <summary>
        /// У поля или свойства может быть увеличено значение.
        /// </summary>
        StructIncrementor = 1 << 1,
        /// <summary>
        /// У поля- или свойства-списка может быть установлен первый и единственный элемент; добавлен элемент; список может быть очищен.
        /// </summary>
        ListSetter = 1 << 2,
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FieldOrPropertyPatternAttribute : System.Attribute
    {
        public FieldOrPropertyPatternType Type { get; set; }
        public FieldOrPropertyPatternAttribute(FieldOrPropertyPatternType type)
        {
            this.Type = type;
        }
    }
}
