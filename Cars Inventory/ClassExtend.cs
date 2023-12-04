/*
 * Extending the mysql data source over here
 */
using MySql.Data.MySqlClient;

namespace Cars_Inventory
{
    public partial class MainWindow
    {
        private myConn m = new myConn();
        private MySqlConnection myConn; // create Mysql Connection
    }

    public class myConn
    {
        // create MySQL Setting
        public string Setting
        {
            get
            {
                return string.Format(@"SERVER={0};
                    DATABASE={1};
                    UID={2};
                    PASSWORD={3};
                    respect binary flags=false; Compress=true; Pooling=true; Min Pool Size=0; Max Pool Size=100; Connection Lifetime=0",
                        Properties.Settings.Default.myhost,
                        Properties.Settings.Default.mytable,
                        Properties.Settings.Default.myuser,
                        Properties.Settings.Default.mypass);
            }
        }
    }
}
