namespace Battleship.Models
{
    public class db_Game
    {
        public int Game_Id { get; set; }
        public int Player_1_Id { get; set; }
        public int Player_1_Board_Id { get; set; }
        public int Player_2_Id { get; set; }
        public int Player_2_Board_Id { get; set; }
        public bool Complete { get; set; }
        public int Turn { get; set; }
    }
}
