using System;

namespace Rsk.Samples.AspNetCore.Authentication.Saml2p.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}