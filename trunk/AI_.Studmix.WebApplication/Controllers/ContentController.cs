using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AI_.Studmix.WebApplication.DAL.Database;
using AI_.Studmix.WebApplication.Models;
using AI_.Studmix.WebApplication.ViewModels.Content;

namespace AI_.Studmix.WebApplication.Controllers
{
    public class ContentController : DataControllerBase
    {
        private const string STATE_VALUES_SEPARATOR = "|";

        public ContentController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public ViewResult Upload()
        {
            var viewModel = new UploadViewModel();
            viewModel.Properties = UnitOfWork.PropertyRepository.Get();
            viewModel.States = new Dictionary<int, string>();

            return View(viewModel);
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