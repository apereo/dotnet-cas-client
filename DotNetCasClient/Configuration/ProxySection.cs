using System.Configuration;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;

[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
public sealed class AuthorizationSection : ConfigurationSection
{
    // Fields
    private bool _EveryoneAllowed;
    private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private static readonly ConfigurationProperty _propRules = new ConfigurationProperty(null, typeof(AuthorizationRuleCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

    // Methods
    static AuthorizationSection()
    {
        _properties.Add(_propRules);
    }

    protected override void PostDeserialize()
    {
        if (Rules.Count > 0)
        {
            // _EveryoneAllowed = (Rules[0].Action == AuthorizationRuleAction.Allow) && Rules[0].Everyone;
        }
    }

    // Properties
    internal bool EveryoneAllowed
    {
        get
        {
            return _EveryoneAllowed;
        }
    }

    protected override ConfigurationPropertyCollection Properties
    {
        get
        {
            return _properties;
        }
    }

    [ConfigurationProperty("", IsDefaultCollection = true)]
    public AuthorizationRuleCollection Rules
    {
        get
        {
            return (AuthorizationRuleCollection)base[_propRules];
        }
    }
}

