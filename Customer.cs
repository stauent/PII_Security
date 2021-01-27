using System;
using System.Collections.Generic;

#nullable disable

namespace PII_Security
{
    public partial class Customer
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] PiiDateOfBirth { get; set; }
        public byte[] PiiSocialInsuranceNumber { get; set; }
        public byte[] PiiAnnualSalary { get; set; }
    }
}
