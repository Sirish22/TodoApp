namespace ToDoDemo.Models
{
    public class Filters
    {
        public Filters(string filterstring)
        {
            FilterString = filterstring ?? "all-all-all";
            string[] filters = FilterString.Split('-');
            CategoryId = filters[0];
            Due = filters[1];
            StatusId = filters[2];
        }

        public string FilterString { get; }

        public string CategoryId { get; }

        public String Due { get; }

        public string StatusId { get; }

        public bool HasCategory => CategoryId.ToLower() != "all";
        public bool HasDue => Due.ToLower() != "all";

        public bool HasStatus => StatusId.ToLower() != "all";


        //public static Dictionary<string, string> DueFilterValues =>
        //    new Dictioznary<string, string> {
        //    {"future","Future" },
        //    {"past","Past" },
        //    {"today","Today" }
        //};
        public List<KeyValuePair<string, string>> DueFilterValue { get; set; }
        public bool IsPast => Due.ToLower() == "past";

        public bool IsFuture => Due.ToLower() == "future";

        public bool IsToday => Due.ToLower() == "today";
        
    }


    //public class Dueee
    //{
    //    public string Key { get; set; }
    //    public string Value { get; set; }
    //}
}
