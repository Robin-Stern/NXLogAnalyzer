namespace NxLogAnalyzer;

public record FullLicenceEntry(DateTime CheckoutTime, string LicenceName, string LicenceTaker, DateTime? ReturnTime);
public record LicenceEvent (TimeOnly? CheckoutTime, TimeOnly? ReturnTime, string LicenceName, string LicenceTaker);
public record RawEvent(TimeOnly CheckoutTime, string LicenceName, string LicenceTaker, EventType Kind);