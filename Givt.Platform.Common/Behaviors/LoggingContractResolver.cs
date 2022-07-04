using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Givt.Donations.Infrastructure.Behaviors
{
    internal class LoggingContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            //   property.ShouldSerialize = o =>  member.GetCustomAttribute<ExcludeFromLoggingAttribute>() == null;
            return property;
        }

        protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
        {
            if (member.GetCustomAttribute<ExcludeFromLoggingAttribute>() != null)
                return new ExcludeValueProvider(member);

            return base.CreateMemberValueProvider(member);
        }
    }
}
