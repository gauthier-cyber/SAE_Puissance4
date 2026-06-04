namespace Puissance4.Systeme
{
    public class Challenge
    {
        public int ScoreJoueur1 { get; set; }
        public int ScoreJoueur2 { get; set; }

        public Challenge(int scoreJ1, int scoreJ2)
        {
            ScoreJoueur1 = scoreJ1;
            ScoreJoueur2 = scoreJ2;
        }
    }
}