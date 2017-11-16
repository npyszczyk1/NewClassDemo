using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinook.Data.DTOs
{
    public class GenreDTO
    {
        public string genre { get; set; }
        public IEnumerable<AlbumDTO> albums { get; set; }
    }
}
