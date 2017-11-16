using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#region Additional Namespaces
using Chinook.Data.Entities;
using ChinookSystem.BLL;
#endregion
public partial class SamplePages_TabbedCRUDReview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Message.Text = "";
    }


    protected void SearchResults_SelectedIndexChanged(object sender, EventArgs e)
    {
        SearchResults.SelectedRowStyle.BackColor = System.Drawing.Color.Turquoise;
        GridViewRow agrow = SearchResults.Rows[SearchResults.SelectedIndex];
        AlbumID.Text = (agrow.FindControl("AlbumID") as Label).Text;
        AlbumTitle.Text = agrow.Cells[1].Text;
        ArtistList.SelectedValue = (agrow.FindControl("ArtistList") as DropDownList).SelectedValue;
        ReleaseYear.Text = agrow.Cells[3].Text;
        ReleaseLabel.Text = agrow.Cells[4].Text;
    }


    protected void SearchResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        SearchResults.SelectedRowStyle.BackColor = System.Drawing.Color.White;
    }

    protected void Add_Click(object sender, EventArgs e)
    {
        try
        {
            Album item = new Album();
            
            item.Title = AlbumTitle.Text;
            item.ArtistId = int.Parse(ArtistList.SelectedValue);
            item.ReleaseYear = int.Parse(ReleaseYear.Text);
            item.ReleaseLabel = string.IsNullOrEmpty(ReleaseLabel.Text) ? null : ReleaseLabel.Text;
            AlbumController sysmgr = new AlbumController();
            int newAlbumid = sysmgr.Albums_Add(item);
            AlbumID.Text = newAlbumid.ToString();
            Message.Text = "Add successful.";
            SearchResults.DataBind();

        }
        catch (Exception ex)
        {
            Message.Text = ex.Message;
        }
    }

    protected void Update_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(AlbumID.Text))
        {
            Message.Text = "Look up an album to edit.";
        }
        else
        {
            try
            {
                Album item = new Album();
                item.AlbumId = int.Parse(AlbumID.Text);
                item.Title = AlbumTitle.Text;
                item.ArtistId = int.Parse(ArtistList.SelectedValue);
                item.ReleaseYear = int.Parse(ReleaseYear.Text);
                item.ReleaseLabel = string.IsNullOrEmpty(ReleaseLabel.Text) ? null : ReleaseLabel.Text;
                AlbumController sysmgr = new AlbumController();
                int rowsaffected = sysmgr.Albums_Update(item);
                if (rowsaffected == 0)
                {
                    Message.Text = "Update had a problem. Album not found.";
                }
                else
                {
                    Message.Text = "Update successful.";
                    SearchResults.DataBind();
                }
            }
            catch(Exception ex)
            {
                Message.Text = ex.Message;
            }
        }
    }

    protected void Delete_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(AlbumID.Text))
        {
            Message.Text = "Look up an album to delete.";
        }
        else
        {
            try
            {
                AlbumController sysmgr = new AlbumController();
                int rowsaffected = sysmgr.Albums_Delete(int.Parse(AlbumID.Text));
                if (rowsaffected == 0)
                {
                    Message.Text = "Delete had a problem. Album not found.";
                }
                else
                {
                    Message.Text = "Delete successful.";
                    SearchResults.DataBind();
                }
            }
            catch (Exception ex)
            {
                Message.Text = ex.Message;
            }
        }
    }

    protected void Clear_Click(object sender, EventArgs e)
    {
        AlbumID.Text = "";
        AlbumTitle.Text = "";
        ArtistList.SelectedIndex = 0;
        ReleaseYear.Text = "";
        ReleaseLabel.Text = "";
    }

    protected void CheckForException(object sender, ObjectDataSourceStatusEventArgs e)
    {
        MessageUserControl.HandleDataBoundException(e);
    }

}