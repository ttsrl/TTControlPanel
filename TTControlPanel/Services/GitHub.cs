using Octokit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TTControlPanel.Models.ViewModel;
using TTControlPanel.Utilities;

namespace TTControlPanel.Services
{
    public class GitHub
    {
        private Credentials credential;
        private GitHubClient client;
        private List<GitCommit> commits;
        private DateTime lastUpdate;

        public TimeSpan IntervalTime = new TimeSpan(0, 20, 0);

        public GitHub()
        {
            credential = new Credentials("ttsrl", "TTsrl092017");
            client = new GitHubClient(new ProductHeaderValue("ttsrl")) { Credentials = credential };
            commits = new List<GitCommit>();
        }

        public async Task<List<GitCommit>> GetCommits(int count = 10)
        {
            if (DateTime.UtcNow < lastUpdate.Add(IntervalTime))
                return this.commits;

            var commits = await client.Repository.Commit.GetAll("ttsrl", "TTControlPanel", new ApiOptions() { PageCount = 1, PageSize = count });
            var list = new List<GitCommit>();
            foreach (var c in commits)
            {
                var tmpc = await client.Repository.Commit.Get("ttsrl", "TTControlPanel", c.Sha);
                int adds = 0;
                int dels = 0;
                foreach (var cf in tmpc.Files)
                {
                    adds += cf.Additions;
                    dels += cf.Deletions;
                }
                var obj = new GitCommit
                {
                    AuthorUsername = c.Author.Login,
                    AuthorName = c.Commit.Author.Name,
                    Date = c.Commit.Author.Date,
                    Message = c.Commit.Message,
                    Files = tmpc.Files.Count,
                    Additions = adds,
                    Deletions = dels
                };
                list.Add(obj);
            }
            this.lastUpdate = DateTime.UtcNow;
            this.commits = list;
            return list;
        }

    }
}
