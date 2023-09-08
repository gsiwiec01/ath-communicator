using AthChat.Server.Dtos;

namespace AthChat.Server.Interfaces;

public interface IClient
{
    public Task ParticipantLogin(User client);
}