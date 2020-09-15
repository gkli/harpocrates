﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Harpocrates.Runtime.Common.Contracts
{
    public class FormattedProcessRequest : ProcessRequest
    {
        public enum RequestedAction
        {
            DoNothing,
            Rotate,
            Cleanup
        }

        public enum SecretEvent
        {
            Unknown,
            Created,
            Expiring,
            Expired
        }

        public enum SecretType
        {
            Unknown,
            Secret,
            Certificate,
            Key
        }

        public FormattedProcessRequest(string json, RequestedAction action) : base(json)
        {
            Action = action;

            //ResetAttemptCount();
        }

        public RequestedAction Action { get; private set; }

        public string ObjectUri { get; set; }

        public string VaultName { get; set; }
        public string ObjectName { get; set; }
        public SecretType ObjectType { get; set; }

        public SecretEvent Event { get; set; }

    }
}