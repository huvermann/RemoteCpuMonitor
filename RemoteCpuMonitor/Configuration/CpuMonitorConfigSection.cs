using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteCpuMonitor.Configuration
{
    public class CpuMonitorConfigSection : ConfigurationSection, ICpuMonitorConfigSection
    {
        public static ICpuMonitorConfigSection Create()
        {
            return (ICpuMonitorConfigSection)ConfigurationManager.GetSection("MonitorHosts");
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public HostCollection Hosts
        {
            get
            {
                HostCollection hosts = (HostCollection)base[""];
                return hosts;
            }
        }

        public class HostCollection : ConfigurationElementCollection
        {
            public HostCollection()
            {
                HostConfigElement details = (HostConfigElement)CreateNewElement();
                if (!string.IsNullOrEmpty(details.SSHServerHostname))
                {
                    Add(details);
                }
            }


            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.BasicMap;
                }
            }

            protected override ConfigurationElement CreateNewElement()
            {
                return new HostConfigElement();
            }

            public HostConfigElement this[int index]
            {
                get
                {
                    return (HostConfigElement)BaseGet(index);
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

            new public HostConfigElement this[string name]
            {
                get
                {
                    return (HostConfigElement)BaseGet(name);
                }
            }

            public int IndexOf(HostConfigElement details)
            {
                return BaseIndexOf(details);
            }

            public void Add(HostConfigElement details)
            {
                BaseAdd(details);
            }

            protected override void BaseAdd(ConfigurationElement element)
            {
                BaseAdd(element, false);
            }
            public void Remove(HostConfigElement details)
            {
                if (BaseIndexOf(details) >= 0) BaseRemove(details.SSHServerHostname);
            }
            public void RemoveAt(int index)
            {
                BaseRemoveAt(index);
            }

            public void Remove(string name)
            {
                BaseRemove(name);
            }

            public void Clear()
            {
                BaseClear();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((HostConfigElement)element).SSHServerHostname;
            }

            protected override string ElementName
            {
                get
                {
                    return "host";
                }
            }


        }

        public class HostConfigElement : ConfigurationElement
        {
            [ConfigurationProperty("SSHServerHostname", IsRequired = true, IsKey = true)]
            [StringValidator(InvalidCharacters = "  ~!@#$%^&*()[]{}/;’\"|\\")]
            public string SSHServerHostname
            {
                get { return (string)this["SSHServerHostname"]; }
                set { this["SSHServerHostname"] = value; }
            }

            [ConfigurationProperty("username", IsRequired = true)]
            [StringValidator(InvalidCharacters = "  ~!@#$%^&*()[]{}/;’\"|\\")]
            public string Username
            {
                get { return (string)this["username"]; }
                set { this["username"] = value; }
            }

            [ConfigurationProperty("SSHport", IsRequired = false, DefaultValue = 22)]
            [IntegerValidator(MinValue = 1, MaxValue = 65536)]
            public int SSHPort
            {
                get { return (int)this["SSHport"]; }
                set { this["SSHport"] = value; }
            }

            [ConfigurationProperty("password", IsRequired = false)]
            public string Password
            {
                get { return (string)this["password"]; }
                set { this["password"] = value; }
            }

        }
    }
}
