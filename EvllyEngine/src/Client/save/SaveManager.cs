using EvllyEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.save
{
    public class SaveManager
    {
        public static void CreateDirectory(string WorldName)
        {
            if (!Directory.Exists(Path.GetFullPath("Saves/")))
            {
                Directory.CreateDirectory(Path.GetFullPath("Saves/"));
            }

            if (!Directory.Exists(Path.GetFullPath("Saves/" + WorldName + "/")))
            {
                Directory.CreateDirectory(Path.GetFullPath("Saves/" + WorldName + "/"));
            }

            if (!Directory.Exists(Path.GetFullPath("Saves/" + WorldName + "/chunks/")))
            {
                Directory.CreateDirectory(Path.GetFullPath("Saves/" + WorldName + "/chunks/"));
            }

            if (!Directory.Exists(Path.GetFullPath("Saves/" + WorldName + "/city/")))
            {
                Directory.CreateDirectory(Path.GetFullPath("Saves/" + WorldName + "/city/"));
            }

            if (!Directory.Exists(Path.GetFullPath("Saves/" + WorldName + "/player/")))
            {
                Directory.CreateDirectory(Path.GetFullPath("Saves./" + WorldName + "/player/"));
            }

            if (!Directory.Exists(Path.GetFullPath("Saves/" + WorldName + "/Entity/")))
            {
                Directory.CreateDirectory(Path.GetFullPath("Saves/" + WorldName + "/Entity/"));
            }
        }

        #region PlayerSave
        public static void SavePlayer(PlayerSaveInfo info, string userid, string universename)
        {
            CreateDirectory(universename);

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Path.GetFullPath("Saves/" + universename + "/player/" + userid + ".database"));

            bf.Serialize(file, info);
            file.Close();
        }

        public static void DeletPlayer(string userid, string universename)
        {
            File.Delete(Path.GetFullPath("Saves/" + universename + "/player/" + userid + ".database"));
        }

        public static PlayerSaveInfo LoadPlayer(string userid, string universename)
        {
            if (File.Exists(Path.GetFullPath("Saves/" + universename + "/player/" + userid + ".database")))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Path.GetFullPath("Saves/" + universename + "/player/" + userid + ".database"), FileMode.Open);

                PlayerSaveInfo dataa = (PlayerSaveInfo)bf.Deserialize(file);
                file.Close();

                return dataa;
            }

            Debug.Log("Dont found this Player File " + Path.GetFullPath("Saves/" + universename + "/player/" + userid + ".database"));
            return new PlayerSaveInfo();
        }
        #endregion

        #region CharacterSave
        public static void SaveChars(CharSaveInfo[] info)
        {
            if (!Directory.Exists(Path.GetFullPath("Saves/")))
            {
                Directory.CreateDirectory(Path.GetFullPath("Saves/"));
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Path.GetFullPath("Saves/characters.database"));

            bf.Serialize(file, info);
            file.Close();
        }

        public static void DeletChars()
        {
            File.Delete(Path.GetFullPath("Saves/characters.database"));
        }

        public static CharSaveInfo[] LoadChars()
        {
            if (File.Exists(Path.GetFullPath("Saves/characters.database")))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Path.GetFullPath("Saves/characters.database"), FileMode.Open);

                CharSaveInfo[] dataa = (CharSaveInfo[])bf.Deserialize(file);
                file.Close();

                return dataa;
            }

            Debug.Log("Dont found this Player File " + Path.GetFullPath("Saves/characters.database"));
            return new CharSaveInfo[] { };
        }
        #endregion

        public static bool LoadWorld()
        {
            return false;
        }
    }
}
