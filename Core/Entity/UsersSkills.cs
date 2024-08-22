namespace Core.Entity
{
    public class UsersSkills
    {
        public int UserId { get; set; }
        public UserInfo User { get; set; }
        public int SkillId { get; set; }
        public Skills Skill { get; set; }
    }
}
