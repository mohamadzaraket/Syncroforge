namespace SyncroForge.Responses
{
    public class MainResponse
    {
        public  int Status { get; set; }

        public  string Message { get; set; }

        public  string Type { get; set; }

        public  bool Success { get; set; }
        public  int Code { get; set; }
        public object? data { get; set; }
   


    }
}
