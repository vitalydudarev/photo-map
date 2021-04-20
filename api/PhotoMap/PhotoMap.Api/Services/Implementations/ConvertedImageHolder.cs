using System;
using System.Collections.Generic;
using PhotoMap.Api.Services.Interfaces;

namespace PhotoMap.Api.Services.Implementations
{
    public class ConvertedImageHolder : IConvertedImageHolder
    {
        private readonly Dictionary<Guid, byte[]> _holder = new Dictionary<Guid, byte[]>();

        public void Add(Guid id, byte[] bytes)
        {
            _holder.Add(id, bytes);
        }

        public byte[] Get(Guid id)
        {
            if (_holder.TryGetValue(id, out var bytes))
            {
                _holder.Remove(id);
                return bytes;
            }

            return null;
        }
    }
}
