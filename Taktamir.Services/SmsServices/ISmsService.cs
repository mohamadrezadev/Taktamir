using Kavenegar;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Taktamir.framework;

namespace Taktamir.Services.SmsServices
{
    public interface ISmsService
    {
        Task<Tuple<bool, string>> SendVerifycode(string phonnumber);
        Task<Tuple<bool, string>> SendLookup(string phonnumber);
        Task Send_sms(string phon_number, string Message);
    }
   
    public class SmsService : ISmsService
    {
        private readonly KavenegarConfig _kavenegarConfig;

        public SmsService(IOptions<KavenegarConfig> kavenegarConfig)
        {
            _kavenegarConfig = kavenegarConfig.Value;
        }
        public async  Task<Tuple<bool, string>> SendLookup(string phonnumber)
        {
            try
            {
                KavenegarApi api = new KavenegarApi(_kavenegarConfig.APIKey);
                var templetneme = "TakTamir";
                string code = new Random((int)DateTime.Now.Ticks).Next(10000, 99999).ToString();
                var result = await api.Send(phonnumber, code, templetneme);
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine(result);
                Console.ResetColor();
                return Tuple.Create(true, code);
            
            }
            catch (Kavenegar.Core.Exceptions.ApiException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Kavenegar.Core.Exceptions.HttpException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task Send_sms(string phon_number, string Message)
        {
            try
            {
                KavenegarApi api = new KavenegarApi(_kavenegarConfig.APIKey);
                var result = await api.Send("1000500055500", phon_number, Message);
               
            }
            catch (Kavenegar.Core.Exceptions.ApiException ex)
            {
        
                Console.Write("Message : " + ex.Message);
                throw new Exception(ex.StackTrace);
            }
            catch (Kavenegar.Core.Exceptions.HttpException ex)
            {
               
                Console.Write("Message : " + ex.Message);
                throw new Exception(ex.StackTrace);
            }
        }
        public async Task<Tuple<bool,string>> SendVerifycode(string phonnumber)
        {
            // var verifyApiAddress = "https://api.kavenegar.com/v1/6545724758465579756F3758713548752F6B326C727531746B6C59302F583858785373726D5133596734593D/verify/lookup.json";
            var verifyApiAddress = "https://api.kavenegar.com/v1/" + _kavenegarConfig.APIKey + "/verify/lookup.json";
            var queryStrings = new List<string>
            {
                "receptor=" + phonnumber,
                "template=" + "TakTamir"
            };
            string code = new Random((int)DateTime.Now.Ticks).Next(10000, 99999).ToString();
            queryStrings.Add($"token={WebUtility.UrlEncode(code)}");

            verifyApiAddress += "?" + string.Join("&", queryStrings);
            using (var client = new HttpClient())
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(await client.GetStringAsync(verifyApiAddress));
                    Console.WriteLine(result);
                    return Tuple.Create(true,code);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return Tuple.Create(false,""); 
                }
            }
        }
    }

}
