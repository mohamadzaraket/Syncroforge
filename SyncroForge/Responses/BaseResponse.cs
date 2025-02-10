namespace SyncroForge.Responses
{

        public abstract class BaseResponse
        {
            public abstract int Status { get; set; }

            public abstract string Message { get; set; }

            public abstract string Type { get; set; }

            public abstract bool Success { get; set; }
            public abstract int Code { get; set; }



    }
}
