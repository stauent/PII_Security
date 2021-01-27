using System;
using System.Collections.Generic;
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
}
