using Azure;
using Azure.Storage.Blobs;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Controllers
{
    public class AdvertisementController : Controller
    {
        private readonly SchoolCommunityContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName = "images";
        public AdvertisementController(SchoolCommunityContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IActionResult> IndexAsync(string id)
        {
            var viewModel = new AdsViewModel();
            viewModel.Community = _context.Communities.Where(x => x.Id == id).Single();
            viewModel.Advertisements = await _context.Advertisements.Where(x => x.community.Id == id).ToListAsync();
            return View(viewModel);
        }
        public IActionResult Upload(string id)
        {
            var viewModel = new FileInputViewModel();
            Community community = _context.Communities.Where(x => x.Id == id).Single();
            viewModel.CommunityId = community.Id;
            viewModel.CommunityTitle = community.Title;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(IFormFile file, string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BlobContainerClient containerClient;
            // Create the container and return a container client object
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                // Give access to public
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }


            try
            {
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(file.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                // add the photo to the database if it uploaded successfully
                var image = new Advertisement();
                image.Url = blockBlob.Uri.AbsoluteUri;
                image.FileName = file.FileName;
                image.community = _context.Communities.Where(i => i.Id == id).Single();
                _context.Advertisements.Add(image);
                _context.SaveChanges();
            }
            catch (RequestFailedException)
            {
                View("Error");
            }

            return RedirectToAction("Index", new { id = id });
        }

        public IActionResult Delete(string communityId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var community = _context.Communities.Where(x => x.Id == communityId).Single();
            var viewModel = new FileInputViewModel();
            viewModel.CommunityId = community.Id;
            viewModel.CommunityTitle = community.Title;
            var ad = _context.Advertisements.Where(m => m.AdvertisementId == id).Single();
            //viewModel.File = Path.Combine(ad.FileName, ad.Url);
            if (ad == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

       /* [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string communityId, int id)
        {



            var image = await _context.Ad.FindAsync(id);


            BlobContainerClient containerClient;
            // Get the container and return a container client object
            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            try
            {
                // Get the blob that holds the data
                var blockBlob = containerClient.GetBlobClient(image.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                _context.Ad.Remove(image);
                await _context.SaveChangesAsync();

            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            return RedirectToAction("Index", new { id = communityId });
        }
    }*/
}
}
