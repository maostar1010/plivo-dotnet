using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plivo.Exception;
using Plivo.Client;
using Plivo.Resource.Node;

namespace Plivo.Resource.Phlo
{
    /// <summary>
    ///(PHLO) Plivo High Level Objects interface 
    /// </summary>
    public class PhloInterface : ResourceInterface
    {
        /// <summary>
        /// PHLO Id
        /// </summary>
        public string _phloId;

        /// <summary>
        /// Authentication ID
        /// </summary>
        private readonly string _authId;

        /// <summary>
        /// Authentication Token
        /// </summary>
        private readonly string _authToken;

        /// <summary>
        /// Authentication Token
        /// </summary>
        private readonly string _accountId;


        /// <summary>
        /// PHLO 
        /// </summary>
        /// <param name="HTTP client"></param>
        /// <param name="authId"></param>
        /// <param name="authToken"></param>
         
        internal PhloInterface(HttpClient client, string authId, string authToken) : base(client)
        {
            _phloId = null;
            _authId = authId;
            _authToken = authToken;
            _accountId = GetAccountID();
            Uri = "";
        }
        
        private NodeInterface NodeI { get; set; }
        private MultiPartyCall.MultiPartyCallInterface MultiPartyCallI { get; set; }

        public PhloRunCallResponse Run(Dictionary<string, object> data = null)
        {
            var qs = data!=null ? Client.AsQueryString(data) : "";
            var result = Task.Run(async () => await Client.Update<PhloRunCallResponse>($"account/{_authId}/phlo/{_phloId}" + qs, data).ConfigureAwait(false)).Result;
            return result.Object;
        }
        
        public Node.Node Node(string nodeType, string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                throw new PlivoValidationException("nodeId is mandatory, can not be null or empty");
            }
            return ((NodeInterface)NodeI)
                .Get( nodeType, nodeId);
        }
        
        public MultiPartyCall.MultiPartyCall MultiPartyCall( string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                throw new PlivoValidationException("nodeId is mandatory, can not be null or empty");
            }
            return ((MultiPartyCall.MultiPartyCallInterface)MultiPartyCallI)
                .Get(_phloId, nodeId);
        }

        public Phlo Get(string phloId)
        {
            if (string.IsNullOrEmpty(phloId))
            {
                throw new PlivoValidationException("phloId is mandatory, can not be null or empty");
            }
            _phloId = phloId;
            var phlo = Task.Run(async () => await GetResource<Phlo>("phlo/" + phloId, new Dictionary<string, object>()).ConfigureAwait(false)).Result;
            phlo.Interface = this;
            phlo._node = new Lazy<NodeInterface>(() => new NodeInterface(Client, phloId));
            phlo._multiPartyCall = new Lazy<MultiPartyCall.MultiPartyCallInterface>(() => new MultiPartyCall.MultiPartyCallInterface(Client, phloId));
            return phlo;
        }

        public string GetAccountID()
        {
            var api = new PlivoApi(_authId, _authToken);
            var accountId = api.Account.Get().Id;
            return accountId;
        }
        
    }
    
}
