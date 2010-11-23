using System.Diagnostics;

namespace DotNetCasClient.Logging
{
    /// <summary>
    /// Defines a logging category.
    /// </summary>
    public class Category
    {
        private static Category config = new Category("DotNetCasClient.Config");
        private static Category httpModule = new Category("DotNetCasClient.HttpModule");
        private static Category protocol = new Category("DotNetCasClient.Protocol");
        private static Category security = new Category("DotNetCasClient.Security");

        private TraceSource source;

        private Category(string name)
        {
            source = new TraceSource(name);
        }

        internal TraceSource Source
        {
            get { return source; }
        }

        /// <summary>
        /// Gets the category name of the logger.
        /// </summary>
        public string Name
        {
            get { return source.Name; }
        }

        /// <summary>
        /// Gets the Config category. 
        /// </summary>
        public static Category Config
        {
            get { return config; }
        }

        /// <summary>
        /// Gets the HttpModule category.
        /// </summary>
        public static Category HttpModule
        {
            get { return httpModule; }
        }

        /// <summary>
        /// Gets the Protocol category.
        /// </summary>
        public static Category Protocol
        {
            get { return protocol; }
        }

        /// <summary>
        /// Gets the Security category.
        /// </summary>
        public static Category Security
        {
            get { return security; }
        }
    }
}