using System.Net;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.IO;
class HomeAssistant
{
    static string lampStartAutomationEntityId = "";
    static string lampPauseAutomationEntityId = "";
    public enum Commands { START, PAUSE };
    HttpHelper _httpHelper = new HttpHelper();

    public void HomeAssistantPost(Commands command)
    {
        // Entry point to HomeAssistant integration.
        // Switch on command enum to invoke various services.
        // When adding new services, update the enum, add a string, and add a case here.
        switch (command)
        {
            case Commands.START:
                _httpHelper.HomeAssistServicePost(lampStartAutomationEntityId);
                break;
            case Commands.PAUSE:
                _httpHelper.HomeAssistServicePost(lampPauseAutomationEntityId);
                break;
            default:
                break;
        }
    }
    class HttpHelper
    {
        // Handles POST requests to Home Assistant API.
        // Supports only the services route.
        // Reference: https://developers.home-assistant.io/docs/api/rest/
        private static readonly HttpClient client = new HttpClient();
        HaTokenData homeAssistantToken = new HaTokenData(null, null, null);
        string haToken = "";
        string HomeAssistantUrl = "";
        public HttpHelper()
        {
            this.homeAssistantToken = ValidateData();
            this.haToken = homeAssistantToken.token;
            this.HomeAssistantUrl = homeAssistantToken.url;
            this.homeAssistantToken.services = homeAssistantToken.services;
            lampPauseAutomationEntityId = homeAssistantToken.services[0].entity_id;
            lampStartAutomationEntityId = homeAssistantToken.services[1].entity_id;
        }
        HaTokenData ValidateData()
        {
            // Check if Api config data is stored locally.
            // If not, prompt the user to enter it.
            // Return the data object and save it to local storage.
            HaTokenData hd = LocalStorage.GetTokenData(LocalStorage.ConfigFiles.HOMEASSISTANTCONFIG) as HaTokenData;
            if (hd == null)
            {
                hd = PromptUserToken();
            }
            return hd;
        }
        HaTokenData PromptUserToken()
        {
            // Prompt for information required for API integration.
            Console.WriteLine("Please enter your Home Assistant URL");
            string url = Console.ReadLine();
            Console.WriteLine("Please enter your Home Assistant personal token");
            string token = Console.ReadLine();
            Console.WriteLine("Please enter the entity ID of your HomeAssistant Lamp start automation");
            string lampStartService = Console.ReadLine();
            Console.WriteLine("Please enter the entity ID of your HomeAssistant lamp pause automation");
            string lampPauseService = Console.ReadLine();

            HomeAssistant.HaAutomationData[] automations = new HomeAssistant.HaAutomationData[2];
            automations[0] = new HomeAssistant.HaAutomationData(lampPauseService);
            automations[1] = new HomeAssistant.HaAutomationData(lampStartService);

            HaTokenData htd = new HaTokenData(token, url, automations);
            string text = JsonSerializer.Serialize(htd);

            LocalStorage.SaveTokenDataPublic(text, LocalStorage.ConfigFiles.HOMEASSISTANTCONFIG);
            return htd;
        }
        async public void HomeAssistServicePost(string homeAssistAutomationEntityId)
        {
            // Send a POST request to the services route.
            string servicesUrl = HomeAssistantUrl + "/api/services/automation/trigger";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", haToken);
            HaAutomationData hd = new HaAutomationData(homeAssistAutomationEntityId);
            string jsonBody = JsonSerializer.Serialize(hd);

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, servicesUrl))
            {
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
            }
        }
    }
    public class HaAutomationData
    {
        // Class for storing the name of the HomeAssistant services.
        public string entity_id { get; set; }
        public HaAutomationData(string Entity_Id)
        {
            this.entity_id = Entity_Id;
        }
    }
}