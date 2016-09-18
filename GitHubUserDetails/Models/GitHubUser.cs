using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace GitHubUserDetails.Models
{
    public class GitHubUser
    {
        public int GitHubUserID { get; set; }

        [Required]
        [DisplayName("GitHub User Name")]
        public string GitHubUserName { get; set; }        
    }

    public class GitHubUserRetrievalInfo
    {
        public string UserName { get; set; }

        public string Location { get; set; }

        [DataType(DataType.ImageUrl)]
        public string AvatarUrl { get; set; }

        [DataType(DataType.Url)]
        public string RepoUrl { get; set; }
    }

    public class GitHubUserDisplayInformation
    {
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [DisplayName("Location")]
        public string Location { get; set; }

        [DisplayName("Avatar")]
        [DataType(DataType.ImageUrl)]
        public string Avatar { get; set; }

        [DisplayName("Repositories (Ones with most number of stargazers count)")]
        public string[] Repos { get; set; }

        public string ErrorMsg { get; set; }

        public bool Empty
        {
            get {
                return (
                  string.IsNullOrEmpty(UserName) &&
                  string.IsNullOrEmpty(Location) &&
                  string.IsNullOrEmpty(Avatar)
                  );
                    }
        }
    }

    public class Repo
    {
        public string RepoName { get; set; }

        public int StargazeCount { get; set; }
    }
}