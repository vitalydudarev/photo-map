using System;
using System.IO;
using SkiaSharp;

namespace GraphicsLibrary
{
    public class ImageProcessor : IDisposable
    {
        private const int Quality = 100;
        private readonly Stream _stream;
        private readonly bool _disposeStream;

        private SKBitmap _bitmap;
        private SKImage _image;
        private readonly SKCodec _codec;

        public ImageProcessor(string filePath) : this(File.ReadAllBytes(filePath))
        {
        }
        
        public ImageProcessor(byte[] bytes) : this(new MemoryStream(bytes))
        {
            _disposeStream = true;
        }
        
        public ImageProcessor(Stream stream)
        {
            _stream = stream;
            _codec = SKCodec.Create(stream);
            _bitmap = SKBitmap.Decode(_codec);
        }
        
        public void Crop(int size)
        {
            int width, height;
            if (_bitmap.Width > _bitmap.Height)
            {
                height = size;
                width = _bitmap.Width * size / _bitmap.Height;
            }
            else
            {
                width = size;
                height = _bitmap.Height * size / _bitmap.Width;
            }

            using (var resizedBitmap = _bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High))
            {
                int x = (width - size) / 2;
                int y = (height - size) / 2;

                using (var image = SKImage.FromBitmap(resizedBitmap))
                {
                    _image = image.Subset(SKRectI.Create(x, y, size, size));
                    _bitmap = SKBitmap.FromImage(_image);
                }
            }
        }

        public void Rotate()
        {
            _bitmap = RotateBitmap();
            _image = SKImage.FromBitmap(_bitmap).Subset(SKRectI.Create(0, 0, _bitmap.Width, _bitmap.Height));
        }

        public byte[] GetImageBytes()
        {
            var encodedData = _image.Encode(SKEncodedImageFormat.Jpeg, Quality);

            return encodedData.ToArray();
        }

        public void Dispose()
        {
            _bitmap?.Dispose();
            _codec?.Dispose();
            _image?.Dispose();
            
            if (_disposeStream)
                _stream?.Dispose();
        }

        private SKBitmap RotateBitmap()
        {
            var orientation = _codec.EncodedOrigin;

            int width, height;
            float dx, dy;
            float degrees;
            
            switch (orientation)
            {
                case SKEncodedOrigin.BottomRight:
                {
                    width = _bitmap.Width;
                    height = _bitmap.Height;
                    dx = _bitmap.Width;
                    dy = _bitmap.Height;
                    degrees = 180;
                    
                    break;
                }

                case SKEncodedOrigin.RightTop:
                {
                    width = _bitmap.Height;
                    height = _bitmap.Width;
                    dx = _bitmap.Height;
                    dy = 0;
                    degrees = 90;
                    
                    break;
                }

                case SKEncodedOrigin.LeftBottom:
                {
                    width = _bitmap.Height;
                    height = _bitmap.Width;
                    dx = 0;
                    dy = _bitmap.Height;
                    degrees = 270;
                    
                    break;
                }

                default:
                    return _bitmap;
            }
            
            var rotated = new SKBitmap(width, height);

            using (var canvas = new SKCanvas(rotated))
            {
                canvas.Translate(dx, dy);
                canvas.RotateDegrees(degrees);
                canvas.DrawBitmap(_bitmap, 0, 0);
            }

            return rotated;
        }
    }
}