﻿using System;
using System.Collections.Generic;
using System.Linq;
using HotelReservationsManager.Data;
using HotelReservationsManager.Data.Models;
using HotelReservationsManager.Models;
using HotelReservationsManager.Models.UserViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationsManager.Controllers
{
    public class UserController : Controller
    {
        private DbContext context;

        public UserController(DbContext context)
        {
            this.context = context;
        }

        public IActionResult Employees()
        {
            List<UserViewModel> employees = 
                context.Users.Where(u => context.UserRoles.First(ur => ur.UserId == u.Id).RoleId == 
                                            context.Roles.First(r => r.Name == "Employee").Id)
                             .ToList()
                             .OrderBy(u => u.FirstName)
                             .ThenBy(u => u.MiddleName)
                             .ThenBy(u => u.LastName)
                             .Select(u => new UserViewModel()
                             {
                                 Id = u.Id,
                                 UserName = u.UserName,
                                 Email = u.Email,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 IsActive = u.IsActive
                             })
                             .ToList();
           
            return View(employees);
        }

        public IActionResult SearchEmployees(SearchEmployeesViewModel model)
        {
            if (model.SearchBy == "Username")
            {
                model.Users =
                context.Users.Where(u => context.UserRoles.First(ur => ur.UserId == u.Id).RoleId ==
                                            context.Roles.First(r => r.Name == "Employee").Id)
                             .Where(u => u.UserName == model.Value)
                             .ToList()
                             .OrderBy(u => u.FirstName)
                             .ThenBy(u => u.MiddleName)
                             .ThenBy(u => u.LastName)
                             .Select(u => new UserViewModel()
                             {
                                 Id = u.Id,
                                 UserName = u.UserName,
                                 Email = u.Email,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 IsActive = u.IsActive
                             })
                             .ToList();
            }
            else if (model.SearchBy == "FirstName")
            {
                model.Users =
                context.Users.Where(u => context.UserRoles.First(ur => ur.UserId == u.Id).RoleId ==
                                            context.Roles.First(r => r.Name == "Employee").Id)
                             .Where(u => u.FirstName == model.Value)
                             .ToList()
                             .OrderBy(u => u.FirstName)
                             .ThenBy(u => u.MiddleName)
                             .ThenBy(u => u.LastName)
                             .Select(u => new UserViewModel()
                             {
                                 Id = u.Id,
                                 UserName = u.UserName,
                                 Email = u.Email,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 IsActive = u.IsActive
                             })
                             .ToList();
            }
            else if (model.SearchBy == "MiddleName")
            {
                model.Users =
                context.Users.Where(u => context.UserRoles.First(ur => ur.UserId == u.Id).RoleId ==
                                            context.Roles.First(r => r.Name == "Employee").Id)
                             .Where(u => u.MiddleName == model.Value)
                             .ToList()
                             .OrderBy(u => u.FirstName)
                             .ThenBy(u => u.MiddleName)
                             .ThenBy(u => u.LastName)
                             .Select(u => new UserViewModel()
                             {
                                 Id = u.Id,
                                 UserName = u.UserName,
                                 Email = u.Email,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 IsActive = u.IsActive
                             })
                             .ToList();
            }
            else if (model.SearchBy == "LastName")
            {
                model.Users =
                context.Users.Where(u => context.UserRoles.First(ur => ur.UserId == u.Id).RoleId ==
                                            context.Roles.First(r => r.Name == "Employee").Id)
                             .Where(u => u.LastName == model.Value)
                             .ToList()
                             .OrderBy(u => u.FirstName)
                             .ThenBy(u => u.MiddleName)
                             .ThenBy(u => u.LastName)
                             .Select(u => new UserViewModel()
                             {
                                 Id = u.Id,
                                 UserName = u.UserName,
                                 Email = u.Email,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 IsActive = u.IsActive
                             })
                             .ToList();
            }
            else if(model.SearchBy == "Email")
            {
                model.Users =
                context.Users.Where(u => context.UserRoles.First(ur => ur.UserId == u.Id).RoleId ==
                                            context.Roles.First(r => r.Name == "Employee").Id)
                             .Where(u => u.Email == model.Value)
                             .ToList()
                             .OrderBy(u => u.FirstName)
                             .ThenBy(u => u.MiddleName)
                             .ThenBy(u => u.LastName)
                             .Select(u => new UserViewModel()
                             {
                                 Id = u.Id,
                                 UserName = u.UserName,
                                 Email = u.Email,
                                 FirstName = u.FirstName,
                                 MiddleName = u.MiddleName,
                                 LastName = u.LastName,
                                 IsActive = u.IsActive
                             })
                             .ToList();
            }
            else
            {
                model.Users = new List<UserViewModel>();
            }

            return View(model);
        }

        public IActionResult UserDetails(string id)
        {
            User user = context.Users.First(u => u.Id == id);

            UserDetailsViewModel model = new UserDetailsViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                EGN = user.EGN,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DismissDate = user.DismissDate,
                HireDate = user.HireDate,
                IsActive = user.IsActive
            };


            return View(model);
        }
    }
}