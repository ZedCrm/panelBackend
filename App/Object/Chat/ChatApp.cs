// App/Object/Chat/ChatApp.cs
using App.Contracts.Object.Chat;
using App.Object.Base.Users;
using App.Object.Chat.MessageApp;
using App.utility;
using AutoMapper;
using MyFrameWork.AppTool;
using Domain.Objects.Base;
using Domain.Objects.Chat;
using SixLabors.ImageSharp.Processing;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Chat
{
    public class ChatApp : IChatApp
    {
        private readonly IMessageRep _messageRep;
        private readonly IMyUserRepository _userRep;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly UserStatusService _statusService;

        public ChatApp(
            IMessageRep messageRep,
            IMyUserRepository userRep,
            IFileService fileService,
            IMapper mapper,
            UserStatusService statusService)
        {
            _messageRep = messageRep;
            _userRep = userRep;
            _fileService = fileService;
            _mapper = mapper;
            _statusService = statusService;
        }

        public async Task<StatusResult> SendMessageAsync(SendMessageDto dto, int senderId)
        {
            var receiver = await _userRep.GetAsync(dto.ReceiverId);
            if (receiver == null)
                return ResultFactory.Status(ResultStatusEnum.NotFound,"گیرنده یافت نشد.");

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                Type = dto.Type,
                SentAt = DateTime.Now,
                DeliveredAt = DateTime.Now
            };

            if (dto.File != null)
            {
                var resize = dto.Type == MessageType.Image
                    ? new ResizeOptions { Size = new(800, 800), Mode = ResizeMode.Max }
                    : null;

                message.FileUrl = await _fileService.UploadAsync(
                    dto.File, "uploads/chat", null, resize);
            }

            await _messageRep.CreateAsync(message);
            await _messageRep.SaveChangesAsync();

            var view = _mapper.Map<MessageView>(message);
            view.SenderName = (await _userRep.GetAsync(senderId))?.FullName ?? "من";
            view.IsMine = true;

            return ResultFactory.Status(ResultStatusEnum.Success, "پیام با موفقیت ارسال شد.");
        }

        public async Task<ListDataResult<MessageView>> GetChatHistoryAsync(int otherUserId, Pagination pagination, int currentUserId)
        {
            var messages = await _messageRep.GetChatHistoryAsync(currentUserId, otherUserId, pagination);
            var views = _mapper.Map<List<MessageView>>(messages);

            var currentUser = await _userRep.GetAsync(currentUserId);
            var otherUser = await _userRep.GetAsync(otherUserId);

            foreach (var v in views)
            {
                v.SenderName = v.SenderId == currentUserId 
                    ? currentUser?.FullName ?? "من" 
                    : otherUser?.FullName ?? "کاربر ناشناس";
                v.IsMine = v.SenderId == currentUserId;
            }

            return ResultFactory.List<MessageView>(ResultStatusEnum.Accepted,views ,messages.Count,pagination);
        }

        public async Task<ListDataResult<ChatListItem>> GetChatListAsync(int userId)
        {
            var sent = await _messageRep.GetFilteredAsync(m => m.SenderId == userId);
            var received = await _messageRep.GetFilteredAsync(m => m.ReceiverId == userId);

            var allMessages = sent.Concat(received);

            var grouped = allMessages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g =>
                {
                    var lastMsg = g.OrderByDescending(m => m.SentAt).First();
                    var (status, lastSeen) = _statusService.GetStatus(g.Key);

                    return new ChatListItem
                    {
                        UserId = g.Key,
                        UserName = "کاربر", // بعداً با join واقعی
                        LastMessage = lastMsg.Content,
                        LastMessageTime = lastMsg.SentAt,
                        UnreadCount = g.Count(m => 
                            m.ReceiverId == userId && 
                            m.SeenAt == null && 
                            !m.IsDeletedForReceiver),
                        IsOnline = status == UserStatus.Online,
                        LastSeen = lastSeen
                    };
                })
                .ToList();

            return ResultFactory.List<ChatListItem>(ResultStatusEnum.Success,grouped);
        }

        public async Task<StatusResult> MarkAsReadAsync(int senderId, int receiverId)
        {
            await _messageRep.MarkAsReadAsync(receiverId, senderId);
            await _messageRep.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Success,"پیام‌ها به عنوان خوانده‌شده علامت‌گذاری شدند.");
        }

        public async Task<SingleDataResult<int>> GetUnreadCountAsync(int receiverId)
        {
            var count = await _messageRep.GetUnreadCountAsync(receiverId, 0);
            return ResultFactory.Single<int>(ResultStatusEnum.Success,count);
        }

        public async Task<SingleDataResult<int>> GetTotalUnreadCountAsync(int receiverId)
        {
            var unreadMessages = await _messageRep.GetFilteredAsync(m =>
                m.ReceiverId == receiverId &&
                m.SeenAt == null &&
                !m.IsDeletedForReceiver
            );

            return ResultFactory.Single<int>(ResultStatusEnum.Success,unreadMessages.Count);
        }
    }
}