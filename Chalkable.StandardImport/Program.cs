using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Chalkable.StandardImport.Services;

namespace Chalkable.StandardImport
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 4)
                throw new Exception("Not enough parameters. You should provide minimum 4 params. ConnectionString, CC_StandardsFileDirectory, Ab_StandardFilesDirectory, ImportLogstFilesDirectory");

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Replace("\"", "\\");
            }
            string error;
            if(!ValidateConnectionString(args[0], out error))
                throw new Exception($"Invalid connection string. {error}");

            if (args.Length == 4)
            {
                if (!Directory.Exists(args[1]))
                    throw new Exception("Invalid CC_StandardFilesDirectory");
                if (!Directory.Exists(args[2]))
                    throw new Exception("Invalid Ab_StandardFilesDirectory");
                if (!Directory.Exists(args[3]))
                    throw new Exception("Invalid ImportLogstFilesDirectory");
                ProcessImport(args[0], args[1], args[2], args[3]);
            }
            else
            {
                if (!Directory.Exists(args[1]))
                    throw new Exception("Invalid CC_StandardFilesDirectory");
                if (!Directory.Exists(args[2]))
                    throw new Exception("Invalid NssMappingFilesDirectory");
                if (!Directory.Exists(args[3]))
                    throw new Exception("Invalid AlMappingFilesDirectory");
                if (!Directory.Exists(args[4]))
                    throw new Exception("Invalid ImportLogstFilesDirectory");

                ProcessAlImport(args[0], args[1], args[2], args[3], args[4]);
            }


        }

        private static void ProcessAlImport(string connectionString, string cc_StandardsDirectoryPath, string nssFilteDirectory, string alMappingFilesPath, string logsFilesDirectory)
        {
            if (!Directory.Exists(logsFilesDirectory))
                Directory.CreateDirectory(logsFilesDirectory);

            ImportService importService = new ImportCCStandardService(connectionString);
            Console.WriteLine(@"Start common core standards import");
            Console.WriteLine(@"----------------------------------");
            ImportStandardFromDirectory(cc_StandardsDirectoryPath, logsFilesDirectory, importService);
            Console.WriteLine(@"----------------------------------");
            Console.WriteLine(@"Finished Common core import");
            Console.WriteLine("");
            Console.WriteLine(@"Starting Nss mapping import to local storage");
            Console.WriteLine(@"----------------------------------");
            importService = new NSSImportService(connectionString);
            ImportStandardFromDirectory(nssFilteDirectory, logsFilesDirectory, importService);
            Console.WriteLine(@"----------------------------------");
            Console.WriteLine(@"Finished Common core import");
            Console.WriteLine("");
            Console.WriteLine(@"Starting Alabama abtocc mapping import");
            Console.WriteLine(@"----------------------------------");
            importService = new ImportAlMappingService(connectionString);
            ImportStandardFromDirectory(alMappingFilesPath, logsFilesDirectory, importService);
            Console.WriteLine(@"----------------------------------");
            Console.WriteLine(@"Finished import");
        }

        private static void ProcessImport(string connectionString, string cc_StandardsDirectoryPath, string ab_standardsDirectoryPath, string logsFilesDirectory)
        {
            if (!Directory.Exists(logsFilesDirectory))
                Directory.CreateDirectory(logsFilesDirectory);

            ImportService importService = new ImportCCStandardService(connectionString);
            Console.WriteLine(@"Start common core standards import");
            Console.WriteLine(@"----------------------------------");
            ImportStandardFromDirectory(cc_StandardsDirectoryPath, logsFilesDirectory, importService);
            Console.WriteLine(@"----------------------------------");
            Console.WriteLine(@"Finished Common core import");
            Console.WriteLine("");
            Console.WriteLine(@"Starting Academic Benchmark import");
            Console.WriteLine(@"----------------------------------");
            importService = new ImportABToCCMappingsService(connectionString);
            ImportStandardFromDirectory(ab_standardsDirectoryPath, logsFilesDirectory, importService);
            Console.WriteLine(@"Finished Academic Benchmark import");
        }


        private static bool ValidateConnectionString(string connectionString, out string  error)
        {
            try
            {
                var con = new SqlConnectionStringBuilder(connectionString);
                error = "";
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static void ImportStandardFromDirectory(string directory, string writingResDirectory, ImportService importService)
        {
            var filePathes = Directory.GetFiles(directory);
            foreach (var filePath in filePathes)
            {
                ImportStandardFromFile(filePath, writingResDirectory, importService);
            }
        }

        private static void ImportStandardFromFile(string filePath, string writingResDirectory, ImportService importService)
        {
            FileStream stream = null, stream2 = null;
            try
            {
                stream = File.OpenRead(filePath);
                var shortFileName = Path.GetFileNameWithoutExtension(stream.Name);
                Console.WriteLine(@"Loading standards from file {0}", shortFileName);
                var newFileName = GenerateImportResultFileName(stream.Name);
                stream2 = File.OpenWrite(Path.Combine(writingResDirectory, newFileName));
                byte[] fileContent = new byte[stream.Length];
                stream.Read(fileContent, 0, fileContent.Length);
                Console.WriteLine(@"Read data from file");
                
                importService.Import(fileContent);
                if (importService.CsvContainer.HasFaildRows())
                {
                    Console.WriteLine(@"Error importing file '{0}'. Check log file '{1}'", shortFileName, newFileName);
                }
                Console.WriteLine(@"Finishing import from  '{0}'", shortFileName);
                var mamoryStream = importService.CsvContainer.ToStream(',');
                mamoryStream.WriteTo(stream2);
                Console.WriteLine(@"Writed  import logs to '{0}'", newFileName);
              
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                stream?.Dispose();
                stream2?.Dispose();
            };
        }

        private static string GenerateImportResultFileName(string oldFileName)
        {
            return $"{Path.GetFileNameWithoutExtension(oldFileName)}_import_log{Path.GetExtension(oldFileName)}";
        }

    }
}
