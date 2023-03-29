
using System.Threading.Tasks;
using UnionAvatars.Log;

namespace UnionAvatars.API
{
    public interface ISession
    {
        public SessionContext UnionAvatarsSession { get; }
        public LogHandler LogHandler { get; }
        public Task<bool> Login(string username, string password);
        public Task<bool> Register(string username, string email, string password);
        public Task<Body[]> GetBodies(int limit = 10, int skip = 0);
        public Task<AvatarMetadata[]> GetAvatars(int limit = 10, int skip = 0);
        public Task<AvatarMetadata> GetAvatar(string avatarId);
        public Task<AvatarMetadata> CreateAvatar(AvatarRequest avatarRequest);
        public Task DeleteAvatar(AvatarMetadata avatar);
        public Task<Head> CreateHead(HeadRequest headRequest);
        public Task<Head[]> GetHeads(int limit = 5, int skip = 0);
        public Task<Head> GetHead(string headId = null);
        public Task DeleteHead(Head head);
    }
}