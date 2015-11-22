using System.Collections.Generic;

namespace HearthstoneCards.Model
{
    public class ReleaseNotes
    {
        public string Version { get; set; }
        public IList<string> Notes { get; set; }

        public ReleaseNotes(string version, IList<string> notes)
        {
            Version = version;
            Notes = notes;
        }

        public static IList<ReleaseNotes> GetReleaseNotes()
        {
            return new List<ReleaseNotes>
            {
                new ReleaseNotes("1.0.0.0", new List<string>
                {
                    "Initial release."
                })
            };
        }
    }
}
