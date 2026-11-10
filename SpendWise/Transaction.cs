using System;
using System.Media;
using System.Windows.Forms;

namespace SpendWise
{
    class Transaction
    {
        readonly Connection con = new Connection();
        // load transactions
        public void LoadTransactions(DataGridView datagrid)
        {
            string queryTransactions = "SELECT ID, Description, Amount, Action, Date  FROM transactions";
            con.LoadData(queryTransactions, datagrid);
            //datagrid.Columns[0].Visible = false;
        }
        // load the number of the month through the name
        public string getMonthNumber(string month)
        {
            string monthNumber;
            switch (month)
            {
                case "January":
                    monthNumber = "01";
                    return monthNumber;
                case "February":
                    monthNumber = "02";
                    return monthNumber;
                case "March":
                    monthNumber = "03";
                    return monthNumber;
                case "April":
                    monthNumber = "04";
                    return monthNumber;
                case "May":
                    monthNumber = "05";
                    return monthNumber;
                case "June":
                    monthNumber = "06";
                    return monthNumber;
                case "July":
                    monthNumber = "07";
                    return monthNumber;
                case "August":
                    monthNumber = "08";
                    return monthNumber;
                case "September":
                    monthNumber = "09";
                    return monthNumber;
                case "October":
                    monthNumber = "10";
                    return monthNumber;
                case "November":
                    monthNumber = "11";
                    return monthNumber;
                case "December":
                    monthNumber = "12";
                    return monthNumber;
                default:
                    monthNumber = "01";
                    return monthNumber;
            }
        }
        // load specific transactions
        public void LoadDate(string date, DataGridView dataGrid)
        {
            try
            {
                con.LoadData($"SELECT ID, Description, Amount, Action, Date FROM Transactions WHERE date LIKE '{date}%'", dataGrid);
            }
            catch (Exception)
            {
                // play chime
                SoundPlayer save = new SoundPlayer(@"sfx/error.wav");
                save.Play();
                // log the error
                con.ExecuteQuery("INSERT INTO events (date, description, location) VALUES(strftime('%Y-%m-%d %H:%M','now','localtime'), 'SQL error', 'loadDate action')");
                MessageBox.Show("Month feature unavailable!, bring help!", "Assistant");
            }
        }
        // load specific month
        public void LoadMonth(string date, int year, DataGridView dataGrid)
        {
            try
            {
                con.LoadData($"SELECT ID, Description, Amount, Action, Date FROM Transactions WHERE strftime('%m', date) = '{getMonthNumber(date)}' AND strftime('%Y', date) = '{year}'", dataGrid);
            }
            catch (Exception)
            {
                // play chime
                SoundPlayer save = new SoundPlayer(@"sfx/error.wav");
                save.Play();
                // log the error
                con.ExecuteQuery("INSERT INTO events (date, description, location) VALUES(strftime('%Y-%m-%d %H:%M','now','localtime'), 'SQL error', 'LoadMonth action')");
                MessageBox.Show("Month feature unavailable!, bring help!", "Assistant");
            }
        }
    }
}
