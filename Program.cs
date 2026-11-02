namespace NxLogAnalyzer;

class Program
{
    static void Main(string[] args) {
        RawEventParser parser = new RawEventParser();
        string pathToFile = "licence.txt";
        List<FullLicenceEntry> fullEventsList = new List<FullLicenceEntry>();
        foreach (var line in File.ReadLines(pathToFile) )
        {
            var parsedLine = parser.FullParse(line);
            if (parsedLine is not null)
            {
                fullEventsList.Add(parsedLine);
            }
        }
        

    }
}