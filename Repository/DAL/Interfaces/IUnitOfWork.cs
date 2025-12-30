using Repository.DAL.Interfaces;
using Repository.Interfaces;
using System;

namespace Repository.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IImagesRepository Images { get; }
        IUserRepository Users { get; }

        void Dispose(bool disposing);
        int Save();
    }
}
