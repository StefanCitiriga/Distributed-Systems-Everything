using PipesAndFilters.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesAndFilters.Filters
{
    class TranslateFilter: IFilter
    {
        public IMessage Run(IMessage message)
        {
            if (message.Headers.ContainsKey("ResponseFormat"))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message.Body);
                string requestBody = "";
                for (int i = 0; i < bytes.Length; i++)
                {
                    requestBody += bytes[i].ToString();
                    if (i + 1 < bytes.Length)
                    {
                        requestBody += "-";
                    }
                }
                message.Body = requestBody;
            }
            else if (message.Headers.ContainsKey("RequestFormat"))
            {
                string responseBody = "";
                string[] byteStrings = message.Body.Split('-');
                byte[] bytes = new byte[byteStrings.Length];
                for (int i = 0; i < byteStrings.Length; i++)
                {
                    bytes[i] = byte.Parse(byteStrings[i]);
                }
                responseBody = Encoding.ASCII.GetString(bytes);
            }
            return message;
        }
    }
}
