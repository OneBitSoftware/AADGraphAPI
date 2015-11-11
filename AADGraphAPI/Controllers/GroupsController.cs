using Microsoft.Azure.ActiveDirectory.GraphClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using AADGraphAPI.Models;

namespace AADGraphAPI.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        // GET: Groups
        public async Task<ActionResult> Index()
        {
            var viewModel = new GroupsMembersViewModel();
            List<Group> groupList = new List<Group>();
            List<User> memberList = new List<User>();
            viewModel.Groups = groupList;
            viewModel.GroupMembers = memberList;

            try
            {
                ActiveDirectoryClient client = AuthenticationHelper.GetActiveDirectoryClient();

                IPagedCollection<IGroup> pagedCollection = await client.Groups.ExecuteAsync();
                do
                {
                    List<IGroup> groups = pagedCollection.CurrentPage.ToList();
                    foreach (IGroup group in groups)
                    {
                        groupList.Add((Group)group);
                    }
                    pagedCollection = pagedCollection.GetNextPageAsync().Result;
                } while (pagedCollection != null);
            }
            catch (Exception e)
            {
                if (Request.QueryString["reauth"] == "True")
                {
                    HttpContext.GetOwinContext()
                        .Authentication.Challenge(OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
                ViewBag.ErrorMessage = "AuthorizationRequired";
                return View(viewModel);
            }
            return View(viewModel);
        }


        public async Task<ActionResult> GetGroupMembers(string objectId)
        {
            var viewModel = new GroupsMembersViewModel();
            List<Group> groupList = new List<Group>();
            List<User> memberList = new List<User>();
            viewModel.Groups = groupList;
            viewModel.GroupMembers = memberList;

            try
            {
                ActiveDirectoryClient client = AuthenticationHelper.GetActiveDirectoryClient();

                #region GetGroups

                IPagedCollection<IGroup> pagedCollection = await client.Groups.ExecuteAsync();
                do
                {
                    List<IGroup> groups = pagedCollection.CurrentPage.ToList();
                    foreach (IGroup group in groups)
                    {
                        groupList.Add((Group)group);
                    }
                    pagedCollection = pagedCollection.GetNextPageAsync().Result;
                } while (pagedCollection != null);
                #endregion

                #region Populate selected group members
                if (string.IsNullOrEmpty(objectId)) return View("Index",viewModel);

                var selectedGroup = await client.Groups.Where
                    (g => g.ObjectId == objectId).ExecuteSingleAsync();
                IGroupFetcher groupFetcher = (IGroupFetcher)selectedGroup;
                IPagedCollection<IDirectoryObject> members =
                    groupFetcher.Members.ExecuteAsync().Result;

                do
                {
                    List<IDirectoryObject> directoryObjects = members.CurrentPage.ToList();
                    foreach (IDirectoryObject member in directoryObjects)
                    {
                        var user = member as User;
                        viewModel.GroupMembers.Add(user);
                    }

                    members = members.MorePagesAvailable ?
                        members = members.GetNextPageAsync().Result : null;
                }
                while (members != null);

                #endregion
            }
            catch (Exception e)
            {
                if (Request.QueryString["reauth"] == "True")
                {   HttpContext.GetOwinContext()
                        .Authentication.Challenge(OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }

                ViewBag.ErrorMessage = "AuthorizationRequired";
                return View("Index", viewModel);
            }
            return View("Index", viewModel);
        }

        // GET: Groups/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Groups/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Groups/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
