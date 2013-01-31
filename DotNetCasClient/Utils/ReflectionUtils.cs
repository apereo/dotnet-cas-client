/*
 * Licensed to Jasig under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Jasig licenses this file to you under the Apache License,
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


using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// Utility methods for performing reflection lookups
    /// </summary>
    /// <author>Eric Domazlicky</author>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Checks all assemblies in the AppDomain that implement an interface
        /// </summary>
        /// <param name="ainterface">the object to check</param>
        /// <returns>
        /// A list of Type with all types that match the provided interface
        /// </returns> 
        /// <remarks>
        /// This is an intrinsically slow function, meant to be called only on initialization when delays are acceptable
        /// </remarks>
        public static List<Type> FindAllTypesWithInterface(Type ainterface)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> retvalue = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (ainterface.IsAssignableFrom(t))
                        retvalue.Add(t);
                }
            }
            return retvalue;
        }

        /// <summary>
        /// Gets a type from a list of types
        /// </summary>
        /// <param name="typename">the type name string</param>
        /// <param name="assumed_prefix">if present, then the code will look for assumed_prefix+typename in the type list</param>
        /// <param name="types">list of types</param>
        /// <param name="possible_aliases">an optional dictionary of type aliases used to maintain backwards-compatability or for user-friendliness</param>
        /// <returns>
        /// the Type or null if not found
        /// </returns>         
        public static Type GetTypeFromList(string typename, string assumed_prefix, List<Type> types,Dictionary<String,String> possible_aliases = null)
        {
            string target_type = typename;
            if (possible_aliases != null)
            {
                if (possible_aliases.ContainsKey(typename))
                    target_type = possible_aliases[typename];
            }
            // if we have an assumed prefix and the caller did not pass a fully qualified type add the prefix
            if (!String.IsNullOrEmpty(assumed_prefix) && (target_type.IndexOf(".") < 0))
                target_type = assumed_prefix + "." + target_type;
            foreach(Type t in types)
            {
                if(String.Compare(t.ToString(),target_type)==0)
                    return t;
            }
            return null;
        }


    }
}
