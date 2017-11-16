using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Chinook.Data.Entities;
using Chinook.Data.DTOs;
using Chinook.Data.POCOs;
using ChinookSystem.DAL;
using System.ComponentModel;
#endregion

namespace ChinookSystem.BLL
{
    public class PlaylistTracksController
    {
        public List<UserPlaylistTrack> List_TracksForPlaylist(
            string playlistname, string username)
        {
            using (var context = new ChinookContext())
            {
                //what would happen if there is no match for the
                //  incoming parameter values.
                //we need to ensure that the results have a valid value
                //this value will the resolve of a query either a null (not found_
                //    or an IEnumerable<T> collection
                //to achieve a valid value encapsulate the query in a
                //   .FirstOrDefault()
                var results = (from x in context.Playlists
                              where x.UserName.Equals(username)
                                && x.Name.Equals(playlistname)
                              select x).FirstOrDefault();
               //test if you should return a null as your collection
               //  or find the tracks for the given PlaylistId in results.
                if (results == null)
                {
                    return null;
                }
                else
                {
                    //now get the tracks
                    var theTracks = from x in context.PlaylistTracks
                                where x.PlaylistId.Equals(results.PlaylistId)
                                orderby x.TrackNumber
                                select new UserPlaylistTrack
                                {
                                    TrackID = x.TrackId,
                                    TrackNumber = x.TrackNumber,
                                    TrackName = x.Track.Name,
                                    Milliseconds = x.Track.Milliseconds,
                                    UnitPrice = x.Track.UnitPrice
                                };
                    return theTracks.ToList();
                }
            }
        }//eom
        public List<UserPlaylistTrack> Add_TrackToPLaylist(string playlistname, string username, int trackid)
        {
            using (var context = new ChinookContext())
            {
                //code to go here
                //Part One:
                //query to get the playlist id
                var exists = (from x in context.Playlists
                               where x.UserName.Equals(username)
                                 && x.Name.Equals(playlistname)
                               select x).FirstOrDefault();

                //initialize the tracknumber
                int tracknumber = 0;
                //I will need to create an instance of PlaylistTrack
                PlaylistTrack newtrack = null;

                //determine if a playlist "parent" instances needs to be
                // created
                if (exists == null)
                {
                    //this is a new playlist
                    //create an instance of playlist to add to Playlist tablge
                    exists = new Playlist();
                    exists.Name = playlistname;
                    exists.UserName = username;
                    exists = context.Playlists.Add(exists);
                    //at this time there is NO phyiscal pkey
                    //the psuedo pkey is handled by the HashSet
                    tracknumber = 1;
                }
                else
                {
                    //playlist exists
                    //I need to generate the next track number
                    tracknumber = exists.PlaylistTracks.Count() + 1;

                    //validation: in our example a track can ONLY exist once
                    //   on a particular playlist
                    newtrack = exists.PlaylistTracks.SingleOrDefault(x => x.TrackId == trackid);
                    if (newtrack != null)
                    {
                        throw new Exception("Playlist already has requested track.");
                    }
                }

                //Part Two: Add the PlaylistTrack instance
                //use navigation to .Add the new track to PlaylistTrack
                newtrack = new PlaylistTrack();
                newtrack.TrackId = trackid;
                newtrack.TrackNumber = tracknumber;

                //NOTE: the pkey for PlaylistId may not yet exist
                //   using navigation one can let HashSet handle the PlaylistId
                //   pkey value
                exists.PlaylistTracks.Add(newtrack);

                //physically add all data to the database
                //commit
                context.SaveChanges();
                return List_TracksForPlaylist(playlistname, username);
            }
        }//eom
        public void MoveTrack(string username, string playlistname, int trackid, int tracknumber, string direction)
        {
            using (var context = new ChinookContext())
            {
                //code to go here 
                var exists = (from x in context.Playlists
                              where x.UserName.Equals(username)
                                && x.Name.Equals(playlistname)
                              select x).FirstOrDefault();

                if (exists == null)
                {
                    throw new Exception("Playlist has been removed");
                }
                else
                {
                    //Limit your search to the particular playlist
                    PlaylistTrack movetrack = (from x in exists.PlaylistTracks
                                               where x.TrackId == trackid
                                              select x).FirstOrDefault();
                    if (movetrack == null)
                    {
                        throw new Exception("Playlist track has been removed");
                    }
                    else
                    {
                        PlaylistTrack othertrack = null;
                        if (direction.Equals("up"))
                        {
                            if (movetrack.TrackNumber == 1)
                            {
                                throw new Exception("Playlist track cannot be moved");
                            }
                            else
                            {
                                //get the other track
                                othertrack = (from x in exists.PlaylistTracks
                                              where x.TrackNumber == movetrack.TrackNumber -1
                                              select x).FirstOrDefault();
                                if (othertrack == null)
                                {
                                    throw new Exception("Playlist track cannot be moved up");
                                }
                                else
                                {
                                    //At this point, you can exchange track numbers
                                    movetrack.TrackNumber -= 1;
                                    othertrack.TrackNumber += 1;
                                }
                            }
                        }
                        else
                        {
                            if (movetrack.TrackNumber == exists.PlaylistTracks.Count)
                            {
                                throw new Exception("Playlist track cannot be moved down");
                            }
                            else
                            {
                                //get the other track
                                othertrack = (from x in exists.PlaylistTracks
                                              where x.TrackNumber == movetrack.TrackNumber + 1
                                              select x).FirstOrDefault();
                                if (othertrack == null)
                                {
                                    throw new Exception("Playlist track cannot be moved down");
                                }
                                else
                                {
                                    //At this point, you can exchange track numbers
                                    movetrack.TrackNumber -= 1;
                                    othertrack.TrackNumber += 1;
                                }
                            }
                        }//End up/down
                        //Stage changes for SaveChanges()
                        //Indicate only the field that needs to be updated
                        context.Entry(movetrack).Property(y => y.TrackNumber).IsModified = true;
                        context.Entry(othertrack).Property(y => y.TrackNumber).IsModified = true;
                        context.SaveChanges();
                    }
                }
            }
        }//eom


        public void DeleteTracks(string username, string playlistname, List<int> trackstodelete)
        {
            using (var context = new ChinookContext())
            {
               //code to go here


            }
        }//eom
    }
}
