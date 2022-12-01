namespace EntityModel
{
    public class Role
    {
        public int Id { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public Team Team { get; set; }
    }
}
