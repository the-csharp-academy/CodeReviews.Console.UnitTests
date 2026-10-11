using PhoneBook.Controller;
using PhoneBook.Models;
using PhoneBook.Repository;
using Microsoft.EntityFrameworkCore;
namespace PhoneBook.IntegrationTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private ContactController _contactController = null!;
        private PhoneBookContext _context = null!;
        [SetUp]
        public void SetUp()
        {
            _contactController = new ContactController();
            _context = new PhoneBookContext();
        }

        [Test]
        public async Task InsertContact_ShouldSaveContactToDatabase()
        {
            Console.WriteLine("1. Test started");

            var contact = new Contact
            {
                FirstName = "mathew",
                LastName = "Doe",


                PhoneNumbers = new List<PhoneNumberDetail>
        {
            new PhoneNumberDetail
            {
                Number = "8456274935",
                Label = LabelType.Mobile
            }
        }
            };

            Console.WriteLine("2. Contact created");

            await _contactController.InsertContactAsync(contact);

            Console.WriteLine("3. Insert completed");

            var savedContact = await _context.Contacts
                .Include(c => c.PhoneNumbers)
                .FirstOrDefaultAsync(c =>
                    c.FirstName == "John" &&
                    c.LastName == "Doe");

            Console.WriteLine("4. Query completed");

            Assert.That(savedContact, Is.Not.Null);
            Assert.That(savedContact!.PhoneNumbers, Has.Count.EqualTo(1));
            Assert.That(savedContact.PhoneNumbers, Has.Count.EqualTo(1));
            Assert.That(savedContact.PhoneNumbers[0].Number,
                Is.EqualTo("123-456-7890"));
            Assert.That(savedContact.PhoneNumbers[0].Label,
                Is.EqualTo(LabelType.Mobile));
        }
        [Test]
        public async Task DeleteContact_ShouldRemoveContactFromDatabase()
        {
            
            var contact = new Contact
            {
                FirstName = "Mathew",
                LastName = "Doe",
                PhoneNumbers = new List<PhoneNumberDetail>
        {
            new PhoneNumberDetail
            {
                Number = "8456274935",
                Label = LabelType.Mobile
            }
        }
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            int contactId = contact.Id;

            
            await _contactController.DeleteContactAsync(contactId);

            
            var deletedContact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == contactId);

            Assert.That(deletedContact, Is.Null);
        }

        [Test]
        public async Task ViewAllContacts_ShouldReturnAllContacts()
        {
            // Arrange
            var contacts = new List<Contact>
    {
        new Contact
        {
            FirstName = "Mathew",
            LastName = "Doe"
        },
        new Contact
        {
            FirstName = "Arun",
            LastName = "Kumar"
        }
    };

            _context.Contacts.AddRange(contacts);
            await _context.SaveChangesAsync();

            // Act
            var result = await _contactController.ViewAllContacts();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(c => c.FirstName == "Mathew"), Is.True);
            Assert.That(result.Any(c => c.FirstName == "Arun"), Is.True);
        }

        [Test]
        public async Task SearchByName_WhenContactDoesNotExist_ShouldReturnEmptyList()
        {
            // Act
            var result = await _contactController
                .SearchByNameAsync("NonExisting", "Person");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task ModifyContact_ShouldUpdateContactInDatabase()
        {
            // Arrange
            var contact = new Contact
            {
                FirstName = "Mathew",
                LastName = "Doe"
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            contact.FirstName = "Matthew";

            // Act
            await _contactController.ModifyContact(contact);

            // Assert
            await _context.Entry(contact).ReloadAsync();

            Assert.That(contact.FirstName, Is.EqualTo("Matthew"));
        }

        [Test]
        public async Task DeleteContact_WhenContactDoesNotExist_ShouldHandleGracefully()
        {
            // Arrange
            int invalidId = -1;

            // Act
            Assert.DoesNotThrowAsync(async () =>
                await _contactController.DeleteContactAsync(invalidId));
        }

        [Test]
        public async Task ModifyContact_WhenContactDoesNotExist_ShouldNotUpdateAnyContact()
        {
            // Arrange
            var contact = new Contact
            {
                Id = -1,
                FirstName = "Unknown",
                LastName = "Person"
            };

            // Act
            Assert.DoesNotThrowAsync(async () =>
                await _contactController.ModifyContact(contact));

            // Assert
            var savedContact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == -1);

            Assert.That(savedContact, Is.Null);
        }

        [Test]
        public void ImportContacts_WhenFileDoesNotExist_ShouldHandleGracefully()
        {
            // Arrange
            string filePath = @"C:\InvalidFolder\MissingFile.xlsx";

            // Act & Assert
            Assert.DoesNotThrow(() =>
                _contactController.ImportContacts(filePath));
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

    }
}
