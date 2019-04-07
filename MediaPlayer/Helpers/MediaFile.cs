using MediaPlayer.Misc;
using System.Drawing;
using System.IO;
using UnityEngine;

/**
 * Created by Moon on 4/5/2019
 * Represents/loads a file
 */

namespace MediaPlayer.Helpers
{
    class MediaFile
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public Sprite CoverImage { get; set; }

        public static MediaFile LoadFromFile(FileInfo file)
        {
            int THUMB_SIZE = 256;
            Bitmap thumbnail = WindowsThumbnailProvider.GetThumbnail(
               file.FullName, THUMB_SIZE, THUMB_SIZE, ThumbnailOptions.None);

            MemoryStream stream = new MemoryStream();
            thumbnail.Save(stream, thumbnail.RawFormat);

            var texture = new Texture2D(thumbnail.Width, thumbnail.Height);
            texture.LoadImage(stream.ToArray());
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 1);

            return new MediaFile
            {
                Path = file.FullName,
                FileName = file.Name,
                CoverImage = sprite
            };
        }
    }
}
