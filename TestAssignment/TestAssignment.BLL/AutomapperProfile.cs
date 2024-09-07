using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAssignment.BLL.Models;
using TestAssignment.DAL.Entities;
using AutoMapper;

namespace TestAssignment.BLL
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, LoginUserDto>().ReverseMap(); 
            CreateMap<User, RegisterUserDto>().ReverseMap();
            CreateMap<UserTask, UserTaskDto>();
            CreateMap<CreateTaskDto, UserTask>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); 
            CreateMap<UserTask, UserTaskDto>();
            CreateMap<UserTask, UserTaskDto>();
            CreateMap<UpdateTaskDto, UserTask>();
        }
    }
}
