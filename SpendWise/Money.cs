namespace SpendWise
{
    public class Money
    {
        readonly Connection con = new Connection();
        // pull the actual money value in the db
        public string CheckMoney()
        {
            string money = con.ReadString("SELECT money FROM wallet");
            return money;
        }
        // pull the savings value in the db
        public string CheckSavings()
        {
            string savings = con.ReadString("SELECT savings FROM wallet WHERE id = 1");
            return savings;
        }
    }
}
