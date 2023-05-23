namespace AdvancedC_
{
    internal class Program
    {
        public delegate void StartSearchDelegate();

        public enum TypeOfEntity
        {
            Folder = 1,
            File = 2
        }

        public enum UserChoiseAbortOrExclude
        {
            Abort = 1,
            Exclude = 2
        }

        public class FoundEntities
        {
            public string Name { get; set; }
            public TypeOfEntity TypeOfEntity { get; set; }
            public bool? SearchedByUserConditions { get; set; }
        }

        public class FileSystemVisitor
        {
            public FileSystemVisitor(string folderPath)
            {
                FolderPath = folderPath;

                StartSearchDelegate = (StartOrdinarySearch);
                StartSearchDelegate += ShowOrdinarySearchForUser;
                StartSearchDelegate();
            }

            public FileSystemVisitor(string folderPath, string fileOrFolderName, TypeOfEntity typeOfEntity, UserChoiseAbortOrExclude abortOrExclude)
            {
                FolderPath = folderPath;
                FileOrFolderName = fileOrFolderName;
                TypeOfEntity = typeOfEntity;
                AbortOrExclude= abortOrExclude;

                StartSearchDelegate = (StartOrdinarySearch);
                StartSearchDelegate += ShowAllFoundedEntitiesWithMarkIfItSatisfiedUserConditions;
                StartSearchDelegate += ShowFinalList;
                StartSearchDelegate();
            }

            public string FolderPath { get; set; }
            public string? FileOrFolderName { get; set; }
            public TypeOfEntity? TypeOfEntity { get; set; }
            public List<FoundEntities> FoundAllEntitiesList { get; set; } = new List<FoundEntities>();
            public List<FoundEntities> FinalFoundEntitiesList { get; set; } = new List<FoundEntities>();
            public StartSearchDelegate StartSearchDelegate { get; set; }
            public UserChoiseAbortOrExclude AbortOrExclude { get; set; }

            public delegate void ShowMessageDelegate(string message);

            //For ordinary search
            public void StartOrdinarySearch()
            {
                ShowMessageDelegate showMessageDelegate = ShowMessage;
                showMessageDelegate("Start searching...");
                Console.WriteLine("OrdinarySearch search is started.");
                if (Directory.Exists(FolderPath))
                {
                    var folders = Directory.GetDirectories(FolderPath);
                    var files = Directory.GetFiles(FolderPath);
                    foreach (var folder in folders)
                    {
                        FoundAllEntitiesList.Add(new FoundEntities(){ Name = folder.Remove(0, FolderPath.Length), TypeOfEntity = Program.TypeOfEntity.Folder});
                    }
                    foreach (var file in files)
                    {
                        FoundAllEntitiesList.Add(new FoundEntities() { Name = file.Remove(0, FolderPath.Length), TypeOfEntity = Program.TypeOfEntity.File });
                    }
                }
                else
                {
                    Console.WriteLine("Error. Can not find directory.");
                }
            }

            public void ShowOrdinarySearchForUser()
            {
                ShowMessageDelegate showMessageDelegate = ShowMessage;
                Console.WriteLine($"Found: " + FoundAllEntitiesList.Count + " entities");
                var i=1;
                foreach (var entity in FoundAllEntitiesList)
                {
                    Thread.Sleep(500);
                    var message =i+ ". Found "+entity.Name+"-"+" "+ entity.TypeOfEntity;
                    showMessageDelegate(message);
                    i++;
                }
                showMessageDelegate("Searching is finished.");
            }

            //For NOT ordinary search
            public void ShowAllFoundedEntitiesWithMarkIfItSatisfiedUserConditions()
            {
                ShowMessageDelegate showMessageDelegate = ShowMessage;
                Console.WriteLine($"Found: " + FoundAllEntitiesList.Count + " entities");

                var i = 1;
                if (AbortOrExclude == UserChoiseAbortOrExclude.Abort)
                {
                    foreach (var entity in FoundAllEntitiesList)
                    {
                        Thread.Sleep(500);
                        string message;
                        if (entity.Name.Contains(FileOrFolderName) && entity.TypeOfEntity == TypeOfEntity)
                        {
                            entity.SearchedByUserConditions = true;
                            message = i + ". Found " + entity.Name + " - " + entity.TypeOfEntity + " FOUNDED by user conditions";
                            showMessageDelegate(message);
                            Console.WriteLine("Abort searching...");
                            return;
                        }
                        else
                        {
                            FinalFoundEntitiesList.Add(entity);
                            message = i + ". Found " + entity.Name + " - " + entity.TypeOfEntity;
                            showMessageDelegate(message);
                        }
                        i++;
                    }
                    showMessageDelegate("Searching is finished not found any entities.");

                }
                else
                {
                    foreach (var entity in FoundAllEntitiesList)
                    {
                        Thread.Sleep(500);
                        string message;
                        int count = 0;
                        if (entity.Name.Contains(FileOrFolderName) && entity.TypeOfEntity == TypeOfEntity)
                        {
                            count++;

                            message = i + ". Found " + entity.Name + "-" + " " + entity.TypeOfEntity + " founded by user conditions.";
                            showMessageDelegate(message);
                            showMessageDelegate("Excluding from final list...");
                            Thread.Sleep(1500);
                            showMessageDelegate("Excluded.");
                        }
                        else
                        {
                            FinalFoundEntitiesList.Add(entity);
                            message = i + ". Found " + entity.Name + "-" + " " + entity.TypeOfEntity;
                            showMessageDelegate(message);
                        }
                        i++;
                    }
                }
            }

            public void ShowFinalList()
            {
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("Final list founded entities.");
                var i = 1;
                foreach (var entity in FinalFoundEntitiesList)
                {
                    Console.WriteLine(i + ". Found " + entity.Name + "-" + " " + entity.TypeOfEntity);
                    i++;
                }
            }

            public void ShowMessage(string message)
            {
                Console.WriteLine(message);
            }
        }


        static void Main(string[] args)
        {
            string folderPath;
            string fileOrFolderName;
            TypeOfEntity typeOfEntity;
            UserChoiseAbortOrExclude abortOrExclude;


            string choiseNeedFind;
            string choiseNeedFindFileOrFolder;
            string userChoiseAbortOrExclude;
            FileSystemVisitor fileSystemVisitor;

            do
            {
                Console.WriteLine("Enter folder path");
                folderPath = Console.ReadLine();
            } while (folderPath.Length < 1);


            do
            {
                Console.WriteLine("Do you need find file/folder \n 1 - Yes \n 2 - No.");
                choiseNeedFind = Console.ReadLine();
            } while (choiseNeedFind != "1" && choiseNeedFind != "2");

            if (choiseNeedFind == "2")
            {
                //create filevisitor ordinary
                fileSystemVisitor = new FileSystemVisitor(folderPath);
            }
            else
            {
                //create filevisitor with delegate
                do
                {
                    Console.WriteLine("What do you want find \n 1 - File \n 2 - Folder.");
                    choiseNeedFindFileOrFolder = Console.ReadLine();
                } while (choiseNeedFindFileOrFolder != "1" && choiseNeedFindFileOrFolder != "2");

                typeOfEntity = choiseNeedFindFileOrFolder == "1" ? TypeOfEntity.File : TypeOfEntity.Folder;

                Console.WriteLine("Enter Name finding entity");
                fileOrFolderName = Console.ReadLine();

                do
                {
                    Console.WriteLine("What should be next step \n 1 - Abort searching if find by user condition " +
                                      "\n 2 - Exclude found file/folder.");
                    userChoiseAbortOrExclude = Console.ReadLine();
                } while (userChoiseAbortOrExclude != "1" && userChoiseAbortOrExclude != "2");

                abortOrExclude = userChoiseAbortOrExclude =="1" ? UserChoiseAbortOrExclude.Abort : UserChoiseAbortOrExclude.Exclude;

                fileSystemVisitor = new FileSystemVisitor(folderPath, fileOrFolderName, typeOfEntity, abortOrExclude);
            }

            Console.WriteLine("Press any key for exit.");
            Console.ReadKey();
        }
    }
}
