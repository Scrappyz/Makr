using Makr.Domain.Enums;

namespace Makr.Domain.Helpers
{
    public class FilesystemUtils
    {
        public static PathType? GetPathType(string path)
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(path);

                if (attributes.HasFlag(FileAttributes.Directory))
                {
                    return PathType.Directory;
                }
                else
                {
                    return PathType.File;
                }
            }
            catch
            {
                return null; // Null means the path does not exist or is inaccessible
            }
        }

        public static FileType? GetFileType(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            PathType? pathType = GetPathType(path);

            if (!pathType.HasValue || pathType.Value == PathType.Directory)
                return null;

            int bytesToCheck = 8000;
            using var stream = File.OpenRead(path);

            if (stream.Length == 0)
                return FileType.Text;

            var buffer = new byte[Math.Min(bytesToCheck, stream.Length)];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // UTF-16/32 BOM present → trust it's text, skip null-byte check
            if (HasUnicodeBom(buffer, bytesRead))
                return FileType.Text;

            for (int i = 0; i < bytesRead; i++)
            {
                if (buffer[i] == 0)
                    return FileType.Binary;
            }

            return FileType.Text;
        }

        public static bool Exists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

        public static void Copy(string source, string destination, bool overwrite = false)
        {
            PathType? sourceType = GetPathType(source);

            if (!sourceType.HasValue)
                throw new FileNotFoundException($"Source path '{source}' does not exist.");

            if (sourceType.Value == PathType.Directory)
            {
                if (Exists(destination))
                    return;

                Directory.CreateDirectory(destination);
            }
            else
            {
                File.Copy(source, destination, overwrite);
            }
        }

        public static void Rename(string path, string newName, bool overwrite = false)
        {
            PathType? pathType = GetPathType(path);

            if (!pathType.HasValue)
                throw new FileNotFoundException($"Path '{path}' does not exist.");

            string parentDir = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Cannot determine parent directory.");
            string newPath = Path.Combine(parentDir, newName);
            
            if (pathType.Value == PathType.Directory)
            {
                if (!Exists(newPath))
                    Directory.Move(path, newPath);
            }
            else
            {
                File.Move(path, newPath, overwrite);
            }
        }

        public static void EmptyDirectory(string path)
        {
            PathType? pathType = GetPathType(path);

            if (!pathType.HasValue || pathType.Value != PathType.Directory)
                return;

            Directory.Delete(path, recursive: true);
            Directory.CreateDirectory(path);
        }

        private static bool HasUnicodeBom(byte[] buffer, int length)
        {
            if (length >= 2 && buffer[0] == 0xFF && buffer[1] == 0xFE) return true; // UTF-16 LE
            if (length >= 2 && buffer[0] == 0xFE && buffer[1] == 0xFF) return true; // UTF-16 BE
            if (length >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF) return true; // UTF-8

            return false;
        }
    }
}
