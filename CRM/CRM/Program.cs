using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CRM
{
    internal class Program
    {
        static Dictionary<string, List<Event>> EventsDictionary = new Dictionary<string, List<Event>>();
        static Dictionary<string, List<Event>> NearestEventsForCity = new Dictionary<string, List<Event>>();


        static void Main(string[] args)
        {

            var events = new List<Event>{
                                    new Event{ Name = "Phantom of the Opera", City = "New York"},
                                    new Event{ Name = "Metallica", City = "Los Angeles"},
                                    new Event{ Name = "Metallica", City = "New York"},
                                    new Event{ Name = "Metallica", City = "Boston"},
                                    new Event{ Name = "LadyGaGa", City = "New York"},
                                    new Event{ Name = "LadyGaGa", City = "Boston"},
                                    new Event{ Name = "LadyGaGa", City = "Chicago"},
                                    new Event{ Name = "LadyGaGa", City = "San Francisco"},
                                    new Event{ Name = "LadyGaGa", City = "Washington"}
                                    };

            var customer = new Customer { Name = "Mr. Fake", City = "New York" };

            var customers = new List<Customer>{
                                        new Customer{ Name = "Nathan", City = "New York"},
                                        new Customer{ Name = "Bob", City = "Boston"},
                                        new Customer{ Name = "Cindy", City = "Chicago"},
                                        new Customer{ Name = "Lisa", City = "Los Angeles"}
                                        };



            #region Question 1 Bad Solution is to loop on all events and get events with the city as the city of the customer
            SendEmailsToCustomerByLoopOnly(customer, events);
            #endregion


            #region Question 1 can be improved by using dictionaries of all events and when sending a mail to a customer it will be faster O(1) to get the list of events from the dictionary 
            AddEventToDictionary(events);
            SendEmailsToCustomerByLoopWithDictionary(customer);
            #endregion

            #region Question 2, to get the distances i will then have to loop on all events to get the distances between Customer and put them into sorted dictionary then get first 5 events in the dictionary
            Send5ClosestEvents(customer, events);
            #endregion

            #region Question 2 enhancement would be first to cache the result using Dictionary so if we encounter the same city then we have our answer , second enhancement would be before getting distances would be checking the events of my current city first
            Send5ClosestEventsBetter(customer, events);
            #endregion


            #region Question 3 if it's too exensive we would solve that by caching the result like done in question 2 using dictionaries

            #endregion

            #region Question 4 it will be a good way to handle errors by wraping the function call with try/catch to protect the app from exceptions 

            #endregion

            #region Question 5 we will implement a function that will sort events based on prices and then take first n events assuming that the price is not related with the distance at all 
            get5LowestPriceEvents(events);
            #endregion

        }

        static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
            + (distance > 0 ? $" ({distance} miles away)" : "")
            + (price.HasValue ? $" for ${price}" : ""));
        }
        static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }
        static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }
        private static int AlphebiticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i
          <
          Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }


        #region Helper Functions

        public static void SendEmailsToCustomerByLoopOnly(Customer _customer, List<Event> events)
        {

            /*foreach (var _event in events)
            {
                if (_event.City == _customer.City)
                {
                    AddToEmail(_customer, _event);
                }
            }
            */


            var EventsToSend = events.Where(e => e.City == _customer.City).ToList();
            foreach (var _event in EventsToSend)
            {
                AddToEmail(_customer, _event);
            }
        }

        public static void SendEmailsToCustomerByLoopWithDictionary(Customer _customer)
        {

            var EventsToSend = EventsDictionary.ContainsKey(_customer.City) ? EventsDictionary[_customer.City] : new List<Event>();
            foreach (var _event in EventsToSend)
            {
                AddToEmail(_customer, _event);
            }
        }

        public static void SendEmailsToCustomersByLoop(List<Customer> customers, List<Event> events)
        {
            foreach (var _customer in customers)
            {
                foreach (var _event in events)
                {
                    if (_event.City == _customer.City)
                    {
                        AddToEmail(_customer, _event);
                    }
                }
            }
        }

        public static void AddEventToDictionary(List<Event> events)
        {
            foreach (var _event in events)
            {
                if (!EventsDictionary.ContainsKey(_event.City))
                    EventsDictionary[_event.City] = new List<Event>();

                EventsDictionary[_event.City].Add(_event);
            }

        }

        public static List<Event> Send5ClosestEvents(Customer _customer, List<Event> _events)
        {
            List<Event> ClostestEvents = new List<Event>();
            SortedDictionary<int, List<Event>> EventsDistances = new SortedDictionary<int, List<Event>>();

            foreach (var _event in _events)
            {
                int distance = -1;
                try
                {
                    distance = GetDistance(_customer.City, _event.City);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }


                if (!EventsDistances.ContainsKey(distance))
                    EventsDistances[distance] = new List<Event>();

                EventsDistances[distance].Add(_event);
            }

            foreach (var key in EventsDistances.Keys)
            {
                if (ClostestEvents.Count == 5) break;
                ClostestEvents.AddRange(EventsDistances[key].Take(ClostestEvents.Count - 5));
            }
            return ClostestEvents;
        }

        private static List<Event> Send5ClosestEventsBetter(Customer customer, List<Event> events)
        {
            if (NearestEventsForCity.ContainsKey(customer.City)) return NearestEventsForCity[customer.City];

            List<Event> ClosestEvents = new List<Event>();

            ClosestEvents.AddRange(EventsDictionary.ContainsKey(customer.City) ? EventsDictionary[customer.City] : new List<Event>());
            if (ClosestEvents.Count >= 5) return ClosestEvents;

            SortedDictionary<int, List<Event>> EventsDistances = new SortedDictionary<int, List<Event>>();

            foreach (var _event in events)
            {
                if (_event.City == customer.City) continue;
                int distance = -1;
                try
                {
                    distance = GetDistance(customer.City, _event.City);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                if (!EventsDistances.ContainsKey(distance))
                    EventsDistances[distance] = new List<Event>();

                EventsDistances[distance].Add(_event);
            }

            foreach (var key in EventsDistances.Keys)
            {
                if (ClosestEvents.Count == 5) break;
                ClosestEvents.AddRange(EventsDistances[key].Take(ClosestEvents.Count - 5));
            }
            NearestEventsForCity[customer.City] = ClosestEvents;
            return ClosestEvents;
        }

        private static List<Event> get5LowestPriceEvents(List<Event> events)
        {
            return events.OrderBy(e => e.price).Take(5).ToList();
        }

        #endregion

    }
}
