using CsvHelper;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Models;
using System.Data;
using System.Globalization;
using System.Text;

namespace PhoneBook.Repository;

public class PhoneBookContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<PhoneNumberDetail> PhoneNumbers { get; set; }
    public DbSet<EmailDetail> Emails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer( @"Server=(localdb)\mssqllocaldb;Database=PhoneBookDb;Trusted_Connection=True;");
    }

    public void SeedDataFromExcel()
    {
        ImportDataFromFile("PhoneBookSeedData.csv");
    }
    public void ImportDataFromFile(string filepath)
    {
        

            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException(
                    "The specified file was not found.",
                    filepath);
            }

            string extension =
                Path.GetExtension(filepath).ToLowerInvariant();

            if (extension != ".xls" &&
                extension != ".xlsx" &&
                extension != ".csv")
            {
                throw new NotSupportedException(
                    "Unsupported file format. Only .xls, .xlsx and .csv files are supported.");
            }

            var contacts = new List<Contact>();
            var phoneNumbers = new List<PhoneNumberDetail>();
            var emails = new List<EmailDetail>();
            var contactMap = new Dictionary<int, Contact>();

            if (extension == ".xls" || extension == ".xlsx")
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using var stream = File.Open( filepath, FileMode.Open, FileAccess.Read);

                using var reader = ExcelReaderFactory.CreateReader(stream);

                var dataSet = reader.AsDataSet();

                if (!dataSet.Tables.Contains("Contacts"))
                {
                    throw new Exception("'Contacts' worksheet was not found.");
                }

                var contactTable = dataSet.Tables["Contacts"]!;

                for (int i = 1; i < contactTable.Rows.Count; i++)
                {
                    DataRow row = contactTable.Rows[i];

                    if (!int.TryParse( row[0]?.ToString(), out int sourceContactId))
                    {
                       throw new Exception("Invalid Contact Id in Contacts row.");
                    }

                    if (!DateTime.TryParse( row[6]?.ToString(),out DateTime createdAt))
                    {
                        throw new Exception($"Invalid CreatedAt in Contacts row {i + 1}.");
                    }

                    var contact = new Contact
                    {
                        FirstName = row[1]?.ToString() ?? "",

                        LastName = row[2]?.ToString() ?? "",

                        OrganizationName = row[3]?.ToString(),

                        JobTitle = row[4]?.ToString(),

                        Notes = row[5]?.ToString(),

                        CreatedAt = createdAt
                    };

                    contacts.Add(contact);
                    contactMap[sourceContactId] = contact;
                }

                if (!dataSet.Tables.Contains("PhoneNumbers"))
                {
                    throw new Exception("'PhoneNumbers' worksheet was not found.");
                }

                var phoneTable = dataSet.Tables["PhoneNumbers"]!;

                for (int i = 1; i < phoneTable.Rows.Count; i++)
                {
                    DataRow row = phoneTable.Rows[i];

                    if (!int.TryParse(row[5]?.ToString(),out int sourceContactId))
                    {
                        throw new Exception($"Invalid ContactId in PhoneNumbers row {i + 1}.");
                    }

                    if (!contactMap.TryGetValue(sourceContactId,out Contact? contact))
                    {
                        throw new Exception($"ContactId {sourceContactId} does not exist in Contacts.");
                    }

                    if (!Enum.TryParse(row[2]?.ToString(),true,out LabelType label))
                    {
                        throw new Exception($"Invalid phone label in PhoneNumbers row {i + 1}.");
                    }

                    if (!DateTime.TryParse(row[3]?.ToString(),out DateTime createdAt))
                    {
                        throw new Exception($"Invalid CreatedAt in PhoneNumbers row {i + 1}.");
                    }

                    var phoneNumber = new PhoneNumberDetail
                    {
                        Number = row[1]?.ToString() ?? "",
                        Label = label,
                        CreatedAt = createdAt,
                        Contact = contact
                    };

                    phoneNumbers.Add(phoneNumber);
                }

                if (!dataSet.Tables.Contains("Emails"))
                {
                   throw new Exception("'Emails' worksheet was not found."); 
                }

                var emailTable = dataSet.Tables["Emails"]!;

                for (int i = 1; i < emailTable.Rows.Count; i++)
                {
                    DataRow row = emailTable.Rows[i];

                    if (!int.TryParse( row[5]?.ToString(), out int sourceContactId))
                    {
                        throw new Exception($"Invalid ContactId in Emails row {i + 1}.");
                    }

                    if (!contactMap.TryGetValue(sourceContactId,out Contact? contact))
                    {
                        throw new Exception($"ContactId {sourceContactId} does not exist in Contacts.");
                    }

                    if (!Enum.TryParse(row[2]?.ToString(),true,out LabelType label))
                    {
                       throw new Exception($"Invalid email label in Emails row {i + 1}.");
                    }

                    if (!DateTime.TryParse(row[3]?.ToString(), out DateTime createdAt))
                    {
                        throw new Exception($"Invalid CreatedAt in Emails row {i + 1}.");
                    }

                    DateTime? updatedAt = null;

                    string updatedAtValue =row[4]?.ToString() ?? "";

                    if (!string.IsNullOrWhiteSpace(  updatedAtValue))
                    {
                        if (!DateTime.TryParse( updatedAtValue, out DateTime parsedUpdatedAt))
                        {
                           throw new Exception($"Invalid UpdatedAt in Emails row {i + 1}.");
                        }
                        updatedAt = parsedUpdatedAt;
                    }

                    var email = new EmailDetail
                    {
                        EmailAddress =row[1]?.ToString() ?? "",
                        Label = label,
                        CreatedAt = createdAt,
                        UpdatedAt = updatedAt,
                        Contact = contact
                    };

                    emails.Add(email);
                }
            }

            else if (extension == ".csv")
            {
                using var streamReader =new StreamReader(filepath);

                using var csv = new CsvReader(streamReader,CultureInfo.InvariantCulture);
                csv.Read();
                csv.ReadHeader();

                if (csv.HeaderRecord == null)
                {
                    throw new Exception("CSV file does not contain a header row.");
                }

                string[] requiredColumns =
                {
                    "ContactId",
                    "FirstName",
                    "LastName",
                    "OrganizationName",
                    "JobTitle",
                    "Notes",
                    "CreatedAt",
                    "PhoneNumber",
                    "PhoneLabel",
                    "PhoneCreatedAt",
                    "EmailAddress",
                    "EmailLabel",
                    "EmailCreatedAt",
                    "EmailUpdatedAt"
                };

                foreach (string column in requiredColumns)
                {
                    if (!csv.HeaderRecord.Contains(column))
                    {
                       throw new Exception($"CSV file is missing required column: {column}.");
                    }
                }

                while (csv.Read())
                {
                   

                    if (!int.TryParse(csv.GetField("ContactId"),out int sourceContactId))
                    {
                      throw new Exception($"Invalid ContactId in CSV row.");
                    }

                    Contact contact;

                    if (!contactMap.TryGetValue( sourceContactId,out contact!))
                    {
                        if (!DateTime.TryParse( csv.GetField("CreatedAt"),out DateTime contactCreatedAt))
                        {
                           
                            throw new Exception($"Invalid CreatedAt for ContactId {sourceContactId}.");
                           
                        }

                        contact = new Contact
                        {
                            FirstName = csv.GetField("FirstName") ?? "",
                            LastName = csv.GetField("LastName") ?? "",
                            OrganizationName = csv.GetField("OrganizationName") ?? "",
                            JobTitle =csv.GetField("JobTitle"),
                            Notes =csv.GetField("Notes"),
                            CreatedAt = contactCreatedAt
                        };

                        contacts.Add(contact);
                        contactMap[sourceContactId] = contact;
                    }

                    string phoneNumberValue =csv.GetField("PhoneNumber") ?? "";

                    if (!string.IsNullOrWhiteSpace( phoneNumberValue))
                    {
                        if (!Enum.TryParse( csv.GetField("PhoneLabel"),true,out LabelType phoneLabel))
                        {
                            throw new Exception($"Invalid phone label for ContactId {sourceContactId}.");
                        }

                        if (!DateTime.TryParse( csv.GetField("PhoneCreatedAt"), out DateTime phoneCreatedAt))
                        {
                            throw new Exception($"Invalid PhoneCreatedAt for ContactId {sourceContactId}.");
                        }

                        phoneNumbers.Add(
                            new PhoneNumberDetail
                            {
                                Number = phoneNumberValue,
                                Label = phoneLabel,
                                CreatedAt = phoneCreatedAt,
                                Contact = contact
                            });
                    }

                    string emailAddress = csv.GetField("EmailAddress") ?? "";

                    if (!string.IsNullOrWhiteSpace(emailAddress))
                    {
                        if (!Enum.TryParse( csv.GetField("EmailLabel"), true, out LabelType emailLabel))
                        {
                            throw new Exception($"Invalid email label for ContactId {sourceContactId}.");
                        }

                        if (!DateTime.TryParse( csv.GetField("EmailCreatedAt"), out DateTime emailCreatedAt))
                        {
                            throw new Exception($"Invalid EmailCreatedAt for ContactId {sourceContactId}.");
                        }

                        DateTime? emailUpdatedAt = null;

                        string updatedValue =  csv.GetField("EmailUpdatedAt") ?? "";

                        if (!string.IsNullOrWhiteSpace( updatedValue))
                        {
                            if (!DateTime.TryParse( updatedValue, out DateTime parsedUpdatedAt))
                            {
                               throw new Exception($"Invalid EmailUpdatedAt for ContactId {sourceContactId}.");
                            }

                            emailUpdatedAt = parsedUpdatedAt;
                        }

                        emails.Add(
                            new EmailDetail
                            {
                                EmailAddress = emailAddress,
                                Label = emailLabel,
                                CreatedAt = emailCreatedAt,
                                UpdatedAt = emailUpdatedAt,
                                Contact = contact
                            });
                    }
                }
            }

            if (contacts.Count == 0)
            {
                throw new Exception("No contacts found in the Excel or CSV file.");
            }

            Contacts.AddRange(contacts);
            PhoneNumbers.AddRange(phoneNumbers);
            Emails.AddRange(emails);

            SaveChanges();

            Console.WriteLine(
                $"Successfully imported {contacts.Count} contacts.");

            Console.WriteLine(
                $"Successfully imported {phoneNumbers.Count} phone numbers.");

            Console.WriteLine(
                $"Successfully imported {emails.Count} emails.");

            Console.WriteLine("All Excel data inserted successfully.");
       
    }
}


