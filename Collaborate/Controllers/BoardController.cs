using CollaborateDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Collaborate.Controllers
{


    public class BoardController : Controller
    {
        
        private CollabContext db = new CollabContext();


        public BoardController()
        {
        }
        

        [HttpGet]
        public ActionResult Index(int id)
        {
            if(Convert.ToBoolean(Session["LoggedIn"]))
            {
                using (SocialContext dbs = new SocialContext())
                {
                    int userID = Convert.ToInt32(Session["UserID"]);
                    if(dbs.UserBoards.FirstOrDefault(a => a.BoardID==id && a.UserID==userID)!=null)
                    {
                        var model = db.Boards.FirstOrDefault(board => board.IDBoard == id);
                        if(model != null)
                        {
                            return View(model);
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Social");
        }



        [HttpGet]
        public ActionResult CreateCard(int id)
        {
            ViewBag.StoryPoints = new short[] { 1, 3, 5, 7, 9, 11 }.Select(a => new SelectListItem() { Text = a.ToString(), Value=a.ToString() }).ToList();
            Card model = new Card() { ColumnID = id };
            return PartialView("_CreateCard", model);
        }


        [HttpPost]
        public int AddCard(Card c)
        {
            db.Cards.Add(c);
            db.SaveChanges();
            return c.IDCard;
        }

        
        [HttpPost]
        public void MoveCard(int cardId, int columnId)
        {
            db.Cards.Where(card => card.IDCard == cardId).FirstOrDefault().ColumnID = columnId;
            db.SaveChanges();
        }
        

        [HttpGet]
        public ActionResult EditCard(int id)
        {
            ViewBag.StoryPoints = new short[] { 1, 3, 5, 7, 9, 11 }.Select(a => new SelectListItem() { Text = a.ToString(), Value=a.ToString() }).ToList();
            var model = db.Cards.Where(a => a.IDCard == id).FirstOrDefault();
            return PartialView("_EditCard", model);
        }

        [HttpPost]
        public void UpdateCard(Card c)
        {
            var model = db.Cards.Where(a => a.IDCard ==c.IDCard).FirstOrDefault();
            model.Name = c.Name;
            model.Description = c.Description;
            model.Points = c.Points;
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        [HttpPost]
        public void DeleteCard(int cardId)
        {
            db.Cards.Remove(db.Cards.Where(card => card.IDCard == cardId).FirstOrDefault());
            db.SaveChanges();
        }
        
        [HttpGet]
        public ActionResult CardView(int id)
        {
            var model = db.Cards.Where(card => card.IDCard==id).FirstOrDefault();
            return PartialView("_Card", model);
        }

        [HttpGet]
        public ActionResult CreateColumn(int id)
        {
            BoardColumn model = new BoardColumn() { BoardID=id };
            return PartialView("_CreateColumn", model);
        }


        [HttpPost]
        public int AddColumn(BoardColumn column)
        {
            int? priority = db.BoardColumns.Where(c => c.BoardID == column.BoardID).Max(a => a.Priority);
            if(priority.HasValue)
            {
                column.Priority = priority.Value + 1;
            }else
            {
                column.Priority = 0;
            }
            db.BoardColumns.Add(column);
            db.SaveChanges();
            return column.IDColumn;
        }


        public ActionResult ColumnView(int id)
        {
            return PartialView("_BoardColumn", db.BoardColumns.Where(bc => bc.IDColumn == id).FirstOrDefault());
        }


        public ActionResult EditBoard(int id)
        {
            var board = db.Boards.Where(m => m.IDBoard==id).FirstOrDefault();
            return PartialView("_EditBoard", board);
        }


        public ActionResult BoardShort(int boardID)
        {
            using (SocialContext dbs = new SocialContext())
            {
                ViewBag.TeamList = dbs.Users.Where(a => dbs.UserBoards.FirstOrDefault(b => (a.IDUser==b.UserID.Value && b.BoardID.Value==boardID)) != null).Select(a => a.Username).ToList<String>();
            }
            var board = db.Boards.Where(b => b.IDBoard == boardID).FirstOrDefault();
            return PartialView("_BoardShort", board);
        }


        public ActionResult CreateBoard(Board board)
        {
            if (!String.IsNullOrEmpty(board.Name) && !String.IsNullOrEmpty(board.Color))
            {
                board.Owner = Convert.ToInt32(Session["UserId"]);
                db.Boards.Add(board);
                UserBoard uBoard = new UserBoard();
                db.SaveChanges();
                uBoard.BoardID = board.IDBoard;
                uBoard.UserID = board.Owner;
                using (SocialContext dbs = new SocialContext())
                {
                    uBoard.IDUserBoard = (dbs.UserBoards.Max(a => a.IDUserBoard) + 1);
                    dbs.UserBoards.Add(uBoard);
                    dbs.SaveChanges();
                }
                return RedirectToAction("Index", "Social");
            }
            return PartialView("_CreateBoard");
        }


    }
}