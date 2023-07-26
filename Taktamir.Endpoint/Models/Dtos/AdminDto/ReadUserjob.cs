using Taktamir.Endpoint.Models.Dtos.WalletDtos;

namespace Taktamir.Endpoint.Models.Dtos.AdminDto
{
    public class ReadUserjob
    {
        public int Id { get; set; }
        public string  firstname { get; set; }
        public string  lastname { get; set; }
        public string  phone_number { get; set; }
        public ReadWalletDto walletDto { get; set; }

    }
}
