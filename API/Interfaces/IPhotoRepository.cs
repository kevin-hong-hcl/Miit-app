using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        Task<Photo> GetPhotoById(int id);
        Task<AppUser> GetUserByPhotoId(int photoId);
        void RemovePhoto(Photo photo);
        
    }
}