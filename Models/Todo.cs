namespace TodoApi.Models
{
    public class Todo
    {
        /// <summary>
        ///  try 
        /// </summary>
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageUrl { get; set; } // علشان نرفع صور
        public string FileUrl { get; set; }  // علشان نرفع أي ملفات
    }
}
