using ElmaSmartFarm.DataLibraryCore;
using ElmaSmartFarm.DataLibraryCore.Config;
using ElmaSmartFarm.SharedLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace ElmaSmartFarm.Service.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PoultryController : ControllerBase
{
    public PoultryController()
    {

    }

    [HttpGet]
    public async Task<ActionResult<PoultryDtoModel>> GetPoultryAsync()
    {
        var res = await Task.Run<PoultryDtoModel>(() =>
        {
            var poultryEntities = Program.ServiceHost.Services.GetService<PoultryEntities>();
            return new()
            {
                Poultry = poultryEntities.Poultry,
                UnknownMqttMessages = poultryEntities.UnknownMqttMessages,
                AlarmableSensorErrors = poultryEntities.AlarmableSensorErrors,
                AlarmableFarmPeriodErrors = poultryEntities.AlarmableFarmPeriodErrors,
                AlarmablePoultryPeriodErrors = poultryEntities.AlarmablePoultryPeriodErrors,
                SystemStartupTime = poultryEntities.SystemStartUpTime
            };
        });
        return res;
    }

    [HttpGet("{FarmNumber}")]
    public async Task<ActionResult<FarmModel>> GetFarmAsync(int FarmNumber)
    {
        var res = await Task.Run(() =>
        {
            var poultryEntities = Program.ServiceHost.Services.GetService<PoultryEntities>();
            return poultryEntities.Poultry.Farms.Where(f => f.FarmNumber == FarmNumber).FirstOrDefault();
        });
        return res;
    }

    [HttpGet("MqttSettings")]
    public async Task<ActionResult<MqttConnectionSettingsModel>> GetMqttSettingsAsync()
    {
        var config = Program.ServiceHost.Services.GetService<Config>();
        return new MqttConnectionSettingsModel()
        {
            mqtt_address = config.mqtt.Broker,
            mqtt_port = config.mqtt.Port,
            mqtt_username = config.mqtt.Username,
            mqtt_password = config.mqtt.Password,
            mqtt_authentication = config.mqtt.AuthenticationEnabled,
            mqtt_subscribe_topic = config.mqtt.ToClientTopic + "#",
            mqtt_request_poultry_topic = config.mqtt.GetPoultryInstanceFullTopic
        };
    }
}