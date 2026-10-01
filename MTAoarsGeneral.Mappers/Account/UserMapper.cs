using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Mappers.Account
{

    public class UserMapper : IMapper
    {


        public void Map()
        {
            MapIdentity();
        }

        void MapIdentity()
        {
            Mapper.CreateMap<Menu, MenuItem>();

            Mapper.CreateMap<User, Identity>()
                 .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.ID))
                 .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                 .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                 .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Company.Code))
                 .ForMember(dest => dest.IsGeneral, opt => opt.UseValue(true))
                 .ForMember(dest => dest.IsFamily, opt => opt.UseValue(false))
                 .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.LookupRole.Code == LookupConstants.Role.ADM || src.LookupRole.Code == LookupConstants.Role.ADMIN))
                 .AfterMap((src, dest) =>
                 {
                     var menuRepository = ObjectContainer.Container.Resolve<IMenuRepository>();
                     var menus = menuRepository.GetMenus(src.RoleID);



                     foreach (var menu in menus)
                     {
                         GroupSubMenu(dest, menu);

                         //if (!dest.IsISM) /* Split license */
                         //{
                         //    if (dest.IsFamily && !dest.IsGeneral)
                         //        menu.Menus.RemoveAll(i => i.Caption.Contains("- General"));
                         //    if (!dest.IsFamily && dest.IsGeneral)
                         //        menu.Menus.RemoveAll(i => i.Caption.Contains("- Family"));
                         //}
                         dest.Menus.Add(menu);
                     }

                     var allMenus = menuRepository.GetMenus();
                     foreach (var menu in allMenus)
                         dest.AllMenus.Add(menu);
                 });

        }

        private void GroupSubMenu(Identity identity, MenuItem menu)
        {
            var groups = new Dictionary<string, string[]>()
            {
                { "Termination", new string[] { "Terminated", "Termination" } },
                { "Invoice", new string[] { "Invoice" } }
            };

            if (identity.IsAdmin && menu.Caption == "Reports")
            {
                foreach (var group in groups)
                {
                    var gMenus = menu.Menus.Where(i => Array.Exists(group.Value, elm => i.Description.Contains(elm)));
                    if (gMenus.Count() > 1) //more than 1 item only group together
                    {
                        menu.Menus = menu.Menus.Except(gMenus).ToList();
                        var newMenuItem = new MenuItem() { Caption = group.Key, Description = group.Key };
                        newMenuItem.Menus = gMenus.ToList();
                        menu.Menus.Insert(0, newMenuItem);
                    }
                }
            }
        }

    }// class

}// namespace
