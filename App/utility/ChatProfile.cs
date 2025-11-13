// App/utility/ChatProfile.cs
using App.Contracts.Object.Chat;
using AutoMapper;
using Domain.Objects.Chat;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Message, MessageView>();
    }
}