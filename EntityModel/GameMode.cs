namespace EntityModel
{
    public class GameMode
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
        public virtual ICollection<Role> WildcardPool { get; set; } = new List<Role>();
        public virtual int NumberOfWildcards { get; set; }
    }
}
