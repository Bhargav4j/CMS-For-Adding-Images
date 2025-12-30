using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGallery.ViewModel
{
    public class PictureViewModel
    {
        public int PictureID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailImagePath { get; set; }
    }
}
