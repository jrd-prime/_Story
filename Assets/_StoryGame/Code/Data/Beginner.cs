namespace _StoryGame.Data
{
    public static class Beginner
    {
        public static void CraftCame(string episode)
        {
        }
    }

    public class Intermediate
    {
        public void a()
        {
            Beginner.CraftCame(episode: "001");
        }
    }
}
