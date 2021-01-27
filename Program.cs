using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace PII_Security
{
    // TO use Entity framework database first, we install
    //  "Microsoft.EntityFrameworkCore" Version="5.0.2" 
    //  "Microsoft.EntityFrameworkCore.Design" Version="5.0.2"
    //  "Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.2" 
    //  "Microsoft.EntityFrameworkCore.Tools" Version="5.0.2"
    //
    // Then to reverse engineer the database:
    // Scaffold-DbContext -provider Microsoft.EntityFrameworkCore.SqlServer -connection "Data Source = OPTIMUS; Initial Catalog = PII_Encryption; Persist Security Info = True; User ID = ma; Password=I8well4sure;"


    class Program
    {
        static void Main(string[] args)
        {
            // Set up configuration to read appsettings.json and override with secrets.json
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetEntryAssembly())
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            // Display the symmetric encryption key to be used. This should be a
            // GUID with no formatting characters (such as brackets and dashes), just the numbers.
            PIISecure._symmetricKey = Configuration["SymmetricEncryptionKey"];
            Console.WriteLine($"Symmetric Encryption Key={PIISecure._symmetricKey}");

            // Demonstrate how to encrypt/decrypt a plain old text string
            string phrase = "My name is bob";
            string encrypted = phrase.EncryptString();
            string decrypted = encrypted.DecryptString();
            if(decrypted== phrase)
                Console.WriteLine("Symmetric decryption successful");
            else
                Console.WriteLine("Symmetric decryption failed");

            // Now demonstrate how to store encrypted data into the database so that
            // NOBODY can read the human readable form. No query in SSMS is possible that
            // will return the decrypted data. All decryption MUST be done here in the client
            // before it's used. It is easy for us to then create a controller that only
            // someone with a claim of "PII Security Administrator" would be allowed to call.

            // Write a few records into the database with encrypted data. All "PiiXXXXXXXX"
            // fields are encrypted.
            PII_EncryptionContext db = new PII_EncryptionContext();
            Customer customer = new Customer();
            customer.FirstName = "Bob";
            customer.LastName = "Smith";
            customer.PiiDateOfBirth = DateTime.Now.AddYears(-20).Date.Encrypt();
            customer.PiiSocialInsuranceNumber = "412-985-900".Encrypt();
            customer.PiiAnnualSalary = 198424.00m.Encrypt();
            db.Customers.Add(customer);

            customer = new Customer();
            customer.FirstName = "Sally";
            customer.LastName = "Jones";
            customer.PiiDateOfBirth = DateTime.Now.AddYears(-22).Date.Encrypt();
            customer.PiiSocialInsuranceNumber = "512-000-111".Encrypt();
            decimal mySalary = 54319.00m;
            customer.PiiAnnualSalary = mySalary.Encrypt();
            db.Customers.Add(customer);

            db.SaveChanges();

            // Retrieve all customers from the database
            List<Customer> myCustomers = db.Customers.ToList();

            // Display all customer data, decrypting the data as we enumerate through the records.
            // If the encrypted column data is not used, then no decryption takes place.
            // In this case, the columns DateOfBirth, SocialInsuranceNumber and AnnualSalary are decrypted
            // before being displayed.
            foreach (Customer c in myCustomers)
            {
                // Displays the encrypted values as they were retrieved from the database
                Console.WriteLine("\r\n\r\n\rn===================================================================");
                Console.WriteLine($"Encrypted DateOfBirth:{BitConverter.ToString(c.PiiDateOfBirth)}");
                Console.WriteLine($"Encrypted SocialInsuranceNumber:{BitConverter.ToString(c.PiiSocialInsuranceNumber)}");
                Console.WriteLine($"Encrypted AnnualSalary:{BitConverter.ToString(c.PiiAnnualSalary)}\r\n\r\n");

                // We know the names of the properties that contain decrypted data, so we can specify them by name
                Console.WriteLine($"{c.FirstName} {c.LastName}\t\tBorn:{c.DateOfBirth.ToShortDateString()}\t\tSIN:{c.SocialInsuranceNumber}\t\tSalary:{c.AnnualSalary:C0}\r\n");

                // This shows a way to use reflection to find ONLY those properties marked with the [PII] attribute.
                // This makes it easy to find the properties containing DECRYPTED data without knowing their name in advance.
                IEnumerable<PropertyInfo> PIIProperties = PIIAttributeHelper.EnumeratePIIProperties<Customer>(customer);
                foreach (PropertyInfo prop in PIIProperties)
                {
                    Console.WriteLine($"{prop.Name}={prop.GetValue(customer)}");
                }
            }

            // Delete all customers
            foreach (Customer c in myCustomers)
            {
                db.Customers.Remove(c);
            }

            db.SaveChanges();




            Console.ReadKey();
        }
    }

}
