#r "Newtonsoft.Json"
​
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
​
const bool format = true;
​
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("Handling CSP Report.");
      
    //dynamic data = await req.Content.ReadAsAsync<object>();
    var json = req.Content.ReadAsStringAsync().Result;
​
    if (format) json = JValue.Parse(json).ToString(Formatting.Indented);
​
    log.Info($"CSP REPORT payload (JSON):\n{json}");
​
    var logicAppUrlBase = "https://prod-19.eastus.logic.azure.com:443";
    var logicAppUrlParams = "/workflows/3f0022cc4d0c46a0b69c132b7390aa3d/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=HT-kp5sSHH8r512JjzzLFhLyLPOmCxTGpnjsSYTHl1w";
​
    // TODO: figure out WHICH (whitelisted) Reporting endpoint posted the report
    // forward to Logic App using above credendials
    using (var client = new HttpClient())
    {
        client.BaseAddress = new Uri(logicAppUrlBase);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await client.PostAsync(logicAppUrlParams, content);
        string resultContent = await result.Content.ReadAsStringAsync();
        log.Info(resultContent);        
        if (result.IsSuccessStatusCode)
        {
            return req.CreateResponse(HttpStatusCode.OK);
        }
        else
        {
            return req.CreateResponse(HttpStatusCode.BadRequest, 
                "Failed while trying to forward to LA");
        }
   
