/*
 * Created by: Erling Reizer
 * Created: 14. september 2008
 * Code heavily cut 'n pasted from the onlinevideos plugin by gregmac
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
        private static int schemaVersion = 2;

        private static FavoritesUtil database;

        private FavoritesUtil(String fileName)
        {
            
            try
            {
                bool dbExists;
                // Open database
                try
                {
                    Directory.CreateDirectory("database");
                }
                catch (Exception e)
                {
                    Log.Error("Something wrong happened.." + e.Message);
                }
                dbExists = File.Exists(Config.GetFile(Config.Dir.Database, fileName));
                sqlClient = new SQLiteClient(Config.GetFile(Config.Dir.Database, fileName));

                DatabaseUtility.SetPragmas(sqlClient);

                if (!dbExists)
                {
                    CreateTables();
                }
                else
                {
                    checkIfUpdateOfSchemaIsNeccessary();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Error("Database exception:{0} stack:{1}", ex.Message, ex.StackTrace);
            }
        }

        private void checkIfUpdateOfSchemaIsNeccessary()
        {
            Log.Info("checkIfUpdateOfSchemaIsNeccessary()");
            SQLiteResultSet rs = sqlClient.Execute("select * from VERSION");
            for (int iRow = 0; iRow < rs.Rows.Count; iRow++)
            {
                int version = Int32.Parse(DatabaseUtility.Get(rs, iRow, "VERSION"));
                if (version < schemaVersion)
                {
                    Log.Info(String.Format("Schemaversion is {0}, while this version of the plugin requires version: {1}", version, schemaVersion));
                    updateSchemaVersion(version);
                }
                else{
                    Log.Info(String.Format("Schemaversion is up to date!( version {0})", version));
                }
            }
        }
        private void updateSchemaVersion(int version)
        {
            Log.Info("updateSchemaVersion(int): " + version);
            if (version == 1)
            {
                sqlClient.Execute("ALTER TABLE FAVORITTER ADD TYPE text");
                sqlClient.Execute("UPDATE FAVORITTER SET TYPE = 'KLIPP'");
                sqlClient.Execute("UPDATE VERSION SET VERSION = 2 WHERE VERSION = 1");
            }
            else
            {
                Log.Error("UNKNOWN VERSION: " + version);
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

        /// <summary>
        /// Call this method to get an instance of the database 
        /// </summary>
        /// <param name="fileName">The database to use. If called with null a default database will be used</param>
        /// <returns>Instance of the database</returns>
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

            sqlClient.Execute(
                "CREATE TABLE FAVORITTER(AUTO_ID integer primary key autoincrement, TITLE text,ID text,DESC text,BILDE text, VERDILINK text, ANTVIST text, KLOKKE text, TYPE text)\n");

            sqlClient.Execute("CREATE TABLE VERSION(VERSION integer primary key)");
            sqlClient.Execute(String.Format("insert into VERSION(VERSION)VALUES({0})", schemaVersion));
        }

        /// <summary>
        /// Adds a clip to the favourites database 
        /// </summary>
        /// <param name="clip">The clip to add</param>
        /// <returns>true if clip was added, false if not</returns>
        public bool addFavoriteVideo(Clip clip)
        {
            Log.Debug(NrkPlugin.PLUGIN_NAME + "addFavoriteVideo(Clip) " + clip);
            //check if the video is already in the favorite list
            String sql = string.Format("select ID from FAVORITTER where ID='{0}'", clip.ID);
            SQLiteResultSet resultSet = sqlClient.Execute(sql);
            if (resultSet.Rows.Count > 0)
            {
                Log.Info("Clip already existed as favorite!");
                return false;
            }
            Log.Debug("inserting favorite:");
            Log.Debug("desc:" + clip.Description);
            Log.Debug("image:" + clip.Bilde);
            Log.Debug("title:" + clip.Title);

            string sqlInsert =
                string.Format(
                    "insert into FAVORITTER(TITLE,ID,DESC,BILDE,VERDILINK, ANTVIST, KLOKKE, TYPE)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                    clip.Title,
                    clip.ID, clip.Description, clip.Bilde, clip.VerdiLink, clip.AntallGangerVist, clip.Klokkeslett, clip.Type);
            sqlClient.Execute(sqlInsert);
            if (sqlClient.ChangedRows() > 0)
            {
                Log.Debug("Favorite [{0}] inserted successfully into database", clip);
                return true;
            }
            else
            {
                Log.Debug("Favorite [{0}] failed to insert into database", clip);
                return false;
            }
        }

        /// <summary>
        /// Tries to remove the given clip from the favourites database
        /// </summary>
        /// <param name="clip">The clip to remove</param>
        /// <returns>true if clip was removed from database, false if not</returns>
        public bool removeFavoriteVideo(Clip clip)
        {
            Log.Debug(NrkPlugin.PLUGIN_NAME + "removeFavoriteVideo(Clip) " + clip);
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

        /// <summary>
        /// Returns a list of the favourite clips
        /// </summary>
        /// <returns></returns>
        public List<Clip> getFavoriteVideos()
        {
            Log.Debug(NrkPlugin.PLUGIN_NAME + "getFavoriteVideos()");
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
                Clip.KlippType type = (Clip.KlippType) Enum.Parse(typeof(Clip.KlippType), DatabaseUtility.Get(resultSet, iRow, "TYPE"));
                clip.Type = type;
                Log.Debug("Pulled {0} out of the database", clip.Title);
                favorittListe.Add(clip);
            }
            return favorittListe;
        }

        /*
    
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
    }
}