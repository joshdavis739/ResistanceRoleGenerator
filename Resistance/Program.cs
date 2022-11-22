using BusinessLogic;

var roleAssignmentGenerator = new RoleAssignmentGenerator();

string? line;

var players = new List<string>();
bool isNotSpace;

do
{
    line = Console.ReadLine();

    isNotSpace = !string.IsNullOrWhiteSpace(line);

    if (!isNotSpace)
    {
        players.Add(line!.Trim());
    }
} while (!isNotSpace);

// TODO literally everything TBH

// roleAssignmentGenerator.GenerateRoleAssignments()
