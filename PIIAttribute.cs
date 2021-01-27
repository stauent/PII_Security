using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PII_Security
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class PIIAttribute : System.Attribute
    {
        public PIIAttribute()
        {
        }
    }

    public static class PIIAttributeHelper
    {
        public static IEnumerable<PropertyInfo> EnumeratePIIProperties<T>(T containsPII) where T:class
        {
            // Get the list of properties that have the [PII] attribute assigned to them
            IEnumerable<PropertyInfo> PIIProperties = from p in typeof(T).GetProperties()
                let attrs = p.GetCustomAttributes(typeof(PIIAttribute), true)
                where attrs.Length != 0
                select p;

            return (PIIProperties);
        }

    }
}
