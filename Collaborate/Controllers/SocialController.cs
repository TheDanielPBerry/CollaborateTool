using Collaborate.Models;
using CollaborateDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Collaborate.Controllers
{
    public class SocialController : Controller
    {

        public SocialContext dbSocial = new SocialContext();
        public CollabContext dbCollab = new CollabContext();

        // GET: Social
        public ActionResult Index()
        {
            if (Convert.ToBoolean(Session["LoggedIn"]))
            {
                String username = Convert.ToString(Session["Username"]);
                var user = dbSocial.Users.Where(u => u.Username.Equals(username)).FirstOrDefault();
                return View(user);
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult CreateUser(UserViewModel vm)
        {
            if (Convert.ToBoolean(Session["LoggedIn"]))
            {
                Session["LoggedIn"] = false;
                Session["Username"] = null;
            }

            if (!String.IsNullOrEmpty(vm.UserName) && !String.IsNullOrEmpty(vm.Password) && vm.Password.Equals(vm.PasswordConfirm))
            {
                CollaborateDataAccessLayer.User user = new CollaborateDataAccessLayer.User();
                user.Created = DateTime.Now;
                user.Email = vm.Email;
                user.Username = vm.UserName;
                user.PasswordHash = PasswordHash(vm.Password);
                dbSocial.Users.Add(user);
                dbSocial.SaveChanges();
                Session["LoggedIn"] = true;
                Session["Username"] = vm.UserName;
                Session["UserId"] = dbSocial.Users.Where(u => u.Username.Equals(vm.UserName)).FirstOrDefault().IDUser;
                return RedirectToAction("Index", "Social");
            }
            return View();
        }



        public ActionResult LoginUser(LoginViewModel vm)
        {
            if (Convert.ToBoolean(Session["LoggedIn"]))
            {
                LoginViewModel model = new LoginViewModel() { Username = Convert.ToString(Session["Username"]) };
                return PartialView("_LoggedIn", model);
            }
            else if (vm.Username != null && vm.Password != null)
            {
                if (PasswordVerify(vm.Username, vm.Password))
                {
                    Session["Username"] = vm.Username;
                    Session["LoggedIn"] = true;
                    Session["UserId"] = dbSocial.Users.Where(u => u.Username.Equals(vm.Username)).FirstOrDefault().IDUser;
                    return RedirectToAction("Index", "Social");
                }
            }if (vm.Username != null || vm.Password!=null)
            {
                return RedirectToAction("Index", "Home");
            }
            return PartialView("_LoginPartial");
        }

        public ActionResult Logout()
        {
            Session["LoggedIn"] = false;
            Session["Username"] = false;
            return RedirectToAction("Index", "Home");
        }

        
        public ActionResult TeamEdit(int boardID)
        {
            TeamBuildViewModel vm = new TeamBuildViewModel();
            vm.Users = dbSocial.Users.OrderBy(u => u.Username).ToList();
            vm.Team = vm.Users.Select(a => a.UserBoards.Where(b => (b.UserID==a.IDUser && b.BoardID==boardID)).FirstOrDefault()!=null).ToList();
            vm.Board = dbCollab.Boards.Where(b => b.IDBoard == boardID).FirstOrDefault();
            return PartialView("_TeamEdit", vm);
        }
        public ActionResult SaveTeam(TeamBuildViewModel vm)
        {
            var userIds = dbSocial.Users.OrderBy(u => u.Username).Select(a => a.IDUser).ToList();
            dbSocial.UserBoards.RemoveRange(dbSocial.UserBoards.Where(a => a.BoardID==vm.Board.IDBoard));
            short count = 1;
            for(int i=0; i<userIds.Count(); i++)
            {
                if(vm.Team[i])
                {
                    dbSocial.UserBoards.Add(new UserBoard() { BoardID=vm.Board.IDBoard, UserID=userIds[i], IDUserBoard=(dbSocial.UserBoards.Max(a=>a.IDUserBoard)+count) } );
                    count++;
                }
            }
            dbSocial.SaveChanges();
            return RedirectToAction("Index", "Social");
        }




        public ActionResult ChatBar()
        {
            ChatViewModel vm = new ChatViewModel();
            int userID = Convert.ToInt32(Session["UserID"]);
            vm.Conversations = dbSocial.UserConversations.Where(c => c.UserID==userID && c.IsOwner.Value).Select(i => i.ConversationID.Value).ToList();
            return PartialView("_ChatBar", vm);
        }


        public ActionResult ChatView(int id)
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            if(dbSocial.UserConversations.FirstOrDefault(a => a.ConversationID==id && a.UserID==userID)!=null)
            {
                var model = dbSocial.Conversations.FirstOrDefault(a => a.IDConversation==id);
                dbSocial.UserConversations.FirstOrDefault(a => a.ConversationID == id && a.UserID == userID).IsOwner = true;
                dbSocial.SaveChanges();
                return PartialView("_Chat", model);
            }
            return null;
        }

        public void PostMessage(Message chat)
        {
            chat.Created = DateTime.Now;
            chat.UserID = Convert.ToInt32(Session["UserID"]);
            chat.IDMessage = dbSocial.Messages.Max(a => a.IDMessage)+1;
            dbSocial.Messages.Add(chat);
            dbSocial.SaveChanges();
        }


        public bool RefreshMessages(DateTime time)
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            List<UserConversation> convos = dbSocial.UserConversations.Where(c => c.UserID==userID).ToList();
            foreach(var conv in convos)
            {
                if(dbSocial.Messages.Where(m => (m.ConversationID==conv.ConversationID && m.Created>time)).Count()>0)
                {
                    return true;
                }
            }
            return false;
        }


        public void CloseChat(int id)
        {
            var userID = Convert.ToInt32(Session["UserID"]);
            var convo = dbSocial.UserConversations.FirstOrDefault(a => a.ConversationID==id && a.UserID==userID);
            convo.IsOwner = false;
            dbSocial.SaveChanges();
        }

        public ActionResult ChatList()
        {
            int userID = Convert.ToInt32(Session["UserID"]);
            var model = dbSocial.UserConversations.
                Join(dbSocial.Conversations, a => a.ConversationID, b => b.IDConversation, (c, a) => new { conversation=a, userID=c.UserID }).
                OrderBy(a => a.conversation.Messages.Max(b => b.Created)).
                Where(b => b.userID==userID).Select(a=> a.conversation).ToList<Conversation>();
            return PartialView("_ChatList", model);
        }









        private String PasswordHash(String password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }



        private bool PasswordVerify(String Username, String Password)
        {
            var user = dbSocial.Users.Where(u => u.Username.Equals(Username)).FirstOrDefault();
            if(user != null)
            {
                /* Fetch the stored value */
                string savedPasswordHash = user.PasswordHash;
                /* Extract the bytes */
                byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
                /* Get the salt */
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                /* Compute the hash on the password the user entered */
                var pbkdf2 = new Rfc2898DeriveBytes(Password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);
                /* Compare the results */
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i]) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }




    }
}