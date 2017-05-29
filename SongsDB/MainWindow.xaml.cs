using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace SongsDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<string> names = new List<string>();
            List<string> urls = new List<string>();

            names = readFiles("C:\\Users\\ASUS\\Downloads\\Newfolder\\1");
            urls = readFiles("C:\\Users\\ASUS\\Downloads\\Newfolder\\2");

            insertDB(names, urls);
        }

        private List<string> readFiles(string path)
        {
            List<string> lines = new List<string>();
            string[] array1 = Directory.GetFiles(path);
            foreach (string filePath in array1)
            {
                List<string> line1 = readLines(filePath);
                lines.AddRange(line1);
            }
            return lines;
        }

        private List<string> readLines(string path)
        {
            List<string> lines = new List<string>();
            string line;

            // Read the file and display it line by line.
            StreamReader file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                lines.Add(line);
            }

            file.Close();
            return lines;
        }

        private void insertDB(List<string> names, List<string> urls)
        {
            SQLiteConnection.CreateFile("SongDatabase.sqlite");
            SQLiteConnection m_dbConnection = null;
            try
            {

                m_dbConnection = new SQLiteConnection("Data Source=SongDatabase.sqlite;Version=3;");
                m_dbConnection.Open();

                string sql = "CREATE TABLE `Song` ("
        + "`ID`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,"
        + "`Name`	TEXT NOT NULL DEFAULT '',"
        + "`Name_Non`	TEXT NOT NULL DEFAULT '',"
        + "`Composer`	TEXT NOT NULL DEFAULT '',"
        + "`FileName`	TEXT NOT NULL DEFAULT '',"
        + "`URL1`	TEXT NOT NULL DEFAULT '',"
        + "`URL2`	TEXT NOT NULL DEFAULT '',"
        + "`Type`	INTEGER NOT NULL DEFAULT 0,"
        + "`Begin`	TEXT NOT NULL DEFAULT '',"
        + "`BeginPhrase`	TEXT"
    + ")";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();

                int l = names.Count;

                for (int i = 0; i < l; i++)
                {
                    string name = names[i];
                    string url = urls[i];

                    sql = "insert into Song (Name, URL1) values ('" + name + "', '" + url + "')";
                    command = new SQLiteCommand(sql, m_dbConnection);
                    command.ExecuteNonQuery();

                    Debug.WriteLine("Index: " + i);
                    i++;
                }
            }
            catch /*(Exception e)*/{ }
            m_dbConnection.Close();
        }

        public string convertToUnSign2(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private void ModifySongs()
        {
            List<Song> songs = loadDB();
            int i = 0;
            SQLiteConnection m_dbConnection = null;
            try
            {

                m_dbConnection = new SQLiteConnection("Data Source=SongDatabase.sqlite;Version=3;");
                m_dbConnection.Open();
                
            
            foreach (Song song in songs)
            {
                if (song.Name.Length > 2)
                {
                    string name_non = convertToUnSign2(song.Name);
                    song.Name_non = name_non;
                }
                else
                {
                    song.Name_non = string.Empty;
                }

                if (song.Url1.Length > 2)
                {
                    string fileName = song.Url1.Substring(song.Url1.LastIndexOf('/') + 1);
                    song.File_name = fileName;
                }
                else
                {
                    song.File_name = string.Empty;
                }
                if (song.Name.Length > 2)
                {
                    string name_begin = song.Name.Substring(0, 1);
                    song.Startwith = name_begin;
                }
                else
                {
                    song.Startwith = string.Empty;
                }
                UpdateDB(m_dbConnection, song);

                Debug.WriteLine("Index: " + i);
                i++;
            }
            }
            catch { }
            m_dbConnection.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ModifySongs();

        }

        private List<Song> loadDB()
        {
            List<Song> songs = new List<Song>();
            SQLiteConnection m_dbConnection = null;
            try
            {

                m_dbConnection = new SQLiteConnection("Data Source=SongDatabase.sqlite;Version=3;");
                m_dbConnection.Open();
                string sql = "select ID, Name, URL1 from Song";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Song song = new Song();
                    song.ID = reader.GetInt16(0);
                    song.Name = reader.GetString(1);
                    song.Url1 = reader.GetString(2);

                    songs.Add(song);
                }

            }
            catch { }
            m_dbConnection.Close();
            return songs;
        }

        private void UpdateDB(Song song)
        {
            string sql = "Update Song "
            + "set FileName = '" + song.File_name + "', Name_Non = '" + song.Name_non + "', [Begin] = '" + song.Startwith + "' "
            + "where ID =" + song.ID;
            SQLiteConnection m_dbConnection = null;
            try
            {

                m_dbConnection = new SQLiteConnection("Data Source=SongDatabase.sqlite;Version=3;");
                m_dbConnection.Open();
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);

                command.ExecuteNonQuery();
            }
            catch { }
        }

        private void UpdateDB(SQLiteConnection m_dbConnection, Song song)
        {
            string sql = "Update Song "
            + "set FileName = '" + song.File_name + "', Name_Non = '" + song.Name_non + "', [Begin] = '" + song.Startwith + "' "
            + "where ID =" + song.ID;
            
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);

                command.ExecuteNonQuery();
            }
            catch { }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            loadDB();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            GoogleDrive.demo();
        }
    }
}
