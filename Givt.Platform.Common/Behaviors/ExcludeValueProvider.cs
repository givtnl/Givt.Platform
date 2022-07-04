using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Givt.Donations.Infrastructure.Behaviors;

internal class ExcludeValueProvider : IValueProvider
{
    private readonly MemberInfo _memberInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectionValueProvider"/> class.
    /// </summary>
    /// <param name="memberInfo">The member info.</param>
    public ExcludeValueProvider(MemberInfo memberInfo)
    {
        _memberInfo = memberInfo;
    }
    public void SetValue(object target, object value)
    {
        target.GetType().GetProperty(_memberInfo.Name)?.SetValue(target, "****");
    }

    public object GetValue(object target)
    {
        var value = target.GetType().GetProperty(_memberInfo.Name)?.GetValue(target);
        return value == null ? null : new string('*', value.ToString()!.Length);
    }
}
