using System;
using System.Collections.Generic;
using System.Linq;
using JasigCasClient.Utils;
using log4net;

namespace JasigCasClient.Security
{
    /// <summary>
    /// Represents a CAS response to a validation request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the .Net port of org.jasig.cas.client.validation.AssertionImpl
    /// </para>
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    [Serializable]
    public class Assertion : IAssertion
    {
        static readonly ILog log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region IAssertion Members

        /// <summary>
        /// The date from which this Assertion is valid.
        /// </summary>
        public DateTime ValidFromDate { get; private set; }

        /// <summary>
        /// The date this Assertion is valid until.
        /// </summary>
        public DateTime ValidUntilDate { get; private set; }

        /// <summary>
        /// key/value pairs for the attributes associated with this Assertion.
        /// e.g. authentication type.
        /// </summary>
        public ILookup<string, IList<string>> Attributes { get; private set; }

        /// <summary>
        /// The PrincipalName for which this Assertion is valid.
        /// </summary>
        public string PrincipalName { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Assertion with the supplied Principal name, a ValidFromDate of now,
        /// no ValidUntilDate, and no attributes.
        /// </summary>
        /// <param name="principalName">the Principal name associated with this Assertion.</param>
        public Assertion(string principalName) : 
          this(principalName, new Dictionary<string, IList<string>>()) { }

        /// <summary>
        /// Creates a new Assertion with the supplied principal name and Assertion attributes,
        /// a ValidFromDate of now, and no ValidUntilDate.
        /// </summary>
        /// <param name="principalName">the Principal name associated with this Assertion.</param>
        /// <param name="attributes">
        /// the key/value pairs for the attributes to associate with this Assertion.
        /// </param>
        public Assertion(string principalName, IDictionary<string, IList<string>> attributes)
        {
            CommonUtils.AssertNotNull(principalName, "principalName cannot be null.");
            CommonUtils.AssertNotNull(attributes, "attributes cannot be null.");

            this.PrincipalName = principalName;
            this.ValidFromDate = DateTime.Now;
            this.Attributes = attributes.ToLookup(x => x.Key, x => x.Value);
        }
            
        /// <summary>
        /// Creates a new Assertion with the supplied principal name, Assertion attributes,
        /// ValidFromDate, and ValidUntilDate.
        /// </summary>
        /// <param name="principalName">the principal name associated with this Assertion.</param>
        /// <param name="validFromDate">The date from which this Assertion is valid.</param>
        /// <param name="validUntilDate">The date this assertion is valid until.</param>
        /// <param name="attributes">
        /// the key/value pairs for the attributes to associate with this Assertion.
        /// </param>
        public Assertion(string principalName,
                         DateTime validFromDate,
                         DateTime validUntilDate,
                         IDictionary<string, IList<string>> attributes)
        {
            CommonUtils.AssertNotNull(principalName, "principalName cannot be null.");
            CommonUtils.AssertNotNull(validFromDate, "validFromDate cannot be null.");
            CommonUtils.AssertNotNull(attributes, "attributes cannot be null.");

            this.PrincipalName = principalName;
            this.ValidFromDate = validFromDate;
            this.ValidUntilDate = validUntilDate;
            this.Attributes = attributes.ToLookup(x => x.Key, x => x.Value);
        }

        #endregion
    }
}
