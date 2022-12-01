using EntityModel;

namespace BusinessLogic.Models
{
    public class RoleAssignment
    {
        public RoleAssignment(
            Player player,
            Role role)
        {
            Player = player;
            Role = role;
        }

        public Player Player { get; }

        public Role Role { get; }
    }
}
