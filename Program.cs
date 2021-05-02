using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace vaccine_slot_availaibility
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            //
            var cowinData=GetCowinData();
            var relevantCenters=GetAvailableSlotsFor18Plus(cowinData);
            string message = String.Empty;
            if(relevantCenters.Count>0)
                message=BuildMessage(relevantCenters);
            Console.Write(message);
        }

        static VaccineSlotModel GetCowinData()
        {
            string currentDate=DateTime.Now.ToString("dd-MM-yyyy");
            string url = "https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByDistrict?district_id=395&date="+currentDate;
            HttpResponseMessage response =  client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<VaccineSlotModel>(responseBody, new JsonSerializerOptions() { WriteIndented = true });  
           
        }

        static List<Center> GetAvailableSlotsFor18Plus(VaccineSlotModel cowinData){
            
            List<Center> relevantCenters=new List<Center>(); 
            List<Center> availableSlots=new List<Center>(); 
            foreach (var center in cowinData.centers)
            {
                foreach (var session in center.sessions)
                {
                    if(session.min_age_limit==18){
                        relevantCenters.Add(center);
                    }
                    if(session.min_age_limit==18 && session.available_capacity>0){
                        availableSlots.Add(center);
                    }
                }
            }
            return availableSlots;
        }

        static string BuildMessage(List<Center> centers){
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.Append("Slots available in:");
            stringBuilder.Append(Environment.NewLine);
            foreach (var item in centers)
            {
                stringBuilder.AppendFormat("Name : {0}, Capacity : {1} ",item.name,item.sessions[0].available_capacity);
                stringBuilder.Append(Environment.NewLine);
            }
            return stringBuilder.ToString();
        }
        
    }
}
