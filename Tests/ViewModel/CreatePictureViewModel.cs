using Repository.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGallery.ViewModel
{
    public class CreatePictureViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }   
            
        public int GalleryID { get; set; }        
        public List<GalleryType> GalleryTypes { get; set; }

    }
}
