using System;

namespace PhotoMap.Api.Services.Interfaces
{
    public interface IConvertedImageHolder
    {
        void Add(Guid id, byte[] bytes);

        byte[] Get(Guid id);
    }
}
