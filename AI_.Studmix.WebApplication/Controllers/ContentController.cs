using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.DAL.FileSystem;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;
using AI_.Studmix.WebApplication.ViewModels.Content;

namespace AI_.Studmix.WebApplication.Controllers
{
    [Authorize]
    public class ContentController : DataControllerBase
    {
        private const string STATE_VALUES_SEPARATOR = "|";
        private readonly IFileStorageManager _fileStorageManager;

        public ContentController(IUnitOfWork unitOfWork, IFileStorageManager fileStorageManager)
            : base(unitOfWork)
        {
            _fileStorageManager = fileStorageManager;
        }

        [HttpGet]
        public ViewResult Upload()
        {
            var viewModel = new UploadViewModel();
            viewModel.Properties = UnitOfWork.PropertyRepository.Get();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Upload(UploadViewModel viewModel)
        {
            viewModel.Properties = UnitOfWork.PropertyRepository.Get();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var package = new ContentPackage();
            package.PropertyStates = new Collection<PropertyState>();
            package.Files = new Collection<ContentFile>();
            package.Description = viewModel.Description;
            package.Caption = viewModel.Caption;
            package.Price = viewModel.Price;
            package.Owner = CurrentUser;

            InportFilesToPackage(package, viewModel.PreviewContentFiles, true);
            InportFilesToPackage(package, viewModel.ContentFiles, false);

            var service = new PropertyStateService();
            var specifiedStates = viewModel.States.Where(pair => !string.IsNullOrEmpty(pair.Value));
            foreach (var pair in specifiedStates)
            {
                //получаем состояние или создаем новое если не существет
                var propertyState = service.GetState(UnitOfWork, pair.Key, pair.Value);
                if (propertyState == null)
                {
                    var property = UnitOfWork.PropertyRepository.GetByID(pair.Key);
                    propertyState = service.CreateState(UnitOfWork, property, pair.Value);
                }

                package.PropertyStates.Add(propertyState);
            }

            _fileStorageManager.Store(package);
            UnitOfWork.ContentPackageRepository.Insert(package);
            UnitOfWork.Save();
            return View(viewModel);
        }

        private void InportFilesToPackage(ContentPackage package,
                                          IEnumerable<HttpPostedFileBase> files,
                                          bool isPreview)
        {
            foreach (var postedFile in files)
            {
                if (postedFile != null)
                {
                    var file = new ContentFile();
                    file.Name = postedFile.FileName;
                    file.Stream = postedFile.InputStream;
                    file.IsPreview = isPreview;
                    package.Files.Add(file);
                }
            }
        }

        [HttpGet]
        public ViewResult Search()
        {
            var viewModel = new SearchViewModel();
            viewModel.Properties = UnitOfWork.PropertyRepository.Get();
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult UpdateStates(Dictionary<int,string> states,int targetPropertyId)
        {
            var specifieStatePairs = states.Where(pair => !string.IsNullOrEmpty(pair.Value));
            var response = GetDefaultStates();

            if (specifieStatePairs.Count() == 0)
                return Json(response.Properties.Single(x => x.ID == targetPropertyId).States);

            var properties = UnitOfWork.PropertyRepository.Get();
            var service = new PropertyStateService();
            foreach (var statePair in specifieStatePairs)
            {
                var state = service.GetState(UnitOfWork, statePair.Key, statePair.Value);

                var property = properties.First(x => x.ID == statePair.Key);

                foreach (var prop in properties.Where(x => x.Order > property.Order))
                {
                    IEnumerable<PropertyState> propertyStates = new Collection<PropertyState>();
                    if (state != null)
                        propertyStates = service.GetBoundedStates(UnitOfWork, prop, state);

                    var joinedStates = string.Join(STATE_VALUES_SEPARATOR,
                                                   propertyStates.Select(st => st.Value));

                    var existingProperty = response.Properties.Single(x => x.ID == prop.ID);
                    response.Properties.Remove(existingProperty);
                    response.Properties.Add(new PropertyViewModel {ID = prop.ID, States = joinedStates});
                }
            }

            return Json(response.Properties.Single(x => x.ID == targetPropertyId).States);
        }

        private AjaxStatesViewModel GetDefaultStates()
        {
            var response = new AjaxStatesViewModel();
            var properties = UnitOfWork.PropertyRepository.Get();

            foreach (var property in properties)
            {
                var propertyViewModel = new PropertyViewModel {ID = property.ID};
                propertyViewModel.States = property.States == null
                                               ? string.Empty
                                               : string.Join(STATE_VALUES_SEPARATOR,
                                                             property.States.Select(state => state.Value));

                response.Properties.Add(propertyViewModel);
            }

            return response;
        }
    }
}