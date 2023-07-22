namespace Taktamir.Endpoint.Models.Dtos.Common
{
    public class Response_jwt
    {
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public DateTime Expiration { get; set; }
        public string phone_number { get; set; }
        public string Role { get; set; }
    }
}
