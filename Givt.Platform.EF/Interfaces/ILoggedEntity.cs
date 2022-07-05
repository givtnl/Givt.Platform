namespace Givt.Platform.EF.Interfaces;

// a marker interface that changes to this item need to be duplicated in a history table
public interface ILoggedEntity
{
    Type HistoryEntityType { get; }
}