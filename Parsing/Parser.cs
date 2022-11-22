using BusinessLogic.Models;
using Microsoft.Extensions.Configuration;

namespace Parsing;

public class Parser
{
    private readonly IDictionary<string, string> _initialsToPlayers;

    public Parser(IConfiguration config) =>
        _initialsToPlayers = config
            .GetSection("PlayerMap")
            .GetChildren()
            .ToDictionary(
                o => o.Key,
                o => o.Value ?? throw new Exception());

    public ICollection<Player> ParsePlayers()
    {
        string? line;

        var initials = new List<string>();
        bool isNotSpace;

        do
        {
            Console.WriteLine("Please enter initials of participating players.");

            line = Console.ReadLine();

            isNotSpace = !string.IsNullOrWhiteSpace(line);

            if (isNotSpace)
            {
                if (line!.Contains(','))
                {
                    initials = line.Split(',')
                        .Select(p => p.Trim())
                        .ToList();

                    break;
                }

                initials.Add(line!.Trim());
            }
        } while (isNotSpace);

        return initials
            .Select(i => _initialsToPlayers.TryGetValue(i.ToUpperInvariant(), out var name)
                ? name
                : null)
            .Where(name => name != null)
            .Select(name => new Player(name!))
            .ToList();
    }

    public void ParseGameMode()
    {

    }
}

