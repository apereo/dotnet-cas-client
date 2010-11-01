using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCasClient.Validation.TicketValidator
{
    abstract class AbstractCasProtocolTicketValidator : AbstractUrlTicketValidator
    {
        private const string CAS_ARTIFACT_PARAM = "ticket";
        private const string CAS_SERVICE_PARAM = "service";

        public override string ArtifactParameterName
        {
            get { return CAS_ARTIFACT_PARAM; }
        }
        
        public override string  ServiceParameterName
        {
            get { return CAS_SERVICE_PARAM; }
        }
    }
}
