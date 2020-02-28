﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelReservationsManager.Data;
using HotelReservationsManager.Data.Models;
using HotelReservationsManager.Data.Models.Enums;
using HotelReservationsManager.Models;
using HotelReservationsManager.Models.ClientViewModels;
using HotelReservationsManager.Models.ReservationViewModels;
using HotelReservationsManager.Models.RoomViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationsManager.Controllers
{
    public class ReservationController : Controller
    {
        private DbContext context;
        private UserManager<User> userManager;

        public ReservationController(DbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ReservationCreateViewModel model = new ReservationCreateViewModel()
            {
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now,
                ChoosenClients = new List<string>(),
                Clients = context.Clients.Select(c => new ClientViewModel()
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Id = c.Id,
                    IsAdult = c.IsAdult
                }).ToList(),
                Rooms = context.Rooms.Select(r => new RoomViewModel()
                {
                    Id = r.Id,
                    Capacity = r.Capacity,
                    Type = r.Type,
                    Number = r.Number

                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(ReservationCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var previousReservation in context.Reservations)
                {
                    if (previousReservation.Room.Id == model.ChoosenRoom
                        && ((previousReservation.CheckInDate <= model.CheckInDate
                            && model.CheckInDate < previousReservation.CheckOutDate) ||
                            (previousReservation.CheckInDate < model.CheckOutDate
                            && model.CheckInDate <= previousReservation.CheckOutDate)))
                    {
                        return Redirect("~/Reservation/RoomNotFree");
                        //ModelState.AddModelError("ChoosenRoom", "There is already created room with this number.");
                    }
                }

                Room room = context.Rooms.Find(model.ChoosenRoom);

                if (model.ChoosenClients.Count > room.Capacity)
                {
                    return Redirect("~/Reservation/TooManyPeople");
                }

                User creator = context.Users.Find(userManager.GetUserId(User));

                Reservation reservation = new Reservation(room, creator, new List<ClientReservation>(), model.CheckInDate, model.CheckOutDate, model.IsBreakfastIncluded, model.IsAllInclusive, 0);

                reservation.ClientReservations = model.ChoosenClients.Select(c => context.Clients.Find(c))
                    .Select(c => new ClientReservation(c.Id, c, reservation.Id, reservation)).ToList();

                foreach (var client in reservation.ClientReservations.Select(cr => cr.Client))
                {
                    if (client.IsAdult)
                    {
                        reservation.Price += reservation.Room.AdultPrice;
                    }
                    else
                    {
                        reservation.Price += reservation.Room.ChildPrice;
                    }
                }

                context.Reservations.Add(reservation);
                context.SaveChanges();

                return Redirect("~/Reservation/Details/" + reservation.Id);
            }

            model.Clients = context.Clients.Select(c => new ClientViewModel()
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                Id = c.Id,
                IsAdult = c.IsAdult
            }).ToList();

            model.Rooms = context.Rooms.Select(r => new RoomViewModel()
            {
                Id = r.Id,
                Capacity = r.Capacity,
                Type = r.Type,
                Number = r.Number

            }).ToList();

            return View(model);
        }

        public IActionResult RoomNotFree()
        {
            return View();
        }

        public IActionResult TooManyPeople()
        {
            return View();
        }

        public IActionResult Details(string id)
        {
            Reservation reservation = context.Reservations.Find(id);

            RoomViewModel room = new RoomViewModel(reservation.Room.Id, reservation.Room.Capacity,
                reservation.Room.Type, reservation.Room.IsFree, reservation.Room.Number);

            UserViewModel creator = new UserViewModel(reservation.Creator.Id, reservation.Creator.UserName,
                reservation.Creator.Email, reservation.Creator.FirstName, reservation.Creator.MiddleName,
                reservation.Creator.LastName, reservation.Creator.IsActive);

            List<ClientViewModel> clients = reservation.ClientReservations
                .Select(cr => new ClientViewModel(cr.Client.Id, cr.Client.FirstName,
                    cr.Client.LastName, cr.Client.IsAdult))
                .ToList();


            ReservationDetailsViewModel model = new ReservationDetailsViewModel(id, room, creator, clients,
                reservation.CheckInDate, reservation.CheckOutDate, reservation.IsBreakfastIncluded,
                reservation.IsAllInclusive, reservation.Price);

            return View(model);
        }

        [HttpGet]
        public IActionResult Search()
        {
            List<ReservationViewModel> reservations = context.Reservations
                .Select(r => new ReservationViewModel(r.Id, r.Room.Number, r.Room.Type, r.CheckInDate,
                    r.CheckOutDate, r.Price))
                .ToList();

            ReservationSearchViewModel model = new ReservationSearchViewModel(reservations);

            return View(model);
        }

        [HttpPost]
        public IActionResult Search(ReservationSearchViewModel model)
        {
            if (model.SearchBy == "RoomType")
            {
                model.Reservations = context.Reservations.Where(u => u.Room.Type == (RoomType)int.Parse(model.Value))
                               .ToList()
                               .OrderBy(u => u.Room.Number)
                               .ThenBy(u => u.Room.Type)
                               .Select(u => new ReservationViewModel(u.Id, u.Room.Number, u.Room.Type, u.CheckInDate,
                                    u.CheckOutDate, u.Price))
                               .ToList();
            }
            else if (model.SearchBy == "RoomNumber")
            {
                model.Reservations = context.Reservations.Where(u => u.Room.Number == int.Parse(model.Value))
                               .ToList()
                               .OrderBy(u => u.Room.Number)
                               .ThenBy(u => u.Room.Type)
                               .Select(u => new ReservationViewModel(u.Id, u.Room.Number, u.Room.Type, u.CheckInDate,
                                    u.CheckOutDate, u.Price))
                               .ToList();
            }
            else
            {
                model.Reservations = context.Reservations
                                               .OrderBy(u => u.Room.Number)
                                               .ThenBy(u => u.Room.Type)
                                               .Select(u => new ReservationViewModel(u.Id, u.Room.Number, u.Room.Type, u.CheckInDate,
                                                    u.CheckOutDate, u.Price))
                                               .ToList();
            }

            return View(model);
        }
    }
}