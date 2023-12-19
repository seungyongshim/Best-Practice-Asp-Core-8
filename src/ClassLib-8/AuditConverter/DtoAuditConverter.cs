namespace ClassLib_8.AuditConverter;

public interface IDtoAuditConverter<T>
{
    public IDictionary<string, string> ToConvert(object obj, Type type);
}
