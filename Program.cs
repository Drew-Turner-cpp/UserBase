class Runtime
{

    public string[] GeneralParserCommands = { "/view", "/addremove", "/close" };
    public string[] AddRemoveParserCommands = { "/adduser", "/removeuser", "/cleardata" };
    public string[] ViewParserCommands = { "/viewall", "/viewalladmin", "/viewuser" };
    public static void Main(string[] args)
    {
        Console.WriteLine(    "===============================\n" +
                              "====Welcome to Userbase 1.0====\n" +
                              "===============================\n" +
                              "====Type '/help' for a list====\n" +
                              "==========of commands.=========\n" +
                              "===============================\n");
        while (true)
        {
            string userQuery = query();

            if (userQuery == "/help")
            {
                Console.WriteLine("/view - more options to view users in UserBase\n" +
                                  "/addremove - more options to add or remove a user to/from UserBase\n" +
                                  "/close - exits UserBase shell");
            }

            else if (userQuery == "/close")
            {
                break;
            }

            else if (userQuery == "/addremove")
            {
                AddRemoveBranch();
            }

            else if (userQuery == "/view")
            {
                ViewBranch();
            }

            else
            {
                Console.WriteLine("Invalid Query");
            }
        }
            
    }

    public static string query(string instring="")
    {
        Console.Write(instring + ">");
        string q = Console.ReadLine();
        Console.WriteLine();
        return q;
    }

    public static void ViewBranch()
    {
        Console.WriteLine("========================================================\n" +
                          "=/viewall - view all usernames and userID's in UserBase=\n" +
                          "=/viewuser = view one user                             =\n" +
                          "=/return - return to UserBase main menu                =\n" +
                          "========================================================\n");
        while (true)
        {
            string userQuery = query();

            if (userQuery == "/viewall")
            {
                FileHandling.ViewDatabase();
            }

            else if (userQuery == "/viewalladmin")
            {
                string adminpassword = query("password\n");
                FileHandling.ViewDatabase(adminpassword);
            }

            else if (userQuery == "/return")
            {
                break;
            }

            else if (userQuery == "/viewuser")
            {
                string username = query("username\n");
                string password = query("password\n");

                User user = FileHandling.GetUser(username, password);

                if (user.name == "UserNotFoundError" && user.password == "UserNotFoundError")
                {
                    Console.WriteLine("User not found.");
                }

                else
                {
                    user.printUser();
                }
            }

        }
    }

    public static void AddRemoveBranch()
    {
        Console.WriteLine("===========================================\n" +
                              "=/adduser - add a user to UserBase        =\n" +
                              "=/removeuser - remove a user from UserBase=\n" +
                              "=/cleardata - clear all data from UserBase=\n" +
                              "=/return - return to UserBase main menu   =\n" +
                              "===========================================\n");
        while (true)
        {
            string userQuery = query();

            if (userQuery == "/adduser")
            {

                string username = query("username\n");
                string password = query("password\n");
                string userID = query("userID\n");
                Console.WriteLine();

                FileHandling.Store(new User(username, password, userID));

                Console.WriteLine("User has been registered to UserBase");
            }

            else if (userQuery == "/removeuser")
            {
                string username = query("username of the user you want to remove\n");
                string password = query("password of the user you want to remove\n");

                FileHandling.RemoveUser(username, password);
            }

            else if (userQuery == "/cleardata")
            {
                string filepath = query("exact path of file\n");
                FileHandling.ClearFile(filepath);
            }

            else if (userQuery == "/return")
            {
                break;
            }

            else
            {
                Console.WriteLine("Invalid Query");
            }
        }
    }
} // MAIN
public class User
{
    public string name;
    public string password;
    public string userID;
    public string adminpermission;
    public User(string name, string password, string userID, string adminpermission="0")
    {
        this.name = name;
        this.password = password;
        this.userID = userID;
        this.adminpermission = adminpermission;
    }

    public void printUser()
    {
        if (adminpermission == "SysOperator")
        {
            Console.WriteLine("Username: " + name);
            Console.WriteLine("Password: " + password);
            Console.WriteLine("User ID: " + userID);
        }
        else
        {
            Console.WriteLine("Username: " + name);
            Console.WriteLine("User ID: " + userID);
        }
    }
}
class UserHandling
{

    public static char[] CharacterArray = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
        'p', 'q','r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' ', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
    
    public static int ShiftIndex(char shiftChar, int key)
    {
        int returnValue = (Array.IndexOf(CharacterArray, shiftChar) + key) % CharacterArray.Length;

        if (returnValue < 0)  // Negative index handler
        {
            returnValue += CharacterArray.Length;
        }

        return returnValue;
    }

    public static string EncryptUser(User user, int customKey = 3)
    {
        string encryptedString = "";
        string[] attributes = { user.name, user.password, user.userID };

        foreach (string attribute in attributes)
        {
            foreach (char character in attribute)
            {
                encryptedString += CharacterArray[ShiftIndex(character, customKey)];
            }
            encryptedString += '$';
        }
       
        return encryptedString.TrimEnd('$');
    }

    public static User DecryptUser(string encryptedString, int customKey = -3)
    {
        string[] decryptElements = encryptedString.Split("$");
        string[] decryptedList = { "", "", "" };
        string appendString = "";


        int k = 0;

        foreach (string attributes in decryptElements)
        {
            foreach (char character in attributes)
            {
                appendString += CharacterArray[ShiftIndex(character, customKey)];
            }

            if (k == 3)  // Maybe figure out how to remove this if/break case
            {
                break;
            }

            decryptedList[k] = appendString;
            appendString = ""; // Reset appendString for a new element in decryptedList
            k++;
        }

       User user = new(decryptedList[0], decryptedList[1], decryptedList[2]);
       return user;
    }
}

class FileHandling
{
    public static string StoragePath = "C:\\Users\\Dtgoo\\source\\repos\\ConsoleApp1\\ConsoleApp1\\UserStorage.txt";

    public static void Store(User user)
    {
        using (StreamWriter writer = File.AppendText(StoragePath))
        {
            writer.WriteLine(UserHandling.EncryptUser(user));
        }
    }

    public static void ClearFile(string path)
    {
        try
        {
            File.WriteAllText(path, String.Empty);
        }
        catch
        {
            Console.WriteLine("Invalid Path");
        }
    }

    public static User GetUser(string username, string password)
    {
        string[] lines = File.ReadAllLines(StoragePath);
        User returnvar = new("UserNotFoundError", "UserNotFoundError", "1344");

        foreach (string line in lines)
        {
            User user = UserHandling.DecryptUser(line);
            if (user.name == username && user.password == password)
            {
                return user;
            }
        }
        
        return returnvar;
    }

    public static void ViewDatabase(string adminpassword="0")
    {
        if (adminpassword != "SysOperator")
        {
            string[] lines = File.ReadAllLines(StoragePath);

            foreach (string line in lines)
            {
                User user = UserHandling.DecryptUser(line);
                user.printUser();
                Console.WriteLine();
            }
        }
        else
        {
            string[] lines = File.ReadAllLines(StoragePath);

            foreach (string line in lines)
            {
                User user = UserHandling.DecryptUser(line);
                user.adminpermission = adminpassword;
                user.printUser();
                Console.WriteLine();
            }
        }
    }

    public static void RemoveUser(string username, string password)
    {
        string[] prelines = File.ReadAllLines(StoragePath);
        List<string> lines = prelines.ToList<string>();

        for (int i = 0; i < lines.Count; i++)
        {
            User user = UserHandling.DecryptUser(lines[i]);

            if (user.name == username && user.password == password)
            {
                lines.RemoveAt(i);
                break;
            }
        }

       
        string[] finallines = lines.ToArray();

        if (finallines.Length < prelines.Length)
        {
            File.WriteAllLines(StoragePath, finallines);
        }
        else
        {
            Console.WriteLine("User not found");
        }
    }

    public static void GetNumberOfUsers()
    {
        string[] users = File.ReadAllLines(StoragePath);
        Console.WriteLine(users.Length);
    }
}