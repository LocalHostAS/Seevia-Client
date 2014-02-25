using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Seevia
{
    public interface IClient
    {
        CreateAccountResponse CreateAccount(CreateAccountRequest request);
        void DeleteAccount(string accountId);
        
        CreateEntryResponse CreateEntry(string accountId, CreateEntryRequest request);
        void UpdateEntry(string accountId, string entryId, UpdateEntryRequest request);
        void DeleteEntry(string accountId, string entryId);
    }

    public class CreateAccountRequest
    {
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string PostalCode { get; private set; }
        public string PostOffice { get; private set; }
        public string Country { get; set; }

        public CreateAccountRequest(string name, string address, string postalCode, string postOffice)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (address == null) throw new ArgumentNullException("address");
            if (postalCode == null) throw new ArgumentNullException("postalCode");
            if (postOffice == null) throw new ArgumentNullException("postOffice");

            this.Name = name;
            this.Address = address;
            this.PostalCode = postalCode;
            this.PostOffice = postOffice;
        }
    }

    public class CreateAccountResponse
    {
        public string AccountId { get; set; }
        public string SearchToken { get; set; }
        public string PhonebookUrl { get; set; }
        public string PhonebookUri { get; set; }
    }

    public class CreateEntryRequest
    {
        public string Name { get; private set; }
        public string VideoAddress { get; private set; }
        public EntryType Type { get; private set; }

        public CreateEntryRequest(string name, string videoAddress, EntryType type)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (videoAddress == null) throw new ArgumentNullException("videoAddress");

            try { new MailAddress(videoAddress); }
            catch (Exception)
            {
                throw new ArgumentException("Invalid video address", "videoAddress");
            }

            this.Name = name;
            this.VideoAddress = VideoAddress;
            this.Type = type;
        }
    }

    public class CreateEntryResponse
    {
        public string EntryId { get; set; }
    }

    public class UpdateEntryRequest : CreateEntryRequest {

        public UpdateEntryRequest(string name, string videoAddress, EntryType type) : base(name,videoAddress,type) { }
    }

    public enum EntryType
    {
        PERSON,
        SYSTEM
    }
}
