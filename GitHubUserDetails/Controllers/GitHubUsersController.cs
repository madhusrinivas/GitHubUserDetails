using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using GitHubUserDetails.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace GitHubUserDetails.Controllers
{
    public class GitHubUsersController : Controller
    {        
        // GET: GitHubUsers/UserInfo
        //The final results page - displays User name, Location, Avatar (image) and top 5 repositories with highest stargazers count
        public ActionResult UserInfo(GitHubUser user)
        {
            GitHubUserDisplayInformation objDisplayInfo = new Models.GitHubUserDisplayInformation();
            try
            {
                GitHubUserRetrievalInfo objRetInfo = new GitHubUserRetrievalInfo();
                objRetInfo.UserName = objRetInfo.Location = objRetInfo.AvatarUrl = objRetInfo.RepoUrl = string.Empty;

                UriBuilder appUri = new UriBuilder(System.Configuration.ConfigurationManager.AppSettings["GitHubUri"]);
                appUri.Path += "/" + user.GitHubUserName;
                HttpWebRequest request = HttpWebRequest.Create(appUri.Uri) as HttpWebRequest;
                request.UserAgent = "GitHubUserInformation";
                
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JObject jResponse = JObject.Parse(new StreamReader(response.GetResponseStream()).ReadToEnd());

                        //Name
                        if (jResponse["name"] != null)
                            objRetInfo.UserName = jResponse["name"].ToString();
                        //Avatar Url
                        if (jResponse["avatar_url"] != null)
                            objRetInfo.AvatarUrl = jResponse["avatar_url"].ToString();
                        //Location
                        if (jResponse["location"] != null)
                            objRetInfo.Location = jResponse["location"].ToString();
                        //Repository Url
                        if (jResponse["repos_url"] != null)
                            objRetInfo.RepoUrl = jResponse["repos_url"].ToString();
                    }
                    else
                    {
                        objDisplayInfo.ErrorMsg = response.StatusDescription; 
                        objDisplayInfo.Repos = null;
                    }
                }
                objDisplayInfo = GetDisplayInfo(objRetInfo);
            }
            catch (WebException we)
            {
                objDisplayInfo.UserName = objDisplayInfo.Location = objDisplayInfo.Avatar = string.Empty;
                objDisplayInfo.ErrorMsg = new StreamReader(we.Response.GetResponseStream()).ReadToEnd().ToString();
                objDisplayInfo.Repos = null;
            }
            return View("Display", objDisplayInfo);
        }

        //GET: GitHubUsers/Display
        //Renders the UserInfo results on the view
        public ActionResult Display (GitHubUserDisplayInformation displayInfo)
        {
            return View();
        }

        //Method to retrieve repository information from Repos_URL
        private GitHubUserDisplayInformation GetDisplayInfo(GitHubUserRetrievalInfo userInfo)
        {
            GitHubUserDisplayInformation objDispInfo = new GitHubUserDisplayInformation();
            objDispInfo.UserName = objDispInfo.Location = objDispInfo.Avatar = string.Empty;
            string[] arrRepoNames;

            try
            {
                //Assign available information
                objDispInfo.UserName = userInfo.UserName;
                objDispInfo.Location = userInfo.Location;
                objDispInfo.Avatar = userInfo.AvatarUrl;

                //Retrieve repositories information            
                List<Repo> repoList = new List<Repo>();
                HttpWebRequest request = HttpWebRequest.Create(userInfo.RepoUrl) as HttpWebRequest;
                request.UserAgent = "GitHubReposInformation";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JArray jResponse = JArray.Parse(new StreamReader(response.GetResponseStream()).ReadToEnd());
                        foreach (JObject jObj in jResponse.Children<JObject>())
                        {
                            Repo objRepo = new Repo();
                            objRepo.RepoName = jObj["name"].ToString();
                            objRepo.StargazeCount = Convert.ToInt32(jObj["stargazers_count"].ToString());
                            repoList.Add(objRepo);
                        }
                    }
                }
                if (repoList.Count > 0)
                { arrRepoNames = repoList.OrderByDescending(o => o.StargazeCount).Take(5).Select(o => o.RepoName).ToArray<string>(); }
                else
                { arrRepoNames = new string[1] { "No repository information available." }; }
                objDispInfo.Repos = arrRepoNames;
            }
            catch (WebException we)
            {
                objDispInfo.UserName = objDispInfo.Location = objDispInfo.Avatar = string.Empty;
                objDispInfo.ErrorMsg = new StreamReader(we.Response.GetResponseStream()).ReadToEnd().ToString();
                objDispInfo.Repos = null;
            }

            return objDispInfo;
        }

        // GET: GitHubUsers/FindUserInfo
        //The home page - submit user name to retrieve information
        public ActionResult FindUserInfo()
        {
            return View();
        }
    }
}
