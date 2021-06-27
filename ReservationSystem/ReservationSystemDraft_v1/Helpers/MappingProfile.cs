using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Models.Reservation;

namespace ReservationSystemDraft_v1.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            //Person
            CreateMap<Person, Areas.Identity.Models.Profile>().ReverseMap();

            //Sitting
            CreateMap<Sitting, Areas.Admin.Models.Sitting.Create>().ReverseMap();
            CreateMap<Sitting, Areas.Admin.Models.Sitting.Details>().ReverseMap();
            CreateMap<SittingTemplate, Areas.Admin.Models.Sitting.EditByType>().ReverseMap();

            //Area
            CreateMap<Area, Areas.Admin.Models.Area.Create>().ReverseMap();
            CreateMap<Area, Areas.Admin.Models.Area.Edit>().ReverseMap();
            CreateMap<Area, Areas.Admin.Models.Area.Details>().ReverseMap();

            //Table
            CreateMap<Table, Areas.Admin.Models.Area.Index>().ReverseMap();
            CreateMap<Table, Areas.Admin.Models.Table.Edit>().ReverseMap();
            CreateMap<Table, Areas.Admin.Models.Table.Details>().ReverseMap();

            //Reservation (Client side)
            CreateMap<Models.Reservation.Create, Models.Reservation.Details>().ReverseMap();
            //DTO to domain
            CreateMap<Details, Person>().ForMember(p => p.RestaurantId, opt => opt.Ignore());
            //Domain to DTO
            CreateMap<Person, Models.Reservation.Details>();
            CreateMap<Models.Reservation.Details, Reservation>().ReverseMap();





        }

    }
}
