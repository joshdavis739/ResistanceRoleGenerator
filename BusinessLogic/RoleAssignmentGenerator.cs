using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using EntityModel;

namespace BusinessLogic
{
    public class RoleAssignmentGenerator
    {
        protected Randomizer<Player> _playerRandomizer = new();
        protected Randomizer<Role> _wildcardPoolRandomizer = new();

        public ICollection<RoleAssignment> GenerateRoleAssignments(
            ICollection<Player> players,
            GameMode gameMode)
        {
            if (players.Count != gameMode.Roles.Count + gameMode.NumberOfWildcards)
            {
                throw new InvalidNumberOfPlayersException();
            }

            if (gameMode.NumberOfWildcards > 0 && gameMode.WildcardPool.Count == 0)
            {
                throw new WildcardPoolEmptyException();
            }

            var randomizedPlayerList = _playerRandomizer.Randomize(players);

            var playersToAssignRoleDirectlyTo = randomizedPlayerList.Take(gameMode.Roles.Count);

            var directRoleAssignments = playersToAssignRoleDirectlyTo
                .Zip(gameMode.Roles)
                .Select(x => new RoleAssignment(x.First, x.Second));

            var playersToAssignRoleToFromWildcardPool = randomizedPlayerList.Skip(gameMode.Roles.Count);

            var wildcardRoleAssignments = playersToAssignRoleToFromWildcardPool
                .Select(x =>
                {
                    var wildcard = _wildcardPoolRandomizer
                        .Randomize(gameMode.WildcardPool)
                        .First();

                    return new RoleAssignment(x, wildcard);
                });

            return directRoleAssignments.Concat(wildcardRoleAssignments).ToArray();
        }
    }
}
