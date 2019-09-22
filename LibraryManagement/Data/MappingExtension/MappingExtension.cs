using AutoMapper;
using Seagull.Core.Data.Model;
using Seagull.Core.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Seagull.Core.Models;
using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Doctors.Areas.Admin.ViewModel;
using Seagull.Doctors.Data.Model;
using Seagull.Doctors.ViewModel;
using System;

namespace Seagull.Core.Data.MappingExtension
{
    public class MappingExtension : Profile
    {
        public MappingExtension()
        {

            //Map your objects


            #region Users
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.IsRtl,
                           opts => opts.MapFrom(src => src.LangId.HasValue ? src.LangId == 1 ? false : true : false))

                           .ForMember(dest => dest.SelectedUserRoles,
                           opts => opts.MapFrom(src => src.fk_UserRoleMap != null ? string.Join(",", src.fk_UserRoleMap.Select(a => a.UserRoleId)) : null))
                           .ForMember(dest => dest.PermissionName,
                           opts => opts.MapFrom(src => src.fk_UserRoleMap != null && src.fk_UserRoleMap.Select(d => d.UserRole.fk_UserRolePermMap.Count() > 0).Count() > 0 ? src.fk_UserRoleMap.SelectMany(a => a.UserRole.fk_UserRolePermMap.Select(p => p.Permission.Name).ToList()).Distinct().ToList() : new List<string>()))
                           .ForMember(dest => dest.UserRoleName,
                           opts => opts.MapFrom(src => src.fk_UserRoleMap != null ? src.fk_UserRoleMap.Select(d => d.UserRole.Name) : new List<string>()));
            //.ForMember(dest => dest.strCountry,
            //opts => opts.MapFrom(src => src.fk_UserRoleMap != null ? src.fk_UserRoleMap.Select(d => d.UserRole.Name) : new List<string>()));

            CreateMap<UserViewModel, User>()
                .ForMember(dest => dest.fk_UserRoleMap,
                opts => new UserRoleMap());

            CreateMap<UserRegisterModel, User>()
                .ForMember(dest => dest.Email, opts => opts.MapFrom(f => f.Email))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(f => f.Name))
                .ForMember(dest => dest.Password, opts => opts.MapFrom(f => f.Password))
                .ForMember(dest => dest.Mobile, opts => opts.MapFrom(f => f.Mobile))
                .ForMember(dest => dest.LangId, opts => opts.MapFrom(f => f.LangId))
                .ForMember(dest => dest.fk_UserRoleMap, opts => new UserRoleMap());

            CreateMap<User, UserRegisterModel>()
                .ForMember(dest => dest.Email, opts => opts.MapFrom(f => f.Email))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(f => f.Name))
                .ForMember(dest => dest.Password, opts => opts.MapFrom(f => f.Password))
                .ForMember(dest => dest.Mobile, opts => opts.MapFrom(f => f.Mobile))
                .ForMember(dest => dest.LangId, opts => opts.MapFrom(f => f.LangId));
            #endregion

            CreateMap<UserRole, UserRoleViewModel>();
            CreateMap<UserRoleViewModel, UserRole>();

            #region EmailTemplate
            CreateMap<EmailTemplateViewModel, EmailTemplate>();
            CreateMap<EmailTemplate, EmailTemplateViewModel>();
            #endregion



            

        }

    }
}
