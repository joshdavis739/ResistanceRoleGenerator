namespace BusinessLogic.Models
{
    public class Player
    {
        public Player(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
