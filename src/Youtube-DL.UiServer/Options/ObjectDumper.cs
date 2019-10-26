// source: https://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Youtube_DL.UiServer.Options
{
    /// <summary>An object dumper.</summary>
    public class ObjectDumper
    {
        #region Fields

        /// <summary>The level.</summary>
        private int _level;

        /// <summary>Size of the indent.</summary>
        private readonly int _indentSize;

        /// <summary>The string builder.</summary>
        private readonly StringBuilder _stringBuilder;

        /// <summary>The hash list of found elements.</summary>
        private readonly List<int> _hashListOfFoundElements;

        #endregion

        #region Constructors

        /// <summary>Constructor.</summary>
        /// <param name="indentSize">   Size of the indent. </param>
        private ObjectDumper(int indentSize)
        {
            _indentSize = indentSize;
            _stringBuilder = new StringBuilder();
            _hashListOfFoundElements = new List<int>();
        }

        #endregion

        /// <summary>Dumps the given element.</summary>
        /// <param name="element">  The element to dump. </param>
        /// <returns>A string.</returns>
        public static string Dump(object element)
        {
            return Dump(element, 2);
        }

        /// <summary>Dumps the given element.</summary>
        /// <param name="element">      The element to dump. </param>
        /// <param name="indentSize">   Size of the indent. </param>
        /// <returns>A string.</returns>
        public static string Dump(object element, int indentSize)
        {
            var instance = new ObjectDumper(indentSize);
            return instance.DumpElement(element);
        }

        /// <summary>Dumps an element.</summary>
        /// <param name="element">  The element to dump. </param>
        /// <returns>A string.</returns>
        private string DumpElement(object element)
        {
            if (element == null || element is ValueType || element is string)
            {
                Write(FormatValue(element));
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    Write("{{{0}}}", objectType.FullName);
                    _hashListOfFoundElements.Add(element.GetHashCode());
                    _level++;
                }

                if (element is IEnumerable enumerableElement)
                {
                    foreach (var item in enumerableElement)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            _level++;
                            DumpElement(item);
                            _level--;
                        }
                        else
                        {
                            if (!AlreadyTouched(item))
                                DumpElement(item);
                            else
                                Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName);
                        }
                    }
                }
                else
                {
                    var members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var memberInfo in members)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        var propertyInfo = memberInfo as PropertyInfo;

                        if (fieldInfo == null && propertyInfo == null)
                            continue;

                        var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                        var value = fieldInfo != null
                                           ? fieldInfo.GetValue(element)
                                           : propertyInfo.GetValue(element, null);

                        if (type.IsValueType || type == typeof(string))
                        {
                            Write("{0}: {1}", memberInfo.Name, FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                            Write("{0}: {1}", memberInfo.Name, isEnumerable ? "..." : "{ }");

                            var alreadyTouched = !isEnumerable && AlreadyTouched(value);
                            _level++;
                            if (!alreadyTouched)
                                DumpElement(value);
                            else
                                Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            _level--;
                        }
                    }
                }

                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    _level--;
                }
            }

            return _stringBuilder.ToString();
        }

        /// <summary>Already touched.</summary>
        /// <param name="value">    The value. </param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private bool AlreadyTouched(object value)
        {
            if (value == null)
                return false;

            var hash = value.GetHashCode();
            return _hashListOfFoundElements.Any(t => t == hash);
        }

        /// <summary>Writes.</summary>
        /// <param name="value">    The value. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>
        private void Write(string value, params object[] args)
        {
            var space = new string(' ', _level * _indentSize);

            if (args != null)
                value = string.Format(value, args);

            _stringBuilder.AppendLine(space + value);
        }

        /// <summary>Format value.</summary>
        /// <param name="o">    An object to process. </param>
        /// <returns>The formatted value.</returns>
        private static string FormatValue(object o)
        {
            switch (o)
            {
                case null:
                    return "null";
                case DateTime _:
                    return ((DateTime)o).ToShortDateString();
                case string _:
                    return $"\"{o}\"";
                case char _ when (char)o == '\0':
                    return string.Empty;
                case ValueType _:
                    return o.ToString();
                case IEnumerable _:
                    return "...";
            }

            return "{ }";
        }
    }
}
