using System.Configuration;
using System.Globalization;
using System.Security.Permissions;
using System.Web;

namespace DotNetCasClient.Configuration
{
    [ConfigurationCollection(typeof(ProxyRule), AddItemName = "allow,deny", CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProxyRuleCollection : ConfigurationElementCollection
    {
        // Fields
        private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

        public void Add(ProxyRule rule)
        {
            BaseAdd(-1, rule);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ProxyRule("");
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            ProxyRule rule = new ProxyRule("");
            string str = elementName.ToLower(CultureInfo.InvariantCulture);
            if (str != null)
            {
                if (str != "allow")
                {
                    if (str == "deny")
                    {
                        rule.Action = ProxyRuleAction.Deny;
                    }
                    return rule;
                }
                rule.Action = ProxyRuleAction.Allow;
            }
            return rule;
        }

        public ProxyRule Get(int index)
        {
            return (ProxyRule)BaseGet(index);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            ProxyRule rule = (ProxyRule)element;
            return rule.ActionString;
        }

        public int IndexOf(ProxyRule rule)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Equals(Get(i), rule))
                {
                    return i;
                }
            }
            return -1;
        }

        protected override bool IsElementName(string elementname)
        {
            string str;
            if (((str = elementname.ToLower(CultureInfo.InvariantCulture)) == null) || (str != "allow" && str != "deny"))
            {
                return false;
            }
            return true;
        }

        public void Remove(ProxyRule rule)
        {
            int index = IndexOf(rule);
            if (index >= 0)
            {
                BaseRemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Set(int index, ProxyRule rule)
        {
            BaseAdd(index, rule);
        }

        // Properties
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }

        protected override string ElementName
        {
            get
            {
                return string.Empty;
            }
        }

        public ProxyRule this[int index]
        {
            get
            {
                return (ProxyRule)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }
    }
}
