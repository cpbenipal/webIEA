namespace Flexpage.Models
{
    public class WhoPermissionsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Permission { get; set; }

        public int ListPermissionsId { get; set; }

        public static string NameTypeFolder => "Security Group";

        public static string NameTypePerson => "Person";

        public static string NameTypeEveryone => "Everyone";

        public static string NameTypeASPNET => "ASPNET";


    }
}