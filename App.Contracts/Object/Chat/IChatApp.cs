// App.Contracts/Object/Chat/IChatApp.cs
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Contracts.Object.Chat
{
    public interface IChatApp
    {
        Task<StatusResult> SendMessageAsync(SendMessageDto dto, int senderId);
        Task<ListDataResult<MessageView>> GetChatHistoryAsync(int otherUserId, Pagination pagination, int currentUserId);
        Task<ListDataResult<ChatListItem>> GetChatListAsync(int userId);
        Task<StatusResult> MarkAsReadAsync(int senderId, int receiverId);
        Task<SingleDataResult<int>> GetTotalUnreadCountAsync(int receiverId);
    }


}