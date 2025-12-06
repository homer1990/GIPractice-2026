using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Api.Models.Lookups;

namespace GIPractice.Api.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        //
        // PATIENTS
        //
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.PersonalNumber,
                       opt => opt.MapFrom(src => src.PersonalNumber.Value));

        //
        // APPOINTMENTS
        //
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientFullName,
                       opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
            .ForMember(dest => dest.PreparationProtocolName,
                       opt => opt.MapFrom(src => src.PreparationProtocol != null ? src.PreparationProtocol.Name : null));

        //
        // ENDOSCOPY
        //
        CreateMap<Endoscopy, EndoscopyDto>()
            .ForMember(dest => dest.Observations, opt => opt.MapFrom(src => src.Observations))
            .ForMember(dest => dest.BiopsyBottles, opt => opt.MapFrom(src => src.BiopsyBottles))
            .ForMember(dest => dest.Report, opt => opt.MapFrom(src => src.Report))
            .ForMember(dest => dest.Media,
                       opt => opt.MapFrom(src => src.MediaFiles)); // or src.EndoscopyMedias depending on your nav name
        CreateMap<Endoscopy, EndoscopyListItemDto>()
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
            .ForMember(dest => dest.BiopsyBottlesCount,
                opt => opt.MapFrom(src => src.BiopsyBottles.Count))
            .ForMember(dest => dest.HasReport,
                opt => opt.MapFrom(src => src.Report != null));


        //
        // OBSERVATION
        //
        CreateMap<Observation, ObservationDto>()
            .ForMember(dest => dest.FindingName,
                opt => opt.MapFrom(src => src.Finding != null ? src.Finding.Name : string.Empty))
            .ForMember(dest => dest.OrganAreaCode,
                opt => opt.MapFrom(src => src.OrganArea != null ? src.OrganArea.Code : string.Empty));


        //
        // BIOPSY BOTTLE
        //
        CreateMap<BiopsyBottle, BiopsyBottleDto>();
        CreateMap<OrganArea, OrganAreaDto>();

        //
        // REPORT
        //
        CreateMap<Report, ReportDto>();

        // VISIT
        CreateMap<Visit, VisitDto>()
            .ForMember(dest => dest.PatientFullName,
                       opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName));
        CreateMap<Visit, VisitListItemDto>()
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName));
        //MEDIA FILES
        // Media
        CreateMap<EndoscopyMedia, MediaFileDto>();

        // Organ
        CreateMap<Organ, OrganLookupDto>();

        // OrganArea
        CreateMap<OrganArea, OrganAreaLookupDto>()
            .ForMember(dest => dest.OrganCodes,
                opt => opt.MapFrom(src => src.OrganAreaOrgans.Select(x => x.Organ.Code)));

        // FINDING lookup
        CreateMap<Finding, FindingLookupDto>();

        // PREPARATION PROTOCOL (full DTO)
        CreateMap<PreparationProtocol, PreparationProtocolDto>();
        // PREPARATION PROTOCOL lookup
        CreateMap<PreparationProtocol, PreparationProtocolLookupDto>()
            .ForMember(dest => dest.EndoscopyType,
                opt => opt.MapFrom(src =>
                    src.EndoscopyType.HasValue ? (int?)src.EndoscopyType.Value : null))
            .ForMember(dest => dest.EndoscopyTypeName,
                opt => opt.MapFrom(src =>
                    src.EndoscopyType.HasValue ? src.EndoscopyType.Value.ToString() : null));
        // DIAGNOSIS
        CreateMap<Diagnosis, DiagnosisDto>();
        CreateMap<DiagnosisCreateDto, Diagnosis>();

        // TREATMENTS (already added earlier, keep!)
        CreateMap<Treatment, TreatmentDto>()
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
            .ForMember(dest => dest.DoctorFullName,
                opt => opt.MapFrom(src => src.Doctor.FirstName + " " + src.Doctor.LastName))
            .ForMember(dest => dest.DiagnosisIds,
                opt => opt.MapFrom(src => src.Diagnoses.Select(d => d.Id)))
            .ForMember(dest => dest.MedicineIds,
                opt => opt.MapFrom(src => src.Medications.Select(m => m.Id)));

        // INFAI TESTS
        CreateMap<InfaiTest, InfaiTestDto>()
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName));

        // LAB TESTS
        CreateMap<Test, TestDto>()
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
            .ForMember(dest => dest.LabName,
                opt => opt.MapFrom(src => src.Lab != null ? src.Lab.Name : null))
            .ForMember(dest => dest.DoctorFullName,
                opt => opt.MapFrom(src => src.Doctor != null
                    ? src.Doctor.FirstName + " " + src.Doctor.LastName
                    : null));

        // OPERATIONS
        CreateMap<Operation, OperationDto>()
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
            .ForMember(dest => dest.DoctorFullName,
                opt => opt.MapFrom(src => src.Doctor != null
                    ? src.Doctor.FirstName + " " + src.Doctor.LastName
                    : null));
        //
        // DOCTOR / LAB / MEDICINE / ACTIVE SUBSTANCE
        //
        CreateMap<Doctor, DoctorDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
            .ForMember(dest => dest.LabName,
                opt => opt.MapFrom(src => src.Lab != null ? src.Lab.Name : null));

        CreateMap<Lab, LabDto>();

        CreateMap<ActiveSubstance, ActiveSubstanceDto>();

        CreateMap<Medicine, MedicineDto>()
            .ForMember(dest => dest.ActiveSubstanceIds,
                opt => opt.MapFrom(src => src.ActiveSubstances.Select(a => a.Id)))
            .ForMember(dest => dest.ActiveSubstanceNames,
                opt => opt.MapFrom(src => src.ActiveSubstances.Select(a => a.Name)));
        // OPERATIONS
        CreateMap<Operation, OperationListItemDto>()
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName));

        // LAB TESTS
        CreateMap<Test, TestListItemDto>()
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
            .ForMember(dest => dest.HasQuantitativeResult,
                opt => opt.MapFrom(src => src.QuantitativeResult != null));

        // INFAI TESTS
        CreateMap<InfaiTest, InfaiTestListItemDto>()
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Patient.FirstName + " " + src.Patient.LastName))
            .ForMember(dest => dest.ResultDisplay,
                opt => opt.MapFrom(src => src.Result.ToString()));
    }
}
