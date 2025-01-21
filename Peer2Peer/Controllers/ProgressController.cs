using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Peer2Peer.Helpers;
using Peer2Peer.Nodes;

namespace Peer2Peer.Controllers
{
    public class ProgressController : Controller
    {
        public IActionResult Index()
        {
            // Console.WriteLine("Progress requested");
            var progress = new List<string>();
            for (int i = 0; i < ProgressStatus.TotalChunks.Count; i++)
            {
                progress.Add($"Password length {i + 1}: {ProgressStatus.CompletedChunks[i]}/{ProgressStatus.TotalChunks[i]}");
            }
            if (ProgressStatus.FoundPassword != null)
            {
                progress.Add("Password found: " + ProgressStatus.FoundPassword);
            }
            // Console.WriteLine("Progress sent");
            // Console.WriteLine(string.Join("\n", progress));
            return View(progress);
        }
    }
}