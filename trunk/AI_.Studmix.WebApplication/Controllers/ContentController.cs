using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using AI_.Studmix.WebApplication.DAL.Database;
using AI_.Studmix.WebApplication.DAL.FileSystem;
using AI_.Studmix.WebApplication.Models;
using AI_.Studmix.WebApplication.ViewModels.Content;

namespace AI_.Studmix.WebApplication.Controllers
{
    public class ContentController : DataControllerBase
    {
        private readonly IFileStorageManager _fileStorageManager;
        private const string STATE_VALUES_SEPARATOR = "|";

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
            var package = new ContentPackage();
            package.PropertyStates = new Collection<PropertyState>();
            package.Files = new Collection<ContentFile>();

            foreach (var postedFile in viewModel.ContentFiles)
            {
                if (postedFile != null)
                {
                    var file = new ContentFile();
                    file.Name = postedFile.FileName;
                    file.Stream = postedFile.InputStream;
                    file.IsPreview = false;
                    package.Files.Add(file);
                }
            }

            foreach (var postedFile in viewModel.PreviewContentFiles)
            {
                if (postedFile != null)
                {
                    var file = new ContentFile();
                    file.Name = postedFile.FileName;
                    file.Stream = postedFile.InputStream;
                    file.IsPreview = true;
                    package.Files.Add(file);
                }
            }

            if (package.Files.Count == 0)
            {
                ModelState.AddModelError("noFiles", "Должен быть добавлен хотя бы один файл");
                return RedirectToAction("Upload");
            }

            var specifiedStates = viewModel.States.Where(pair => !string.IsNullOrEmpty(pair.Value));
            foreach (var pair in specifiedStates)
            {
                //получаем состояние или создаем новое если не существет
                var propertyState = PropertyState.Get(UnitOfWork, pair.Key, pair.Value);
                if (propertyState == null)
                {
                    var property = UnitOfWork.PropertyRepository.GetByID(pair.Key);
                    propertyState = new PropertyState { Value = pair.Value, Property = property };
                    UnitOfWork.PropertyStateRepository.Insert(propertyState);
                }

                package.PropertyStates.Add(propertyState);
            }

            _fileStorageManager.Store(package);
            UnitOfWork.ContentPackageRepository.Insert(package);
            UnitOfWork.Save();
            return RedirectToAction("Upload");
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
                var property = state.Property;

                foreach (var prop in properties.Where(x => x.Order > property.Order))
                {
                    var propertyStates = prop.GetBoundedStates(UnitOfWork, state);
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