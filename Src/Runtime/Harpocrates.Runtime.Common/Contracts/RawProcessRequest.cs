using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Contracts
{
    public class RawProcessRequest : ProcessRequest
    {
        private static class KnownEvents
        {
            public static class Secret
            {
                public const string Expired = "Microsoft.KeyVault.SecretExpired";
                public const string Expiring = "Microsoft.KeyVault.SecretNearExpiry";
                public const string Created = "Microsoft.KeyVault.SecretNewVersionCreated";
            }
        }
        public RawProcessRequest(string json) : base(json) { }
        //TODO: Figure out how to handle Key / Certificate events... do we just ignore those?
        public FormattedProcessRequest FormatRequest()
        {
            JObject item;
            try
            {
                item = JObject.Parse(OriginalMessageJson);
            }
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(OriginalMessageJson));
                item = JObject.Parse(json);
                OriginalMessageJson = json;
            }

            FormattedProcessRequest.RequestedAction action = FormattedProcessRequest.RequestedAction.DoNothing;
            FormattedProcessRequest.SecretEvent et = FormattedProcessRequest.SecretEvent.Unknown;

            string eventType = item.GetValue("eventType").ToString();

            if (!string.IsNullOrWhiteSpace(eventType))
            {
                switch (eventType)
                {
                    case KnownEvents.Secret.Created:
                        et = FormattedProcessRequest.SecretEvent.Created;
                        action = FormattedProcessRequest.RequestedAction.ScheduleDependencyUpdates;
                        break;
                    case KnownEvents.Secret.Expiring:
                        et = FormattedProcessRequest.SecretEvent.Expiring;
                        action = FormattedProcessRequest.RequestedAction.Rotate;
                        break;
                    case KnownEvents.Secret.Expired:
                        et = FormattedProcessRequest.SecretEvent.Expired;
                        action = FormattedProcessRequest.RequestedAction.Cleanup;
                        break;
                }
            }

            FormattedProcessRequest request = new FormattedProcessRequest(OriginalMessageJson, action)
            {
                Event = et,
                ObjectType = FormattedProcessRequest.SecretType.Unknown
            };

            string topic = item.GetValue("topic").ToString();

            if (!string.IsNullOrWhiteSpace(topic))
            {
                string startToken = "/subscriptions/", endToken = "/resourceGroups/";

                int startIndex = topic.IndexOf(startToken);
                if (startIndex >= 0)
                {
                    startIndex += startToken.Length;
                    int endIndex = topic.IndexOf(endToken, startIndex);
                    if (endIndex > startIndex)
                    {
                        request.SubscriptionId = topic.Substring(startIndex, endIndex - startIndex);
                    }
                }
            }

            JObject data = item.GetValue("data") as JObject;

            if (null != data)
            {
                request.ObjectUri = data.GetValue("Id").ToString();
                request.ObjectName = data.GetValue("ObjectName").ToString();
                request.VaultName = data.GetValue("VaultName").ToString();

                string ot = data.GetValue("ObjectType").ToString();
                if (false == string.IsNullOrWhiteSpace(ot))
                {
                    switch (ot.ToLower())
                    {
                        case "secret":
                            request.ObjectType = FormattedProcessRequest.SecretType.Secret;
                            break;
                        case "certificate":
                            request.ObjectType = FormattedProcessRequest.SecretType.Certificate;
                            break;
                        case "key":
                            request.ObjectType = FormattedProcessRequest.SecretType.Key;
                            break;
                    }
                }
                return request;
            }

            return null;


        }
    }
}
