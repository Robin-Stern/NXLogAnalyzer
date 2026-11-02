namespace NxLogAnalyzer;

public record FullLicenceEntry(string LicenceName, string LicenceTaker, TimeOnly? ReturnTime, TimeOnly CheckoutTime, DateOnly? CurrentDate, EventType Kind );
public record RawEvent(TimeOnly CheckoutTime, string LicenceName, string LicenceTaker, EventType Kind);