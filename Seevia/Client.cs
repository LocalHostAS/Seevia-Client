using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;

namespace Seevia
{
    public class Client : IClient
    {
        private string baseUrl;
        private string partnerToken;

        public Client(string baseUrl, string partnerToken)
        {
            if (baseUrl == null) throw new ArgumentNullException("baseUrl");
            if (partnerToken == null) throw new ArgumentNullException("partnerToken");
            this.baseUrl = baseUrl;
            this.partnerToken = partnerToken;
        }

        public CreateAccountResponse CreateAccount(CreateAccountRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            var response = PerformRequest<AccountRes>("/signupbydealer", "POST", new AccountReq() { 
                Organization = request.Name,
                Address = request.Address,
                PostalCode = request.PostalCode,
                PostOffice = request.PostOffice
            });

            var integrationInfo = PerformRequest<IntegrationRes>("/organizations/" + response.ID + "/integrationinfo", "GET", response.ID + "/" + partnerToken,null);

            return new CreateAccountResponse { 
                AccountId = response.ID, 
                SearchToken = integrationInfo.SearchToken, 
                PhonebookUri = integrationInfo.PhonebookSipUri,
                PhonebookUrl = integrationInfo.PhonebookTmsUri
            };
        }

        public void DeleteAccount(string accountId)
        {
            if (accountId == null) throw new NotImplementedException("accountId");
            PerformRequest<object>("/organizations/" + accountId + "/terminate", "POST", accountId + "/" + partnerToken, null);
        }

        public CreateEntryResponse CreateEntry(string accountId, CreateEntryRequest request)
        {
            if (accountId == null) throw new ArgumentNullException("accountId");
            if (request == null) throw new ArgumentNullException("request");
            var response = PerformRequest<EntryRes>("/organizations/" + accountId + "/contacts", "POST", accountId + "/" + partnerToken, new EntryReq() { 
                Name = request.Name,
                H323URI = request.VideoAddress,
                SipURI = request.VideoAddress,
                Type = request.Type.ToString()
            });
            return new CreateEntryResponse { EntryId = response.ID };
        }

        public void DeleteEntry(string accountId, string entryId)
        {
            if (accountId == null) throw new ArgumentNullException("accountId");
            if (entryId == null) throw new ArgumentNullException("entryId");
            PerformRequest<object>("/contacts/" + entryId, "DELETE", accountId + "/" + partnerToken, null);
        }

        public void UpdateEntry(string accountId, string entryId, UpdateEntryRequest request)
        {
            if (accountId == null) throw new ArgumentNullException("accountId");
            if (entryId == null) throw new ArgumentNullException("entryId");
            if (request == null) throw new ArgumentNullException("request");
            PerformRequest<object>("/contacts/" + entryId, "PUT", accountId + "/" + partnerToken, new EntryReq()
            {
                Name = request.Name,
                H323URI = request.VideoAddress,
                SipURI = request.VideoAddress,
                Type = request.Type.ToString()
            });
        }

        private T PerformRequest<T>(string url, string method, object data = null)
        {
            return PerformRequest<T>(url, method, partnerToken, data);
        }

        private T PerformRequest<T>(string url, string method, string username, object data = null)
        {
            method = method.Trim().ToUpper();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl + url);
            httpWebRequest.Method = method.ToUpper();
            if (data != null) httpWebRequest.ContentType = "application/json";
            httpWebRequest.Credentials = new NetworkCredential(username, "token");

            if (method != "GET")
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    if (data != null)
                    {
                        string json = new JavaScriptSerializer().Serialize(data);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
            }

            
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return new JavaScriptSerializer().Deserialize<T>(streamReader.ReadToEnd());
            }
        }
    }

    class AccountReq
    {
        public string Organization { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PostOffice { get; set; }
    }

    class EntryReq
    {
        public string Name { get; set; }
        public string SipURI { get; set; }
        public string H323URI { get; set; }
        public string Type { get; set; }
    }

    class AccountRes
    {
        public string ID { get; set; }
    }

    class EntryRes
    {
        public string ID { get; set; }
    }

    class IntegrationRes
    {
        public string SearchToken { get; set; }
        public string PhonebookSipUri { get; set; }
        public string PhonebookTmsUri { get; set; }
    }
}
