// ConfApp/Rep/Chat/IMessageRep.cs
using App;
using Domain.Objects.Chat;
using MyFrameWork.AppTool;

namespace App.Object.Chat.MessageApp

{
    public interface IMessageRep : IBaseRep<Message, int>
    {
        Task<List<Message>> GetChatHistoryAsync(int userId, int otherUserId, Pagination pagination);
        Task<int> GetUnreadCountAsync(int receiverId, int senderId);
        Task MarkAsReadAsync(int receiverId, int senderId);
    }
}