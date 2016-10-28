namespace Model
{
    public class UserSwitchModel
    {
        public int UserId { get; set; }

        public string Description { get; set; }

        public string NtLogin { get; set; }

        public bool IsDefault { get; set; }
    }
}
