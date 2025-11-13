// App.Contracts/Object/Chat/IChatApp.cs
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Chat
{
    public interface IChatApp
    {
        Task<ApiResult<MessageView>> SendMessageAsync(SendMessageDto dto, int senderId);
        Task<ApiResult<List<MessageView>>> GetChatHistoryAsync(int otherUserId, Pagination pagination, int currentUserId);
        Task<ApiResult<List<ChatListItem>>> GetChatListAsync(int userId);
        Task<ApiResult> MarkAsReadAsync(int senderId, int receiverId);
        Task<ApiResult<int>> GetTotalUnreadCountAsync(int receiverId);
    }
}