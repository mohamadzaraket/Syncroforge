namespace SyncroForge.Responses.Guest.RefreshToken
{
    public class RefreshTokenResponse: BaseResponse
    {
        public override int Status { get; set; }
        public override string Message { get; set; }
        public override string Type { get; set; }
        public override bool Success { get; set; }
        public override int Code { get; set; }
        public object? data { get; set; }
    }
}
