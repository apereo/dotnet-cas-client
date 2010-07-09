/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.Collections.Generic;
using DotNetCasClient.Utils;

namespace DotNetCasClient.Security
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
        #region IAssertion Members
        /// <summary>
        /// The date from which this Assertion is valid.
        /// </summary>
        public DateTime ValidFromDate
        {
            get; 
            private set;
        }

        /// <summary>
        /// The date this Assertion is valid until.
        /// </summary>
        public DateTime ValidUntilDate
        {
            get; 
            private set;
        }

        /// <summary>
        /// key/value pairs for the attributes associated with this Assertion.
        /// e.g. authentication type.
        /// </summary>
        public Dictionary<string, IList<string>> Attributes
        {
            get; 
            private set;
        }

        /// <summary>
        /// The PrincipalName for which this Assertion is valid.
        /// </summary>
        public string PrincipalName
        {
            get; 
            private set;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Assertion with the supplied Principal name, a
        /// ValidFromDate of now, no ValidUntilDate, and no attributes.
        /// </summary>
        /// <param name="principalName">
        /// the Principal name associated with this Assertion.
        /// </param>
        public Assertion(string principalName) : this(principalName, new Dictionary<string, IList<string>>()) { }

        /// <summary>
        /// Creates a new Assertion with the supplied principal name and
        /// Assertion attributes, a ValidFromDate of now, and no ValidUntilDate.
        /// </summary>
        /// <param name="principalName">
        /// the Principal name associated with this Assertion.
        /// </param>
        /// <param name="attributes">
        /// the key/value pairs for the attributes to associate with this
        /// Assertion.
        /// </param>
        public Assertion(string principalName, IDictionary<string, IList<string>> attributes)
        {
            CommonUtils.AssertNotNull(principalName, "principalName cannot be null.");
            CommonUtils.AssertNotNull(attributes, "attributes cannot be null.");

            PrincipalName = principalName;
            ValidFromDate = DateTime.Now;
            Attributes = new Dictionary<string, IList<string>>(attributes);
        }

        /// <summary>
        /// Creates a new Assertion with the supplied principal name, Assertion
        /// attributes, ValidFromDate, and ValidUntilDate.
        /// </summary>
        /// <param name="principalName">
        /// the principal name associated with this Assertion.
        /// </param>
        /// <param name="validFromDate">
        /// The date from which this Assertion is valid.
        /// </param>
        /// <param name="validUntilDate">
        /// The date this assertion is valid until.
        /// </param>
        /// <param name="attributes">
        /// the key/value pairs for the attributes to associate with this
        /// Assertion.
        /// </param>
        public Assertion(string principalName, DateTime validFromDate, DateTime validUntilDate, IDictionary<string, IList<string>> attributes)
        {
            CommonUtils.AssertNotNull(principalName, "principalName cannot be null."); 
            CommonUtils.AssertNotNull(validFromDate, "validFromDate cannot be null.");
            CommonUtils.AssertNotNull(attributes, "attributes cannot be null.");

            PrincipalName = principalName;
            ValidFromDate = validFromDate;
            ValidUntilDate = validUntilDate;
            Attributes = new Dictionary<string, IList<string>>(attributes);
        }
        #endregion
    }
}
