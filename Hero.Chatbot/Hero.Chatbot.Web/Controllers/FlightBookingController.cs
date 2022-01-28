using Hero.Chatbot.Domain;
using Hero.Chatbot.Service.Contracts;
using Hero.Chatbot.ViewModel.Flight;
using Hero.Chatbot.Web.Constants;
using Hero.Chatbot.Web.Enums;
using Hero.Chatbot.Web.Extensions;
using Hero.Chatbot.Web.Mappings;
using Hero.Chatbot.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Hero.Chatbot.Web.Controllers
{
    public class FlightBookingController : ControllerBase
    {
        private readonly ISession _session;
        private readonly IFlightService _flightService;

        public FlightBookingController(IHttpContextAccessor httpContextAccessor,
            IFlightService flightService)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _flightService = flightService;
        }

        public IActionResult SelectDepartureTime()
        {
            var criteria = _session.Get<FlightSearchCriteriaViewModel>(SessionKeyConsts.FlightSearchCriteria);
            var flights = _flightService.GetFlights(new FlightFilter
            {
                DepartureDate = criteria.DepartureDate?.ToString("yyyy-MM-dd"),
                Origin = criteria.Origin,
                Destination = criteria.Destination
            });

            var result = new FlightSearchResultViewModel();
            result.DepartureDate = criteria.DepartureDate ?? throw new InvalidOperationException("Departure date is null");
            result.Flights = flights.Select(x => x.ToView());

            var currentState = _session.Get<FlightBookingStateViewModel>(SessionKeyConsts.FlightBookingState);
            if (currentState == null)
                return RedirectToActionPermanent("Index", "FlightSearch");

            return View(result);
        }

        public IActionResult SelectArrivalTime(DateTime departureTime, decimal amount)
        {
            var currentState = _session.Get<FlightBookingStateViewModel>(SessionKeyConsts.FlightBookingState);
            if (currentState == null)
                return RedirectToActionPermanent("Index", "FlightSearch");

            var criteria = _session.Get<FlightSearchCriteriaViewModel>(SessionKeyConsts.FlightSearchCriteria);
            var flights = _flightService.GetFlights(new FlightFilter
            {
                DepartureDate = criteria.ArrivalDate?.ToString("yyyy-MM-dd"),
                Origin = criteria.Destination,
                Destination = criteria.Origin
            });

            var result = new FlightSearchResultViewModel();
            result.ArrivalDate = criteria.ArrivalDate ?? throw new InvalidOperationException("Arrival date is null");
            result.Flights = flights.Select(x => x.ToView());

            currentState.DepartureDateTime = departureTime;
            currentState.DepartureAmount = amount;
            currentState.CurrentStep = EnumFlightBookingStep.SelectArrivalTime;
            currentState.NextStep = EnumFlightBookingStep.SelectPassengers;
            _session.Set(SessionKeyConsts.FlightBookingState, currentState);

            return View(result);
        }

        public IActionResult SelectPassengers(DateTime arrivalTime, decimal amount)
        {
            var currentState = _session.Get<FlightBookingStateViewModel>(SessionKeyConsts.FlightBookingState);
            if (currentState == null)
                return RedirectToActionPermanent("Index", "FlightSearch");

            currentState.ArrivalDateTime = arrivalTime;
            currentState.ArrivalAmount = amount;
            currentState.CurrentStep = EnumFlightBookingStep.SelectPassengers;
            currentState.NextStep = EnumFlightBookingStep.SelectPayment;
            _session.Set(SessionKeyConsts.FlightBookingState, currentState);

            return View();
        }

        public IActionResult SelectPayment(PassengerInfoViewModel passengerInfo)
        {
            var currentState = _session.Get<FlightBookingStateViewModel>(SessionKeyConsts.FlightBookingState);
            if (currentState == null)
                return RedirectToActionPermanent("Index", "FlightSearch");

            currentState.Passenger = passengerInfo;
            currentState.CurrentStep = EnumFlightBookingStep.SelectPayment;
            currentState.NextStep = EnumFlightBookingStep.BookingConfirmation;
            _session.Set(SessionKeyConsts.FlightBookingState, currentState);

            return View();
        }

        public IActionResult BookingConfirmation(PaymentInfoViewModel paymentInfo)
        {
            var currentState = _session.Get<FlightBookingStateViewModel>(SessionKeyConsts.FlightBookingState);
            if (currentState == null)
                return RedirectToActionPermanent("Index", "FlightSearch");

            currentState.Payment = paymentInfo;
            currentState.CurrentStep = EnumFlightBookingStep.BookingConfirmation;
            currentState.NextStep = EnumFlightBookingStep.ThankYou;
            _session.Set(SessionKeyConsts.FlightBookingState, currentState);

            return View();
        }

        public IActionResult ThankYou()
        {
            _session.Remove(SessionKeyConsts.FlightBookingState);
            return View();
        }
    }
}
