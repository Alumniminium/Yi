using System.IO;


namespace ItemCreator
{
    public static class Misc
    {
        public static void Rename(string source)
        {
            var path = Path.GetDirectoryName(source);
            if (!File.Exists(source)) return;
            File.Move(source, path + Equipment.Instance.Name + ".dds");
        }
        public static void Copy(string source, string folderName)
        {
            var oldPath = source;
            var newpath = @"A:\Temp\" + folderName + "\\";
            var newFileName = "Blabla";
            var f1 = new FileInfo(oldPath);
            if (f1.Exists)
            {
                if (!Directory.Exists(newpath))
                {
                    Directory.CreateDirectory(newpath);
                }
                f1.CopyTo($"{newpath}{newFileName}{f1.Extension}");
            }
        }
    }
}
