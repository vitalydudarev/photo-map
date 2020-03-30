using System;
using System.IO;
using SkiaSharp;

namespace Image.Service.Services
{
    public class ImageProcessor : IDisposable
    {
        private const int Quality = 100;
        private readonly Stream _stream;

        public ImageProcessor(string fileName)
        {
            var bytes = File.ReadAllBytes(fileName);
            _stream = new MemoryStream(bytes);
        }
        
        public ImageProcessor(byte[] bytes)
        {
            _stream = new MemoryStream(bytes);
        }
        
        public ImageProcessor(Stream stream)
        {
            _stream = stream;
        }
        
        public byte[] Resize(int size)
        {
            using (var bitmap = SKBitmap.Decode(_stream))
            {
                int width, height;
                if (bitmap.Width > bitmap.Height)
                {
                    height = size;
                    width = bitmap.Width * size / bitmap.Height;
                }
                else
                {
                    width = size;
                    height = bitmap.Height * size / bitmap.Width;
                }

                using (var resizedBitmap = bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High))
                {
                    if (resizedBitmap == null)
                        return null;

                    int x = (width - size) / 2;
                    int y = (height - size) / 2;

                    using (var image = SKImage.FromBitmap(resizedBitmap))
                    {
                        var croppedImage = image.Subset(SKRectI.Create(x, y, height, height));
                        var encodedData = croppedImage.Encode(SKEncodedImageFormat.Jpeg, Quality);

                        return encodedData.ToArray();
                    }
                }
            }
        }

        public byte[] Rotate()
        {
            using (var bitmap = RotateBitmap(_stream))
            using (var image = SKImage.FromBitmap(bitmap))
            {
                var encodedData = image.Encode(SKEncodedImageFormat.Jpeg, Quality);

                return encodedData.ToArray();
            }
        }

        private static SKBitmap RotateBitmap(Stream stream)
        {
            using (var codec = SKCodec.Create(stream))
            {
                var orientation = codec.EncodedOrigin;
                var bitmap = SKBitmap.Decode(codec);

                int width, height;
                float dx, dy;
                float degrees;
                
                switch (orientation)
                {
                    case SKEncodedOrigin.BottomRight:
                    {
                        width = bitmap.Width;
                        height = bitmap.Height;
                        dx = bitmap.Width;
                        dy = bitmap.Height;
                        degrees = 180;
                        
                        break;
                    }

                    case SKEncodedOrigin.RightTop:
                    {
                        width = bitmap.Height;
                        height = bitmap.Width;
                        dx = bitmap.Height;
                        dy = 0;
                        degrees = 90;
                        
                        break;
                    }

                    case SKEncodedOrigin.LeftBottom:
                    {
                        width = bitmap.Height;
                        height = bitmap.Width;
                        dx = 0;
                        dy = bitmap.Height;
                        degrees = 270;
                        
                        break;
                    }

                    default:
                        return bitmap;
                }
                
                var rotated = new SKBitmap(width, height);

                using (var canvas = new SKCanvas(rotated))
                {
                    canvas.Translate(dx, dy);
                    canvas.RotateDegrees(degrees);
                    canvas.DrawBitmap(bitmap, 0, 0);
                }

                return rotated;
            }
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}