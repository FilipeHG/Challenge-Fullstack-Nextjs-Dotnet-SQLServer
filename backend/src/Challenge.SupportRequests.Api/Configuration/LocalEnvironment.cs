namespace Challenge.SupportRequests.Api.Configuration;

internal static class LocalEnvironment
{
    public static void Load()
    {
        // Resolve backend/.env from the project, repository or published working directory.
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory()); directory is not null; directory = directory.Parent)
        {
            var candidates = new[] { Path.Combine(directory.FullName, ".env"), Path.Combine(directory.FullName, "backend", ".env") };
            var path = candidates.FirstOrDefault(File.Exists);
            if (path is null) continue;
            foreach (var line in File.ReadLines(path))
            {
                var value = line.Trim();
                if (value.Length == 0 || value.StartsWith('#')) continue;
                var separator = value.IndexOf('=');
                if (separator <= 0) continue;
                var key = value[..separator].Trim();
                var content = value[(separator + 1)..].Trim();
                if (content.Length >= 2 && ((content[0] == '"' && content[^1] == '"') || (content[0] == '\'' && content[^1] == '\'')))
                    content = content[1..^1];
                if (Environment.GetEnvironmentVariable(key) is null)
                    Environment.SetEnvironmentVariable(key, content);
            }
            break;
        }
    }
}
