using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotNetCasClient.Configuration;

namespace DotNetCasClient.Validation
{
  /// <summary>
  /// Abstract base class for all CAS protocol-based validation strategy classes.
  /// <author>Marvin S. Addison</author>
  /// </summary>
  abstract class AbstractCasProtocolValidator : AbstractUrlTicketValidator
  {
    private const string DEFAULT_ARTIFACT = "ticket";
    private const string DEFAULT_SERVICE = "service";

    protected AbstractCasProtocolValidator(CasClientConfiguration config) : base(config) { }

    protected override string DefaultArtifactParameterName
    {
      get { return DEFAULT_ARTIFACT; }
    }
  
    protected override string DefaultServiceParameterName
    {
    	get { return DEFAULT_SERVICE; }
    }
  }
}
