using Newtonsoft.Json;

namespace Givt.Platform.Common.Exceptions;

public class NotFoundException : GivtException
{
    public string EntityName { get; }
    public object LookUpData { get; }

    public NotFoundException(string entityName, object lookUpData) : base($"Entity {entityName} with request {JsonConvert.SerializeObject(lookUpData)} not found")
    {
        EntityName = entityName;
        LookUpData = lookUpData;

        AdditionalInformation.Add(nameof(EntityName), entityName);
        AdditionalInformation.Add(nameof(LookUpData), lookUpData);
    }
    public override int ErrorCode => 404;
}