using Taktamir.Core.Domain._01.Common;

namespace Taktamir.Core.Domain._09.Chats
{
    public interface IChatRepository:IRepository<Chat>
    {
        Task<List<ChatGroup>> Search(string title, long userId);
        Task<List<ChatGroup>> GetUserGroups(long userId);
        Task<ChatGroup> InsertGroup(ChatGroup model);
        Task<ChatGroup> InsertPrivateGroup(long userId, long receiverId);
        Task<ChatGroup> GetGroupBy(long id);
        Task<ChatGroup> GetGroupBy(string token);
    }
}
