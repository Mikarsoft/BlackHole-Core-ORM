using BlackHole.Enums;
using System.ComponentModel.DataAnnotations;

namespace BlackHole.FunctionalObjects
{
    public class DatabaseInfo
    {
        public string Servername { get; set; } = "";

        [Required(ErrorMessage = "Please Give a Name to your Database")]
        public string DatabaseName { get; set; } = "";
        public int Port { get; set; }
        public string User { get; set; } = "";
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Please Select the Database Type")]
        public BHSqlTypes SqlType { get; set; }
    }
}
