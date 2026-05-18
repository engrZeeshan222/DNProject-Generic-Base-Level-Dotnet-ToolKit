using GenericToolKit.Domain.Entities;
using GenericToolKit.Domain.Models;

namespace GenericToolKit.Domain.Interfaces
{

    public interface IProjectableSpecifications<T, TResult>
        where T : BaseEntity
        where TResult : BaseInOutDTO
    {
    }
}

