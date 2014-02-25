using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Configuration;

namespace SeeviaTestRunner
{
    class Program
    {
        private static dynamic GetInput(IDictionary<string,string> fields)
        {
            dynamic res = new ExpandoObject();
            var p = res as IDictionary<String, object>;
            
            foreach (var field in fields.Keys)
            {
                Console.Write(fields[field] + ": ");
                p[field] = Console.ReadLine();
                Console.WriteLine();
            }
            return res;
        }

        static void Main(string[] args)
        {
            var baseUrl = System.Configuration.ConfigurationManager.AppSettings["baseUrl"];
            var partnerToken = System.Configuration.ConfigurationManager.AppSettings["partnerToken"];

            var seeviaClient = new Seevia.Client(baseUrl,partnerToken);

            Console.WriteLine("-- Create Seevia account --");

            var req = GetInput(new Dictionary<string, string>() {
                {"Name","Name"},
                {"Address","Address"},
                {"PostalCode", "Postal code"},
                {"PostOffice", "Post office"}
            });

            var accountRes = seeviaClient.CreateAccount(new Seevia.CreateAccountRequest(req.Name, req.Address, req.PostalCode, req.PostOffice));
            Console.WriteLine("Account created :)");
            Console.WriteLine("Account id: " + accountRes.AccountId);
            Console.WriteLine("Phonebook url: " + accountRes.PhonebookUrl);
            Console.WriteLine(string.Empty);

            try
            {
                Console.WriteLine("-- Create Seevia entry --");
                var entryReq = GetInput(new Dictionary<string, string>() {
                    {"Name","Name"},
                    {"VideoAddress","Video address"},
                    {"Type", "Type (PERSON or SYSTEM)"}
                });

                var entryRes = seeviaClient.CreateEntry(accountRes.AccountId, new Seevia.CreateEntryRequest(entryReq.Name, entryReq.VideoAddress, Enum.Parse(typeof(Seevia.EntryType), entryReq.Type)));
                Console.WriteLine("Entry created :)");
                Console.WriteLine("Entry id: " + entryRes.EntryId);
                Console.WriteLine(string.Empty);

                Console.WriteLine("-- Update Seevia entry --");
                var updateReq = GetInput(new Dictionary<string, string>() {
                    {"Name","Name"},
                    {"VideoAddress","Video address"},
                    {"Type", "Type (PERSON or SYSTEM)"}
                });
                seeviaClient.UpdateEntry(accountRes.AccountId, entryRes.EntryId, new Seevia.UpdateEntryRequest(updateReq.Name, updateReq.VideoAddress, Enum.Parse(typeof(Seevia.EntryType), entryReq.Type)));
                Console.WriteLine("Entry updated :)");
                Console.WriteLine(string.Empty);

                Console.WriteLine("-- Delete Seevia entry --");
                Console.WriteLine("Press any key to delete entry...");
                Console.ReadLine();
                seeviaClient.DeleteEntry(accountRes.AccountId, entryRes.EntryId);
                Console.WriteLine("Entry deleted :)");
                Console.WriteLine(string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed.....");
                Console.Write(ex.Message);
            }
            finally {
                Console.WriteLine("-- Delete Seevia account --");
                Console.WriteLine("Press any key to delete account...");
                Console.ReadLine();
                seeviaClient.DeleteAccount(accountRes.AccountId);
                Console.WriteLine("Account deleted :)");
                Console.WriteLine(string.Empty);

                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
        }
    }
}
