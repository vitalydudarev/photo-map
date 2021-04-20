using System;
using System.IO;
using System.Linq;
using ImageMagick;
using PhotoMap.Worker.Models.Image;
using SkiaSharp;

namespace PhotoMap.Worker.Services.Implementations
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

        public ImageProcessor(byte[] bytes)
        {
            _disposeStream = true;

            var heicSignature = new byte[] { 0x66, 0x74, 0x79, 0x70 };    // ftyp

            var firstBytes = bytes.Skip(4).Take(4);
            if (firstBytes.SequenceEqual(heicSignature))
            {
                using (var image = new MagickImage(bytes))
                {
                    var byteArray = image.ToByteArray(MagickFormat.Jpeg);
                    _stream = new MemoryStream(byteArray);
                }
            }
            else
                _stream = new MemoryStream(bytes);

            _codec = SKCodec.Create(_stream);
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

                var image = SKImage.FromBitmap(resizedBitmap);
                _image = image.Subset(SKRectI.Create(x, y, size, size));
                _bitmap = SKBitmap.FromImage(_image);
            }
        }

        public void Rotate()
        {
            _bitmap = RotateBitmap();
            _image = SKImage.FromBitmap(_bitmap).Subset(SKRectI.Create(0, 0, _bitmap.Width, _bitmap.Height));
        }

        public byte[] GetImageBytes()
        {
            if (_image != null)
            {
                var encodedData = _image.Encode(SKEncodedImageFormat.Jpeg, Quality);

                return encodedData.ToArray();
            }

            var data = SKImage.FromBitmap(_bitmap).Encode(SKEncodedImageFormat.Jpeg, Quality);

            return data.ToArray();
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

            var bitmapOptions = GetBitmapOptions(orientation);
            if (bitmapOptions == null)
                return _bitmap;

            var rotated = new SKBitmap(bitmapOptions.Width, bitmapOptions.Height);

            using (var canvas = new SKCanvas(rotated))
            {
                canvas.Translate(bitmapOptions.Dx, bitmapOptions.Dy);
                canvas.RotateDegrees(bitmapOptions.Degrees);
                canvas.DrawBitmap(_bitmap, 0, 0);
            }

            return rotated;
        }

        private BitmapOptions GetBitmapOptions(SKEncodedOrigin orientation)
        {
            switch (orientation)
            {
                case SKEncodedOrigin.BottomRight:
                    return new BitmapOptions(_bitmap.Width, _bitmap.Height, _bitmap.Width, _bitmap.Height, 180);

                case SKEncodedOrigin.RightTop:
                    return new BitmapOptions(_bitmap.Height, _bitmap.Width, _bitmap.Height, 0, 90);

                case SKEncodedOrigin.LeftBottom:
                    return new BitmapOptions(_bitmap.Height, _bitmap.Width, 0, _bitmap.Height, 270);

                default:
                    return null;
            }
        }
    }
}
