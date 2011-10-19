using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AI_.Studmix.WebApplication.DAL.Database;
using AI_.Studmix.WebApplication.DAL.FileSystem;
using AI_.Studmix.WebApplication.Models;
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

            var package = new ContentPackage();
            package.PropertyStates = new Collection<PropertyState>();
            package.Files = new Collection<ContentFile>();
            package.Description = viewModel.Description;
            package.Caption = viewModel.Caption;
            package.Price = viewModel.Price;
            package.Owner = CurrentUser;

            InportFilesToPackage(package, viewModel.PreviewContentFiles, true);
            InportFilesToPackage(package, viewModel.ContentFiles, false);

            if (package.Files.Count == 0)
            {
                ModelState.AddModelError("noFiles", "Должен быть добавлен хотя бы один файл");
                return View(viewModel);
            }

            var specifiedStates = viewModel.States.Where(pair => !string.IsNullOrEmpty(pair.Value));
            foreach (var pair in specifiedStates)
            {
                //получаем состояние или создаем новое если не существет
                var propertyState = PropertyState.Get(UnitOfWork, pair.Key, pair.Value);
                if (propertyState == null)
                {
                    var property = UnitOfWork.PropertyRepository.GetByID(pair.Key);
                    propertyState = new PropertyState {Value = pair.Value, Property = property};
                    UnitOfWork.PropertyStateRepository.Insert(propertyState);
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

        public ViewResult Download()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UpdateStates(UploadViewModel viewModel)
        {
            var specifieStatePairs = viewModel.States.Where(pair => !string.IsNullOrEmpty(pair.Value));
            var response = GetDefaultStates();

            if (specifieStatePairs.Count() == 0)
                return Json(response);

            var properties = UnitOfWork.PropertyRepository.Get();

            foreach (var statePair in specifieStatePairs)
            {
                var state = PropertyState.Get(UnitOfWork, statePair.Key, statePair.Value);

                var property = properties.First(x => x.ID == statePair.Key);

                foreach (var prop in properties.Where(x => x.Order > property.Order))
                {
                    IEnumerable<PropertyState> propertyStates = new Collection<PropertyState>();
                    if (state != null)
                        propertyStates = prop.GetBoundedStates(UnitOfWork, state);

                    var states = string.Join(STATE_VALUES_SEPARATOR,
                                             propertyStates.Select(st => st.Value));
                    response.Properties.Add(new PropertyViewModel {ID = prop.ID, States = states});
                }
            }

            return Json(response);
        }

        private dynamic GetDefaultStates()
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