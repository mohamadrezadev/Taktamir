using Taktamir.Core.Domain._01.Common;

namespace Taktamir.Core.Domain._08.Verifycodes
{
    public interface IVerifycodeRepository : IRepository<Verifycode>
    {
        Task<Tuple<bool, int>> Isvalidcode(string phone_number, string code);
        Task add_or_update_verifycode(Verifycode verifycode);


    }
}
