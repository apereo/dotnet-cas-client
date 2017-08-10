/*
 * Licensed to Apereo under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Apereo licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

#pragma warning disable 1591

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Cas20.ClearPass
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.yale.edu/tp/cas")]
    [XmlRoot("clearPassResponse", Namespace = "http://www.yale.edu/tp/cas", IsNullable = false)]
    public class ClearPassResponse
    {
        internal ClearPassResponse() { }

        public static ClearPassResponse ParseResponse(string responseXml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ClearPassResponse));
            using (StringReader sr = new StringReader(responseXml))
            {
                return (ClearPassResponse) xs.Deserialize(sr);
            }
        }

        [XmlElement("clearPassFailure", typeof(ClearPassFailure))]
        [XmlElement("clearPassSuccess", typeof(ClearPassSuccess))]
        public object Item
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool IsClearPassSuccess
        {
            get
            {
                return (Item != null && Item is ClearPassSuccess);
            }
        }

        [XmlIgnore]
        public bool IsClearPassFailure
        {
            get
            {
                return (Item != null && Item is ClearPassFailure);
            }
        }    
    }
}

#pragma warning restore 1591