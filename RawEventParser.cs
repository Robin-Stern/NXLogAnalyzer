using System.Globalization;

namespace NxLogAnalyzer;

public class RawEventParser
{
    DateOnly? _currentDate = null;


    //Beispiel String 15:23:44 (ugslmd) DENIED: "NX13100N" jdoe@WS01  (Licensed number of users already reached. (-4,342))
    private static bool TryParseEvent(string line, out RawEvent? result)
    {
        result = null;
        //Typ Erkennung
        int parenClose = line.IndexOf(')');
        if (parenClose == -1) return false;

        string afterParen = line.Substring(parenClose + 1); //substring erzeugt für jeden einen neuen Index
        int colon = afterParen.IndexOf(':');
        if (colon == -1) return false;

        string typToken = afterParen.Substring(0, colon).Trim();
        if (!Enum.TryParse<EventType>(typToken, true, out var kind)) return false;
        //Ergebnis DENIED
        // ab hier: TYP erkannt, Rest identisch für Out/In/Denied
        int parenOpen = line.IndexOf('(');
        if (parenOpen == -1) return false;
        if (!TimeOnly.TryParse(line.Substring(0, parenOpen).Trim(), out var timestamp)) return false;

        string afterColon = afterParen.Substring(colon + 1);
        int q1 = afterColon.IndexOf('"');
        if (q1 == -1) return false;
        int q2 = afterColon.IndexOf('"', q1 + 1);
        if (q2 == -1) return false;

        string licenceName = afterColon.Substring(q1 + 1, q2 - q1 - 1);
        string licenceTaker =
            afterColon.Substring(q2 + 1).Trim().Split(' ')[0]; //String ist Array, wir nehmen erstes Element

        result = new RawEvent(timestamp, licenceName, licenceTaker, kind);
        return true;
    }

    //Beispiel String 0:01:04 (ugslmd) TIMESTAMP 10/19/2025
    private static bool TryParseTimestamp(string line, out DateOnly date)
    {
        date = default;
        int parenClose = line.IndexOf(')');
        if (parenClose == -1) return false;

        string afterParen = line.Substring(parenClose + 1).Trim();
        const string marker = "TIMESTAMP ";
        if (!afterParen.StartsWith(marker)) return false;

        //FlexLM schreibt immer M/d/yyyy (US-Format), unabhängig von der Systemsprache
        string dateToken = afterParen.Substring(marker.Length).Trim();
        return DateOnly.TryParseExact(dateToken, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
            out date);
    }

    public FullLicenceEntry? FullParse(string line)
    {

        if (TryParseTimestamp(line, out var date))
        {
            _currentDate = date;
            return null;
        }

        var parsedEvent = TryParseEvent(line, out var result);

        if (parsedEvent)
        {
                var fullEvent = new FullLicenceEntry(

                    LicenceName: result.LicenceName,
                    LicenceTaker: result.LicenceTaker,
                    CheckoutTime: result.CheckoutTime,
                    CurrentDate: date,
                    ReturnTime: null,
                    Kind: result.Kind
                );
                return fullEvent;
        }

        return null;
    }
}


   