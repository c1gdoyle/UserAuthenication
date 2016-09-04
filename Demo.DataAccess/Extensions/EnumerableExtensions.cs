using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Demo.DataAccess.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Creates a <see cref="DataTable"/> from a specified <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="items"/>.</typeparam>
        /// <param name="items">The <see cref="IEnumerable{T}"/> to create the <see cref="DataTable"/> from.</param>
        /// <returns>A <see cref="DataTable"/> with columns matching the properties of T and rows matching the elements of <paramref name="items"/>.</returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            //Create the result table, and gather all properties of T
            var table = new DataTable(typeof(T).Name);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //Add the properties as columns to the data-table
            foreach (PropertyInfo prop in properties)
            {
                Type propType = prop.PropertyType;

                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propType = new NullableConverter(propType).UnderlyingType;
                }

                if (propType.IsEnum)
                {
                    table.Columns.Add(prop.Name, typeof(string));

                }
                else
                {
                    table.Columns.Add(prop.Name, propType);
                }
            }

            //Add the property values as rows to the table
            foreach (var item in items)
            {
                var values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}
