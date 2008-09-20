/*
 * Created by: Erling Reizer
 * Created: 14. september 2008
 */

using System;
using System.Collections.Generic;
using System.IO;
using MediaPortal.Configuration;
using MediaPortal.Database;
using MediaPortal.GUI.Library;
using SQLite.NET;

namespace NrkBrowser
{
    public class FavoritesUtil
    {
        private static string DB_FILENAME = "NrkBrowser.db3";
        private SQLiteClient sqlClient;

        private static FavoritesUtil database;

        private FavoritesUtil(String fileName)
        {
            bool dbExists;
            try
            {
                // Open database
                try
                {
                    Directory.CreateDirectory("database");
                }
                catch (Exception)
                {
                }
                dbExists = File.Exists(Config.GetFile(Config.Dir.Database, fileName));
                sqlClient = new SQLiteClient(Config.GetFile(Config.Dir.Database, fileName));

                DatabaseUtility.SetPragmas(sqlClient);

                if (!dbExists)
                {
                    CreateTables();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Error("Database exception:{0} stack:{1}", ex.Message, ex.StackTrace);
            }
        }

        public void Dispose()
        {
            if (sqlClient != null)
            {
                sqlClient.Close();
                sqlClient.Dispose();
                sqlClient = null;
            }
        }

        public static FavoritesUtil getDatabase(String fileName)
        {
            if (fileName == null || fileName.Equals(string.Empty))
            {
                fileName = DB_FILENAME;
            }
            if (database == null)
            {
                database = new FavoritesUtil(fileName);
            }
            return database;
        }

        private void CreateTables()
        {
            if (sqlClient == null)
            {
                return;
            }
            try
            { 
                sqlClient.Execute(
                    "CREATE TABLE FAVORITTER(AUTO_ID integer primary key autoincrement, TITLE text,ID text,DESC text,BILDE text, VERDILINK text, ANTVIST text, KLOKKE text)\n");
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
        }

        public bool addFavoriteVideo(Clip clip)
        {
            //check if the video is already in the favorite list
            String sql = string.Format("select ID from FAVORITTER where ID='{0}'", clip.ID);
            SQLiteResultSet resultSet = sqlClient.Execute(sql);
            if (resultSet.Rows.Count > 0)
            {
                Log.Info("Clip already existed as favorite!");
                return false;
            }
            Log.Info("inserting favorite:");
            Log.Info("desc:" + clip.Description);
            Log.Info("image:" + clip.Bilde);
//      Log.Info("tags:"+foVideo.Tags);
            Log.Info("title:" + clip.Title);
//      Log.Info("url"+foVideo.VideoUrl);

//      DatabaseUtility.RemoveInvalidChars(ref clip.Description);
//      DatabaseUtility.RemoveInvalidChars(ref foVideo.ImageUrl);
//      DatabaseUtility.RemoveInvalidChars(ref foVideo.Tags);
//      DatabaseUtility.RemoveInvalidChars(ref foVideo.Title);
//      DatabaseUtility.RemoveInvalidChars(ref foVideo.VideoUrl);

            string sqlInsert =
                string.Format("insert into FAVORITTER(TITLE,ID,DESC,BILDE,VERDILINK, ANTVIST, KLOKKE)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", clip.Title,
                              clip.ID, clip.Description, clip.Bilde, clip.VerdiLink, clip.AntallGangerVist, clip.Klokkeslett);
            sqlClient.Execute(sqlInsert);
            if (sqlClient.ChangedRows() > 0)
            {
                Log.Info("Favorite {0} inserted successfully into database", clip.Title);
                return true;
            }
            else
            {
                Log.Info("Favorite {0} failed to insert into database", clip.Title);
                return false;
            }
        }


        public bool removeFavoriteVideo(Clip clip)
        {
            String lsSQL = string.Format("delete from FAVORITTER where ID='{0}' ", clip.ID);
            sqlClient.Execute(lsSQL);
            if (sqlClient.ChangedRows() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


//    public List<GUIOnlineVideos.VideoInfo> getAllFavoriteVideos()
//    {
//    	return getFavoriteVideos(false,null);
//    }
//    public List<GUIOnlineVideos.VideoInfo> getSiteFavoriteVideos(String fsSiteId){
//    	return getFavoriteVideos(true,fsSiteId);
//    }
        public List<Clip> getFavoriteVideos()
        {
            //createFavorite("Default2");
            string lsSQL = string.Format("select * from FAVORITTER");

            SQLiteResultSet resultSet = sqlClient.Execute(lsSQL);

            List<Clip> favorittListe = new List<Clip>();
            if (resultSet.Rows.Count == 0) return favorittListe;

            for (int iRow = 0; iRow < resultSet.Rows.Count; iRow++)
            {
                Clip clip = new Clip("", "");
                clip.Description = DatabaseUtility.Get(resultSet, iRow, "DESC");
                clip.Title = DatabaseUtility.Get(resultSet, iRow, "TITLE");
                clip.Bilde = DatabaseUtility.Get(resultSet, iRow, "BILDE");
                clip.ID = DatabaseUtility.Get(resultSet, iRow, "ID");
                clip.VerdiLink = DatabaseUtility.Get(resultSet, iRow, "VERDILINK");
                clip.AntallGangerVist = DatabaseUtility.Get(resultSet, iRow, "ANTVIST");
                clip.Klokkeslett = DatabaseUtility.Get(resultSet, iRow, "KLOKKE");
                Log.Info("Pulled {0} out of the database", clip.Title);
                favorittListe.Add(clip);
            }
            return favorittListe;
        }

        /*
  public string [] getSiteIDs(){
      string lsSQL = "select distinct VDO_SITE_ID from favorite_videos";
      SQLiteResultSet loResultSet = sqlClient.Execute(lsSQL);
      string [] siteIdList = new string[loResultSet.Rows.Count];
       for (int iRow = 0; iRow < loResultSet.Rows.Count; iRow++)
      {
          siteIdList[iRow] = DatabaseUtility.Get(loResultSet,iRow,"VDO_SITE_ID");
    	 		
       }
       return siteIdList;
    	
  }
    
   public List<GUIOnlineVideos.VideoInfo> searchFavoriteVideos(String fsQuery){
    	
    //createFavorite("Default2");
    string lsSQL;
    //if(!fbLimitBySite){
      lsSQL = string.Format("select * from favorite_videos where VDO_NM like '%{0}%' or VDO_DESC like '%{0}%' or VDO_TAGS like '%{0}%'",fsQuery);
    //}else{
      //lsSQL = string.Format("select * from favorite_videos where VDO_SITE_ID='{0}'",fsSiteId);
    //}
    SQLiteResultSet loResultSet = sqlClient.Execute(lsSQL);
    List<GUIOnlineVideos.VideoInfo> loFavoriteList = new List<GUIOnlineVideos.VideoInfo>();
    if (loResultSet.Rows.Count == 0) return loFavoriteList ;
      
      for (int iRow = 0; iRow < loResultSet.Rows.Count; iRow++)
      {
          GUIOnlineVideos.VideoInfo video = new GUIOnlineVideos.FavoriteVideoInfo();
          video.Description = DatabaseUtility.Get(loResultSet, iRow, "VDO_DESC");
          video.ImageUrl = DatabaseUtility.Get(loResultSet, iRow, "VDO_IMG_URL");
          video.Length = DatabaseUtility.Get(loResultSet, iRow, "VDO_LENGTH");
          video.Tags = DatabaseUtility.Get(loResultSet, iRow, "VDO_TAGS");
          video.Title = DatabaseUtility.Get(loResultSet, iRow, "VDO_NM");
          video.VideoUrl = DatabaseUtility.Get(loResultSet,iRow,"VDO_URL");
          video.SiteID = DatabaseUtility.Get(loResultSet,iRow,"VDO_SITE_ID");
        	
          Log.Info("Pulled {0} out of the database",video.Title);
          loFavoriteList.Add(video);
        	
      }
      return loFavoriteList;
  }
*/
        /*
    public List<GUIOnlineVideos.FavoriteVideos> getSiteFavoriteVideos(GUIOnlineVideos.Site foSite)
    {
      List<YahooVideo> loFavoriteList = new List<YahooVideo>();
      string lsSQL = String.Format("select FAVORITE_ID from FAVORITE where FAVORITE_NM='{0}'", fsFavoriteNm.Replace("'", "''"));
      SQLiteResultSet loResultSet = sqlClient.Execute(lsSQL);

      string lsFavID = (String)loResultSet.GetColumn(0)[0];
      lsSQL = string.Format("select SONG_NM,SONG_ID,ARTIST_NM,ARTIST_ID,COUNTRY from FAVORITE_VIDEOS where FAVORITE_ID={0}", lsFavID);
      loResultSet = sqlClient.Execute(lsSQL);

      foreach (ArrayList loRow in loResultSet.RowsList)
      {
        YahooVideo loVideo = new YahooVideo();
        IEnumerator en = loRow.GetEnumerator();
        en.MoveNext();
        loVideo.songName = (String)en.Current;
        en.MoveNext();
        loVideo.songId = (String)en.Current;
        en.MoveNext();
        loVideo.artistName = (String)en.Current;
        en.MoveNext();
        loVideo.artistId = (String)en.Current;
        en.MoveNext();
        loVideo.countryId = (String)en.Current;
        loFavoriteList.Add(loVideo);

      }

      return loFavoriteList;
    }
    */
    }
}