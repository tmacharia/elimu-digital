using AutoMapper;
using Common.ViewModels;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class AutoMappingProfile : AutoMapper.Profile
    {
        public AutoMappingProfile()
        {
            CreateMap<School, SchoolViewModel>();
            CreateMap<SchoolViewModel, School>();

            CreateMap<Course, CourseViewModel>();
            CreateMap<CourseViewModel, Course>();

            CreateMap<Unit, UnitViewModel>();
            CreateMap<UnitViewModel, Unit>();

            CreateMap<Content, ContentViewModel>();
            CreateMap<ContentViewModel, Content>();

            CreateMap<Comment, CommentViewModel>();
            CreateMap<CommentViewModel, Comment>();
        }
    }
}
