using Hero.Chatbot.Domain;
using Hero.Chatbot.Repository.Contracts;
using Hero.Shared.DbContext;
using Hero.Shared.Repository;
using Hero.Shared.UnitOfWork;
using Hero.Chatbot.ViewModel.Flight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Hero.Chatbot.Repository
{
    public class FlightRepository : AdoRepository<Flight>, IFlightRepository
    {
        public FlightRepository(DbProviderFactory dbProviderFactory, IDBContext dbContext, IUnitOfWork unitOfWork)
            : base(dbProviderFactory, dbContext, unitOfWork)
        {
        }

        public IEnumerable<Flight> GetFlights(FlightFilter filter)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                SELECT
                    *
                FROM
                    flight f
                LEFT JOIN
                    airline a
                    ON a.airlineId = f.airlineId
                WHERE
                    CAST(departureTime as date) = @departureDate
            ");

            if (!string.IsNullOrEmpty(filter.Origin))
            {
                sb.Append(@"
                AND
                    (LOWER(departureIataCode) = @departureIataCode OR LOWER(departureCityName) = @departureIataCode)
                ");
            }

            if (!string.IsNullOrEmpty(filter.Destination))
            {
                sb.Append(@"
                AND
                    (LOWER(arrivalIataCode) = @arrivalIataCode OR LOWER(arrivalCityName) = @arrivalIataCode)
                ");
            }

            var result = GetRecords(sb.ToString(), CommandType.Text,
                "@departureDate", filter.DepartureDate,
                "@departureIataCode", filter.Origin?.ToLower(),
                "@arrivalIataCode", filter.Destination?.ToLower());
            return result;
        }

        public override Flight PopulateRecord(IDataReader reader)
        {
            var model = new Flight
            {
                FlightId = (int)reader["flightId"],
                FlightNumber = (string)reader["flightNumber"],
                AirlineId = (int)reader["airlineId"],
                AirlineName = (string)reader["airlineName"],
                DepartureTime = ((DateTime)reader["departureTime"]),
                ArrivalTime = ((DateTime)reader["arrivalTime"]),
                DepartureIataCode = (string)reader["departureIataCode"],
                DepartureCityName = (string)reader["departureCityName"],
                DepartureAirportName = (string)reader["departureAirportName"],
                ArrivalIataCode = (string)reader["arrivalIataCode"],
                ArrivalCityName = (string)reader["arrivalCityName"],
                ArrivalAirportName = (string)reader["arrivalAirportName"],
                Duration = (decimal)reader["duration"],
                TotalSeats = (int)reader["totalSeats"]
            };

            return model;
        }
    }
}
