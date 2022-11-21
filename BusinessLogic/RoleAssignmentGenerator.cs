using AutoFixture;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using EntityModel;
using Moq;
using Xunit;

namespace BusinessLogic
{
    public class RoleAssignmentGenerator
    {
        private Randomizer<Player> _playerRandomizer = new();
        private Randomizer<Role> _wildcardPoolRandomizer = new();
        private Fixture _fixture = new();

        internal ICollection<RoleAssignment> GenerateRoleAssignments(
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
                .Select(x => new RoleAssignment
                {
                    Player = x.First,
                    Role = x.Second
                });

            var playersToAssignRoleToFromWildcardPool = randomizedPlayerList.Skip(gameMode.Roles.Count);

            var wildcardRoleAssignments = playersToAssignRoleToFromWildcardPool
                .Select(x =>
                {
                    var wildcard = _wildcardPoolRandomizer
                        .Randomize(gameMode.WildcardPool)
                        .First();

                    return new RoleAssignment
                    {
                        Player = x,
                        Role = wildcard
                    };
                });

            return directRoleAssignments.Concat(wildcardRoleAssignments).ToArray();
        }

        [Fact]
        protected void GenerateRoleAssignments_ShouldReturnEmptyCollectionOfRoleAssignmentsWhenNoPlayers()
        {
            var gameMode = new Mock<GameMode>();
            gameMode.Setup(x => x.Roles.Count).Returns(It.IsAny<int>());
            gameMode.Setup(x => x.NumberOfWildcards).Returns(It.IsAny<int>());

            GenerateRoleAssignments(new List<Player>(), gameMode.Object);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1, 0, 0)]
        [InlineData(2, 1, 2)]
        [InlineData(6, 5, 2)]
        protected void GenerateRoleAssignments_ShouldThrowIfTotalPlayerCountIsDifferentToNumberOfRolesPlusWildcards(
            int numberOfPlayers,
            int numberOfRoles,
            int numberOfWildcards)
        {
            var players = new Mock<ICollection<Player>>();
            players.Setup(x => x.Count).Returns(numberOfPlayers);

            var gameMode = new Mock<GameMode>();
            gameMode.Setup(x => x.Roles.Count).Returns(numberOfRoles);
            gameMode.Setup(x => x.NumberOfWildcards).Returns(numberOfWildcards);

            Assert.ThrowsAny<InvalidNumberOfPlayersException>(delegate { GenerateRoleAssignments(players.Object, gameMode.Object); });
        }

        [Fact]
        protected void GenerateRoleAssignments_ShouldThrowIfNumberOfWildcardsPositiveButNoWildcardsInPool()
        {
            var players = new Mock<ICollection<Player>>();
            players.Setup(x => x.Count).Returns(1);

            var gameMode = new Mock<GameMode>();
            gameMode.Setup(x => x.Roles.Count).Returns(0);
            gameMode.Setup(x => x.NumberOfWildcards).Returns(1);
            gameMode.Setup(x => x.WildcardPool).Returns(new List<Role>());

            Assert.ThrowsAny<WildcardPoolEmptyException>(delegate { GenerateRoleAssignments(players.Object, gameMode.Object); });
        }

        [Fact]
        protected void GenerateRoleAssignments_ShouldRandomizePlayerList()
        {
            var players = _fixture.CreateMany<Player>(1).ToArray();
            var roles = _fixture.CreateMany<Role>(1).ToArray();

            var gameMode = new Mock<GameMode>();
            gameMode.Setup(x => x.Roles).Returns(roles);
            gameMode.Setup(x => x.NumberOfWildcards).Returns(0);
            gameMode.Setup(x => x.WildcardPool).Returns(new List<Role>());

            var randomizer = new Mock<Randomizer<Player>>();
            randomizer
                .Setup(x => x.Randomize(It.IsAny<ICollection<Player>>()))
                .Returns(players.OrderBy(x => x));
            _playerRandomizer = randomizer.Object;

            GenerateRoleAssignments(players, gameMode.Object);

            randomizer.Verify(x => x.Randomize(It.IsAny<ICollection<Player>>()), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        protected void GenerateRoleAssignments_ShouldRandomizeWildcardPoolForEachWildcardPlayer(
            int numberOfWildcards)
        {
            var numberOfRoles = _fixture.Create<int>();

            var players = _fixture.CreateMany<Player>(numberOfRoles + numberOfWildcards).ToArray();
            var roles = _fixture.CreateMany<Role>(numberOfRoles).ToArray();
            var wildcardPool = _fixture.CreateMany<Role>(numberOfWildcards).ToArray();

            var gameMode = new Mock<GameMode>();
            gameMode.Setup(x => x.Roles).Returns(roles);
            gameMode.Setup(x => x.NumberOfWildcards).Returns(numberOfWildcards);
            gameMode.Setup(x => x.WildcardPool).Returns(wildcardPool);

            var randomizer = new Mock<Randomizer<Role>>();
            randomizer
                .Setup(x => x.Randomize(It.IsAny<ICollection<Role>>()))
                .Returns(wildcardPool.OrderBy(x => x.Id));
            _wildcardPoolRandomizer = randomizer.Object;

            GenerateRoleAssignments(players, gameMode.Object);

            randomizer.Verify(x => x.Randomize(It.IsAny<ICollection<Role>>()), Times.Exactly(numberOfWildcards));
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(5, 0)]
        [InlineData(5, 1)]
        protected void GenerateRoleAssignments_NumberOfAssigmentsShouldMatchNumberOfPlayers(
            int numberOfPlayers,
            int numberOfWildcards)
        {
            var players = _fixture.CreateMany<Player>(numberOfPlayers).ToArray();
            var wildcardPool = _fixture.CreateMany<Role>(numberOfWildcards).ToArray();
            var roles = _fixture.CreateMany<Role>(numberOfPlayers - numberOfWildcards).ToArray();

            var gameMode = new Mock<GameMode>();

            gameMode.Setup(x => x.Roles).Returns(roles);
            gameMode.Setup(x => x.NumberOfWildcards).Returns(numberOfWildcards);
            gameMode.Setup(x => x.WildcardPool).Returns(wildcardPool);

            var assignments = GenerateRoleAssignments(players, gameMode.Object);

            Assert.Equal(numberOfPlayers, assignments.Count);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(5, 1)]
        protected void GenerateRoleAssignments_ShouldHaveEachPlayerMatchedToARole(
            int numberOfPlayers,
            int numberOfWildcards)
        {
            var players = _fixture.CreateMany<Player>(numberOfPlayers).ToArray();
            var wildcardPool = _fixture.CreateMany<Role>(numberOfWildcards).ToArray();
            var roles = _fixture.CreateMany<Role>(numberOfPlayers - numberOfWildcards).ToArray();

            var gameMode = new Mock<GameMode>();

            gameMode.Setup(x => x.Roles).Returns(roles);
            gameMode.Setup(x => x.NumberOfWildcards).Returns(numberOfWildcards);
            gameMode.Setup(x => x.WildcardPool).Returns(wildcardPool);

            var assignments = GenerateRoleAssignments(players, gameMode.Object);

            Assert.All(assignments, x =>
            {
                Assert.NotNull(x.Role);
            });
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(5, 1)]
        protected void GenerateRoleAssignments_ShouldHaveEachRoleMatchedToAPlayer(
            int numberOfPlayers,
            int numberOfWildcards)
        {
            var players = _fixture.CreateMany<Player>(numberOfPlayers).ToArray();
            var wildcardPool = _fixture.CreateMany<Role>(numberOfWildcards).ToArray();
            var roles = _fixture.CreateMany<Role>(numberOfPlayers - numberOfWildcards).ToArray();

            var gameMode = new Mock<GameMode>();

            gameMode.Setup(x => x.Roles).Returns(roles);
            gameMode.Setup(x => x.NumberOfWildcards).Returns(numberOfWildcards);
            gameMode.Setup(x => x.WildcardPool).Returns(wildcardPool);

            var assignments = GenerateRoleAssignments(players, gameMode.Object);

            Assert.All(roles, x =>
            {
                Assert.Contains(x, assignments.Select(y => y.Role));
            });
        }
    }
}