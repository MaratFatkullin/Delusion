using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AI_.Studmix.ApplicationServices.Services.Abstractions;
using AI_.Studmix.Domain.Entities;
using AI_.Studmix.WebApplication.ViewModels.Content;

namespace AI_.Studmix.WebApplication.Controllers
{
    [Authorize]
    public class ContentController : DataControllerBase
    {
        public IContentService ContentService { get; set; }

        public ContentController(IMembershipService membershipService, IContentService contentService)
            : base(membershipService)
        {
            ContentService = contentService;
        }

        [HttpGet]
        public ViewResult Upload()
        {
            var viewModel = new UploadViewModel();
            viewModel.Properties = ContentService.GetProperties();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Upload(UploadViewModel viewModel)
        {

            var contentPackage = new ContentPackage();
            contentPackage.Caption = viewModel.Caption;
            contentPackage.Description = viewModel.Description;
            contentPackage.Owner = CurrentUser;
            contentPackage.Price = viewModel.Price;
            foreach (var pair in viewModel.States)
            {
                new PropertyState()
            }
            contentPackage.PropertyStates = 
        }

        [HttpGet]
        public ViewResult Search()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ViewResult Search(SearchViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public JsonResult UpdateStates(Dictionary<int, string> states, int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ViewResult Details(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult Download(int id)
        {
            throw new NotImplementedException();
        }
    }
}