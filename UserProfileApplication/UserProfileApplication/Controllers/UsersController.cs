using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Stripe;
using System.Web.Script.Serialization;

namespace UserProfileApplication.Controllers
{
    public class UsersController : Controller
    {
        private userprofiledatabaseEntities db = new userprofiledatabaseEntities();

        // GET: Users/Sign In
        public ActionResult SignIn()
        {
            if(Session["userId"] == null)
                return View();
            else
                return RedirectToAction("Details", new { id= Session["userId"] });
        }

        // GET: Users/Sign Out
        public ActionResult SignOut()
        {
            Session["userEmail"] = Session["userId"]= null;
            return RedirectToAction("SignIn");
        }


        // POST: Users/SignIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "UserId,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                if (user.Email != null && user.Password != null)
                {
                    string query = "select * from users where email='" + user.Email + "' and password='" + user.Password + "'";

                    var result = db.Users.SqlQuery(query).ToList<User>();

                    if (result.Count > 0)
                    {
                        Session["userEmail"] = user.Email;
                        Session["userId"] = result[0].UserId;
                        return RedirectToAction("Details", new { id = result[0].UserId });
                    }
                    else
                        return RedirectToAction("SignIn");
                }
               
                return RedirectToAction("SignIn");

            }
            else
            {
                return RedirectToAction("SignIn");
            }
        }


        // GET: Users
        public ActionResult Index()
        {
           return RedirectToAction("SignIn");
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult CreateCard()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp([Bind(Include = "Id,Email,Password")] User user)
        {
            if (user.Email != null && user.Password != null)
            {

                if (ModelState.IsValid)
                {
                    db.Users.Add(user);
                    db.SaveChanges();

                    string query = "select * from users where email='" + user.Email + "' and password='" + user.Password + "'";

                    var result = db.Users.SqlQuery(query).ToList<User>();

                    if (result.Count > 0)
                    {
                        Session["userEmail"] = user.Email;
                        Session["userId"] = result[0].UserId;
                        return RedirectToAction("Details", new { id = result[0].UserId });
                    }
                }
            
                return RedirectToAction("SignUp");
            }

            return View(user);
        }

        [HttpPost]
        public bool SaveCard(string TokenId)
        {
            Console.WriteLine(TokenId);
            StripeConfiguration.SetApiKey("sk_test_sM18FjGmOrWQ550zlR6wlpxs");

            var myCustomer = new StripeCustomerCreateOptions();
            myCustomer.Email = Session["userEmail"].ToString();
            myCustomer.Description = "New User";

            myCustomer.SourceToken = TokenId;
            
            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Create(myCustomer);
                       
            User user = new User();
            user = db.Users.Find(Session["userId"]);
            user.CustomerId = stripeCustomer.Id;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return true;
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public User GetUser(int id)
        {
            User user = db.Users.Find(id);
            return user;
        }
    }
}
