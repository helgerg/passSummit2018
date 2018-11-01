using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Interfaces.Streaming;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImageOutputter
{
    [SqlUserDefinedExtractor(AtomicFileProcessing = true)]
    public class ImageOutputter : IOutputter
    {
        public override void Output(IRow input, IUnstructuredWriter output)
        {
            var image = input.Get<object>(0);
            
            byte[] imageArray = (byte[])image;
            using (MemoryStream ms = new MemoryStream(imageArray))
            {
                var img = Image.FromStream(ms);
                img.Save(output.BaseStream, ImageFormat.Jpeg);
            }
        }
    }
}