namespace LootScaler
{
    public class socketBonus
    {
        public int id;
        public int family;
        public double value;

        public socketBonus()
        {
        }

        public socketBonus(socketBonus socket)
        {
            id = socket.id;
            family = socket.family;
            value = socket.value;
        }
    }
}
