namespace Taktamir.Core.Domain._03.Users
{
    public class Specialty
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
