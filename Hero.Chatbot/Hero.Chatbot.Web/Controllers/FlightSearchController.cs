using FluentValidation;
using Hero.Chatbot.Service.Contracts;
using Hero.Chatbot.Web.Constants;
using Hero.Chatbot.Web.Enums;
using Hero.Chatbot.Web.Extensions;
using Hero.Chatbot.Web.Models;
using Hero.Chatbot.Web.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Hero.Chatbot.Web.Controllers
{
    public class FlightSearchController : ControllerBase
    {
        private readonly ISession _session;
        private readonly ICityService _cityService;

        public FlightSearchController(IHttpContextAccessor httpContextAccessor,
            ICityService cityService)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _cityService = cityService;
        }

        public IActionResult Index([FromQuery] FlightSearchViewModel searchViewModel)
        {
            var viewModel = searchViewModel ?? new FlightSearchViewModel();
            viewModel.Cities = _cityService.GetCities()
                .Select(x => new AutocompleteViewModel { Id = x.CityIataCode, Name = x.CityName });

            var criteriaCache = _session.Get<FlightSearchCriteriaViewModel>(SessionKeyConsts.FlightSearchCriteria);
            if (criteriaCache != null)
            {
                viewModel.Origin = criteriaCache.Origin;
                viewModel.Destination = criteriaCache.Destination;
                viewModel.DepartureDate = criteriaCache.DepartureDate;
                viewModel.ArrivalDate = criteriaCache.ArrivalDate;
            }
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult FlightSearchResult([FromQuery] FlightSearchCriteriaViewModel viewModel)
        {
            try
            {
                if (viewModel is null)
                {
                    throw new ArgumentNullException(nameof(viewModel));
                }

                _session.Set(SessionKeyConsts.FlightSearchCriteria, viewModel);
                new FlightSearchCriteriaValidator().ValidateAndThrow(viewModel);

                var currentState = _session.Get<FlightBookingStateViewModel>(SessionKeyConsts.FlightBookingState);
                if (currentState == null)
                    currentState = new FlightBookingStateViewModel();

                currentState.CurrentStep = EnumFlightBookingStep.SelectDepartureTime;
                currentState.NextStep = EnumFlightBookingStep.SelectArrivalTime;
                _session.Set(SessionKeyConsts.FlightBookingState, currentState);

                return RedirectToAction("SelectDepartureTime", "FlightBooking");
            }
            catch (ValidationException ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
