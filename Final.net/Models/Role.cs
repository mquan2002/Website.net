namespace Final.net.Models
{
    public class Role : BaseEntity

    {
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
