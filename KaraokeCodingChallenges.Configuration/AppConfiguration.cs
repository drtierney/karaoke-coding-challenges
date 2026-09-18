using System;
using System.Collections.Generic;
using System.Text;

namespace KaraokeCodingChallenges.Configuration
{
    public class AppConfiguration
    {
        public required IReadOnlyCollection<LibrarySource> LibrarySources { get; init; }
    }
}
