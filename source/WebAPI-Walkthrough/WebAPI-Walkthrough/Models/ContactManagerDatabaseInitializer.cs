using System.Data.Entity;

namespace ContactManager.Models
{
    public class ContactManagerDatabaseInitializer :
        DropCreateDatabaseAlways<ContactManagerContext>
    {
        protected override void Seed(ContactManagerContext context)
        {
            base.Seed(context);

            context.Contacts.Add(
                new Contact
                {
                    Name = "Jon Galloway",
                    Email = "jongalloway@gmail.com",
                    Twitter = "jongalloway",
                    City = "San Diego",
                    State = "CA"
                });

            context.Contacts.Add(
                new Contact
                {
                    Name = "Scott Hanselman",
                    Email = "shanselman@microsoft.com",
                    Twitter = "shanselman",
                    City = "Portland",
                    State = "Oregon"
                });

            context.Contacts.Add(
                new Contact
                {
                    Name = "Umit Sunar",
                    Email = "umits@microsoft.com",
                    Twitter = "UmitSunar",
                    City = "Istanbul",
                    State = "Turkey"
                });
        }
    }
}