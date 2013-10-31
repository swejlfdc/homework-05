using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using System.IO;

namespace GoldNumberServer
{
    class User
    {
        public User(string id)
        {
            this.ID = id;
        }
        internal string ID
        {
            get;
            set;
        }
        internal string Name { get; set; }
        internal string HashKey { get; set; }
    }
    class UserList : Dictionary<string, User> {
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        Hashtable NameSet = new Hashtable();
        internal bool AddStorageUser(string name, string HashKey) {
            if(NameSet.ContainsKey(name)) return false;
            string uid = Guid.NewGuid().ToString(); // suppose all user is legal
            User user = new User(uid);
            user.Name = name;
            user.HashKey = HashKey;
            // need to reconstruction
            /*
             * need judge if two user has the same name, it is illicit
             * */
            NameSet.Add(name, uid);
            this.Add(uid, user);
            return true;
        }
        public bool Add(string name, string password)
        {
            string HashKey = CalculateMD5Hash(password); 
            return AddStorageUser(name, HashKey);
        }
        public bool Contains(string name)
        {
            return NameSet.Contains(name);
        }
        public bool Confirm(string name, string password) {
            if (Contains(name) == false)
                return false;
            string uid = NameSet[name] as string;
            User user = this[uid];
            string HashKey = CalculateMD5Hash(password);
            return (HashKey == user.HashKey);
        }
        public string GetUid(string name) {
            return NameSet[name] as string;
        }
        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
    public class UserModule
    {
        private UserList users = new UserList();
        public HashSet<string> CurrentUserList  = new HashSet<string>(); // name
        private Object Datasource;
        private string FilePath;
        public UserModule() { }
        public UserModule(string FilePath) {
            FileInfo   TheFile   =   new   FileInfo(FilePath);  
            if(!TheFile.Exists) {
                File.Create(FilePath);
            }
            StreamReader file = new StreamReader(FilePath);
            // storage file may be replaced
            for (string str; (str = file.ReadLine()) != null; )
            {
                string[] imp = str.Split(' ');
                this.AddStorageUser(imp[0], imp[1]);  // split by space
            }
            file.Close();
            this.FilePath = FilePath;
        }
        public void Close()
        {
            StreamWriter sw = new StreamWriter(FilePath);
            foreach (User user in users.Values)
            {
                sw.WriteLine("{0} {1}", user.Name, user.HashKey);
            }
            sw.Close();
        }
        private bool AddStorageUser(string name, string HashKey) {
            return users.AddStorageUser(name, HashKey);
        }
        public bool AddUser(string name, string password)
        {
            return users.Add(name, password);
        }
        public bool Login(string name, string password)
        {
            if (!users.Confirm(name, password))
            {
                return false;
            }
            if (CurrentUserList.Contains(name)) return false;
            CurrentUserList.Add(name);
            return true;
        }
        public bool Register(string name, string password)
        {
            if (password == "") return false;
            if (users.Contains(name)) return false;
            users.Add(name, password);
            return true;
        }
        public void Logout(string name) {
            CurrentUserList.Remove(name);
        }
    }
}
