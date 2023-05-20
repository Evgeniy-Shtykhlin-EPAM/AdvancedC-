namespace AdvancedC_
{
    internal class Program
    {
        public enum TypeOfEntity
        {
            Folder = 1,
            File = 2
        }
        public class FoundEntities
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool SearchedByUserConditions { get; set; }
        }

        public class FileSystemVisitor
        {
            public string FolderPath { get; set; }
            public string FileOrFolderName { get; set; }
            public TypeOfEntity TypeOfEntity { get; set; }
            public List<FoundEntities> FoundEntitiesList { get; set; }= new List<FoundEntities>();

            public delegate void SetValuesDelegate(string folderPath, string fileOrFolderName, TypeOfEntity typeOfEntity);
            public delegate bool ShowNotificationDelegate(string key, string value);

            public void SetValuesForEntity(string folderPath, string fileOrFolderName, TypeOfEntity typeOfEntity)
            {
                FolderPath = folderPath;
                TypeOfEntity = typeOfEntity;
                FileOrFolderName = fileOrFolderName;
            }
            public void Searching()
            {
                Console.WriteLine("Searching is started.");
                try
                {
                    var folders = Directory.GetDirectories(FolderPath);
                    foreach (var folder in folders)
                    {
                        FoundEntitiesList.Add(new FoundEntities{Name = folder.Remove(0, FolderPath.Length),Type = "Folder"});
                    }

                    var files = Directory.GetFiles(FolderPath);

                    foreach (var file in files)
                    {
                        FoundEntitiesList.Add(new FoundEntities() { Name = file.Remove(0, FolderPath.Length), Type = "File" });
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Searching Error.");
                    throw;
                }
                finally
                {
                    Console.WriteLine("Searching is finished.");
                }
            }

            public void ShowAllFoundEntities()
            {
                Console.WriteLine();
                Console.WriteLine("Folders and files found:" + FoundEntitiesList.Count);
                Console.WriteLine("_________________");
                var i = 1;
                foreach (var entity in FoundEntitiesList)
                {
                    if (entity.Name.Equals(FileOrFolderName) && entity.Type == TypeOfEntity.ToString())
                    {
                        ShowNotificationDelegate notificationDelegate = (ShowNotificationSuccessFound);
                        entity.SearchedByUserConditions = notificationDelegate(entity.Name, entity.Type);
                        Console.WriteLine(i + ". " + entity.Name + " - " + entity.Type+ " FOUNDED");
                    }
                    else
                    {
                        Console.WriteLine(i + ". " + entity.Name + " - " + entity.Type);
                    }
                    i++;
                }
                Console.WriteLine("_________________");
            }

            public void ShowFoundEntitiesByType()
            {
                Console.WriteLine("_________________");
                var i = 1;
                var entitiesByType = FoundEntitiesList.Where(entity => entity.Type == TypeOfEntity.ToString()).Select(entity => entity.Name).ToList();

                Console.WriteLine("Found " + entitiesByType.Count + " type of " + TypeOfEntity);
                foreach (var entity in entitiesByType)
                {
                    Console.WriteLine(i + " " + entity);
                    i++;
                }

                Console.WriteLine("_________________");
            }

            public bool ShowNotificationSuccessFound(string key, string value) => true;
        }

        public delegate void StartSearchDelegate();
        public static event StartSearchDelegate StartSearchEvent;

        static void Main(string[] args)
        {
            string folderPath;
            string fileOrFolderName;
            TypeOfEntity typeOfEntity;
            string startSearch;
            string userChoise;

            do
            {
                Console.WriteLine("Enter folder path");
                folderPath = Console.ReadLine();
            } while (folderPath.Length < 1);


            Console.WriteLine("Enter file(or folder) name:");
            fileOrFolderName = Console.ReadLine();


            string choiseForTypeOfEntity;
            do
            {
                Console.WriteLine("Enter the type of entity: \n 1 - File \n 2 - Folder.");
                choiseForTypeOfEntity = Console.ReadLine();
            } while (choiseForTypeOfEntity != "1" && choiseForTypeOfEntity != "2");
            typeOfEntity = choiseForTypeOfEntity == "1" ? TypeOfEntity.File : TypeOfEntity.Folder;

            FileSystemVisitor fileSystemVisitor = new FileSystemVisitor();

            FileSystemVisitor.SetValuesDelegate setValuesDelegate = (fileSystemVisitor.SetValuesForEntity);
            setValuesDelegate(folderPath, fileOrFolderName, typeOfEntity);

            do
            {
                Console.WriteLine("Enter start");
                startSearch = Console.ReadLine().ToLower();
            } while (startSearch != "start");

            StartSearchEvent += fileSystemVisitor.Searching;
            StartSearchEvent();

            var chancel=Console.ReadLine();

            Console.WriteLine("_________________");

            do
            {
                Console.WriteLine("Choose action: " +
                                  "\n 1 - Show all founded folders and files with founded by user NAME condition marks " +
                                  "\n 2 - Show founded by user TYPE condition marks.");
                userChoise = Console.ReadLine().ToLower();
            } while (userChoise != "1" && userChoise != "2");

            switch (userChoise)
            {
                case "1":
                    fileSystemVisitor.ShowAllFoundEntities();
                    break;
                case "2":
                    fileSystemVisitor.ShowFoundEntitiesByType();
                    break;
            }
            
            Console.ReadKey();
        }
    }
}
