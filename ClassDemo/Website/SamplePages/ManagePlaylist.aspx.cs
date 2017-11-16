using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#region Additional Namespaces
using ChinookSystem.BLL;
using Chinook.Data.POCOs;

#endregion
public partial class SamplePages_ManagePlaylist : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Request.IsAuthenticated)
        {
            Response.Redirect("~/Account/Login.aspx");
        }
        else
        {
            TracksSelectionList.DataSource = null;
        }
    }

    protected void CheckForException(object sender, ObjectDataSourceStatusEventArgs e)
    {
        MessageUserControl.HandleDataBoundException(e);
    }

    protected void Page_PreRenderComplete(object sender, EventArgs e)
    {
        //PreRenderComplete occurs just after databinding page events
        //load a pointer to point to your DataPager control
        DataPager thePager = TracksSelectionList.FindControl("DataPager1") as DataPager;
        if (thePager !=null)
        {
            //this code will check the StartRowIndex to see if it is greater that the
            //total count of the collection
            if (thePager.StartRowIndex > thePager.TotalRowCount)
            {
                thePager.SetPageProperties(0, thePager.MaximumRows, true);
            }
        }
    }

    protected void ArtistFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        TracksBy.Text = "Artist";
        SearchArgID.Text = ArtistDDL.SelectedValue;
        //refresh the track list display
        TracksSelectionList.DataBind();
    }

    protected void MediaTypeFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        TracksBy.Text = "MediaType";
        SearchArgID.Text = MediaTypeDDL.SelectedValue;
        //refresh the track list display
        TracksSelectionList.DataBind();
    }

    protected void GenreFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        TracksBy.Text = "Genre";
        SearchArgID.Text = GenreDDL.SelectedValue;
        //refresh the track list display
        TracksSelectionList.DataBind();
    }

    protected void AlbumFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        TracksBy.Text = "Album";
        SearchArgID.Text = AlbumDDL.SelectedValue;
        //refresh the track list display
        TracksSelectionList.DataBind();
    }

    protected void PlayListFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        //standard query lookup
        if (string.IsNullOrEmpty(PlaylistName.Text))
        {
            //able to display a message to the user via the MessageUserControl
            //one of the methods of MessageUserControl is .ShowInfo()
            MessageUserControl.ShowInfo("Warning", "Playlist Name is required.");
        }
        else
        {
            //obtain the username from the security Identity class
            string username = User.Identity.Name;

            //the MessageUserControl has embedded in its code the try/catch logic
            //you do not need to code your own try/catch
            MessageUserControl.TryRun(() =>
            {
                //code to be run under the "watchful eyes" of the user control
                //this is the try{your code} of the try/catch
                PlaylistTracksController sysmgr = new PlaylistTracksController();
                List<UserPlaylistTrack> info = sysmgr.List_TracksForPlaylist(PlaylistName.Text, username);
                PlayList.DataSource = info;
                PlayList.DataBind();
            },"","Here is your current playlist.");
        }
    }

    protected void TracksSelectionList_ItemCommand(object sender,
        ListViewCommandEventArgs e)
    {
        //code to go here
        if (string.IsNullOrEmpty(PlaylistName.Text))
        {
            MessageUserControl.ShowInfo("Warning", "Playlist Name is required.");
        }
        else
        {
            string username = User.Identity.Name;

            //where does TrackId come from
            //ListViewCommandEventArgs e contains the parameter values for this
            //    event; CommandArgument
            //CommandArgument is an object
            int trackid = int.Parse(e.CommandArgument.ToString());

            //send your collection of parameter values to the BLL for processing
            MessageUserControl.TryRun(() =>
            {
                //the process
                PlaylistTracksController sysmgr = new PlaylistTracksController();
                List<UserPlaylistTrack> refreshResults = sysmgr.Add_TrackToPLaylist(PlaylistName.Text, username, trackid);
                PlayList.DataSource = refreshResults;
                PlayList.DataBind();
            },"Success","Your track has been added to your playlist.");

        }
    }

    protected void MoveUp_Click(object sender, EventArgs e)
    {
        //code to go here
        if (PlayList.Rows.Count == 0)
        {
            MessageUserControl.ShowInfo("Warning", "No Playlist has been retrieved");
        }
        else
        {
            if (string.IsNullOrEmpty(PlaylistName.Text))
            {
                MessageUserControl.ShowInfo("Warning", "No Playlist name has been supplied");
            }
            else
            {
                //Check only one row selected
                int trackid = 0;
                int tracknumber = 0; //Optional
                int rowselected = 0; //Search flag

                //Create a pointer to use for the access of the gridview control
                CheckBox playlistselection = null;

                //Traverse the gridview, checking each row for a checked checkbox
                for (int i = 0; i < PlayList.Rows.Count; i++)
                {
                    //Find the checkbox on the indexed gridview row
                    //playlistselection will point to the checkbox
                    playlistselection = PlayList.Rows[i].FindControl("Selected") as CheckBox;
                    //if is checked
                    if (playlistselection.Checked)
                    {
                        trackid = int.Parse((PlayList.Rows[i].FindControl("TrackId") as TextBox).Text);
                        tracknumber = int.Parse((PlayList.Rows[i].FindControl("TrackNumber") as Label).Text);
                        rowselected++;
                    }
                }
                if (rowselected != 1)
                {
                    MessageUserControl.ShowInfo("Warning", "Select one track to move");
                }
                else
                {
                    if (tracknumber == 1)
                    {
                        MessageUserControl.ShowInfo("FYI", "Track can not be moved");
                    }
                    else
                    {
                        //At this point you have playlistname, username, trackid;
                        //Which is needed to move the track

                        //Move the track via your BLL
                        MoveTrack(trackid, tracknumber, "up");
                    }
                }
            }
        }
    }

    protected void MoveDown_Click(object sender, EventArgs e)
    {
        //code to go here
        if (PlayList.Rows.Count == 0)
        {
            MessageUserControl.ShowInfo("Warning", "No Playlist has been retrieved");
        }
        else
        {
            if (string.IsNullOrEmpty(PlaylistName.Text))
            {
                MessageUserControl.ShowInfo("Warning", "No Playlist name has been supplied");
            }
            else
            {
                //Check only one row selected
                int trackid = 0;
                int tracknumber = 0; //Optional
                int rowselected = 0; //Search flag

                //Create a pointer to use for the access of the gridview control
                CheckBox playlistselection = null;

                //Traverse the gridview, checking each row for a checked checkbox
                for (int i = 0; i < PlayList.Rows.Count; i++)
                {
                    //Find the checkbox on the indexed gridview row
                    //playlistselection will point to the checkbox
                    playlistselection = PlayList.Rows[i].FindControl("Selected") as CheckBox;
                    //if is checked
                    if (playlistselection.Checked)
                    {
                        trackid = int.Parse((PlayList.Rows[i].FindControl("TrackId") as TextBox).Text);
                        tracknumber = int.Parse((PlayList.Rows[i].FindControl("TrackNumber") as Label).Text);
                        rowselected++;
                    }
                }
                if (rowselected != 1)
                {
                    MessageUserControl.ShowInfo("Warning", "Select one track to move");
                }
                else
                {
                    if (tracknumber == PlayList.Rows.Count)
                    {
                        MessageUserControl.ShowInfo("FYI", "Track can not be moved");
                    }
                    else
                    {
                        //At this point you have playlistname, username, trackid;
                        //Which is needed to move the track

                        //Move the track via your BLL
                        MoveTrack(trackid, tracknumber, "down");
                    }
                }
            }
        }
    }
    protected void MoveTrack(int trackid, int tracknumber, string direction)
    {
        //code to go here
        MessageUserControl.TryRun(() =>
        {
            //Standard call to BLL method
            PlaylistTracksController sysmgr = new PlaylistTracksController();
            sysmgr.MoveTrack(User.Identity.Name, PlaylistName.Text, trackid, tracknumber, direction);
            List<UserPlaylistTrack> results = sysmgr.List_TracksForPlaylist(PlaylistName.Text, User.Identity.Name);
            PlayList.DataSource = results;
            PlayList.DataBind();
        }, "Success", "Track Moved");
    }
    protected void DeleteTrack_Click(object sender, EventArgs e)
    {
        //code to go here
    }
}
