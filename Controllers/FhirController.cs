using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using Hl7.Fhir.Model;

namespace WebApplication5.Controllers
{
    /// <summary>
    /// 呼叫方式, 進 debug mode 後, 在 browser 上執行 http://localhost:1234/api/fhir/patient 或其他 function name
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FhirController : ControllerBase
    {
        [HttpGet("patient")]
        public ActionResult<string> Patient()
        {
            var pt = new Patient();
            pt.Gender = AdministrativeGender.Male; //性別
            pt.Name.Add(new HumanName() { Text = "蔡小明" }); //病人姓名
            pt.BirthDate = "1941-09-09"; //生日, 為 YYYY, YYYY-MM 或 YYYY-MM-DD 三選一, 不包含時區
            pt.Identifier.Add(new Identifier()
            {
                Use = Identifier.IdentifierUse.Official,
                System = "http://cch.org/patients/mrn", //這裡是指醫院定義的MRN, 即 Chart Number
                Value = "12345678" //病歷號
            });

            var ser = new FhirJsonSerializer();
            return new JsonResult(ser.SerializeToDocument(pt));
        }

        [HttpGet("condition")]
        public IActionResult Condition()
        {
            var c = new Condition();
            c.Code = new CodeableConcept()
            {
                Coding = new List<Coding>() {
                     new Coding() {
                         System = "http://hl7.org/fhir/sid/icd-10",
                         Code = "I25.10",
                         Display = "Atherosclerotic heart disease of native coronary artery without angina pectoris"
                     }
                },
                Text = "自體的冠狀動脈粥樣硬化心臟病未伴有心絞痛"
            };
            c.Subject = new ResourceReference()
            {
                Identifier = new Identifier()
                {
                    System = "http://cch.org/patients/mrn",
                    Value = "12345678"
                },
                Display = "蔡小明"
            };

            var ser = new FhirJsonSerializer();
            return new JsonResult(ser.SerializeToDocument(c));
        }

        [HttpGet("appointment")]
        public IActionResult Appointment() {
            var a = new Appointment();
            a.SupportingInformation = new List<ResourceReference>() { new ResourceReference() { Display = "HD" } };
            a.Status = Hl7.Fhir.Model.Appointment.AppointmentStatus.Booked;
            a.Participant = new List<Appointment.ParticipantComponent>() {
                new Appointment.ParticipantComponent() {
                    Actor = new ResourceReference() {
                            Identifier = new Identifier()
                            {
                                System = "http://cch.org/patients/mrn",
                                Value = "12345678"
                            },
                            Display = "蔡小明"
                    },
                    Status = ParticipationStatus.Tentative
                }
            };
            a.Start = new DateTimeOffset(new DateTime(2019, 4, 8, 14, 35, 46, DateTimeKind.Local));
            a.Description = "Hemodialysis Schedule";

            var ser = new FhirJsonSerializer();
            return new JsonResult(ser.SerializeToDocument(a));
        }
    }
}
