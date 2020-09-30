using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.User
{
    public class AuthManager
    {
        public static AuthManager Instance;

        protected static string UserName = "DevUser";
        protected static int UserID = 666;
        protected bool isOriginal = false;


        public AuthManager()
        {
            Instance = this;
        } 

        public void Login()
        {

        }

        public static string GetUserName { get { return AuthManager.UserName; } }
        public static int GetUserId { get { return AuthManager.UserID; } }
    }
}
