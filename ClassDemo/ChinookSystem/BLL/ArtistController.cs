
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Chinook.Data.Entities;
using Chinook.Data.POCOs;
using ChinookSystem.DAL;
using System.ComponentModel;  //expose methods for ODS wizard
#endregion

namespace ChinookSystem.BLL
{
    [DataObject]
    public class ArtistController
    {
        [DataObjectMethod(DataObjectMethodType.Select,false)]
        public List<Artist> Artists_List()
        {
            using (var context = new ChinookContext())
            {
                return context.Artists.ToList();
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<SelectionList> List_ArtistNames()
        {
            using (var context = new ChinookContext())
            {
                var results = from x in context.Artists
                              orderby x.Name
                              select new SelectionList
                              {
                                  IDValueField = x.ArtistId,
                                  DisplayText = x.Name
                              };
                return results.ToList();
            }
        }
    }
}
