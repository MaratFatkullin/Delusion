using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.DAL.FileSystem;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;
using AI_.Studmix.Model.Services.Abstractions;
using AI_.Studmix.WebApplication.ViewModels.Content;
using AI_.Studmix.WebApplication.ViewModels.Shared;

namespace AI_.Studmix.WebApplication.Controllers
{
    [Authorize]
    public class ContentController : DataControllerBase
    {
        private const string STATE_VALUES_SEPARATOR = "|";
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IFinanceService _financeService;

        public ContentController(IUnitOfWork unitOfWork,
                                 IFileStorageManager fileStorageManager,
                                 IFinanceService financeService)
            : base(unitOfWork)
        {
            _fileStorageManager = fileStorageManager;
            _financeService = financeService;
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

            ImportFilesToPackage(package, viewModel.PreviewContentFiles, true);
            ImportFilesToPackage(package, viewModel.ContentFiles, false);

            var service = new PropertyStateService(UnitOfWork);
            var specifiedStates = viewModel.States.Where(pair => !string.IsNullOrEmpty(pair.Value));
            foreach (var pair in specifiedStates)
            {
                //получаем состояние или создаем новое если не существет
                var propertyState = service.GetState(pair.Key, pair.Value);
                if (propertyState == null)
                {
                    var property = UnitOfWork.PropertyRepository.GetByID(pair.Key);
                    propertyState = service.CreateState(property, pair.Value);
                }

                package.PropertyStates.Add(propertyState);
            }

            _fileStorageManager.Store(package);
            UnitOfWork.ContentPackageRepository.Insert(package);
            UnitOfWork.Save();

            return InformationView("Загрузка завершена",
                                   "Контент успешно загружен. Благодарим за использование нашего ресурса.",
                                   new ActionLinkInfo("Content", "Upload", "Вернуться"));
        }

        private void ImportFilesToPackage(ContentPackage package,
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
        public ViewResult Search(SearchViewModel viewModel)
        {
            viewModel.Properties = UnitOfWork.PropertyRepository.Get();

            var stateService = new PropertyStateService(UnitOfWork);
            var propertyStates = viewModel.States
                .Select(pair => stateService.GetState(pair.Key, pair.Value))
                .Where(propertyState => propertyState != null)
                .ToList();

            var searchService = new SearchService(UnitOfWork);
            viewModel.Packages = searchService.FindPackageWithSamePropertyStates(propertyStates);

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult UpdateStates(Dictionary<int,string> states,int id)
        {
            var specifieStatePairs = states.Where(pair => !string.IsNullOrEmpty(pair.Value));
            var response = GetDefaultStates();

            if (specifieStatePairs.Count() == 0)
                return Json(response.Properties.Single(x => x.ID == id).States);

            var properties = UnitOfWork.PropertyRepository.Get();
            var service = new PropertyStateService(UnitOfWork);
            foreach (var statePair in specifieStatePairs)
            {
                var state = service.GetState(statePair.Key, statePair.Value);

                var property = properties.First(x => x.ID == statePair.Key);

                foreach (var prop in properties.Where(x => x.Order > property.Order))
                {
                    IEnumerable<PropertyState> propertyStates = new Collection<PropertyState>();
                    if (state != null)
                        propertyStates = service.GetBoundedStates(prop, state);

                    var joinedStates = string.Join(STATE_VALUES_SEPARATOR,
                                                   propertyStates.Select(st => st.Value));

                    var existingProperty = response.Properties.Single(x => x.ID == prop.ID);
                    response.Properties.Remove(existingProperty);
                    response.Properties.Add(new PropertyViewModel {ID = prop.ID, States = joinedStates});
                }
            }

            return Json(response.Properties.Single(x => x.ID == id).States);
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

        [HttpGet]
        public ViewResult Details(int id)
        {
            var contentPackage = UnitOfWork.ContentPackageRepository.GetByID(id);
            var properties = UnitOfWork.PropertyRepository.Get();
            if (contentPackage == null)
                return ErrorView("Материал не найден", "Указанный материал отсутствует в базе данных.");

            var viewModel = new DetailsViewModel {Package = contentPackage, Properties = properties};

            var userHasPermissions = _financeService.UserHasPermissions(CurrentUser,contentPackage);
            var userIsAdmin = User.IsInRole("admin");

            viewModel.IsFullAccessGranted = userHasPermissions || userIsAdmin;

            return View(viewModel);
        }

        public ActionResult Download(int id)
        {
            var contentFile = UnitOfWork.ContentFileRepository.GetByID(id);
            if (contentFile == null)
                return ErrorView("Файл не найден", "Указаный файл отсутствует или был удален.");
            var accessGranted = _financeService.UserHasPermissions(CurrentUser,contentFile.ContentPackage);
            var userIsAdmin = User.IsInRole("admin");
            if (!accessGranted && !userIsAdmin)
                return ErrorView("Ошибка доступа", "Доступ к скачиванию файла закрыт.");

            return new FileStreamResult(_fileStorageManager.GetFileStream(contentFile), "image/jpeg");
        }
    }
}